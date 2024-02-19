const path = require("path");
require("../output/webpack-tools/link.xml");

//获取generate方法
const generate = (function () {
    var _g = global || globalThis || this;
    if (!_g || !_g.generate || typeof (_g.generate) !== 'function')
        throw new Error(`invaild environment`);
    return _g.generate;
})();

setTimeout(() => {
    const tsconfigFile = path.resolve(__dirname, "../../TsProject/tsconfig.json");
    const { types, underlyingTypes, callMethods } = generate(tsconfigFile);
    console.log("========================================================");
    console.log(types?.join(', '));
    console.log("========================================================");
    console.log(underlyingTypes?.join(', '));
    console.log("========================================================");
    if (callMethods) {
        console.log([...callMethods.keys()].map(typeName => `${typeName}(${callMethods.get(typeName).length})`).join(', '));
    }
    console.log("========================================================");
}, 1000);