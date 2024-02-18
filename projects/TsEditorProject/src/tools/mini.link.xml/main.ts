import * as fs from "fs";
import * as tsm from "ts-morph";

type Declaration = tsm.ClassDeclaration | tsm.InterfaceDeclaration | tsm.EnumDeclaration;

class CSharpReferencesReolsver {
    private readonly tsConfigFile: string;
    private readonly project: tsm.Project;
    private readonly types: Set<string>;
    private readonly underlyingTypes: Set<string>;

    constructor(tsConfigFile: string) {
        let { config, error } = tsm.ts.readConfigFile(tsConfigFile, (path) => fs.readFileSync(path, 'utf8'));
        if (error) {
            throw error;
        }
        const options: tsm.CompilerOptions = config?.compilerOptions || {};
        //@ts-ignore
        const moduleResolution = options.moduleResolution == "node" ? tsm.ModuleResolutionKind.NodeJs :
            tsm.ModuleResolutionKind.Classic;
        const project = new tsm.Project({
            tsConfigFilePath: tsConfigFile,
            compilerOptions: {
                ...options,
                moduleResolution: moduleResolution,
                incremental: false,
                noEmit: true,
            },
        });

        this.tsConfigFile = tsConfigFile;
        this.project = project;
        this.types = new Set();
        this.underlyingTypes = new Set();
    }

    public process() {
        for (let sourceFile of this.project.getSourceFiles()) {
            if (sourceFile.isDeclarationFile() || sourceFile.isInNodeModules())
                continue;
            for (let statement of sourceFile.getStatements()) {
                this.resolveStatement(statement);
            }
        }
    }
    public getResults() {
        return {
            types: [...this.types],
            underlyingTypes: [...this.underlyingTypes],
        }
    }

    private resolveClass(cls: tsm.ClassDeclaration) {
        //分析字段
        let properties = cls.getProperties();
        properties?.forEach(property => {
            this.loadType(property.getTypeNode());
        });
        //分析方法
        let methods = cls.getMethods();
        methods?.forEach(method => {
            this.resolveMethod(method);
        });
    }
    private resolveMethod(method: tsm.MethodDeclaration | tsm.FunctionDeclaration) {
        //分析参数
        let argsTypes = [...method.getParameters().map(p => p.getTypeNode()), method.getReturnTypeNode()];
        argsTypes?.forEach(node => {
            this.loadType(node);
        });
        //分析方法主体
        let body = method.getBody();
        if (body && tsm.Node.isBlock(body)) {
            this.resolveStatement(body);
        }
    }

    private resolveStatement(statement: tsm.Statement) {
        if (tsm.Node.isClassDeclaration(statement)) {
            this.resolveClass(statement);
        }
        else if (tsm.Node.isMethodDeclaration(statement) ||
            tsm.Node.isFunctionDeclaration(statement)) {
            this.resolveMethod(statement);
        }
        else if (tsm.Node.isModuleBlock(statement)) {
            for (let _statement of statement.getStatements()) {
                this.resolveStatement(_statement);
            }
        }
        else if (tsm.Node.isBlock(statement)) {
            for (let child of statement.forEachChildAsArray()) {
                this.resolveBodyStatement(child);
            }
        }
        else {
            this.resolveBodyStatement(statement);
        }
    }
    private resolveBodyStatement(statement: tsm.Node) {
        //<expression>
        if (tsm.Node.isExpressionStatement(statement)) {
            this.resolveExpression(statement.getExpression());
        }
        //if (xxxx)
        else if (tsm.Node.isIfStatement(statement)) {
            //condition
            this.resolveExpression(statement.getExpression());
            //content
            let children = [
                statement.getThenStatement(),
                statement.getElseStatement()
            ];
            for (let child of children) {
                if (!child)
                    continue;
                this.resolveStatement(child);
            }
        }
        //try { xxx } ...
        else if (tsm.Node.isTryStatement(statement)) {
            let children = [
                statement.getTryBlock(),
                statement.getCatchClause()?.getBlock(),
                statement.getFinallyBlock(),
            ];
            for (let child of children) {
                if (!child)
                    continue;
                this.resolveStatement(child);
            }
        }
        //throw xxxx
        else if (tsm.Node.isThrowStatement(statement)) {
            this.resolveExpression(statement.getExpression());
        }
        //while(xxxx), do { xxx } while(xxx)
        else if (tsm.Node.isDoStatement(statement) ||
            tsm.Node.isWhileStatement(statement)) {
            //condition
            this.resolveExpression(statement.getExpression());
            //content
            this.resolveStatement(statement.getStatement());
        }
        //for(var,condition,incrementor )
        else if (tsm.Node.isForStatement(statement)) {
            //initializer丶condition丶incrementor
            let initializer = statement.getInitializer(),
                condition = statement.getCondition(),
                incrementor = statement.getIncrementor();
            if (initializer) {
                if (tsm.Node.isExpression(initializer)) {
                    this.resolveExpression(initializer);
                } else {
                    for (let declaration of initializer.getDeclarations()) {
                        let initValue = declaration.getInitializer();
                        if (initValue) this.resolveExpression(initValue);
                    }
                }
            }
            if (condition) this.resolveExpression(condition);
            if (incrementor) this.resolveExpression(incrementor);
            //content
            this.resolveStatement(statement.getStatement());
        }
        //for( xxx in xxx), for( xxx of xxx)
        else if (tsm.Node.isForInStatement(statement) ||
            tsm.Node.isForOfStatement(statement)) {
            //initializer丶condition
            let initializer = statement.getInitializer(),
                condition = statement.getExpression();
            if (initializer) {
                if (tsm.Node.isExpression(initializer)) {
                    this.resolveExpression(initializer);
                } else {
                    for (let declaration of initializer.getDeclarations()) {
                        let initValue = declaration.getInitializer();
                        if (initValue) this.resolveExpression(initValue);
                    }
                }
            }
            this.resolveExpression(condition);
            //content
            this.resolveStatement(statement.getStatement());
        }
        else if (tsm.Node.isWithStatement(statement)) {
            //content
            this.resolveStatement(statement.getStatement());
        }
        //switch(xxx) { xxx } 
        else if (tsm.Node.isSwitchStatement(statement)) {
            //condition
            this.resolveExpression(statement.getExpression());
            //content
            for (let clause of statement.getClauses()) {
                if (tsm.Node.isCaseClause(clause)) {
                    this.resolveExpression(clause.getExpression());
                }
                for (let clauseStatement of clause.getStatements()) {
                    this.resolveStatement(clauseStatement);
                }
            }
        }
        //return xxx
        else if (tsm.Node.isReturnStatement(statement)) {
            //condition
            this.resolveExpression(statement.getExpression());
        }
    }
    private resolveExpression(expression: tsm.Expression) {
        if (tsm.Node.isCallExpression(expression)) {
            this.resolveExpression(expression.getExpression());
        }
        else if (tsm.Node.isNewExpression(expression) ||
            tsm.Node.isPropertyAccessExpression(expression)) {
            this.loadType(expression.getExpression());
        }
        else if (tsm.Node.isArrayLiteralExpression(expression)) {
            expression.getElements().forEach(e => {
                this.resolveExpression(e);
            });
        }
        else if (tsm.Node.isBinaryExpression(expression)) {
            this.resolveExpression(expression.getLeft());
            this.resolveExpression(expression.getRight());
        }
    }

    private loadType(node: tsm.Node) {
        let ut = utils.getUnderlyingType(node);
        if (!ut || ut.types.length === 0)
            return;
        ut.types.forEach(t => this.underlyingTypes.add(t));
        this.types.add(ut.full);
    }
}

enum Module {
    Global = `global`,
    CS = `CS`,
    CSharp = `"csharp"`,
}
namespace utils {
    const unicode16 = new class {
        decode(data: string) {
            return unescape(data.replace(/\\u/g, '%u'));
            /*
            return data.replace(/\\u[\dA-F]{4}/gi, (match) => {
                return String.fromCharCode(parseInt(match.replace(/\\u/g, ''), 16));
            });
            //*/
        }
        encode(data: string) {
            return escape(data.replace(/%u/g, '\\u'));
        }
    }

    /**是否为C#类型声明
     * @param moduleName 
     * @param filePath 
     */
    export function isCSharpTypeDeclare(moduleName: string, className: string) {
        return moduleName === Module.CSharp || moduleName === Module.CS ||
            moduleName === Module.Global && className.startsWith("CS.");
    }
    /**获取C#类型名称
     * @param filePath 
     * @param moduleName 
     * @param className 
     */
    export function getCSharpTypeName(moduleName: string, className: string) {
        if (moduleName === Module.Global && className.startsWith("CS.")) {
            className = className.substring(3);
        }
        return className;
    }
    /**获取声明类 */
    export function getDeclaration(node: tsm.Node | tsm.Type): Declaration {
        if (!node)
            return null;
        let declaration: Declaration;

        if (node instanceof tsm.Node && (
            node.isKind(tsm.SyntaxKind.EnumDeclaration) ||
            node.isKind(tsm.SyntaxKind.ClassDeclaration) ||
            node.isKind(tsm.SyntaxKind.InterfaceDeclaration)
        )) {
            declaration = node;
        }
        else {
            let symbol = node.getSymbol();
            if (!symbol && node instanceof tsm.Node) {
                symbol = node.getType().getSymbol();
            }
            let declarations = symbol?.getDeclarations();
            if (declarations && declarations.length > 0) {
                declaration = (declarations.find(d =>
                    d.isKind(tsm.SyntaxKind.EnumDeclaration) ||
                    d.isKind(tsm.SyntaxKind.ClassDeclaration)
                ) || declarations[0]) as typeof declaration;
            }
        }
        return declaration;
    }

    export function getTypeInfo(node: tsm.Node) {
        if (!node)
            return null;
        const type = node.getType(), symbol = type.getSymbol();

        let className: string, moduleName: string, isFile: boolean = false;

        let fullName = type.getText(),         //import("file path").NamsspaceName.ClassName;
            fullyQualifiedName = symbol?.getFullyQualifiedName();

        if (fullName.includes("import(")) {
            let startIndex = fullName.indexOf("("), endIndex = fullName.lastIndexOf(")");
            className = fullName.substring(endIndex + 2);
        }
        else {
            className = fullName;
        }

        if (fullyQualifiedName && fullyQualifiedName.length > className.length) {
            moduleName = unicode16.decode(
                fullyQualifiedName.substring(0, fullyQualifiedName.length - className.length - 1)
            );
            //如果是字符串, 则判断是不是文件模块(非declare module "xxxx"扩大模块)
            if ((moduleName[0] === `"` || moduleName[0] === `'`) &&
                moduleName[0] === moduleName[moduleName.length - 1]
            ) {
                let path = moduleName.substring(1, moduleName.length - 1),
                    filepath = getDeclaration(node)?.getSourceFile().getFilePath();
                if (filepath && filepath.startsWith(path)) {
                    moduleName = path;
                }
            }

        } else {
            moduleName = Module.Global;
        }

        return {
            className,
            moduleName,
            isFile
        }
    }

    /**获取潜在的C#类型
     * @param node 
     * @param depth 
     */
    export function getUnderlyingType(node: tsm.Node, depth: number = 0): { types: string[], full: string } {
        if (depth > 10) {
            return null;
        }
        if (!node)
            return null;

        let types = new Set<string>(), full: string;
        if (node.isKind(tsm.SyntaxKind.ParenthesizedType)) {
            return getUnderlyingType(node.getTypeNode(), depth + 1);
        }
        else if (node.isKind(tsm.SyntaxKind.ArrayType)) {
            let _ts = getUnderlyingType(node.getElementTypeNode(), depth + 1);
            if (_ts && _ts.types.length > 0) {
                _ts.types.forEach(_t => types.add(_t));
                full = `${_ts.types[0]}[]`;
            }
        }
        else if (node.isKind(tsm.SyntaxKind.MappedType)) {

        }
        else if (node.isKind(tsm.SyntaxKind.UnionType)) {
            let typeNodes = node.getTypeNodes(), fullList: string[] = [];;
            typeNodes.forEach(n => {
                let _node = getDeclaration(n);
                if (!_node || !_node.isKind(tsm.SyntaxKind.ClassDeclaration))
                    return;
                let _ts = getUnderlyingType(_node, depth + 1);
                if (_ts && _ts.types.length > 0) {
                    _ts.types.forEach(_t => types.add(_t));
                    fullList.push(_ts.full);
                }
            });
            if (fullList.length > 0) {
                full = fullList.join('|');
            }
        }
        else {
            let { className, moduleName } = getTypeInfo(node);
            if (isCSharpTypeDeclare(moduleName, className)) {
                full = getCSharpTypeName(moduleName, className);
                types.add(full);
            }
            else if (node.isKind(tsm.SyntaxKind.TypeReference)) {
                if (node.getTypeName().getText() === "Array" && node.getTypeArguments().length === 1) {
                    let _ts = getUnderlyingType(node.getTypeArguments().at(0), depth + 1);
                    if (_ts && _ts.types.length > 0) {
                        _ts.types.forEach(_t => types.add(_t));
                        full = `${_ts.types[0]}[]`;
                    }
                }
                else {
                    let declaration = getDeclaration(node);
                    if (declaration) {
                        return getUnderlyingType(declaration, depth + 1);
                    }
                }
            }
        }
        return { types: [...types], full };
    }
}

function generate(tsConfigFile: string) {
    const resolver = new CSharpReferencesReolsver(tsConfigFile);
    resolver.process();

    return resolver.getResults();
}
(function () {
    var _g = global || globalThis || this;
    _g.generate = generate;
})();