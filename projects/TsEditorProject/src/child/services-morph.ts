import * as csharp from "csharp";
import * as glob from "fast-glob";
import * as puerts from "puerts";
import * as tsm from "ts-morph";

import File = csharp.System.IO.File;
import Directory = csharp.System.IO.Directory;
import DirectoryInfo = csharp.System.IO.DirectoryInfo;
import Path = csharp.System.IO.Path;
import Type = csharp.System.Type;

type ClassInfo = {
    readonly guid: string;
    readonly node: tsm.ClassDeclaration;
};

type Declaration = tsm.ClassDeclaration | tsm.InterfaceDeclaration | tsm.EnumDeclaration;
type TsconfigFile = {
    readonly files?: string[];
    readonly include?: string[];
    readonly exclude?: string[];
    readonly references?: string[];
    readonly compilerOptions: tsm.CompilerOptions;
};

enum Module {
    Global = `global`,
    CS = `CS`,
    CSharp = `"csharp"`,
}
enum Classes {
    TsComponent = `xor.TsComponent`,
}
enum Decorator {
    Guid = `xor.guid`,
    Route = `xor.route`,
    Field = `xor.field`,
}

class FileSystemHost implements tsm.FileSystemHost {
    private _currentDirectory: string;
    constructor(currentDirectory?: string) {
        this._currentDirectory = currentDirectory?.replace(/\\/g, "/") || "";
    }

    public isCaseSensitive(): boolean {
        return false;
    }
    public async delete(path: string): Promise<void> {
        await this.waitNextTick();
        this.deleteSync(path);
    }
    public deleteSync(path: string): void {
        if (File.Exists(path)) {
            File.Delete(path);
        }
        else if (Directory.Exists(path)) {
            Directory.Delete(path, true);
        }
    }
    public readDirSync(dirPath: string): tsm.RuntimeDirEntry[] {
        let results: tsm.RuntimeDirEntry[] = [];
        if (Directory.Exists(dirPath)) {
            this._currentDirectory = dirPath;
            let dir = new DirectoryInfo(dirPath),
                dirInfos = dir.GetDirectories(),
                fileInfos = dir.GetFiles();
            for (let i = 0; i < dirInfos.Length; i++) {
                results.push({
                    name: dirInfos.get_Item(i).Name,
                    isFile: false,
                    isDirectory: true,
                    isSymlink: false
                });
            }
            for (let i = 0; i < fileInfos.Length; i++) {
                results.push({
                    name: fileInfos.get_Item(i).Name,
                    isFile: true,
                    isDirectory: false,
                    isSymlink: false
                });
            }
        }
        return results;
    }
    public async readFile(filePath: string, encoding?: string): Promise<string> {
        await this.waitNextTick();
        return this.readFileSync(filePath, encoding);
    }
    public readFileSync(filePath: string, encoding?: string): string {
        if (File.Exists(filePath)) {
            return File.ReadAllText(filePath);
        }
        return null;
    }
    public async writeFile(filePath: string, fileText: string): Promise<void> {
        await this.waitNextTick();
        this.writeFileSync(filePath, fileText);
    }
    public writeFileSync(filePath: string, fileText: string): void {
        File.WriteAllText(filePath, fileText);
    }
    public async mkdir(dirPath: string): Promise<void> {
        this.mkdirSync(dirPath);
    }
    public mkdirSync(dirPath: string): void {
        Directory.CreateDirectory(dirPath);
    }
    public async move(srcPath: string, destPath: string): Promise<void> {
        await this.waitNextTick();
        this.moveSync(srcPath, destPath);
    }
    public moveSync(srcPath: string, destPath: string): void {
        if (File.Exists(srcPath)) {
            File.Move(srcPath, destPath);
        }
        else if (Directory.Exists(srcPath)) {
            Directory.Move(srcPath, destPath);
        }
    }
    public async copy(srcPath: string, destPath: string): Promise<void> {
        await this.waitNextTick();
        this.copySync(srcPath, destPath);
    }
    public copySync(srcPath: string, destPath: string): void {
        if (File.Exists(srcPath)) {
            File.Copy(srcPath, destPath);
        }
        else if (Directory.Exists(srcPath)) {
            let dir = new DirectoryInfo(srcPath),
                dirInfos = dir.GetDirectories(),
                fileInfos = dir.GetFiles();
            for (let i = 0; i < fileInfos.Length; i++) {
                let name = fileInfos.get_Item(i).Name;
                this.copySync(Path.Combine(srcPath, name), Path.Combine(destPath, name));
            }
            for (let i = 0; i < dirInfos.Length; i++) {
                let name = dirInfos.get_Item(i).Name;
                this.copySync(Path.Combine(srcPath, name), Path.Combine(destPath, name));
            }
        }
    }
    public async fileExists(filePath: string): Promise<boolean> {
        await this.waitNextTick();
        return this.fileExistsSync(filePath);
    }
    public fileExistsSync(filePath: string): boolean {
        return File.Exists(filePath);
    }
    public async directoryExists(dirPath: string): Promise<boolean> {
        await this.waitNextTick();
        return this.directoryExistsSync(dirPath);
    }
    public directoryExistsSync(dirPath: string): boolean {
        return Directory.Exists(dirPath);
    }
    public realpathSync(path: string): string {
        return Path.GetFullPath(path);
    }
    public getCurrentDirectory(): string {
        return this._currentDirectory;
    }
    public glob(patterns: readonly string[]): Promise<string[]> {
        return glob([...patterns]);
    }
    public globSync(patterns: readonly string[]): string[] {
        return glob.sync([...patterns]);
    }


    private async waitNextTick() {
        await new Promise(resolve => setTimeout(resolve, 1));   //wait next tick
    }
}

export class Program {
    private readonly cp: csharp.XOR.Services.Program;

    private readonly project: tsm.Project;
    private readonly mapping: Map<string, ClassInfo[]>;

    //是否正在进行解析中
    private resolving: boolean;
    private pending: Set<string>;

    constructor(cp: csharp.XOR.Services.Program, tsConfigFile: string, options?: tsm.CompilerOptions) {
        this.cp = cp;

        if (!options) {
            options = util.readTsconfig(tsConfigFile)?.compilerOptions || {};
        }

        this.project = new tsm.Project({
            tsConfigFilePath: tsConfigFile,
            compilerOptions: {
                ...options,
                incremental: true,
                noEmit: true,
            },
            //useInMemoryFileSystem: true,
            //fileSystem: new FileSystemHost(Path.GetDirectoryName(tsConfigFile)),
            //skipFileDependencyResolution: true,
            //skipAddingFilesFromTsConfig: true,
            //skipLoadingLibFiles: true,
        });
        //this.project.addSourceFilesFromTsConfig(tsConfigFile);

        this.mapping = new Map();

        this.process();
    }

    private async process() {
        this.resolving = true;
        this.cp.state = csharp.XOR.Services.ProgramState.Analyzing;
        //解析文件并为class分配guid
        await this.resolveFiles(this.project.getSourceFiles());

        //推送所有文件的定义
        this.cp.state = csharp.XOR.Services.ProgramState.Allocating;
        for (let [_, classes] of this.mapping) {
            this.register(classes, true);
            await new Promise(resolve => setTimeout(resolve, 1));
        }

        this.cp.state = csharp.XOR.Services.ProgramState.Completed;
        this.resolving = false;

        this.resolvePending();
    }

    private async resolveFiles(files: tsm.SourceFile[]) {
        for (let file of files) {
            if (file.isDeclarationFile() || file.isInNodeModules())
                continue;

            await this.resolveStatements(file.getStatements());
            if (!file.isSaved()) {
                file.saveSync();
                File.WriteAllText(file.getFilePath(), file.getFullText());
            }
        }
    }
    private async resolveStatements(statements: tsm.Statement[]) {
        if (!statements || statements.length == 0)
            return;
        for (let statement of statements) {
            if (tsm.Node.isClassDeclaration(statement)) {
                await this.resolveClass(statement);
            }
            else if (tsm.Node.isModuleBlock(statement) ||
                tsm.Node.isModuleDeclaration(statement)) {
                await this.resolveStatements(statement.getStatements());
            }
        }
    }
    private async resolveClass(node: tsm.ClassDeclaration) {
        if (!this.isExportClass(node))
            return;

        let guid = util.getGuidDecorator(node);
        if (!guid) {
            //分配Decorator
            guid = csharp.System.Guid.NewGuid().ToString();
            node.addDecorator({
                name: Decorator.Guid,
                arguments: [`"${guid}"`]
            });
        }
        let fileName = node.getSourceFile().getFilePath();

        let classes = this.mapping.get(fileName);
        if (!classes) {
            classes = [];
            this.mapping.set(fileName, classes);
        }
        classes.push({ guid, node });
    }

    private async resolvePending() {
        if (this.resolving)
            return;
        if (!this.pending || this.pending.size === 0)
            return;

        let files = [...this.pending];
        this.pending.clear();
        try {
            this.resolving = true;
            this.cp.state = csharp.XOR.Services.ProgramState.Allocating;

            while (files.length > 0) {
                let filePath = files.shift();

                let newSourceFile: tsm.SourceFile;
                let oldSource = this.project.getSourceFile(filePath);
                if (oldSource) {
                    let refreshResult = await oldSource.refreshFromFileSystem();
                    if (refreshResult === tsm.FileSystemRefreshResult.NoChange) {       //文件没有更新
                        continue;
                    } else if (refreshResult === tsm.FileSystemRefreshResult.Deleted) { //文件已删除
                        this.unregister(filePath);
                        continue;
                    }
                    newSourceFile = oldSource;
                } else {
                    newSourceFile = this.project.addSourceFileAtPath(filePath);
                }

                //新增或更新文件内容
                this.unregister(filePath);
                await this.resolveStatements(newSourceFile.getStatements());
                if (!newSourceFile.isSaved()) {
                    newSourceFile.saveSync();
                    //File.WriteAllText(filePath, newSourceFile.getFullText());
                }
                this.register(filePath);
            }

            await new Promise<void>((resolve) => setTimeout(resolve, 1));
        } finally {
            this.resolving = false;
            this.cp.state = csharp.XOR.Services.ProgramState.Completed;

            this.resolvePending();
        }
    }

    /**文件更新: 新增丶修改丶删除
     * @param files 
     */
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

    /**注册类
     * @param fileName 
     * @param force 
     */
    private register(fileName: string, force?: boolean): void;
    private register(classes: ClassInfo[], force?: boolean): void;
    private register() {
        let classes: ClassInfo[] = arguments[0], force: boolean = arguments[1];
        if (typeof (classes) === "string") {
            classes = this.mapping.get(classes);
        }
        if (!classes || classes.length === 0)
            return;

        for (let { guid, node } of classes) {
            if (!guid || !this.isExportClass(node)) {
                continue;
            }
            //以class全文文本计算sha256结果作为version
            let version = csharp.XOR.HashUtil.SHA256(node.getFullText());

            //检验当前节点是否需要重新生成声明
            if (!force && this.cp.GetStatement(guid)?.version === version) {
                continue;
            }

            let { className, moduleName } = util.getTypeInfo(node);
            let filePath = node.getSourceFile().getFilePath();
            //创建声明对象
            let ctd = new csharp.XOR.Services.TypeDeclaration();
            ctd.guid = guid;
            ctd.version = version;
            ctd.name = node.getName();
            ctd.module = moduleName || filePath || "<global>";
            ctd.path = filePath;
            ctd.line = node.getStartLineNumber();       //计算文件所在行
            //成员声明
            let properties = this.getExportProperties(node);
            if (properties && properties.size > 0) {
                for (let [name, property] of properties) {
                    let fargs = util.getFieldArguments(property),
                        ftype = util.toCSharpType([property.getTypeNode(), fargs?.type]);
                    if (!ftype || !ftype.type) {
                        continue;
                    }
                    let cpd = new csharp.XOR.Services.PropertyDeclaration();
                    cpd.name = name;
                    cpd.valueType = ftype.type;

                    if (ftype.references && ftype.references.size > 0) {
                        for (let [guid, name] of ftype.references) {
                            cpd.AddReference(guid, name);
                        }
                    }
                    if (ftype.enumerable) {
                        for (let [name, value] of ftype.enumerable) {
                            cpd.AddEnum(name, value);
                        }
                    }
                    else if (fargs && fargs.range) {
                        cpd.SetRange(fargs.range[0], fargs.range[1]);
                    }
                    if (fargs && fargs.value !== undefined) {
                        cpd.defaultValue = fargs.value;
                    }

                    ctd.AddProperty(cpd);
                }
            }
            //成员方法声明
            let methods = this.getExportMethods(node);
            if (methods && methods.size > 0) {
                for (let [name, ms] of methods) {
                    for (let method of ms) {
                        let parameterTypes = method.getParameters()
                            .map(p => util.toCSharpTypeByTypeNode(p.getTypeNode()));
                        if (parameterTypes.some(t => !t)) {
                            continue;
                        }

                        let cmd = new csharp.XOR.Services.MethodDeclaration();
                        cmd.name = name;
                        cmd.returnType = util.toCSharpType(util.getDeclaration(method.getReturnType()))?.type;
                        cmd.parameterTypes = util.toCSharpArray(parameterTypes) as typeof cmd.parameterTypes;

                        ctd.AddMethod(cmd);
                    }
                }
            }

            this.cp.AddStatement(ctd);
        }
    }
    /**移除(已)注册类
     * @param fileName 
     */
    private unregister(fileName: string): void;
    private unregister(classes: ClassInfo[]): void;
    private unregister() {
        if (typeof (arguments[0]) === "string") {
            let filePath = arguments[0];
            let classes: ClassInfo[] = this.mapping.get(filePath);
            if (!classes) {
                return;
            }
            this.mapping.delete(filePath);
            classes.forEach(cls => this.cp.RemoveStatement(cls.guid));
        }
        else {
            let classes: ClassInfo[] = arguments[0];
            if (!classes || classes.length === 0) {
                return;
            }
            for (let cls of [...classes]) {
                this.cp.RemoveStatement(cls.guid);
                if (!cls.node)
                    continue;
                let filePath = cls.node.getSourceFile().getFilePath();
                let infos = this.mapping.get(filePath);
                if (!infos || !infos.includes(cls))
                    continue;
                infos.splice(infos.indexOf(cls), 1);
                if (infos.length === 0) {
                    this.mapping.delete(filePath);
                }
            }
        }
    }

    /**是否为导出的类型
     * @param node 
     * @returns 
     */
    private isExportClass(node: tsm.ClassDeclaration) {
        if (!node || !node.isExported() || node.isAbstract() || util.isDeclare(node)) {
            return false;
        }
        //如果属于模块声明(declare module "xxxxx"), 则不进行导出
        /*
        let {  moduleName, } = util.getTypeInfo(node);
        if (util.getModuleName(node)) {
            return false;
        }
        //*/
        return util.isInheritTsComponent(node.getBaseClass());
    }
    /**是否为导出的字段; 使用declare丶public或@xor.field(...)装饰器的字段
     * @param node 
     * @returns 
     */
    private isExportField(node: tsm.PropertyDeclaration) {
        if (util.isDeclare(node) || util.isPublic(node)) {
            return true;
        }
        if (util.getFieldDecorator(node)) {
            return true;
        }
        return false;
    }
    /**是否为导出的方法;
     * 
     * @param node 
     */
    private isExportMethod(node: tsm.MethodDeclaration) {
        if (util.isPublic(node))
            return true;
        return false;
    }


    /**获取需要导出的字段
     * @param node 
     * @param inherit 
     */
    private getExportProperties(node: tsm.ClassDeclaration, inherit?: boolean) {
        if (util.isTsComponent(node)) {
            return null;
        }
        const members = new Map<string, tsm.PropertyDeclaration>();

        let properties = node.getProperties();
        if (properties) {
            for (let property of properties) {
                if (property.isStatic() || !this.isExportField(property))
                    continue;
                let nameNode = property.getNameNode();
                if (nameNode.isKind(tsm.SyntaxKind.StringLiteral) || nameNode.isKind(tsm.SyntaxKind.Identifier)) {
                    members.set(property.getName(), property);
                }
            }
        }

        if (inherit && node.getBaseClass()) {
            const _members = this.getExportProperties(node.getBaseClass(), true);
            for (let [name, type] of _members) {
                if (members.has(name)) continue;
                members.set(name, type);
            }
        }

        return members;
    }
    /**获取需要导出的方法
     * @param node 
     * @param inherit 
     */
    private getExportMethods(node: tsm.ClassDeclaration, inherit?: boolean) {
        if (util.isTsComponent(node)) {
            return null;
        }
        const members = new Map<string, Set<tsm.MethodDeclaration>>();
        let methods = node.getMethods()
        if (methods) {
            for (let method of methods) {
                if (method.isStatic() || !this.isExportMethod(method))
                    continue;
                let nameNode = method.getNameNode();
                if (!nameNode.isKind(tsm.SyntaxKind.StringLiteral) && !nameNode.isKind(tsm.SyntaxKind.Identifier)) {
                    continue;
                }
                let name = method.getName(), _ms = members.get(name)
                if (!_ms) {
                    _ms = new Set();
                    members.set(name, _ms);
                }
                _ms.add(method);
            }
        }
        if (inherit && node.getBaseClass()) {

        }

        return members;
    }
}

const util = new class {

    public isDeclare(node: tsm.ModifierableNode, parent?: boolean): boolean {
        let modifiers = node.getModifiers();
        if (modifiers && modifiers.find(m => m.isKind(tsm.SyntaxKind.DeclareKeyword))) {
            return true;
        }
        if (parent && (<tsm.ModuleChildableNode><any>node).getParentModule) {
            return this.isDeclare((<tsm.ModuleChildableNode><any>node).getParentModule(), true);
        }
        return false;
    }
    public isExport(node: tsm.ModifierableNode, parent?: boolean): boolean {
        let modifiers = node.getModifiers();
        if (modifiers && modifiers.find(m => m.isKind(tsm.SyntaxKind.ExportKeyword))) {
            return true;
        }
        if (parent && (<tsm.ModuleChildableNode><any>node).getParentModule) {
            return this.isExport((<tsm.ModuleChildableNode><any>node).getParentModule(), true);
        }
        return false;
    }
    public isAbstract(node: tsm.ModifierableNode): boolean {
        let modifiers = node.getModifiers();
        if (modifiers && modifiers.find(m => m.isKind(tsm.SyntaxKind.AbstractKeyword))) {
            return true;
        }
        return false;
    }
    public isStatic(node: tsm.ModifierableNode): boolean {
        let modifiers = node.getModifiers();
        if (modifiers && modifiers.find(m => m.isKind(tsm.SyntaxKind.StaticKeyword))) {
            return true;
        }
        return false;
    }
    public isPrivateOrProtected(node: tsm.ModifierableNode) {
        let modifiers = node.getModifiers();
        if (modifiers) {
            for (let m of modifiers) {
                if (m.asKind(tsm.SyntaxKind.ProtectedKeyword) !== undefined ||
                    m.asKind(tsm.SyntaxKind.PrivateKeyword) !== undefined) {
                    return true;
                }
                if (m.asKind(tsm.SyntaxKind.ProtectedKeyword) !== undefined) {
                    return false;
                }
            }
        }
        return false;
    }
    public isPublic(node: tsm.ModifierableNode): boolean {
        return !this.isPrivateOrProtected(node);
    }

    /**当前声明类型为xor.TsComponent */
    public isTsComponent(node: tsm.ClassDeclaration) {
        let info = util.getTypeInfo(node);
        if (info.className === Classes.TsComponent && info.moduleName === Module.Global) {
            return true;
        }
        return false;
    }
    /**是否继承自xor.TsComponent类
     * @param node 
     * @returns 
     */
    public isInheritTsComponent(node: tsm.ClassDeclaration) {
        if (!node) {
            return false;
        }
        if (this.isTsComponent(node)) {
            return true;
        }
        return this.isInheritTsComponent(node.getBaseClass());
    }

    /**获取类型的基础信息
     * @param node 
     * @returns 
     */
    public getTypeInfo(node: tsm.Node) {
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
                    filepath = this.getDeclaration(node)?.getSourceFile().getFilePath();
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
    /**获取类型声明实例 */
    public getDeclaration(node: tsm.Node | tsm.Type): Declaration {
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
    public getGuidDecorator(node: tsm.ClassDeclaration): string {
        let decorators = node.getDecorators();
        if (!decorators) {
            return null;
        }
        let firstArgument = decorators
            .find(d => d.getFullName() === Decorator.Guid)
            ?.getArguments().at(0);
        if (firstArgument && firstArgument.isKind(tsm.SyntaxKind.StringLiteral)) {
            return firstArgument.getLiteralValue()
        }
        return null;
    }
    public getFieldDecorator(node: tsm.PropertyDeclaration) {
        let decorators = node.getDecorators();
        return decorators?.find(d => d.getFullName() === Decorator.Field);
    }
    public getFieldArguments(node: tsm.PropertyDeclaration) {
        let decorator = this.getFieldDecorator(node);
        if (!decorator)
            return null;

        let type: tsm.PropertyAccessExpression, range: [min: number, max: number], value: any;

        let firstArgument = decorator.getArguments()?.at(0);
        if (firstArgument) {
            if (firstArgument.isKind(tsm.SyntaxKind.ObjectLiteralExpression)) {
                for (let property of firstArgument.getProperties()) {
                    if (!property.isKind(tsm.SyntaxKind.PropertyAssignment))
                        continue;
                    let initializer = property.getInitializer();
                    if (initializer.isKind(tsm.SyntaxKind.NullKeyword) || initializer.isKind(tsm.SyntaxKind.UndefinedKeyword))
                        continue;
                    let name = property.getName();
                    switch (name) {
                        case "type":
                            type = <tsm.PropertyAccessExpression>initializer;
                            break;
                        case "range":
                            let [min, max] = (<tsm.ArrayLiteralExpression>initializer).getElements();
                            if (min && min.isKind(tsm.SyntaxKind.NumericLiteral) &&
                                max && max.isKind(tsm.SyntaxKind.NumericLiteral)) {
                                range = [util.getExpressionValue(min), util.getExpressionValue(max)];
                            }
                            break;
                        case "value":
                            value = util.getExpressionValue(initializer);
                            break;
                    }
                }
            }
            else if (firstArgument.isKind(tsm.SyntaxKind.PropertyAccessExpression)) {
                type = firstArgument;
            }
        }
        return { type, range, value };
    }

    /**解析Expression并获取其值: 基础类型丶C#类型或其数组类型
     * @param node 
     * @param depth 
     * @returns 
     */
    public getExpressionValue(node: tsm.Expression, depth: number = 0) {
        if (depth > 3)
            return null;

        let result: any;
        if (node.isKind(tsm.SyntaxKind.StringLiteral) ||
            node.isKind(tsm.SyntaxKind.NumericLiteral) ||
            node.isKind(tsm.SyntaxKind.BigIntLiteral) ||
            node.isKind(tsm.SyntaxKind.TrueKeyword) ||
            node.isKind(tsm.SyntaxKind.FalseKeyword)) {
            result = node.getLiteralValue();
        }
        else if (node.isKind(tsm.SyntaxKind.ArrayLiteralExpression)) {
            result = this.toCSharpArray(node.getElements().map(e => this.getExpressionValue(e, depth + 1)));
        }
        else if (node.isKind(tsm.SyntaxKind.NewExpression)) {
            result = this.newCSharpObject(node, depth + 1);
        }
        else if (node.isKind(tsm.SyntaxKind.PropertyAccessExpression)) {
            result = this.accessPropertyValue(node, depth + 1);
        }

        return result;
    }
    /**获取声明成员值: 基础类型丶C#类型或其数组类型
     * @param declaration 
     * @param member 
     * @param depth 
     */
    public getDeclarationMemberValue(declaration: Declaration, member: string | [name: string, isStatic: boolean], depth: number = 0) {
        let memberName: string, isStatic: boolean;
        if (Array.isArray(member)) {
            memberName = member[0];
            isStatic = !!member[1];
        } else {
            memberName = member;
            isStatic = true;
        }

        let result: any;
        if (declaration.isKind(tsm.SyntaxKind.ClassDeclaration)) {
            for (let member of declaration.getMembers()) {
                if (member.isKind(tsm.SyntaxKind.PropertyDeclaration) &&
                    member.getInitializer() &&
                    member.getName() === memberName &&
                    member.isStatic() === isStatic) {
                    result = this.getExpressionValue(member.getInitializer(), depth);
                    break;
                }
            }
        }
        else if (declaration.isKind(tsm.SyntaxKind.EnumDeclaration)) {
            let initializerValue = 0;
            for (let member of declaration.getMembers()) {
                let value = member.getInitializer() ? this.getExpressionValue(member.getInitializer(), depth) : initializerValue;
                if (memberName === member.getName()) {
                    result = value;
                    break;
                }
                if (typeof (value) === "number") {
                    initializerValue = value + 1;
                }
            }
        }
        return result;
    }
    /**构建C#对象
     * @param node 
     * @param depth 
     */
    public newCSharpObject(node: tsm.NewExpression, depth: number = 0) {
        let typeExpression = node.getExpression();
        //目标是否为C#类型?
        let { moduleName, className } = this.getTypeInfo(typeExpression);
        if (util.isCSharpTypeDeclare(moduleName, className)) {
            let val: any = csharp;
            util.getCSharpTypeName(moduleName, className).split(".").forEach(name => {
                if (!val) return;
                val = val[name];
            });
            if (val) {
                try {
                    let agrs = node.getArguments().map(arg => this.getExpressionValue(<tsm.Expression>arg, depth));
                    return new val(...agrs);
                } catch (e) {
                    console.warn(e);
                }
            }
        }
        return null;
    }
    /**访问成员字段并获取其值: 基础类型丶C#类型或其数组类型
     * @param node 
     * @param depth 
     */
    public accessPropertyValue(node: tsm.PropertyAccessExpression, depth: number = 0) {
        let propertyName = node.getName(), typeExpression = node.getExpression();

        let result: any;
        //目标是否为C#类型?
        let { moduleName, className } = this.getTypeInfo(typeExpression);
        if (util.isCSharpTypeDeclare(moduleName, className)) {
            let val: any;
            if (typeExpression.isKind(tsm.SyntaxKind.NewExpression)) {
                val = this.newCSharpObject(typeExpression, depth);
                if (val) val = val[propertyName];
            }
            else {
                val = csharp;
                try {
                    [...util.getCSharpTypeName(moduleName, className).split("."), propertyName].forEach(name => {
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
        //目标为ts中的类型(非C#类型)
        else {
            let typeDeclaration = this.getDeclaration(typeExpression);
            if (typeDeclaration) {
                let isStatic = !typeExpression.isKind(tsm.SyntaxKind.NewExpression);
                result = this.getDeclarationMemberValue(typeDeclaration, [propertyName, isStatic], depth);
            }
        }
        return result;
    }

    private _baseTypes: { [t: string]: Type } = {
        ["string"]: puerts.$typeof(csharp.System.String),
        ["number"]: puerts.$typeof(csharp.System.Double),
        ["boolean"]: puerts.$typeof(csharp.System.Boolean),
        ["bigint"]: puerts.$typeof(csharp.System.Int64),
    };
    /**转为对应的C#类型
     * @param node 
     * @param depth 
     * @returns 
     */
    public toCSharpType(node: tsm.Node | [type: tsm.Node, explicitType: tsm.Node], depth: number = 0)
        : { type: Type, enumerable: Map<string, string | number>, references: Map<string, string> } {
        if (depth > 3) {
            return null;
        }
        let _node: tsm.Node, _explicit: tsm.Node;
        if (Array.isArray(node)) {
            _node = node[0];
            _explicit = node[1];
        } else {
            _node = node;
        }
        if (!_node && !_explicit) {
            return null;
        }

        let type: Type, enumerable: Map<string, string | number>, references: Map<string, string>;
        if (_node.isKind(tsm.SyntaxKind.ParenthesizedType)) {
            return this.toCSharpType([_node.getTypeNode(), _explicit], depth + 1);
        }
        else if (_node.isKind(tsm.SyntaxKind.ArrayType)) {
            let e = this.toCSharpType([_node.getElementTypeNode(), _explicit], depth + 1);
            if (e && e.type) {
                type = csharp.System.Array.CreateInstance(e.type, 0).GetType();
                enumerable = e.enumerable;
                references = e.references;
            }
        }
        else if (_node.isKind(tsm.SyntaxKind.UnionType)) {
            let typeNodes = _node.getTypeNodes();
            //如果是基础数据类型: string, number, bigint, boolean
            if (typeNodes.every(n => n.isKind(tsm.SyntaxKind.LiteralType))) {
                let members: Array<number | string> = typeNodes.map(n => this.getExpressionValue((<tsm.LiteralTypeNode>n).getLiteral()));
                let ce = this.toCSharpEnumerable(members.map(value => ({ name: `${value}`, value })));
                if (ce) {
                    enumerable = ce.enumerable;
                    type = ce.type;
                }
            }
            //如果是XOR.TsComponent声明
            else {
                references = new Map();
                typeNodes.forEach(n => {
                    let _node = this.getDeclaration(n);
                    if (!_node || !_node.isKind(tsm.SyntaxKind.ClassDeclaration) || !this.isInheritTsComponent(_node))
                        return;
                    let guid = this.getGuidDecorator(_node);
                    if (guid || this.isTsComponent(_node)) {
                        type ??= puerts.$typeof(csharp.XOR.TsComponent);
                        references.set(guid, _node.getName());
                    }
                });
            }
        }
        else if (_node.isKind(tsm.SyntaxKind.EnumDeclaration)) {
            let initValue = 0;
            let members = _node.getMembers().map(m => {
                let value = m.getInitializer() ? this.getExpressionValue(m.getInitializer()) : initValue;
                if (typeof (value) === "number") {
                    initValue = value + 1;
                }
                return {
                    name: m.getName(),
                    value
                }
            });
            let ce = this.toCSharpEnumerable(members);
            if (ce) {
                enumerable = ce.enumerable;
                type = ce.type;
            }
        }
        else {
            let { className, moduleName } = this.getTypeInfo(_explicit ?? _node);
            if (moduleName === Module.Global && className in this._baseTypes) {
                type = this._baseTypes[className];
            }
            else if (this.isCSharpTypeDeclare(moduleName, className)) {
                type = csharp.XOR.ReflectionUtil.GetType(util.getCSharpTypeName(moduleName, className));
            }
            else if (_node.isKind(tsm.SyntaxKind.TypeReference)) {
                if (_node.getTypeName().getText() === "Array" && _node.getTypeArguments().length === 1) {
                    let element = this.toCSharpType([_node.getTypeArguments().at(0), _explicit], depth + 1);
                    if (element && element.type) {
                        type = csharp.System.Array.CreateInstance(element.type, 0).GetType();
                        enumerable = element.enumerable;
                        references = element.references;
                    }
                }
                else {
                    let declaration = this.getDeclaration(_node);
                    if (declaration) {
                        return this.toCSharpType(declaration, depth + 1);
                    }
                }
            }
            else if (_node.isKind(tsm.SyntaxKind.ClassDeclaration) && this.isInheritTsComponent(_node)) {
                let guid = this.getGuidDecorator(_node);
                if (guid) {
                    references = new Map();
                    references.set(guid, _node.getName());
                }
                if (guid || this.isTsComponent(_node)) {
                    type = puerts.$typeof(csharp.XOR.TsComponent);
                }
            }
        }
        return { type, enumerable, references };
    }
    public toCSharpTypeByTypeNode(node: tsm.TypeNode): Type {
        if (node.isKind(tsm.SyntaxKind.BooleanKeyword)) {
            return this._baseTypes["boolean"];
        }
        else if (node.isKind(tsm.SyntaxKind.NumberKeyword)) {
            return this._baseTypes["number"];
        }
        else if (node.isKind(tsm.SyntaxKind.StringKeyword)) {
            return this._baseTypes["string"];
        }
        else if (node.isKind(tsm.SyntaxKind.BigIntKeyword)) {
            return this._baseTypes["bigint"];
        }
        return util.toCSharpType(util.getDeclaration(node))?.type;
    }
    /**转为C#数组类型
     * @param array 
     * @returns 
     */
    public toCSharpArray(array: any[]) {
        let firstIndex = array?.findIndex(e => e !== undefined && e !== null && e !== void 0) ?? -1;
        if (firstIndex >= 0) {
            let result: csharp.System.Array, firstObj = array[firstIndex];
            switch (typeof (firstObj)) {
                case "string":
                    result = csharp.System.Array.CreateInstance(puerts.$typeof(csharp.System.String), array.length);
                    break;
                case "number":
                    result = csharp.System.Array.CreateInstance(puerts.$typeof(csharp.System.Double), array.length);
                    break;
                case "boolean":
                    result = csharp.System.Array.CreateInstance(puerts.$typeof(csharp.System.Boolean), array.length);
                    break;
                case "bigint":
                    result = csharp.System.Array.CreateInstance(puerts.$typeof(csharp.System.Int64), array.length);
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
    }
    /**转为C#枚举类型
     * @param members 
     * @returns 
     */
    public toCSharpEnumerable(members: Array<{ name: string, value: string | number }>) {
        if (!members || members.length === 0 || !members.every(({ value: v }) => typeof (v) === "string" || typeof (v) === "number" || typeof (v) === "bigint")) {
            return null;
        }
        let type: Type, enumerable = new Map<string, string | number>();
        if (members.every(m => typeof (m.value) === "number")) {
            type = puerts.$typeof(csharp.System.Int32);
            members.forEach((({ name, value }) => enumerable.set(name, value)));
        } else {
            type = puerts.$typeof(csharp.System.String);
            members.forEach((({ name, value }) => enumerable.set(name, `${value}`)));
        }
        return { type, enumerable };
    }

    /**是否为C#类型声明
     * @param moduleName 
     * @param filePath 
     */
    public isCSharpTypeDeclare(moduleName: string, className: string) {
        return moduleName === Module.CSharp || moduleName === Module.CS ||
            moduleName === Module.Global && className.startsWith("CS.");
    }
    /**获取C#类型名称
     * @param filePath 
     * @param moduleName 
     * @param className 
     */
    public getCSharpTypeName(moduleName: string, className: string) {
        if (moduleName === Module.Global && className.startsWith("CS.")) {
            className = className.substring(3);
        }
        return className;
    }

    /**尝试读取文件
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
    /**读取tsconfig文件 */
    public readTsconfig(path: string): TsconfigFile {
        let { config, error } = tsm.ts.readConfigFile(path, (path) => File.ReadAllText(path));
        if (error) {
            throw error;
        }
        return config;
    }
}
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