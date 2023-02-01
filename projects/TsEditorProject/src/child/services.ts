import * as csharp from "csharp";
import { $typeof } from "puerts";
import * as ts from "typescript";

import Type = csharp.System.Type;
const { Guid } = csharp.System;
const { HashUtil } = csharp.XOR;
const { File, Directory, Path } = csharp.System.IO;

type ConfigFile = {
    readonly files?: string[];
    readonly include?: string[];
    readonly exclude?: string[];
    readonly references?: string[];
    readonly compilerOptions: ts.CompilerOptions;
};

const EmptyCharacters = [" ", "\t"], EnterCharacters = ["\n", "\r"];

enum ModuleFlags {
    Global = "global",
    CS = "CS",
    CSharp = "csharp",
}
enum ClassesFlags {
    TsComponent = "xor.TsComponent",
}
enum DecoratorFlags {
    GUID = "xor.guid",
    Route = "xor.route",
    Field = "xor.field",
}

/**读取tsconfig.json文件并反序化成对象
 * @param path 
 * @returns 
 */
export function readConfigFile(path: string): ConfigFile {
    let { config, error } = ts.readConfigFile(path, (path) => File.ReadAllText(path));
    if (error) {
        throw error;
    }
    return config;
}

/**读取并解析tsconfig.json文件, 获取编译的脚本等
 * @param tsconfigFile 
 * @param options 
 * @returns 
 */
export function parseConfigFile(tsconfigFile: string, options?: {
    /**文件夹搜索最大深度, default:100  */
    maxDepth?: number;
    /**文件搜索状态回调 */
    status?: (folders: number, files: number) => void;
}): ts.ParsedCommandLine {
    let { maxDepth = 100, status } = options ?? {};

    const configFile = readConfigFile(tsconfigFile);
    const jsonConfigFile = ts.readJsonConfigFile(tsconfigFile, (path) => File.ReadAllText(path));

    let basePath = Path.GetDirectoryName(tsconfigFile);
    if (configFile.include) {
        let maxUpperLevel = 0, curUpperLevel: number;
        for (let p of configFile.include) {
            p = p.replace(/\\/g, '/');
            curUpperLevel = 0;
            while (p.startsWith("../")) {
                p = p.substring(3);
                curUpperLevel++;
            }
            if (curUpperLevel > maxUpperLevel) maxUpperLevel = curUpperLevel;
        }
        for (let i = 0; i < maxUpperLevel; i++) {
            basePath = Path.GetDirectoryName(basePath);
        }
    }

    let folderCount = 0, fileCount = 0;
    const readDirectory = (rootDir: string, extensions: string[], excludes: string[], includes: string[], depth?: number) => {
        if (depth && depth > maxDepth)
            return null;
        const results = new Array<string>();

        if (status) {
            folderCount++;
            status(folderCount, fileCount);
        }

        let files = Directory.GetFiles(rootDir);
        for (let i = 0; i < files.Length; i++) {
            let file = files.get_Item(i);
            if (extensions && !extensions.find(ext => file.endsWith(ext))) {
                continue;
            }
            results.push(file);
            if (status) {
                fileCount++;
                status(folderCount, fileCount);
            }
        }

        let dirs = Directory.GetDirectories(rootDir);
        for (let i = 0; i < dirs.Length; i++) {
            let subFiles = readDirectory(dirs.get_Item(i), extensions, excludes, includes, (depth ?? 0) + 1);
            if (subFiles && subFiles.length > 0) {
                results.push(...subFiles);
            }
        }
        return results;
    };

    return ts.parseJsonConfigFileContent(jsonConfigFile, {
        useCaseSensitiveFileNames: false,
        readDirectory,
        fileExists: (path) => File.Exists(path),
        readFile: (path) => File.ReadAllText(path),
        trace: (s) => console.log(s),
    }, basePath,);
}

export class Program {
    private readonly cp: csharp.XOR.Services.Program;
    private readonly program: ts.Program;
    private readonly checker: ts.TypeChecker;

    private readonly types: Map<string, ts.ClassDeclaration>;
    private readonly typeDefinitions: Map<string, TypeDefinition>;

    private readonly sourceHash: Map<string, string>;

    private readonly throttler: Throttler;

    constructor(cp: csharp.XOR.Services.Program, rootNames: string[], options: ts.CompilerOptions) {
        cp.state = csharp.XOR.Services.ProgramState.Compiling;
        cp.stateMessage = '';

        this.cp = cp;
        this.program = ts.createProgram({
            rootNames,
            options: {
                target: options.target,
                module: options.module,
                noEmit: true,
            }
        });
        this.checker = this.program.getTypeChecker();
        this.types = new Map();
        this.typeDefinitions = new Map();
        this.sourceHash = new Map();

        this.throttler = new Throttler(100, 1);

        this.resolves();
    }
    private async resolves() {
        await this.resolveSources();
        await this.resolveComponents();

        await this.resolveUnknownGuid();

        this.cp.state = csharp.XOR.Services.ProgramState.Completed;
        this.cp.stateMessage = '';
    }
    /**文件修改状态 */
    public change(path: string) {
        console.log("file change: " + path);
        //文件已删除
        if (!File.Exists(path)) {

            return;
        }
        //文件已同步(被Program内部修改)
        if (this.sourceHash.get(path) === HashUtil.SHA256File(path))
            return;
        //文件新增或更新
    }

    //#region SourceFile 处理流程
    private async resolveSources() {
        let sourceFiles = this.program.getSourceFiles();
        this.cp.errors = 0;
        this.cp.scripts = sourceFiles.length;

        this.cp.state = csharp.XOR.Services.ProgramState.Analyzing;
        this.cp.stateMessage = 'file';
        //开始解析文件
        for (const source of sourceFiles) {
            if (!source.statements)
                continue;
            await this.resolveStatements(source.statements);
        }
    }
    private async resolveStatements(statements: ts.NodeArray<ts.Statement>) {
        for (let statement of statements) {
            switch (statement.kind) {
                case ts.SyntaxKind.ClassDeclaration:
                    await this.resolveClassDeclaration(<ts.ClassDeclaration>statement);
                    break;
                case ts.SyntaxKind.ModuleDeclaration:
                    await this.resolveModuleDeclaration(<ts.ModuleDeclaration>statement);
                    break;
            }
        }
    }
    private async resolveClassDeclaration(node: ts.ClassDeclaration) {
        await this.throttler.complete();
        this.types.set(this.getAbsoluteName(node), node);
    }
    private async resolveModuleDeclaration(node: ts.ModuleDeclaration) {
        for (let childNode of node.getChildren()) {
            switch (childNode.kind) {
                case ts.SyntaxKind.ModuleBlock:
                    await this.resolveStatements((<ts.ModuleBlock>childNode).statements);
                    break;
                case ts.SyntaxKind.Identifier:
                    break;
            }
        }
    }
    //#endregion


    //#region   Decorator定义处理流程
    private async resolveComponents() {
        this.cp.state = csharp.XOR.Services.ProgramState.Analyzing;
        this.cp.stateMessage = 'component';

        for (let [absoluteName, cd] of this.types) {
            await this.throttler.complete();

            let definition: TypeDefinition = {
                absoluteName,
                isExport: util.isExport(cd),
                isDeclare: util.isDeclare(cd, true),
                isAbstract: util.isAbstract(cd),
                isComponent: this.isInheritFromTsComponent(cd) && !this.isTsComponent(cd)
            };
            this.typeDefinitions.set(absoluteName, definition);
            this.resolveComponentDecorator(cd, definition);
            this.pushType(definition);
        }
    }
    private resolveComponentDecorator(node: ts.ClassDeclaration, define: TypeDefinition) {
        if (!node.modifiers)
            return;
        for (let modifier of node.modifiers) {
            if (modifier.kind !== ts.SyntaxKind.Decorator)
                continue;
            let callExpression = (<ts.Decorator>modifier).expression;
            if (callExpression.kind !== ts.SyntaxKind.CallExpression)
                continue;

            let { expression: target, arguments: args } = <ts.CallExpression>callExpression;
            if (this.getModuleFromNode(target) !== ModuleFlags.Global)
                continue;
            switch (this.getFullName(target)) {
                case DecoratorFlags.GUID:
                    define.guid = (<ts.StringLiteral>args[0]).text;
                    break;
                case DecoratorFlags.Route:
                    define.route = (<ts.StringLiteral>args[0]).text;
                    break;
            }
        }
    }
    private async resolveUnknownGuid() {
        this.cp.state = csharp.XOR.Services.ProgramState.Allocating;
        this.cp.stateMessage = '';

        for (let [, type] of this.typeDefinitions) {
            if (type.guid || !this.isExportTsCompoent(type)) {
                continue;
            }
            let td = this.types.get(type.absoluteName);
            if (!td) {
                console.warn(`节点数据缺失: ${type.absoluteName}`);
                return;
            }
            await this.throttler.complete();

            let guid = csharp.System.Guid.NewGuid().ToString();

            let sourceFile = this.allocDecorator(td, `@xor.guid("${guid}")`);
            if (sourceFile) {
                this.sourceHash.delete(sourceFile.fileName);
                type.guid = guid;

                this.pushType(type);
                File.WriteAllText(sourceFile.fileName, sourceFile.text);
            }
        }
    }
    //#endregion

    private pushType(type: TypeDefinition) {
        if (!type.guid || !this.isExportTsCompoent(type)) {
            return;
        }

        let node = this.types.get(type.absoluteName);
        if (!node) {
            console.warn(`节点数据缺失: ${type.absoluteName}`);
            return;
        }
        let [module, name] = type.absoluteName.split("|");

        //创建type声明
        let ctd = new csharp.XOR.Services.TypeDeclaration();
        ctd.name = name;
        ctd.module = module;
        ctd.guid = type.guid;
        ctd.version = this.getSourceFileHash(node);
        //成员声明
        let members = this.getFields(node, true);
        if (members) {
            for (let [name, property] of members) {
                let fieldArgs = this.getFieldArguments(property),
                    fieldType = this.convertToCSharpType([property.type, fieldArgs?.type]);
                if (!fieldType || !fieldType.type) {
                    continue;
                }
                let cpd = new csharp.XOR.Services.PropertyDeclaration();
                cpd.name = name;
                cpd.valueType = fieldType.type;
                ctd.AddProperty(cpd);

                if (fieldType.enumerable) {
                    for (let [name, value] of fieldType.enumerable) {
                        cpd.AddEnum(name, value);
                    }
                }
                else if (fieldArgs && fieldArgs.range) {
                    cpd.SetRange(fieldArgs.range[0], fieldArgs.range[1]);
                }
                if (fieldArgs && fieldArgs.value !== undefined) {
                    cpd.defaultValue = fieldArgs.value;
                }
            }
        }
        this.cp.AddStatement(ctd);
    }
    private allocDecorator(node: ts.Node, decoratorContent: string) {
        let sourceFile = node.getSourceFile();

        let content = sourceFile.getFullText();
        let start = node.getStart(), end = node.getEnd();

        let stringBuilder = [decoratorContent, "\n"];
        //检查语句前面的空格字符, 检查前一句是否有换行
        let preIndex = start - 1, hasEnter = false;
        while (preIndex > 0) {
            let curChar = content[preIndex];
            if (EmptyCharacters.includes(curChar)) {
                stringBuilder.unshift(curChar);
                preIndex--;
            } else {
                hasEnter = EnterCharacters.includes(curChar);
                break;
            }
        }
        if (!hasEnter) stringBuilder.unshift("\n");
        start = preIndex + 1;

        //新的ts脚本内容
        let insert = stringBuilder.join("");
        let newContent = `${content.slice(0, start)}${insert}${content.slice(start)}`;

        return ts.updateSourceFile(sourceFile, newContent, {
            span: {
                start: preIndex,
                length: 0
            },
            newLength: insert.length,
        }, false);;
    }

    private isExportTsCompoent(type: TypeDefinition) {
        return !(
            !type.isComponent ||
            !type.isExport ||
            type.isDeclare ||
            type.isAbstract
        );
    }

    /**当前类型声明是否为xor.TsComponent */
    private isTsComponent(node: ts.ClassDeclaration): boolean {
        return this.getFullName(node) === ClassesFlags.TsComponent &&
            this.getModuleFromNode(node) === ModuleFlags.Global;
    }
    /**当前类型是否为xor.TsComponent或其派生类 */
    private isInheritFromTsComponent(node: ts.ClassDeclaration): boolean {
        if (this.isTsComponent(node)) {
            return true;
        }
        //检测继承类
        if (node.heritageClauses) {
            let ewtaList = node.heritageClauses.map(clause => clause.types).flat();
            for (let ewta of ewtaList) {
                let cd = this.types.get(this.getAbsoluteName(ewta.expression));
                if (!cd) {
                    //console.warn(`无效的继承对象:${this.getAbsoluteName(ewta.expression)}`);
                    continue;
                }
                if (this.isInheritFromTsComponent(cd)) {
                    return true;
                }
            }
        }
        return false;
    }
    /**是否为序列化字段声明(public丶declare或xor.field装饰器) */
    private isField(node: ts.PropertyDeclaration): boolean {
        if (util.isDeclare(node, false) || util.isPublic(node))
            return true;
        if (!node.modifiers)
            return false;
        let decoratorArgs = this.getFieldDecorator(node);
        if (decoratorArgs) {
            return true;
        }
        return false;
    }
    /**获取Class中所有序列化字段 */
    private getFields(node: ts.ClassDeclaration, inherit?: boolean): Map<string, ts.PropertyDeclaration> {
        if (this.isTsComponent(node)) {
            return null;
        }
        const members = new Map<string, ts.PropertyDeclaration>();
        if (node.members) {
            for (let m of node.members) {
                if (m.kind !== ts.SyntaxKind.PropertyDeclaration || !this.isField(<ts.PropertyDeclaration>m))
                    continue;
                let name = (<ts.PropertyDeclaration>m).name;
                if (name.kind === ts.SyntaxKind.StringLiteral || name.kind === ts.SyntaxKind.Identifier) {
                    members.set(name.text, <ts.PropertyDeclaration>m);
                }
            }
        }
        if (inherit && node.heritageClauses) {
            let ewtaList = node.heritageClauses.map(clause => clause.types).flat();
            for (let ewta of ewtaList) {
                let cd = this.types.get(this.getAbsoluteName(ewta.expression));
                if (!cd) {
                    //console.warn(`无效的继承对象:${this.getAbsoluteName(ewta.expression)}`);
                    continue;
                }
                const _members = this.getFields(cd, true);
                if (!_members) {
                    continue;
                }
                for (let [name, type] of _members) {
                    if (members.has(name)) continue;
                    members.set(name, type);
                }
            }
        }
        return members;
    }
    /**是否显式声明为序列化字段, 用于非公开属性或指定RawType */
    private getFieldDecorator(node: ts.PropertyDeclaration): ts.NodeArray<ts.Expression> {
        if (!node.modifiers)
            return null;
        for (let modifier of node.modifiers) {
            if (modifier.kind !== ts.SyntaxKind.Decorator)
                continue;
            let callExpression = (<ts.Decorator>modifier).expression;
            if (callExpression.kind !== ts.SyntaxKind.CallExpression)
                continue;

            let { expression: target, arguments: args } = <ts.CallExpression>callExpression;
            if (this.getModuleFromNode(target) !== ModuleFlags.Global || this.getFullName(target) !== DecoratorFlags.Field)
                continue;
            return args;
        }
        return null;
    }
    /**获取序列化字段参数声明(解析xor.field参数)
     * @param decoratorArgsExpressions 
     */
    private getFieldArguments(node: ts.PropertyDeclaration) {
        let args: ts.NodeArray<ts.Expression> = this.getFieldDecorator(node);
        if (!args || args.length === 0)
            return null;
        let type: ts.PropertyAccessExpression, range: [min: number, max: number], value: any;

        let first = args[0];
        switch (first.kind) {
            case ts.SyntaxKind.ObjectLiteralExpression:
                for (let property of (<ts.ObjectLiteralExpression>first).properties) {
                    if (property.kind !== ts.SyntaxKind.PropertyAssignment)
                        continue;
                    let { name, initializer } = <ts.PropertyAssignment>property;
                    if (initializer.kind === ts.SyntaxKind.NullKeyword || initializer.kind === ts.SyntaxKind.UndefinedKeyword)
                        continue;
                    switch (name.getText()) {
                        case "type":
                            type = <ts.PropertyAccessExpression>initializer;
                            break;
                        case "range":
                            let [min, max] = (<ts.ArrayLiteralExpression>initializer).elements;
                            if (min && min.kind === ts.SyntaxKind.NumericLiteral &&
                                max && max.kind === ts.SyntaxKind.NumericLiteral) {
                                range = [
                                    util.getExpressionValue(this.checker, min),
                                    util.getExpressionValue(this.checker, max)
                                ];
                            }
                            break;
                        case "value":
                            value = util.getExpressionValue(this.checker, initializer);
                            break;
                    }
                }
                break;
            case ts.SyntaxKind.PropertyAccessExpression:
                type = <ts.PropertyAccessExpression>first;
                break;
        }
        return { type, range, value };
    }

    private getSourceFileHash(node: ts.Node) {
        let path = node.getSourceFile().fileName;
        let hash = this.sourceHash.get(path);
        if (!hash) {
            hash = HashUtil.SHA256(node.getSourceFile().text);
            this.sourceHash.set(path, hash);
        }
        return hash;
    }

    //#region 类型方法
    /**获取类型唯一值名称, 以`moduleName + namespaceName +typeName`组合并返回
     * @param node 
     * @returns 
     */
    private getAbsoluteName(node: ts.Node) {
        let module = this.getModuleFromNode(node), fullName = this.getFullName(node);
        return `${module}|${fullName}`;
    }
    /**获取类型全名称(包含namespace, 不包含module前缀)
     * @param node 
     * @returns 
     */
    private getFullName(node: ts.Node) {
        const type = this.checker.getTypeAtLocation(node);
        const symbol = type.getSymbol();
        if (symbol) {
            /*
            const flags = ts.SymbolFlags.Class |
                ts.SymbolFlags.Interface |
                ts.SymbolFlags.Enum |
                ts.SymbolFlags.Type |
                ts.SymbolFlags.Namespace |
                ts.SymbolFlags.Module |
                ts.SymbolFlags.PropertyOrAccessor |
                ts.SymbolFlags.ClassMember;//*/
            const formatFlags = ts.SymbolFormatFlags.WriteTypeParametersOrArguments |
                ts.SymbolFormatFlags.UseOnlyExternalAliasing |
                ts.SymbolFormatFlags.AllowAnyNodeKind |
                ts.SymbolFormatFlags.UseAliasDefinedOutsideCurrentScope;
            return this.checker.symbolToString(
                symbol,
                node,
                0xffffffff,
                formatFlags
            );
        }
        else {
            return this.checker.typeToString(
                type,
                node,
                ts.TypeFormatFlags.NodeBuilderFlagsMask
            )
        }
    }
    /**获取类型声明所在的模块
     * @param node 
     * @returns 
     */
    private getModuleFromNode(node: ts.Node) {
        const type = this.checker.getTypeAtLocation(node);
        const symbol = type.getSymbol();
        if (symbol) {
            let declarations = symbol.getDeclarations();
            if (declarations && declarations.length > 0) {
                return this.getModuleFromDeclaration(
                    declarations.find(d => d.kind === ts.SyntaxKind.ClassDeclaration) ||
                    declarations.find(d => d.kind === ts.SyntaxKind.InterfaceDeclaration) ||
                    declarations[0]
                );
            }
        }
        return ModuleFlags.Global;
    }
    /**获取类型声明所在的模块
     * @param declaration 
     * @returns 
     */
    private getModuleFromDeclaration(declaration: ts.Declaration) {
        let module: string;

        let node: ts.Node = declaration, flags: ts.NodeFlags;
        while (node) {
            if (node.kind === ts.SyntaxKind.ModuleDeclaration) {
                flags = node.flags;
                module = (<ts.ModuleDeclaration>node).name.text;
                //如果是global声明或module声明(非namespace), 直接返回模块名
                if ((flags & ts.NodeFlags.Namespace) !== ts.NodeFlags.Namespace &&
                    (flags & ts.NodeFlags.NestedNamespace) !== ts.NodeFlags.NestedNamespace
                ) {
                    return module;
                }
            }
            node = node.parent;
        }
        //sourceFile为全局声明模块(非扩大声明模块), 且存在顶层namspace声明
        let sourceFile = declaration.getSourceFile();
        if (module && (sourceFile.flags & ts.NodeFlags.ExportContext) === ts.NodeFlags.ExportContext) {
            return module;
        }
        return sourceFile.fileName;
    }
    /**获取类型声明信息
     * @param node 
     * @returns 
     */
    private getDeclarationByReference(node: ts.TypeReferenceNode) {
        const type = this.checker.getTypeAtLocation(node);
        const symbol = type.getSymbol();
        if (symbol) {
            let declarations = symbol.getDeclarations();
            if (declarations && declarations.length > 0) {
                return declarations.find(d => d.kind === ts.SyntaxKind.ClassDeclaration) ||
                    declarations.find(d => d.kind === ts.SyntaxKind.InterfaceDeclaration) ||
                    declarations[0];
            }
        }
        return null;
    }

    private _baseTypes: { [t: string]: Type } = {
        ["string"]: $typeof(csharp.System.String),
        ["number"]: $typeof(csharp.System.Double),
        ["boolean"]: $typeof(csharp.System.Boolean),
        ["bigint"]: $typeof(csharp.System.Int64),
    };
    /**获取ts.NodeType其对应的C#类型:
     * 如果是UnionTypes或enum声明, 则额外获取其enumerable信息
     * @param node 
     * @param depth 
     * @returns 
     */
    private convertToCSharpType(node: ts.Node | [type: ts.Node, explicitType: ts.Node], depth: number = 0): { type: Type, enumerable?: Map<string, string | number> } {
        if (depth > 3) {
            return null;
        }
        let typeNode: ts.Node, explicitTypeNode: ts.Node;
        if (Array.isArray(node)) {
            typeNode = node[0];
            explicitTypeNode = node[1];
        } else {
            typeNode = node;
        }

        let type: Type, enumerable: Map<string, string | number>;
        switch (typeNode.kind) {
            case ts.SyntaxKind.ParenthesizedType:
                {
                    return this.convertToCSharpType([(<ts.ParenthesizedTypeNode>typeNode).type, explicitTypeNode], depth + 1);
                }
                break;
            case ts.SyntaxKind.ArrayType:
                {
                    let element = this.convertToCSharpType([(<ts.ArrayTypeNode>typeNode).elementType, explicitTypeNode], depth + 1);
                    if (element && element.type) {
                        type = csharp.System.Array.CreateInstance(element.type, 0).GetType();
                        enumerable = element.enumerable;
                    }
                }
                break;
            case ts.SyntaxKind.UnionType:
                {
                    let types: Array<number | string> = (<ts.UnionTypeNode>typeNode).types.map(t => {
                        if (t.kind === ts.SyntaxKind.LiteralType) {
                            return util.getExpressionValue(this.checker, (<ts.LiteralTypeNode>t).literal);
                        }
                        return null;
                    });
                    let ce = util.toCSharpEnumerable(types.map(value => ({ name: `${value}`, value })));
                    if (ce) {
                        enumerable = ce.enumerable;
                        type = ce.type;
                    }
                }
                break;
            case ts.SyntaxKind.EnumDeclaration:
                {
                    let initializerValue = 0;
                    let members = (<ts.EnumDeclaration>typeNode).members.map(m => {
                        let value = m.initializer ? util.getExpressionValue(this.checker, m.initializer) : initializerValue;
                        if (typeof (value) === "number") {
                            initializerValue = value + 1;
                        }
                        return {
                            name: m.name.getText(),
                            value: value
                        };
                    });
                    let ce = util.toCSharpEnumerable(members);
                    if (ce) {
                        enumerable = ce.enumerable;
                        type = ce.type;
                    }
                }
                break;
            default:
                {
                    let module = this.getModuleFromNode(explicitTypeNode ?? typeNode),
                        fullName = this.getFullName(explicitTypeNode ?? typeNode);
                    if (module === ModuleFlags.Global && fullName in this._baseTypes) {
                        type = this._baseTypes[fullName];
                    }
                    else if (module === ModuleFlags.CSharp || (module === ModuleFlags.Global || module === ModuleFlags.CS) && fullName.startsWith("CS.")) {
                        if (fullName.startsWith("CS.")) {
                            fullName = fullName.substring(3);
                        }
                        type = csharp.XOR.ReflectionUtil.GetType(fullName);
                    }
                    else if (typeNode.kind === ts.SyntaxKind.TypeReference) {
                        let declaration = this.getDeclarationByReference(<ts.TypeReferenceNode>typeNode);
                        if (declaration) {
                            return this.convertToCSharpType(declaration, depth + 1);
                        }
                        let trNode = <ts.TypeReferenceNode>typeNode;
                        if (trNode.typeName.getText() === "Array" && trNode.typeArguments.length === 1) {
                            let element = this.convertToCSharpType([(<ts.TypeReferenceNode>typeNode).typeArguments[0], explicitTypeNode], depth + 1);
                            if (element && element.type) {
                                type = csharp.System.Array.CreateInstance(element.type, 0).GetType();
                                enumerable = element.enumerable;
                            }
                            return { type, enumerable };
                        }
                    }
                }
                break;
        }
        return { type, enumerable };
    }
    //#endregion
}

/**访问修饰符: public/private/protected */
type AccessModifier = ts.SyntaxKind.PublicKeyword | ts.SyntaxKind.PrivateKeyword | ts.SyntaxKind.ProtectedKeyword;

type TypeDefinition = {
    /**为xor Component类型分配的GUID */
    guid?: string;
    /**为xor Component类型分配的唯一路由 */
    route?: string;

    readonly absoluteName: string;
    /**是否为xor Component类型 */
    readonly isComponent: boolean;
    /**是否有export标识符 */
    readonly isExport: boolean;
    /**是否有declare标识符(包括父节点) */
    readonly isDeclare: boolean;
    /**是否有abstract标识符 */
    readonly isAbstract: boolean;
}

const util = new class {
    public getAccessModifier(modifiers: ReadonlyArray<ts.ModifierLike>): AccessModifier {
        if (modifiers) {
            let m = modifiers.find(_m =>
                _m.kind === ts.SyntaxKind.PublicKeyword ||
                _m.kind === ts.SyntaxKind.PrivateKeyword ||
                _m.kind === ts.SyntaxKind.ProtectedKeyword
            );
            if (m) return m.kind as AccessModifier;
        }
        return ts.SyntaxKind.PublicKeyword;
    }

    public isDeclare(node: ts.Node, inherit?: boolean): boolean {
        if (!node)
            return false;
        if (ts.canHaveModifiers(node)) {
            let modifiers = ts.getModifiers(node);
            if (modifiers) {
                for (let modifier of modifiers) {
                    if (modifier.kind === ts.SyntaxKind.DeclareKeyword) {
                        return true;
                    }
                }
            }
        }
        return this.isDeclare(node.parent, inherit);
    }
    public isExport(node: ts.Node): boolean {
        if (!ts.canHaveModifiers(node))
            return false;
        let modifiers = ts.getModifiers(node);
        if (modifiers) {
            for (let modifier of modifiers) {
                if (modifier.kind === ts.SyntaxKind.ExportKeyword) {
                    return true;
                }
            }
        }
        return false;
    }
    public isAbstract(node: ts.Node): boolean {
        if (!ts.canHaveModifiers(node))
            return false;
        let modifiers = ts.getModifiers(node);
        if (modifiers) {
            for (let modifier of modifiers) {
                if (modifier.kind === ts.SyntaxKind.AbstractKeyword) {
                    return true;
                }
            }
        }
        return false;
    }
    public isPublic(node: ts.Node, defaultValue: boolean = true): boolean {
        if (!ts.canHaveModifiers(node))
            return false;
        let modifiers = ts.getModifiers(node);
        if (modifiers) {
            for (let modifier of modifiers) {
                switch (modifier.kind) {
                    case ts.SyntaxKind.PublicKeyword:
                        return true;
                        break;
                    case ts.SyntaxKind.PrivateKeyword:
                    case ts.SyntaxKind.ProtectedKeyword:
                        return false;
                        break;
                }
            }
        }
        return defaultValue;
    }
    public isPrivateOrProtected(node: ts.Node, defaultValue: boolean = true): boolean {
        return !this.isPublic(node, defaultValue);
    }

    public getExpressionValue(checker: ts.TypeChecker, expression: ts.Expression, depth: number = 0) {
        if (depth > 1)
            return null;
        let result: any;
        switch (expression.kind) {
            case ts.SyntaxKind.StringLiteral:
                result = (<ts.StringLiteral>expression).text;
                break;
            case ts.SyntaxKind.NumericLiteral:
                result = Number((<ts.NumericLiteral>expression).text);
                break;
            case ts.SyntaxKind.BigIntLiteral:
                result = BigInt((<ts.BigIntLiteral>expression).text);
                break;
            case ts.SyntaxKind.TrueKeyword:
                result = true;
                break;
            case ts.SyntaxKind.FalseKeyword:
                result = false;
                break;
            case ts.SyntaxKind.ArrayLiteralExpression:
                result = this.toCSharpArray(
                    (<ts.ArrayLiteralExpression>expression).elements.map(e => this.getExpressionValue(checker, e, depth + 1))
                );
                break;
            case ts.SyntaxKind.NewExpression:
                //TODO 构造对象
                //(<ts.NewExpression>expression);
                break;
            case ts.SyntaxKind.PropertyAccessExpression:
                //TODO 访问静态属性
                break;
        }
        return result;
    }
    public toCSharpArray(array: Array<any>) {
        let firstIndex = array?.findIndex(e => e !== undefined && e !== null && e !== void 0) ?? -1;
        if (firstIndex >= 0) {
            let result: csharp.System.Array, firstObj = array[firstIndex];
            switch (typeof (firstObj)) {
                case "string":
                    result = csharp.System.Array.CreateInstance($typeof(csharp.System.String), array.length);
                    break;
                case "number":
                    result = csharp.System.Array.CreateInstance($typeof(csharp.System.Double), array.length);
                    break;
                case "boolean":
                    result = csharp.System.Array.CreateInstance($typeof(csharp.System.Boolean), array.length);
                    break;
                case "bigint":
                    result = csharp.System.Array.CreateInstance($typeof(csharp.System.Int64), array.length);
                    break;
                case "object":
                    if (firstObj instanceof csharp.System.Object) {
                        result = csharp.System.Array.CreateInstance(firstObj.GetType(), array.length);
                    }
                    break;
            }
            if (result) {
                for (let i = 0; i < array.length; i++) {
                    result.SetValue(array[i], i);
                }
            }
            return result;
        }
        return null;
    }
    public toCSharpEnumerable(members: Array<{ name: string, value: string | number }>) {
        if (!members || members.length === 0 || !members.every(({ value: v }) => typeof (v) === "string" || typeof (v) === "number" || typeof (v) === "bigint")) {
            return null;
        }
        let type: Type, enumerable = new Map<string, string | number>();
        if (members.every(m => typeof (m.value) === "number")) {
            type = $typeof(csharp.System.Int32);
            members.forEach((({ name, value }) => enumerable.set(name, value)));
        } else {
            type = $typeof(csharp.System.String);
            members.forEach((({ name, value }) => enumerable.set(name, `${value}`)));
        }
        return { type, enumerable };
    }
}
class Throttler {
    private tick: number;
    private readonly frequency: number;
    private readonly duration: number;
    public constructor(frequency: number = 1, duration: number = 1) {
        this.tick = 0;
        this.duration = duration;
        this.frequency = frequency > 0 ? frequency : 1;
    }
    public async complete() {
        if ((++this.tick) % this.frequency === 0) {
            await new Promise<void>(resolve => setTimeout(resolve, this.duration));
        }
    }
}
