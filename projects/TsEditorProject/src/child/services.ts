import * as csharp from "csharp";
import * as ts from "typescript";

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
 * @param maxDepth 
 * @returns 
 */
export function parseConfigFile(tsconfigFile: string, maxDepth: number = 10): ts.ParsedCommandLine {
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

    const readDirectory = (rootDir: string, extensions: string[], excludes: string[], includes: string[], depth?: number) => {
        if (depth && depth > maxDepth)
            return null;
        const results = new Array<string>();

        let files = Directory.GetFiles(rootDir);
        for (let i = 0; i < files.Length; i++) {
            let file = files.get_Item(i);
            if (extensions && !extensions.find(ext => file.endsWith(ext))) {
                continue;
            }
            results.push(file);
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

    private readonly classes: Map<string, ts.ClassDeclaration>;


    constructor(cp: csharp.XOR.Services.Program, rootNames: string[], options: ts.CompilerOptions) {
        cp.state = csharp.XOR.Services.ProgramState.Compiling;

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
        this.classes = new Map();

        cp.state = csharp.XOR.Services.ProgramState.Compiled;

        this.resolveSources().then(() => this.test());
    }
    public getChecker() { return this.checker; }


    private async resolveSources() {
        let sourceFiles = this.program.getSourceFiles();
        this.cp.errors = 0;
        this.cp.scripts = sourceFiles.length;

        //开始解析文件
        for (const source of sourceFiles) {
            if (!source.statements)
                continue;
            await new Promise<void>(resolve => setTimeout(resolve, 10));

            this.resolveStatements(source.statements);
        }
    }
    private resolveStatements(statements: ts.NodeArray<ts.Statement>) {
        for (let statement of statements) {
            switch (statement.kind) {
                case ts.SyntaxKind.ClassDeclaration:
                    this.resolveClassDeclaration(<ts.ClassDeclaration>statement);
                    break;
                case ts.SyntaxKind.ModuleDeclaration:
                    this.resolveModuleDeclaration(<ts.ModuleDeclaration>statement);
                    break;
            }
        }
    }
    private resolveClassDeclaration(node: ts.ClassDeclaration) {
        this.classes.set(
            util.getAbsoluteName(node, this.checker),
            node
        );
        if (!util.hasExport(node) || util.isAbstract(node))
            return;
    }
    private resolveModuleDeclaration(node: ts.ModuleDeclaration) {
        let sourceFile = node.getSourceFile().fileName;
        let childrens = node.getChildren();
        for (let childNode of node.getChildren()) {
            switch (childNode.kind) {
                case ts.SyntaxKind.ModuleBlock:
                    this.resolveStatements((<ts.ModuleBlock>childNode).statements);
                    break;
                case ts.SyntaxKind.Identifier:
                    break;
            }
        }
    }

    public isInheritFromTsComponent(node: ts.ClassDeclaration) {
        if (util.getFullName(node, this.checker) === "xor.TsComponent" &&
            util.getModuleFromNode(node, this.checker) === "global"
        ) {
            return false;
        }
        //检测继承类
        if (node.heritageClauses) {
            for (let clause of node.heritageClauses ?? []) {
                for (let ewta of clause.types) {
                    let cd = this.classes.get(util.getAbsoluteName(ewta.expression, this.checker));
                    if (cd) {
                        if (this.isInheritFromTsComponent(cd))
                            return true;
                    } else {
                        console.warn(`无效的继承对象:${util.getAbsoluteName(ewta.expression, this.checker)}`);
                    }
                }
            }
        }
        return false;
    }

    private test() {
        for (let [absoluteName, cd] of this.classes) {
            if (absoluteName.includes("AnalyzeTest6")) {
                let a = this.isInheritFromTsComponent(cd);
                console.log(a);
            }
        }
    }
}

/**访问修饰符: public/private/protected */
type AccessModifier = ts.SyntaxKind.PublicKeyword | ts.SyntaxKind.PrivateKeyword | ts.SyntaxKind.ProtectedKeyword;

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

    public isGlobal(node: ts.Node) {

    }
    public hasExport(node: ts.Node) {
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

    public getAbsoluteName(node: ts.Node, checker: ts.TypeChecker) {
        let module = util.getModuleFromNode(node, checker);
        if (module.includes("CS") || module.includes("global")) {
            debugger;
        }
        let fullName = util.getFullName(node, checker);
        return `${module}|${fullName}`;
    }
    public getFullName(node: ts.Node, checker: ts.TypeChecker) {
        const type = checker.getTypeAtLocation(node);
        const symbol = type.getSymbol();
        if (symbol) {
            const flags = ts.SymbolFlags.Class |
                ts.SymbolFlags.Interface |
                ts.SymbolFlags.Enum |
                ts.SymbolFlags.Type |
                ts.SymbolFlags.Namespace |
                ts.SymbolFlags.Module |
                ts.SymbolFlags.PropertyOrAccessor |
                ts.SymbolFlags.ClassMember;
            const formatFlags = ts.SymbolFormatFlags.WriteTypeParametersOrArguments |
                ts.SymbolFormatFlags.UseOnlyExternalAliasing |
                ts.SymbolFormatFlags.AllowAnyNodeKind |
                ts.SymbolFormatFlags.UseAliasDefinedOutsideCurrentScope;
            return checker.symbolToString(
                symbol,
                node,
                flags,
                formatFlags
            );
        }
        else {
            return checker.typeToString(
                type,
                node,
                ts.TypeFormatFlags.NodeBuilderFlagsMask
            )
        }
    }
    public getModuleFromNode(node: ts.Node, checker: ts.TypeChecker) {
        const type = checker.getTypeAtLocation(node);
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
    }
    public getModuleFromDeclaration(declaration: ts.Declaration) {
        let moduleName: string;

        let node: ts.Node = declaration;
        while (node) {
            if (node.kind === ts.SyntaxKind.ModuleDeclaration) {
                moduleName = (<ts.ModuleDeclaration>node).name.text;
            }
            node = node.parent;
        }
        if (moduleName) {
            return moduleName;
        }
        return declaration.getSourceFile().fileName;
    }
}
