import * as csharp from "csharp";
import * as ts from "typescript";

import Type = csharp.System.Type;
const { File, Directory, Path } = csharp.System.IO;

type ConfigFile = {
    readonly files?: string[];
    readonly include?: string[];
    readonly exclude?: string[];
    readonly references?: string[];
    readonly compilerOptions: ts.CompilerOptions;
};

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

        this.resolves();
    }
    private async resolves() {
        await this.resolveSources();
        await this.resolveComponents();
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
            await new Promise<void>(resolve => setTimeout(resolve, 10));
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
        this.types.set(this.getAbsoluteName(node), node);
        if (this.types.size % 100 === 0) {
            await new Promise<void>(resolve => setTimeout(resolve, 1));
        }
        if (!util.isExport(node) || util.isAbstract(node))
            return;
    }
    private async resolveModuleDeclaration(node: ts.ModuleDeclaration) {
        let sourceFile = node.getSourceFile().fileName;
        let childrens = node.getChildren();
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
            let definition: TypeDefinition = {
                isExport: util.isExport(cd),
                isDeclare: util.isDeclare(cd, true),
                isAbstract: util.isAbstract(cd),
                isComponent: this.isInheritFromTsComponent(cd) && !this.isTsComponent(cd)
            };
            this.resolveComponentDecorator(cd, definition);
            this.typeDefinitions.set(absoluteName, definition);

            if (definition.isComponent) {

            }
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
            let fullName = this.getFullName(target), module = this.getModuleFromNode(target);
            if (module !== "global")
                continue;
            switch (fullName) {
                case "xor.guid":
                    define.guid = (<ts.StringLiteral>args[0]).text;
                    break;
                case "xor.route":
                    define.route = (<ts.StringLiteral>args[0]).text;
                    break;
            }
        }
    }
    //#endregion


    private isTsComponent(node: ts.ClassDeclaration) {
        return this.getFullName(node) === "xor.TsComponent" &&
            this.getModuleFromNode(node) === "global";
    }
    private isInheritFromTsComponent(node: ts.ClassDeclaration) {
        if (this.isTsComponent(node)) {
            return true;
        }
        //检测继承类
        if (node.heritageClauses) {
            for (let clause of node.heritageClauses ?? []) {
                for (let ewta of clause.types) {
                    let cd = this.types.get(this.getAbsoluteName(ewta.expression));
                    if (cd) {
                        if (this.isInheritFromTsComponent(cd))
                            return true;
                    } else {
                        //console.warn(`无效的继承对象:${this.getAbsoluteName(ewta.expression)}`);
                    }
                }
            }
        }
        return false;
    }
    private getCharpType(node: ts.Node, underlyingType?: Type) {
        let module = this.getModuleFromNode(node),
            fullName = this.getFullName(node);
        if (module === "global") {
            if (!fullName.startsWith("CS."))
                return null;
            fullName = fullName.substring(3);
        } else if (module !== "csharp") {
            return null;
        }
        let type = Type.GetType(fullName, false);
        if (type && underlyingType && underlyingType.IsAssignableFrom(type)) {
            type = underlyingType;
        }
        return type;
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
        return "global";
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
    //#endregion
}

/**访问修饰符: public/private/protected */
type AccessModifier = ts.SyntaxKind.PublicKeyword | ts.SyntaxKind.PrivateKeyword | ts.SyntaxKind.ProtectedKeyword;

type TypeDefinition = {
    /**为xor Component类型分配的GUID */
    guid?: string;
    /**为xor Component类型分配的唯一路由 */
    route?: string;
    /**文件版本(以lastWriteTime) */
    version?: string;
    /**文件哈希 */
    hash?: string;

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

    public isDeclare(node: ts.Node, inherit?: boolean) {
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
    public isExport(node: ts.Node) {
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
    public isAbstract(node: ts.Node) {
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
    public isPublic(node: ts.Node, defaultValue: boolean = true) {
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
    public isPrivateOrProtected(node: ts.Node, defaultValue: boolean = true) {
        return !this.isPublic(node, defaultValue);
    }
}