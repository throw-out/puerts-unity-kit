import * as csharp from "csharp";
import * as json5 from "json5";
import ts from "typescript";

const { File, Directory, Path } = csharp.System.IO;

namespace XOR {
    type Tsconfig = {
        readonly files?: string[];
        readonly include?: string[];
        readonly exclude?: string[];
        readonly compilerOptions: ts.CompilerOptions;
    };

    function scanFiles(dirpath: string, extensions?: readonly string[]) {
        let results = new Array<string>();

        let files = Directory.GetFiles(dirpath);
        for (let i = 0; i < files.Length; i++) {
            let file = files.get_Item(i);
            if (extensions && !extensions.find(e => file.endsWith(e)))
                continue;
            results.push(file);
        }
        let dirs = Directory.GetDirectories(dirpath);
        for (let i = 0; i < dirs.Length; i++) {
            let subfiles = scanFiles(dirs.get_Item(i));
            if (subfiles.length > 0) results.push(...subfiles);
        }

        return results;
    }
    function readTsconfig(path: string): Tsconfig {
        return json5.parse(File.ReadAllText(path));
    }
    function scanTsconfigFiles(root: string, cfg: Tsconfig) {
        if (File.Exists(root)) root = Path.GetDirectoryName(root);
        let normal = (path: string) => {
            if (path.startsWith(".") || path.startsWith(".."))
                return Path.GetFullPath(Path.Combine(root, path));
            return path;
        }

        const results = new Set<string>();
        if (cfg.files) {
            cfg.files.forEach(p => results.add(normal(p)));
        }
        if (cfg.include) {
            let files = scanFiles(normal("./typeing"), [".ts"]);
            files?.forEach(p => results.add(p));

            files = scanFiles(normal("./src"), [".ts"]);
            files?.forEach(p => results.add(p));
        }
        if (cfg.exclude) {

        }
        return [...results];
    }

    class ClassDeclaration {

    }
    class TypeDeclaration {
        public readonly name: string;
        public readonly fullName: string;
        public readonly module: ModuleDeclaration;
    }
    class ModuleDeclaration {
        /**is declare in global */
        public readonly global: boolean;
        public readonly name: string;
        public readonly fullName: string;
        public readonly path: string[];

        constructor(modulePath: string[]) {
            this.global = !modulePath || modulePath.length == 0 || !!modulePath.find(m => m === "global");
            this.path = modulePath;
            if (!this.global) {
                this.name = modulePath[0];
                this.fullName = modulePath.join(".");
            }
        }
    }


    export class Program {
        private readonly program: ts.Program;
        private readonly checker: ts.TypeChecker;

        constructor(tsconfigPath: string) {
            const timer = new Timer();

            let cfg = readTsconfig(tsconfigPath);
            let rootNames = scanTsconfigFiles(tsconfigPath, cfg);
            console.log("==================================================");
            console.log(`scan duration ${timer.duration()}ms, total ${rootNames.length} files:\n${rootNames.join("\n")}`);
            timer.reset();

            this.program = ts.createProgram({
                rootNames,
                options: {
                    target: cfg.compilerOptions.target,
                    module: cfg.compilerOptions.module,
                    noEmit: true,
                }
            });
            this.checker = this.program.getTypeChecker();

            console.log(`program parse duration ${timer.duration()}ms`);
        }

        public async print(filter?: (statement: ts.Statement) => boolean) {
            for (const source of this.program.getSourceFiles()) {
                if (!source.statements)
                    continue;
                await new Promise<void>(resolve => setTimeout(resolve, 1));

                for (let statement of source.statements) {
                    if (filter && !filter(statement))
                        continue;

                    switch (statement.kind) {
                        case ts.SyntaxKind.ClassDeclaration:
                            this.printClassDeclaration(<ts.ClassDeclaration>statement);
                            break;
                        default:
                            console.warn(`Unknown statement kind: ts.ScriptKind.${ts.ScriptKind[statement.kind]}(${statement.kind})`);
                            break;
                    }
                }
            }
        }
        public printClassDeclaration(classDeclaration: ts.ClassDeclaration) {
            console.log("==================================================");
            console.log(classDeclaration.name.getText() + "\n" + classDeclaration.getSourceFile().fileName);
            if (!classDeclaration.members)
                return;

            let memberProps = classDeclaration.members
                .filter(m => m.kind === ts.SyntaxKind.PropertyDeclaration)
                .map(m => <ts.PropertyDeclaration>m)
                .map(p => `${p.name.getText()}: ${this.typeNodeToString(p.type)}`)
            console.log(memberProps.join("\n"));
        }

        private typeNodeToString(node: ts.Node) {
            const type = this.checker.getTypeAtLocation(node);
            if (type.getSymbol()) {
                return this.symbolToString(node, type.getSymbol());
            } else {
                return this.typeToString(node, type);
            }
        }
        private typeToString(node: ts.Node, type: ts.Type) {
            let stringBuilder = new Array<string>();
            stringBuilder.push(this.checker.typeToString(
                type,
                node,
                ts.TypeFormatFlags.NodeBuilderFlagsMask
            ));
            stringBuilder.push(`type`);
            return stringBuilder.join(" | ")
        }
        private symbolToString(node: ts.Node, symbol: ts.Symbol) {
            let stringBuilder = new Array<string>();
            stringBuilder.push(this.checker.symbolToString(
                symbol,
                node,
                ts.SymbolFlags.PropertyOrAccessor | ts.SymbolFlags.ClassMember,
                ts.SymbolFormatFlags.WriteTypeParametersOrArguments |
                ts.SymbolFormatFlags.UseOnlyExternalAliasing |
                ts.SymbolFormatFlags.AllowAnyNodeKind |
                ts.SymbolFormatFlags.UseAliasDefinedOutsideCurrentScope
            ));
            stringBuilder.push(`symbol`);

            let declarations = symbol.getDeclarations();
            if (declarations && declarations.length > 0) {
                let typeDeclaration = (declarations.find(o => o.kind === ts.SyntaxKind.ClassDeclaration) ??
                    declarations.find(o => o.kind === ts.SyntaxKind.InterfaceDeclaration)) as ts.ClassDeclaration | ts.InterfaceDeclaration;

                let moduleName = this.getModuleByTypeDeclaration(typeDeclaration);
                if (moduleName) {
                    stringBuilder.push(moduleName);
                }
            }
            return stringBuilder.join(" | ");
        }

        private getModuleByTypeDeclaration(declaration: ts.Declaration) {
            let node: ts.Node = declaration, moduleName: string = "";
            while (node) {
                if (node.kind === ts.SyntaxKind.ModuleDeclaration) {
                    if (moduleName) moduleName = "." + moduleName;

                    moduleName = (<ts.ModuleDeclaration>node).name.getText() + moduleName;
                }
                node = node.parent;
            }
            return moduleName;
        }
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
}
setTimeout(() => {
    let tsconfigPath = Path.GetFullPath(Path.Combine(
        Path.GetDirectoryName(csharp.UnityEngine.Application.dataPath),
        "TsProject/tsconfig.json"
    ));
    let program = new XOR.Program(tsconfigPath);
    //program.print();
    program.print(statement =>
        statement.kind === ts.SyntaxKind.ClassDeclaration &&
        (<ts.ClassDeclaration>statement).name.getText().includes("AnalyzeTest")
    );
}, 2000);