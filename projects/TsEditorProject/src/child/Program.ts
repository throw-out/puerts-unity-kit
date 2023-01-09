import * as csharp from "csharp";
import * as json5 from "json5";
import * as ts from "typescript";

const { File, Directory, Path } = csharp.System.IO;

export namespace XOR {
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
            let files = scanFiles(normal("./type"), [".ts"]);
            files?.forEach(p => results.add(p));

            files = scanFiles(normal("./src"), [".ts"]);
            files?.forEach(p => results.add(p));
        }
        if (cfg.exclude) {

        }
        return [...results];
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
    }

    class ClassDeclaration {
        constructor(
            public readonly name: string,
            public readonly type: TypeDeclaration,
            public readonly properties: PropertyDeclaration[],
        ) { }

        public static from(node: ts.ClassDeclaration, checker: ts.TypeChecker) {
            let properties: PropertyDeclaration[] = [];


        }
    }
    class PropertyDeclaration {
        constructor(
            public readonly name: string,
            public readonly type: TypeDeclaration,
            public readonly accessModifier: AccessModifier,
        ) { }

        public static from(node: ts.PropertyDeclaration, checker: ts.TypeChecker) {
            let name = node.name.getText();
            let type = TypeDeclaration.from(node.type, checker);
            let am = util.getAccessModifier(node.modifiers);

            return new PropertyDeclaration(
                name,
                type,
                am
            );
        }
    }
    class TypeDeclaration {
        constructor(
            public readonly name: string,
            public readonly fullName: string,
            public readonly module: ModuleDeclaration,
        ) { }

        public static from(node: ts.TypeNode, checker: ts.TypeChecker) {
            let name: string, fullName: string, module: ModuleDeclaration = ModuleDeclaration.NONE;

            const type = checker.getTypeAtLocation(node);
            const symbol = type.getSymbol();
            if (symbol) {
                name = checker.symbolToString(
                    symbol,
                    node,
                );
                fullName = checker.symbolToString(
                    symbol,
                    node,
                    ts.SymbolFlags.PropertyOrAccessor | ts.SymbolFlags.ClassMember,
                    ts.SymbolFormatFlags.WriteTypeParametersOrArguments |
                    ts.SymbolFormatFlags.UseOnlyExternalAliasing |
                    ts.SymbolFormatFlags.AllowAnyNodeKind |
                    ts.SymbolFormatFlags.UseAliasDefinedOutsideCurrentScope
                );
                let declarations = symbol.getDeclarations();
                if (declarations && declarations.length > 0) {
                    module = ModuleDeclaration.from(
                        declarations.find(o => o.kind === ts.SyntaxKind.ClassDeclaration) ??
                        declarations.find(o => o.kind === ts.SyntaxKind.InterfaceDeclaration)
                    )
                }
            }
            else {
                name = checker.typeToString(
                    type,
                    node,
                    ts.TypeFormatFlags.NodeBuilderFlagsMask
                );
                fullName = name;
            }
            return new TypeDeclaration(name, fullName, module);
        }
    }
    class ModuleDeclaration {
        public static readonly NONE = new ModuleDeclaration(null);

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

        public static from(node: ts.Declaration) {
            let _node: ts.Node = node,
                modulePath = new Array<string>();

            while (_node) {
                if (_node.kind === ts.SyntaxKind.ModuleDeclaration) {
                    modulePath.unshift((<ts.ModuleDeclaration>_node).name.getText());
                }
                _node = _node.parent;
            }
            return new ModuleDeclaration(modulePath);
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
                .map(p => {
                    let { name, type } = PropertyDeclaration.from(p, this.checker);
                    return `${name}: ${type.fullName}(${type.module.fullName})`;
                })
            console.log(memberProps.join("\n"));
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