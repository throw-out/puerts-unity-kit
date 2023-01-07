import * as cs from "csharp";
import ts from "typescript";

export namespace Analyze {
    abstract class IMeta {
        protected abstract assertion(): void;

        public toJSON() {
            let result = Object.assign({}, this);
            Object.keys(result).forEach(key => {
                if (key.startsWith('_') || Array.isArray(result[key]) && result[key].length === 0) {
                    delete result[key];
                }
            });
            return result;
        }
    }


    export class File extends IMeta {
        public name: string;
        public modules: Module[] = [];
        public imports: Import[] = [];
        public importReferences: Import[] = [];
        public exports: Export[] = [];
        public exportAssignments: Export[] = [];
        public variables: Variable[] = [];
        public typeVariables: TypeVariable[] = [];
        public classes: Class[] = [];

        private _source: ts.SourceFile;

        constructor(source: ts.SourceFile) {
            super();
            this._source = source;
            this.assertion();
        }
        protected assertion(): void {
            this.name = this._source.fileName;
            this._source.statements?.forEach(child => {
                switch (child.kind) {
                    case ts.SyntaxKind.ModuleDeclaration:
                        this.modules.push(
                            new Module(this._source, <ts.ModuleDeclaration>child)
                        );
                        break;
                    case ts.SyntaxKind.ImportDeclaration:
                        this.imports.push(
                            new Import(this._source, <ts.ImportDeclaration>child)
                        );
                        break;
                    case ts.SyntaxKind.ImportEqualsDeclaration:
                        this.importReferences.push(
                            new Import(this._source, <ts.ImportDeclaration>child)
                        );
                        break;
                    case ts.SyntaxKind.ExportDeclaration:
                        this.exports.push(
                            new Export(this._source, <ts.ExportDeclaration>child)
                        );
                        break;
                    case ts.SyntaxKind.ExportAssignment:
                        this.exportAssignments.push(
                            new Export(this._source, <ts.ExportAssignment>child)
                        );
                        break;
                    case ts.SyntaxKind.TypeAliasDeclaration:
                        this.typeVariables.push(
                            new TypeVariable(this._source, <ts.TypeAliasDeclaration>child)
                        );
                        break;
                    case ts.SyntaxKind.FirstStatement:
                        (<ts.VariableStatement>child).declarationList.declarations.forEach(variableNode => {
                            this.variables.push(new Variable(this._source, variableNode));
                        });
                        break;
                    case ts.SyntaxKind.ClassDeclaration:
                        this.classes.push(
                            new Class(this._source, <ts.ClassDeclaration>child)
                        );
                        break;
                }
            });
        }
    }
    export class Module extends IMeta {
        public name: string;
        public imports: Import[] = [];
        public importReferences: Import[] = [];
        public exports: Export[] = [];
        public exportAssignments: Export[] = [];
        public variables: Variable[] = [];
        public typeVariables: TypeVariable[] = [];
        public classes: Class[] = [];

        private _source: ts.SourceFile;
        private _node: ts.ModuleDeclaration;

        constructor(source: ts.SourceFile, node: ts.ModuleDeclaration) {
            super();
            this._source = source;
            this._node = node;
            this.assertion();
        }

        protected assertion() {
            let moduleName = this._node.name.getText(this._source);
            this.name = moduleName.slice(1, moduleName.length - 1);

            const body = this._node.body;
            switch (body.kind) {
                case ts.SyntaxKind.ModuleBlock:
                    body.statements?.forEach(child => {
                        switch (child.kind) {
                            case ts.SyntaxKind.ImportDeclaration:
                                this.imports.push(
                                    new Import(this._source, <ts.ImportDeclaration>child)
                                );
                                break;
                            case ts.SyntaxKind.ImportEqualsDeclaration:
                                this.importReferences.push(
                                    new Import(this._source, <ts.ImportDeclaration>child)
                                );
                                break;
                            case ts.SyntaxKind.ExportDeclaration:
                                this.exports.push(
                                    new Export(this._source, <ts.ExportDeclaration>child)
                                );
                                break;
                            case ts.SyntaxKind.ExportAssignment:
                                this.exportAssignments.push(
                                    new Export(this._source, <ts.ExportAssignment>child)
                                );
                                break;
                            case ts.SyntaxKind.TypeAliasDeclaration:
                                this.typeVariables.push(
                                    new TypeVariable(this._source, <ts.TypeAliasDeclaration>child)
                                );
                                break;
                            case ts.SyntaxKind.FirstStatement:
                                (<ts.VariableStatement>child).declarationList.declarations.forEach(variableNode => {
                                    this.variables.push(new Variable(this._source, variableNode));
                                });
                                break;
                            case ts.SyntaxKind.ClassDeclaration:
                                this.classes.push(
                                    new Class(this._source, <ts.ClassDeclaration>child)
                                );
                                break;
                        }
                    });
                    break;
            }
        }
    }
    export class Import extends IMeta {
        public name: string;
        public module: string;
        public clause: string[] = [];
        public clauseAlias: { [name: string]: string } = {};

        private _source: ts.SourceFile;
        private _node: ts.ImportDeclaration | ts.ImportEqualsDeclaration;

        constructor(source: ts.SourceFile, node: ts.ImportDeclaration | ts.ImportEqualsDeclaration) {
            super();
            this._source = source;
            this._node = node;
            this.assertion();
        }
        protected assertion() {
            switch (this._node.kind) {
                case ts.SyntaxKind.ImportDeclaration:
                    {
                        let moudleName = this._node.moduleSpecifier.getText(this._source);
                        this.module = moudleName.slice(1, moudleName.length - 1);
                        if (this._node.importClause && this._node.importClause.namedBindings) {
                            let bindings = this._node.importClause.namedBindings;
                            switch (bindings.kind) {
                                case ts.SyntaxKind.NamespaceImport:
                                    {
                                        let _node = <ts.NamespaceImport>bindings;
                                        this.name = _node.name.getText(this._source);
                                    }
                                    break;
                                case ts.SyntaxKind.NamedImports:
                                    {
                                        let _node = <ts.NamedImports>bindings;
                                        _node.elements.forEach(element => {
                                            let name = element.name.getText(this._source);
                                            this.clause.push(name);
                                            if (element.propertyName) {
                                                this.clauseAlias[element.propertyName.getText(this._source)] = name;
                                            } else {
                                                //this.clauseAlias[name] = name;
                                            }
                                        });
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                case ts.SyntaxKind.ImportEqualsDeclaration:
                    {
                        this.name = this._node.name.getText(this._source);
                        this.module = getFullName(<ts.QualifiedName>this._node.moduleReference, this._source);
                    }
                    break;
            }

        }
    }
    export class Export extends IMeta {
        public assignment: string;
        public assert: Array<{ name: string, value: string }> = [];
        public typeOnly: boolean = false;

        private _source: ts.SourceFile;
        private _node: ts.ExportDeclaration | ts.ExportAssignment;

        constructor(source: ts.SourceFile, node: ts.ExportDeclaration | ts.ExportAssignment) {
            super();
            this._source = source;
            this._node = node;
            this.assertion();
        }
        protected assertion() {
            if (this._node.kind === ts.SyntaxKind.ExportDeclaration) {
                this._node.assertClause?.elements.forEach(child => {
                    this.assert.push({
                        name: getFullName(child.name, this._source),
                        value: getFullName(child.value, this._source),
                    })
                });
            } else {
                this.assignment = getFullName(this._node.expression, this._source);
            }
        }
    }
    export class Variable extends IMeta {
        public name: string | string[];
        public type: string;

        private _source: ts.SourceFile;
        private _node: ts.VariableDeclaration

        constructor(source: ts.SourceFile, node: ts.VariableDeclaration) {
            super();
            this._source = source;
            this._node = node;
            this.assertion();
        }
        protected assertion() {
            let { name } = this._node;
            switch (name.kind) {
                case ts.SyntaxKind.Identifier:
                    this.name = getFullName(<ts.Identifier>name, this._source);
                    break;
                case ts.SyntaxKind.ObjectBindingPattern:
                    this.name = (<ts.ObjectBindingPattern>name).elements
                        .map(o => getFullName(<ts.BindingElement>o, this._source));
                    break;
                case ts.SyntaxKind.ArrayBindingPattern:
                    this.name = (<ts.ArrayBindingPattern>name).elements
                        .filter(o => o.kind === ts.SyntaxKind.BindingElement)
                        .map(o => getFullName(<ts.BindingElement>o, this._source));
                    break;
            }
            this.type = getFullName(this._node.initializer, this._source);
            this._node.initializer;
        }
    }
    export class TypeVariable extends IMeta {
        public name: string;
        public type: string;

        private _source: ts.SourceFile;
        private _node: ts.TypeAliasDeclaration

        constructor(source: ts.SourceFile, node: ts.TypeAliasDeclaration) {
            super();
            this._source = source;
            this._node = node;
            this.assertion();
        }
        protected assertion() {
            this.name = this._node.name.getText(this._source);
            this.type = this.getTypeName(this._node.type);

        }
        private getTypeName(node: ts.TypeNode): string {
            let typeName: string;
            switch (node.kind) {
                case ts.SyntaxKind.TypeReference:
                    typeName = getFullName((<ts.TypeReferenceNode>node).typeName, this._source);
                    break;
            }
            return typeName;
        }
    }
    export class Class extends IMeta {
        public name: string;
        public propertys: Property[] = [];
        public methods: Method[] = [];

        private _source: ts.SourceFile;
        private _node?: ts.ClassDeclaration;

        constructor(source: ts.SourceFile, node?: ts.ClassDeclaration) {
            super();
            this._source = source;
            this._node = node;
            this.assertion();
        }
        protected assertion() {
            this.name = this._node.name.getText(this._source);

            this._node.members?.forEach(child => {
                switch (child.kind) {
                    case ts.SyntaxKind.GetAccessor:
                    case ts.SyntaxKind.SetAccessor:
                    case ts.SyntaxKind.PropertyDeclaration:
                        this.propertys.push(
                            new Property(this._source, <ts.PropertyDeclaration>child)
                        );
                        break;
                    case ts.SyntaxKind.MethodDeclaration:
                        this.methods.push(
                            new Method(this._source, <ts.MethodDeclaration>child)
                        );
                        break;
                }
            });
        }
    }
    export class Property extends IMeta {
        public name: string;
        public type: string;
        public modifier: string;
        public setter: boolean;
        public getter: boolean;

        public get isFiled() {
            return !this.setter && !this.getter;
        }

        private _source: ts.SourceFile;
        private _node?: ts.PropertyDeclaration | ts.GetAccessorDeclaration | ts.SetAccessorDeclaration;
        constructor(source: ts.SourceFile, node?: ts.PropertyDeclaration | ts.GetAccessorDeclaration | ts.SetAccessorDeclaration) {
            super();
            this._source = source;
            this._node = node;
            this.assertion();
        }
        protected assertion() {
            let { name, type, modifiers, kind } = this._node;
            this.name = name.getText(this._source);
            this.type = type?.getText(this._source);
            this.setter = kind === ts.SyntaxKind.SetAccessor;
            this.getter = kind === ts.SyntaxKind.GetAccessor;

            switch (type?.kind) {
                case ts.SyntaxKind.StringKeyword:
                case ts.SyntaxKind.NumberKeyword:
                case ts.SyntaxKind.BigIntKeyword:
                case ts.SyntaxKind.ObjectKeyword:
                case ts.SyntaxKind.BooleanKeyword:

                    break;
                case ts.SyntaxKind.ArrayType:
                    break;
                case ts.SyntaxKind.SymbolKeyword:
                    break;
                case ts.SyntaxKind.TypeReference:
                    break;
            }
            let modifier = modifiers[0];
            if (modifier) {
                this.modifier =
                    modifier.kind === ts.SyntaxKind.PrivateKeyword ? "private" :
                        modifier.kind === ts.SyntaxKind.ProtectedKeyword ? "protected" :
                            "public";
            }
        }
        public isTypeReference() {
            return this._node.type?.kind === ts.SyntaxKind.TypeReference;
        }
    }
    export class Method extends IMeta {
        public name: string;
        public modifier: string;

        private _source: ts.SourceFile;
        private _node: ts.MethodDeclaration

        constructor(source: ts.SourceFile, node: ts.MethodDeclaration) {
            super();
            this._source = source;
            this._node = node;
            this.assertion();
        }
        protected assertion() {
            this.name = this._node.name.getText(this._source);
        }
    }


    function getFullName(node: ts.Identifier | ts.QualifiedName | ts.BindingElement | ts.Expression, source: ts.SourceFile): string {
        let typeName: string;
        switch (node.kind) {
            case ts.SyntaxKind.Identifier:
            case ts.SyntaxKind.BindingElement:
                typeName = node.getText(source);
                break;
            case ts.SyntaxKind.PropertyAccessExpression:
                let _node = <ts.PropertyAccessExpression>node;
                typeName = _node.name.getText(source);
                if (_node.expression) {
                    typeName = `${getFullName(_node.expression, source)}.${typeName}`;
                }
                break;
            case ts.SyntaxKind.QualifiedName:
                let left = getFullName((<ts.QualifiedName>node).left, source),
                    right = getFullName((<ts.QualifiedName>node).right, source);
                typeName = `${left}.${right}`;
                break;
            default:
                typeName = node.getText(source);
                break;
        }
        return typeName;
    }

    function matchType(file: File, type: string) {
        let firstName = type, separatorIdx: number = type.indexOf(".");
        if (separatorIdx > 0) {
            firstName = type.substring(0, separatorIdx);
        }

        let fromName: string | "*", fromModule: string;

        for (let i of file.imports) {
            if (i.name === firstName) {
                fromName = "*";
            }
            else if (firstName in i.clauseAlias) {
                fromName = i.clauseAlias[firstName];
            }
            if (fromName) {
                fromModule = i.module;
                break;
            }
        }

        file.typeVariables;
    }
}

export function createFile(path: string) {
    let fileName = cs.System.IO.Path.GetFileName(path);
    let fileContent = cs.System.IO.File.ReadAllText(path);

    let fileSource = ts.createSourceFile(fileName, fileContent, ts.ScriptTarget.ES2020);
}
