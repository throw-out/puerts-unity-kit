import * as csharp from "csharp";
import { $typeof } from "puerts";
import * as ts from "typescript";

import Type = csharp.System.Type;
const { Guid } = csharp.System;
const { HashUtil } = csharp.XOR;
const { File, Directory, Path } = csharp.System.IO;

const UTF8 = csharp.System.Text.Encoding.UTF8;
Object.setPrototypeOf(UTF8, csharp.System.Text.Encoding.prototype);

const EmptyCharacters = [" ", "\t"], EnterCharacters = ["\n", "\r"];

type ConfigFile = {
    readonly files?: string[];
    readonly include?: string[];
    readonly exclude?: string[];
    readonly references?: string[];
    readonly compilerOptions: ts.CompilerOptions;
};

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
class Timer {
    private _start: number;
    constructor() {
        this.reset();
    }
    public reset() {
        this._start = new Date().valueOf();
    }
    public duration() {
        return new Date().valueOf() - this._start;
    }
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
    private readonly sourceTypes: Map<string, ts.ClassDeclaration[]>;
    private readonly sourceDefinitions: Map<string, TypeDefinition[]>;

    private readonly throttler: Throttler;

    //是否正在进行解析中
    private resolving: boolean;
    private pending: Set<string>;

    constructor(cp: csharp.XOR.Services.Program, rootNames: string[], options: ts.CompilerOptions) {
        cp.state = csharp.XOR.Services.ProgramState.Compiling;

        let timer = new Timer();
        cp.compile = 'compiling';
        this.cp = cp;
        this.program = ts.createProgram({
            rootNames,
            options: {
                ...options,
                incremental: true,
                noEmit: true,
            }
        });
        this.checker = this.program.getTypeChecker();
        cp.compile = `${timer.duration()}ms`;
        cp.errors = ts.getPreEmitDiagnostics(this.program).length;

        this.types = new Map();
        this.sourceTypes = new Map();
        this.sourceDefinitions = new Map();

        this.throttler = new Throttler(10, 1);

        this.resolves();
    }
    private async resolves() {

        this.resolving = true;
        await this.resolveSources();
        await this.resolveDefinitions();

        this.cp.state = csharp.XOR.Services.ProgramState.Allocating;
        for (let [fileName, declarations] of this.sourceDefinitions) {
            await this.throttler.complete();
            this.pushDefinitions(declarations);
        }
        this.resolving = false;

        this.cp.state = csharp.XOR.Services.ProgramState.Completed;

        this.resolvePending();
    }
    /**文件修改状态: 新增丶更新或删除 */
    public change(files: string[]) {
        for (let file of files) {
            //console.log("file change: " + file);
            file = Path.GetFullPath(file).replace(/\\/g, "/");
            if (!this.pending) {
                this.pending = new Set()
            }
            this.pending.add(file);
        }
        this.resolvePending();
    }

    //#region SourceFile 处理流程
    /**
     * 解析SourceFiles
     */
    private resolveSources(): Promise<void>;
    private resolveSources(sync: true): void;
    private async resolveSources() {
        let sync: boolean = arguments[0];

        let sourceFiles = this.program.getSourceFiles();
        this.cp.errors = 0;
        this.cp.scripts = sourceFiles.length;

        this.cp.state = csharp.XOR.Services.ProgramState.Analyzing;
        //开始解析文件
        this.types.clear();
        this.sourceTypes.clear();
        for (const source of sourceFiles) {
            if (!source.statements)
                continue;
            if (sync) {
                this.resolveStatements(source.statements, true);
            } else {
                await this.resolveStatements(source.statements);
            }
        }
    }
    private resolveStatements(statements: ts.NodeArray<ts.Statement>): Promise<void>;
    private resolveStatements(statements: ts.NodeArray<ts.Statement>, sync: true): void;
    private async resolveStatements() {
        let statements: ts.NodeArray<ts.Statement> = arguments[0],
            sync: boolean = arguments[1];
        for (let statement of statements) {
            switch (statement.kind) {
                case ts.SyntaxKind.ClassDeclaration:
                    if (sync) {
                        this.resolveClassDeclaration(<ts.ClassDeclaration>statement, true);
                    } else {
                        await this.resolveClassDeclaration(<ts.ClassDeclaration>statement);
                    }
                    break;
                case ts.SyntaxKind.ModuleDeclaration:
                    if (sync) {
                        this.resolveModuleDeclaration(<ts.ModuleDeclaration>statement, true);
                    } else {
                        await this.resolveModuleDeclaration(<ts.ModuleDeclaration>statement);
                    }
                    break;
            }
        }
    }
    private resolveModuleDeclaration(node: ts.ModuleDeclaration): Promise<void>;
    private resolveModuleDeclaration(node: ts.ModuleDeclaration, sync: true): void;
    private async resolveModuleDeclaration() {
        let node: ts.ModuleDeclaration = arguments[0],
            sync: boolean = arguments[1];
        for (let childNode of node.getChildren()) {
            switch (childNode.kind) {
                case ts.SyntaxKind.ModuleBlock:
                    if (sync) {
                        this.resolveStatements((<ts.ModuleBlock>childNode).statements, sync)
                    } else {
                        await this.resolveStatements((<ts.ModuleBlock>childNode).statements)
                    }
                    break;
                case ts.SyntaxKind.Identifier:
                    break;
            }
        }
    }
    private resolveClassDeclaration(node: ts.ClassDeclaration): Promise<void>;
    private resolveClassDeclaration(node: ts.ClassDeclaration, sync: true): void;
    private async resolveClassDeclaration() {
        let node: ts.ClassDeclaration = arguments[0],
            sync: boolean = arguments[1];
        if (!sync) {
            await this.throttler.complete();
        }
        this.types.set(this.getAbsoluteName(node), node);

        let types = this.sourceTypes.get(node.getSourceFile().fileName);
        if (!types) {
            types = [];
            this.sourceTypes.set(node.getSourceFile().fileName, types);
        }
        types.push(node);
    }
    //#endregion


    //#region Component-Decorator定义处理流程
    private resolveDefinitions(): Promise<void>;
    private resolveDefinitions(files: string[]): void;
    private async resolveDefinitions() {
        let files: string[] = arguments[0];

        let resolve = (absoluteName: string, node: ts.ClassDeclaration) => {
            let definition: TypeDefinition = {
                absoluteName,
                isExport: util.isExport(node),
                isDeclare: util.isDeclare(node, true),
                isAbstract: util.isAbstract(node),
                isComponent: this.isInheritFromTsComponent(node) && !this.isTsComponent(node)
            };
            this.resolveDefinitionDecorator(node, definition);

            let file = node.getSourceFile().fileName;
            let definitions = this.sourceDefinitions.get(file);
            if (!definitions) {
                definitions = [];
                this.sourceDefinitions.set(file, definitions);
            }
            definitions.push(definition);
        };

        if (files && files.length > 0) {
            for (let file of files) {
                let types = this.sourceTypes.get(file);
                if (!types || types.length === 0)
                    continue;
                for (let node of types) {
                    resolve(this.getAbsoluteName(node), node);
                }
            }
        }
        else {
            this.sourceDefinitions.clear();
            for (let [absoluteName, node] of this.types) {
                await this.throttler.complete();
                resolve(absoluteName, node)
            }
        }
    }
    private resolveDefinitionDecorator(node: ts.ClassDeclaration, define: TypeDefinition) {
        if (!node.modifiers)
            return;
        for (let modifier of node.modifiers) {
            if (modifier.kind !== ts.SyntaxKind.Decorator)
                continue;
            let callExpression = (<ts.Decorator>modifier).expression;
            if (callExpression.kind !== ts.SyntaxKind.CallExpression)
                continue;

            let { expression: target, arguments: args } = <ts.CallExpression>callExpression;
            if (this.getModuleFromNode(target) !== ModuleFlags.Global || args.length === 0 || args[0].kind !== ts.SyntaxKind.StringLiteral)
                continue;
            switch (this.getFullName(target)) {
                case DecoratorFlags.GUID:
                    define.guid = this.getExpressionValue<string>(args[0]);
                    break;
                case DecoratorFlags.Route:
                    define.route = this.getExpressionValue<string>(args[0]);
                    break;
            }
        }
    }
    /**为目标类分配指定内容 */
    private allocDecorator(node: ts.Node, decoratorContent: string) {
        let sourceFile = node.getSourceFile();

        let content = sourceFile.text;
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

        let newSourceFile = ts.updateSourceFile(
            sourceFile,
            newContent,
            util.getTextChangeRange(sourceFile.text, newContent, true),
            false
        );
        return newSourceFile;
    }
    /**移除SourceFile所有定义 */
    private removeDeclarations(file: string) {
        let declarations = this.sourceDefinitions.get(file);
        //移除文件声明
        declarations?.forEach(declaration => {
            this.types.delete(declaration.absoluteName);
            this.cp.RemoveStatement(declaration.guid);
        });
        this.sourceTypes.delete(file);
        this.sourceDefinitions.delete(file);
    }
    /**更新了SourceFile文件, 重建TypeChecker对象
     * @param files 
     */
    private rebuildTypeChecker(...files: string[]) {
        let rootNames = [...this.program.getRootFileNames()];

        let updateFiles: string[] = [], change: number = 0;
        for (let file of files) {
            let oldSourceFile = this.program.getSourceFile(file);
            if (!oldSourceFile) {                       //new file
                rootNames.push(file);
                updateFiles.push(file);
                change++;
                continue;
            }
            let { exist, content: newContent, error } = util.tryReadFile(file);
            if (error) {
                console.warn(error);
                this.pending.add(file);
                continue;
            }
            if (!exist) {                               //delete file
                let idx = rootNames.indexOf(file);
                if (idx >= 0) {
                    rootNames.splice(idx, 1);
                    this.removeDeclarations(file);
                    change++;
                }
                continue;
            }
            if (newContent === oldSourceFile.text) {    //same content
                continue;
            }
            //更新文件内容
            this.removeDeclarations(file);
            updateFiles.push(file);
            change++;
            ts.updateSourceFile(
                oldSourceFile,
                newContent,
                util.getTextChangeRange(oldSourceFile.text, newContent),
                false
            );
        }
        if (change === 0 && files.length !== 0)
            return null;
        //重建TypeChecker对象
        let timer = new Timer();
        this.cp.compile = 'compiling';
        //@ts-ignore
        this.program = ts.createProgram({
            rootNames,
            options: this.program.getCompilerOptions(),
            oldProgram: this.program
        });
        //@ts-ignore
        this.checker = this.program.getTypeChecker();
        this.cp.compile = `${timer.duration()}ms`;
        this.cp.errors = ts.getPreEmitDiagnostics(this.program).length;

        //重新解析文件
        if (updateFiles.length > 0) {
            this.cp.state = csharp.XOR.Services.ProgramState.Analyzing;
            for (let file of updateFiles) {
                let sourceFile = this.program.getSourceFile(file);
                if (!sourceFile || !sourceFile.statements)
                    continue;
                this.resolveStatements(sourceFile.statements, true);
            }

            this.cp.state = csharp.XOR.Services.ProgramState.Completed;
        }
        return updateFiles;
    }
    //#endregion

    private async resolvePending() {
        if (this.resolving)
            return;
        if (!this.pending || this.pending.size === 0)
            return;

        this.resolving = true;

        let files = [...this.pending];
        this.pending.clear();
        let newFiles = this.rebuildTypeChecker(...files);
        if (newFiles && newFiles.length > 0) {
            //重新解析关系
            this.resolveDefinitions(newFiles);
            //重新推送定义
            for (let file of newFiles) {
                let declarations = this.sourceDefinitions.get(file);
                if (!declarations || declarations.length === 0)
                    continue;
                this.pushDefinitions(declarations);
            }
        }
        await new Promise<void>((resolve) => setTimeout(resolve, 1));

        this.resolving = false;
        this.resolvePending();
    }

    private pushDefinitions(declarations: TypeDefinition[]) {
        let newSourceFile: ts.SourceFile;
        //分配新的guid
        for (let declaration of declarations) {
            if (declaration.guid || !this.isExportTsComponent(declaration)) {
                continue;
            }
            let td = this.types.get(declaration.absoluteName);
            if (!td) {
                console.warn(`节点数据缺失: ${declaration.absoluteName}`);
                continue;
            }
            let guid = Guid.NewGuid().ToString();
            let _newSourceFile = this.allocDecorator(td, `@xor.guid("${guid}")`);
            if (_newSourceFile) {
                declaration.guid = guid;
                newSourceFile = _newSourceFile;
            }
        }
        //保存SourceFile文件
        if (newSourceFile) {
            File.WriteAllText(newSourceFile.fileName, newSourceFile.text);
            this.rebuildTypeChecker();
        }
        //推送定义到C#
        for (let declaration of declarations) {
            if (!declaration.guid || !this.isExportTsComponent(declaration)) {
                continue;
            }
            this.pushDefinition(declaration);
        }
    }
    private pushDefinition(declaration: TypeDefinition) {
        if (!declaration.guid || !this.isExportTsComponent(declaration)) {
            return;
        }

        let node = this.types.get(declaration.absoluteName);
        if (!node) {
            console.warn(`节点数据缺失: ${declaration.absoluteName}`);
            return;
        }
        let [module, name] = declaration.absoluteName.split("|");

        //创建type声明
        let ctd = new csharp.XOR.Services.TypeDeclaration();
        ctd.name = name;
        ctd.module = module;
        ctd.guid = declaration.guid;
        ctd.version = csharp.XOR.HashUtil.SHA256(node.getFullText());
        ctd.path = node.getSourceFile().fileName;
        ctd.line = util.getTextLine(node.getSourceFile().text, node.getStart());
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
    private isExportTsComponent(type: TypeDefinition) {
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
                                range = [this.getExpressionValue(min), this.getExpressionValue(max)];
                            }
                            break;
                        case "value":
                            value = this.getExpressionValue(initializer);
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
    private getDeclarationByReference(node: ts.TypeReferenceNode | ts.Expression) {
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

    /**解析ts.Expression并获取其值: 基础类型丶C#类型或其数组类型 */
    public getExpressionValue<T = any>(expression: ts.Expression, depth: number = 0): T {
        if (depth > 3)
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
                result = util.toCSharpArray(
                    (<ts.ArrayLiteralExpression>expression).elements.map(e => this.getExpressionValue(e, depth + 1))
                );
                break;
            case ts.SyntaxKind.NewExpression:               //构造对象
                result = this.newObjectIfCSharp(<ts.NewExpression>expression, depth + 1);
                break;
            case ts.SyntaxKind.PropertyAccessExpression:    //访问属性
                {
                    let propertyAccess = <ts.PropertyAccessExpression>expression,
                        propertyName = propertyAccess.name.getText();
                    let typeExpression = propertyAccess.expression;
                    //目标是否为C#类型?
                    let module = this.getModuleFromNode(typeExpression), fullName = this.getFullName(typeExpression);
                    if (util.isCSharpDeclare(module, fullName)) {
                        let val: any;
                        if (typeExpression.kind === ts.SyntaxKind.NewExpression) {
                            val = this.newObjectIfCSharp(<ts.NewExpression>typeExpression, depth + 1);
                            if (val) val = val[propertyName];
                        }
                        else {
                            val = csharp;
                            try {
                                [...util.getCSharpFullname(module, fullName).split("."), propertyName].forEach(name => {
                                    if (!val) return;
                                    val = val[name];
                                });
                            }
                            catch (e) {
                                console.warn(e);
                                val = null;
                            }
                        }
                        if (val instanceof csharp.System.Object || (typeof (val) in this._baseTypes)) {
                            result = val;
                        }
                    }
                    //目标为ts类型
                    else {
                        let typeDeclaration = this.getDeclarationByReference(typeExpression);
                        if (typeDeclaration) {
                            let isStatic = typeExpression.kind !== ts.SyntaxKind.NewExpression
                            result = this.getDeclarationMemberValue(typeDeclaration, [propertyName, isStatic], depth);
                        }
                    }
                }
                break;
        }
        return result;
    }
    /**获取声明成员值: 基础类型丶C#类型或其数组类型*/
    public getDeclarationMemberValue(declaration: ts.Declaration, member: string | [name: string, isStatic: boolean], depth: number = 0) {
        let memberName: string, isStatic: boolean;
        if (Array.isArray(member)) {
            memberName = member[0];
            isStatic = !!member[1];
        } else {
            memberName = member;
            isStatic = true;
        }

        let result: any;
        switch (declaration.kind) {
            case ts.SyntaxKind.ClassDeclaration:
                {
                    for (let member of (<ts.ClassDeclaration>declaration).members) {
                        if (member.kind === ts.SyntaxKind.PropertyDeclaration &&
                            (<ts.PropertyDeclaration>member).initializer &&
                            memberName == member.name.getText() && util.isStatic(member) === isStatic) {
                            result = this.getExpressionValue((<ts.PropertyDeclaration>member).initializer, depth);
                            break;
                        }
                    }
                    console.log(1);
                }
                break;
            case ts.SyntaxKind.EnumDeclaration:
                {
                    let initializerValue = 0;
                    for (let member of (<ts.EnumDeclaration>declaration).members) {
                        let value = member.initializer ? this.getExpressionValue(member.initializer, depth) : initializerValue;
                        if (memberName === member.name.getText()) {
                            result = value;
                            break;
                        }
                        if (typeof (value) === "number") {
                            initializerValue = value + 1;
                        }
                    }
                }
                break;
        }
        return result;
    }
    /**构建C#对象 */
    private newObjectIfCSharp(expression: ts.NewExpression, depth: number = 0) {
        let typeExpression = expression.expression;
        //目标是否为C#类型?
        let module = this.getModuleFromNode(typeExpression), fullName = this.getFullName(typeExpression);
        if (util.isCSharpDeclare(module, fullName)) {
            let val: any = csharp;
            util.getCSharpFullname(module, fullName).split(".").forEach(name => {
                if (!val) return;
                val = val[name];
            });
            if (val) {
                try {
                    return new val(...expression.arguments.map(o => this.getExpressionValue(o, depth)));
                } catch (e) {
                    console.warn(e);
                }
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
                            return this.getExpressionValue((<ts.LiteralTypeNode>t).literal);
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
                        let value = m.initializer ? this.getExpressionValue(m.initializer) : initializerValue;
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
                    else if (util.isCSharpDeclare(module, fullName)) {
                        type = csharp.XOR.ReflectionUtil.GetType(util.getCSharpFullname(module, fullName));
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
    public isStatic(node: ts.Node): boolean {
        if (!ts.canHaveModifiers(node))
            return false;
        let modifiers = ts.getModifiers(node);
        if (modifiers) {
            for (let modifier of modifiers) {
                switch (modifier.kind) {
                    case ts.SyntaxKind.StaticKeyword:
                        return true;
                        break;
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

    /**比对源文件, 并获取TextChangeRange */
    public getTextChangeRange(content: string, newContent: string, forwardRemoveEmpty: boolean = true): ts.TextChangeRange {
        let contentLength = content.length,
            newContentLength = newContent.length;

        let start: number = 0, end: number = -1;
        //find start
        for (let i = 0; i < contentLength; i++) {
            if (content[i] !== newContent[i])
                break;
            start = i;
        }
        if (forwardRemoveEmpty) {
            for (let i = start; i >= 0; i--) {
                start = i;
                if (!EmptyCharacters.includes(content[i]))
                    break;
            }
        }
        //find end
        for (let i = 0; i < contentLength; i++) {
            let index1 = contentLength - i - 1, index2 = newContentLength - i - 1;
            if (content[contentLength - i - 1] !== newContent[index2] || index1 < start)
                break;
            end = index1;
        }

        let spanLength: number, newLength: number;
        if (end < 0) {
            spanLength = contentLength - start;
            newLength = newContentLength - start;
        } else {
            spanLength = end - start;
            newLength = end - start + newContentLength - contentLength;
        }
        return {
            span: {
                start,
                length: spanLength
            },
            newLength: newLength
        };
    }
    /**获取字符位置其对应的行数 */
    public getTextLine(content: string, charIndex: number): number {
        let line = 0;
        for (let i = 0; i < content.length && i < charIndex; i++) {
            if (content[i] === "\n") {
                line++;
            }
        }
        return line;
    }

    /**是否为C#类型声明 */
    public isCSharpDeclare(module: string, fullName: string) {
        return module === ModuleFlags.CSharp ||
            (module === ModuleFlags.Global || module === ModuleFlags.CS) && fullName.startsWith("CS.");
    }
    /**获取C#类型声明 */
    public getCSharpFullname(module: string, fullName: string) {
        if (fullName.startsWith("CS.") && module !== ModuleFlags.CSharp) {
            fullName = fullName.substring(3);
        }
        return fullName;
    }
    /**转为C#数组类型, 长度必需大于1且至少有一个非null成员, 成员类型必需一致 */
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
    /**转为enumerable数据 */
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

    /**读取文件
     * @param path 
     * @returns 
     */
    public tryReadFile(path: string) {
        let exist: boolean, content: string, error: Error;
        try {
            exist = File.Exists(path);
            if (exist) {
                content = File.ReadAllText(path);
            }
        } catch (e) {
            error = e;
        }
        return { exist, content, error }
    }
    /**取IterableIterator<T>下表 */
    public at<T>(iterator: IterableIterator<T>, index: number) {
        if (!iterator)
            return null;
        let i = 0;
        for (let value of iterator) {
            if (i === index)
                return value;
            i++;
        }
        return null;
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
