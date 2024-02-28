import * as fs from "fs";
import * as tsm from "ts-morph";

type Declaration = tsm.ClassDeclaration | tsm.InterfaceDeclaration | tsm.EnumDeclaration;

class CSharpReferencesReolsver {
    private readonly tsConfigFile: string;
    private readonly project: tsm.Project;
    private readonly types: Set<string>;
    private readonly underlyingTypes: Set<string>;
    private readonly callMethods: Map<string, Set<string>>;

    //不需要额外处理的"节点"
    private readonly unresolveNodes = [
        tsm.SyntaxKind.ImportDeclaration,
        tsm.SyntaxKind.ExportDeclaration,
        tsm.SyntaxKind.ExportAssignment,
        tsm.SyntaxKind.EnumDeclaration,
        tsm.SyntaxKind.InterfaceDeclaration,
        tsm.SyntaxKind.EmptyStatement,
        tsm.SyntaxKind.BreakStatement,
        tsm.SyntaxKind.ContinueStatement,
        tsm.SyntaxKind.Identifier,
        tsm.SyntaxKind.StringLiteral,
        tsm.SyntaxKind.NumericLiteral,
        tsm.SyntaxKind.BigIntLiteral,
        tsm.SyntaxKind.BooleanKeyword,
        tsm.SyntaxKind.FalseKeyword,
        tsm.SyntaxKind.TrueKeyword,
        tsm.SyntaxKind.NumberKeyword,
        tsm.SyntaxKind.NullKeyword,
        tsm.SyntaxKind.ThisKeyword,
        tsm.SyntaxKind.VoidExpression,
        tsm.SyntaxKind.NoSubstitutionTemplateLiteral,
        tsm.SyntaxKind.RegularExpressionLiteral,
        tsm.SyntaxKind.TypeOfExpression,
        tsm.SyntaxKind.TypeAliasDeclaration,
        tsm.SyntaxKind.ImportEqualsDeclaration,
        //-------------------------------------------
        //should be retained?
        tsm.SyntaxKind.ElementAccessExpression,     //ClassA['a']['b']
    ];

    constructor(tsConfigFile: string) {
        let { config, error } = tsm.ts.readConfigFile(tsConfigFile, (path) => fs.readFileSync(path, 'utf8'));
        if (error) {
            throw error;
        }
        const options: tsm.CompilerOptions = config?.compilerOptions || {};
        if (typeof (options.moduleResolution) === "string") {
            options.moduleResolution = options.moduleResolution === "node" ? tsm.ModuleResolutionKind.NodeJs : tsm.ModuleResolutionKind.Classic;
        }
        const project = new tsm.Project({
            tsConfigFilePath: tsConfigFile,
            compilerOptions: {
                ...options,
                incremental: false,
                noEmit: true,
            },
        });

        this.tsConfigFile = tsConfigFile;
        this.project = project;
        this.types = new Set();
        this.underlyingTypes = new Set();
        this.callMethods = new Map();
    }

    public process() {
        for (let sourceFile of this.project.getSourceFiles()) {
            if (sourceFile.isDeclarationFile() || sourceFile.isInNodeModules())
                continue;
            //TODO: test code
            /* if (!sourceFile.getBaseName().includes('test'))
                continue; */
            for (let statement of sourceFile.getStatements()) {
                this.resolveStatement(statement);
            }
        }
    }
    public getResults() {
        let callMethods = new Map<string, string[]>();
        for (let [typeName, methodNames] of this.callMethods) {
            callMethods.set(typeName, [...methodNames]);
        }
        return {
            types: [...this.types],
            underlyingTypes: [...this.underlyingTypes],
            callMethods: callMethods
        }
    }

    private resolveClass(cls: tsm.ClassDeclaration | tsm.ClassExpression) {
        if (!cls)
            return;
        //分析类装饰器
        cls.getDecorators()?.forEach(decorator => {
            this.resolveDecorator(decorator);
        });
        //分析字段
        let properties = cls.getProperties();
        properties?.forEach(property => {
            this.loadType(property.getTypeNode());
            property.getDecorators()?.forEach(decorator => {
                this.resolveDecorator(decorator);
            });
        });
        //分析方法
        let methods = cls.getMethods();
        methods?.forEach(method => {
            this.resolveMethod(method);
            method.getDecorators()?.forEach(decorator => {
                this.resolveDecorator(decorator);
            });
        });
    }
    private resolveMethod(method: tsm.MethodDeclaration | tsm.FunctionDeclaration | tsm.ArrowFunction | tsm.FunctionExpression) {
        if (!method)
            return;
        //分析参数
        method.getParameters()?.forEach(arg => {
            this.loadType(arg.getTypeNode());
            this.resolveExpression(arg.getInitializer());
        });
        this.loadType(method.getReturnTypeNode());
        //分析方法主体
        let body = method.getBody();
        if (body) {
            this.resolveAnyNode(body);
        }
    }
    private resolveDecorator(decorator: tsm.Decorator) {
        if (!decorator)
            return;
        this.resolveExpression(decorator.getCallExpression());
        decorator.getArguments()?.forEach(arg => {
            this.resolveAnyNode(arg);
        });
    }

    private resolveAnyNode(node: tsm.Node) {
        if (!node)
            return;
        if (tsm.Node.isExpression(node)) {
            this.resolveExpression(node);
        }
        else if (tsm.Node.isStatement(node)) {
            this.resolveStatement(node);
        }
        else {
            this.printNode(node, 'unknown node');
        }
    }
    private resolveStatement(statement: tsm.Node) {
        if (!statement)
            return;
        if (tsm.Node.isClassDeclaration(statement)) {
            this.resolveClass(statement);
        }
        else if (tsm.Node.isMethodDeclaration(statement) ||
            tsm.Node.isFunctionDeclaration(statement)) {
            this.resolveMethod(statement);
        }
        else if (tsm.Node.isModuleDeclaration(statement) ||
            tsm.Node.isModuleBlock(statement)) {
            for (let _statement of statement.getStatements()) {
                this.resolveStatement(_statement);
            }
        }
        else if (tsm.Node.isBlock(statement)) {
            for (let child of statement.forEachChildAsArray()) {
                this.resolveStatement(child);
            }
        }
        //<expression>
        else if (tsm.Node.isExpressionStatement(statement)) {
            this.resolveExpression(statement.getExpression());
        }
        //let a=xxx, var b=xxxx, const c=xxx
        else if (tsm.Node.isVariableStatement(statement)) {
            for (let variable of statement.getDeclarations()) {
                this.loadType(variable.getTypeNode());   //不支持类型推断???
                this.resolveExpression(variable.getInitializer());
            }
        }
        //if (xxxx)
        else if (tsm.Node.isIfStatement(statement)) {
            //condition
            this.resolveExpression(statement.getExpression());
            //content
            for (let child of [statement.getThenStatement(), statement.getElseStatement()]) {
                if (!child)
                    continue;
                this.resolveStatement(child);
            }
        }
        //try { xxx } ...
        else if (tsm.Node.isTryStatement(statement)) {
            let catchClause = statement.getCatchClause(),
                variable = catchClause?.getVariableDeclaration();
            if (variable) {
                this.loadType(variable.getTypeNode());
                this.resolveExpression(variable.getInitializer());
            }
            //content
            for (let child of [statement.getTryBlock(), catchClause?.getBlock(), statement.getFinallyBlock(),]) {
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
                    for (let variable of initializer.getDeclarations()) {
                        this.loadType(variable.getTypeNode());
                        this.resolveExpression(variable.getInitializer());
                    }
                }
            }
            this.resolveExpression(condition);
            this.resolveExpression(incrementor);
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
                    for (let variable of initializer.getDeclarations()) {
                        this.loadType(variable.getTypeNode());
                        this.resolveExpression(variable.getInitializer());
                    }
                }
            }
            this.resolveExpression(condition);
            //content
            this.resolveStatement(statement.getStatement());
        }
        //with (xxxx) ...  --@deprecated
        else if (tsm.Node.isWithStatement(statement)) {
            //condition
            this.resolveExpression(statement.getExpression());
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
        else {
            this.printNode(statement, 'unknown statement');
        }
    }
    private resolveExpression(expression: tsm.Expression) {
        if (!expression)
            return;
        //obj.methodA(xxx);
        if (tsm.Node.isCallExpression(expression)) {
            let resultType = utils.getDeclaration(expression.getReturnType());
            if (resultType && !tsm.Node.isSourceFile(resultType)) {     //require(xxx) 函数
                this.loadType(utils.getDeclaration(expression.getReturnType()));
            }
            this.loadCallMethod(expression);
            this.resolveExpression(expression.getExpression());
            for (let arg of expression.getArguments()) {
                this.resolveAnyNode(arg);
            }
        }
        //class A { ... }
        else if (tsm.Node.isClassExpression(expression)) {
            this.resolveClass(expression);
        }
        //new ClassA(xxx)
        else if (tsm.Node.isNewExpression(expression)) {
            this.loadType(expression.getExpression());
            for (let arg of expression.getArguments()) {
                this.resolveAnyNode(arg);
            }
        }
        //ClassA.a;
        else if (tsm.Node.isPropertyAccessExpression(expression)) {
            this.loadType(expression.getExpression());
        }
        //[a, b, c,]
        else if (tsm.Node.isArrayLiteralExpression(expression)) {
            expression.getElements().forEach(e => {
                this.resolveExpression(e);
            });
        }
        //let a= xxx;
        else if (tsm.Node.isBinaryExpression(expression)) {
            this.resolveExpression(expression.getLeft());
            this.resolveExpression(expression.getRight());
        }
        //()=>{ xxx }, function(){ xxx }
        else if (tsm.Node.isArrowFunction(expression) ||
            tsm.Node.isFunctionExpression(expression)) {
            this.resolveMethod(expression);
        }
        //await xxx
        else if (tsm.Node.isAwaitExpression(expression)) {
            this.resolveExpression(expression.getExpression());
        }
        //yield xxx
        else if (tsm.Node.isYieldExpression(expression)) {
            this.resolveExpression(expression.getExpression());
        }
        //(xxx)
        else if (tsm.Node.isParenthesizedExpression(expression)) {
            this.resolveExpression(expression.getExpression());
        }
        //a++, ++b, !c;
        else if (tsm.Node.isPrefixUnaryExpression(expression) ||
            tsm.Node.isPostfixUnaryExpression(expression)) {
            this.resolveExpression(expression.getOperand());
        }
        //objA as ClassB
        else if (tsm.Node.isAsExpression(expression)) {
            this.loadType(expression.getTypeNode());
            this.resolveExpression(expression.getExpression());
        }
        //<ClassB>objA
        else if (tsm.Node.isTypeAssertion(expression)) {
            this.loadType(expression.getTypeNode());
            this.resolveExpression(expression.getExpression());
        }
        //delete objA.a
        else if (tsm.Node.isDeleteExpression(expression)) {
            this.resolveExpression(expression.getExpression());
        }
        //[...array];
        else if (tsm.Node.isSpreadElement(expression)) {
            this.resolveExpression(expression.getExpression());
        }
        //a ? b : c
        else if (tsm.Node.isConditionalExpression(expression)) {
            this.resolveExpression(expression.getCondition());
            this.resolveExpression(expression.getWhenTrue());
            this.resolveExpression(expression.getWhenFalse());
        }
        //let objA={a:xxx, b:xxx, ...};
        else if (tsm.Node.isObjectLiteralExpression(expression)) {
            for (let property of expression.getProperties()) {
                if (tsm.Node.isPropertyAssignment(property)) {
                    this.resolveExpression(property.getInitializer());
                }
                else if (tsm.Node.isShorthandPropertyAssignment(property)) {
                    this.resolveExpression(property.getInitializer());
                    this.resolveExpression(property.getObjectAssignmentInitializer());
                }
                else if (tsm.Node.isSpreadAssignment(property)) {
                    this.resolveExpression(property.getExpression());
                }
                else if (tsm.Node.isMethodDeclaration(property)) {
                    this.resolveMethod(property);
                }
                else /* if (tsm.Node.isGetAccessorDeclaration(property) ||
                    tsm.Node.isSetAccessorDeclaration(property))  */ {
                    this.resolveAnyNode(property.getBody());
                }
            }
        }
        //`${a}xxx`     模板字符串
        else if (tsm.Node.isTemplateExpression(expression)) {
            for (let templateSpan of expression.getTemplateSpans()) {
                this.resolveExpression(templateSpan.getExpression());
            }
        }
        //a!            断言表达式
        else if (tsm.Node.isNonNullExpression(expression)) {
            this.resolveExpression(expression.getExpression());
        }
        else {
            this.printNode(expression, 'unknown expression');
        }
    }

    private loadType(node: tsm.Node) {
        if (!node)
            return;
        try {
            let ut = utils.getUnderlyingType(node);
            if (!ut || ut.types.length === 0)
                return;
            ut.types.forEach(t => this.underlyingTypes.add(t));
            this.types.add(ut.full);
        } catch (e) {
            this.printNode(node, 'exception');
            console.error(e);
        }
    }
    private loadCallMethod(expression: tsm.CallExpression) {
        if (!expression)
            return;
        try {
            //获取类型名称和方法名称
            let accessExpression = expression.getExpression();
            if (!accessExpression || !tsm.Node.isPropertyAccessExpression(accessExpression))
                return;
            let clsName = utils.getUnderlyingType(accessExpression.getExpression())?.full,
                methodName = accessExpression.getName();
            if (!clsName || !methodName)
                return;

            let methods = this.callMethods.get(clsName);
            if (!methods) {
                methods = new Set();
                this.callMethods.set(clsName, methods);
            }
            methods.add(methodName);
        } catch (e) {
            this.printNode(expression, 'exception');
            console.error(e);
        }
    }

    private printNode(node: tsm.Node, title?: string) {
        if (this.unresolveNodes.includes(node.getKind()))
            return;
        if (!title) title = `unknown node`;
        let head = `==============${title}==========================================`,
            tail = `===============================================`;
        console.log(`
${head.slice(0, tail.length)}
${node.getKindName()}, ${node.getKind()}
${node.getText()}
${tail}`);
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
            tsm.Node.isEnumDeclaration(node) ||
            tsm.Node.isClassDeclaration(node) ||
            tsm.Node.isInterfaceDeclaration(node)
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
                    tsm.Node.isEnumDeclaration(d) ||
                    tsm.Node.isClassDeclaration(d)
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
        if (tsm.Node.isParenthesizedTypeNode(node)) {
            return getUnderlyingType(node.getTypeNode(), depth + 1);
        }
        else if (tsm.Node.isArrayTypeNode(node)) {
            let _ts = getUnderlyingType(node.getElementTypeNode(), depth + 1);
            if (_ts && _ts.types.length > 0) {
                _ts.types.forEach(_t => types.add(_t));
                full = `${_ts.types[0]}[]`;
            }
        }
        else if (tsm.Node.isMappedTypeNode(node)) {
            let _ts: ReturnType<typeof getUnderlyingType>, key: string, value: string;
            _ts = getUnderlyingType(node.getNameTypeNode(), depth + 1);
            if (_ts && _ts.types.length > 0) {
                _ts.types.forEach(_t => types.add(_t));
                key = _ts.full;
            }
            _ts = getUnderlyingType(node.getTypeNode(), depth + 1);
            if (_ts && _ts.types.length > 0) {
                _ts.types.forEach(_t => types.add(_t));
                value = _ts.full;
            }
            if (key && value) {
                full = `Map<${key},${value}>`;
            }
        }
        else if (tsm.Node.isUnionTypeNode(node)) {
            let typeNodes = node.getTypeNodes(), fullList: string[] = [];;
            typeNodes.forEach(n => {
                let _node = getDeclaration(n);
                if (!_node || !tsm.Node.isClassDeclaration(_node))
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
            else if (tsm.Node.isTypeReference(node)) {
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