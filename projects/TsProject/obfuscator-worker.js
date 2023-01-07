import JavaScriptObfuscator from 'javascript-obfuscator';

let inputSources = {
    //[filename]; sourceCode
};
let inputOptions = {
    //输出一行紧凑的代码
    compact: true,
    //代码扁平化, 使阅读更加困难(极大影响性能, 最高1.5倍)
    controlFlowFlattening: true,
    //控制扁平化程度, 0-1
    controlFlowFlatteningThreshold: 0.75,
    //添加随机死代码, 使阅读更加困难(极大增加代码大小, 最高2.0倍), 强制启用stringArray=true
    deadCodeInjection: true,
    //控制死代码影响的节点, 0-1
    deadCodeInjectionThreshold: 0.4,
    //将console改为空函数, 使调试更加困难
    disableConsoleOutput: true,

    //冻结浏览器, 使得几乎不可能打开开发人员工具功能
    debugProtection: false,
    debugProtectionInterval: 0,
    //浏览器专用, 仅允许特定域名或子域名上允许, 是的代码很难被复制在其他地方运行
    //例:www.example.com/example.com/sub.example.com/.example.com
    //domainLock: [],
    //浏览器专业, 源代码未在指定域名上运行时, 重定向到此域名
    //domainLockRedirectUrl:"",

    //文件名或globs, 不进行模糊处理的文件
    exclude: [],
    //字符串文本强制转化(允许RegExp匹配), 不影响stringArrayThreshold配置
    forceTransformStrings: [],
    //多个源文件模糊处理期间, 使用相同名称的标识名称
    /*identifierNamesCache: {
        globalIdentifiers: {},      //全局标识符: 所有全局标识符都将写入缓存; 所有匹配的未声明全局标识符将被缓存中的值替换;
        propertyIdentifiers: {},    //属性标识符, 仅当启用renameProperties时: 所有属性标识符将写入缓存; 所有匹配的属性标识符替换为缓存中的值;
    },
    //*/
    //标识符名称生成器: dictionary字典列表; hexadecimal十六进制; mangled短标识名称a丶b丶c; mangled-shuffled与mangled相同但带随机字母
    identifierNamesGenerator: 'hexadecimal',
    //设置字典列表,区分大小写: identifierNamesGenerator=dictionary时生效
    //identifiersDictionary:[],
    //全局标识符前缀, 避免多文件处理时的名称冲突
    //identifiersPrefix: '',
    //忽略导入混淆,某些原因需要静态字符串导入时, 这个选项将有所帮助
    //ignoreRequireImports: false,
    //内部用于源映射生成
    //inputFileName: '',
    //将信息记录到控制台
    log: false,
    //数字转换为表达式: const foo = 1234 = -0xd93+-0x10b4+0x41*0x67+0x84e*0x3+-0xff8;
    numbersToExpressions: true,
    //预设选项: default默认设置; low-obfuscation低混淆高性能; medium-obfuscation中等混淆最佳性能; high-obfuscation高等混淆最低性能;
    //optionsPreset:'default',
    //对全局变量和函数名模糊处理(将破坏代码, 谨慎使用)
    renameGlobals: false,
    //自我防御(以任何方式修改输出的代码都将使代码失效), 强制启用compact=true
    selfDefending: true,
    simplify: true,
    splitStrings: true,
    splitStringsChunkLength: 10,
    stringArray: true,
    stringArrayCallsTransform: true,
    stringArrayCallsTransformThreshold: 0.75,
    stringArrayEncoding: ['base64'],
    stringArrayIndexShift: true,
    stringArrayRotate: true,
    stringArrayShuffle: true,
    stringArrayWrappersCount: 2,
    stringArrayWrappersChainedCalls: true,
    stringArrayWrappersParametersMaxCount: 4,
    stringArrayWrappersType: 'function',
    stringArrayThreshold: 0.75,
    transformObjectKeys: true,
    unicodeEscapeSequence: false
};
JavaScriptObfuscator.obfuscateMultiple(inputSources, inputOptions);