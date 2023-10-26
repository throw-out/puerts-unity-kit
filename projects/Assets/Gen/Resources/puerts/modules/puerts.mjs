
const puerts = (function () {
    let _g = this || global || globalThis;
    return _g['puerts'] || require('puerts');
})();

export default puerts;


export const loadType = puerts.loadType;
export const getNestedTypes = puerts.getNestedTypes;
export const getGenericMethod = puerts.getGenericMethod;
export const evalScript = puerts.evalScript;
export const getLastException = puerts.getLastException;
export const loadFile = puerts.loadFile;
export const fileExists = puerts.fileExists;
export const console = puerts.console;
export const __$NamespaceType = puerts.__$NamespaceType;
export const $ref = puerts.$ref;
export const $unref = puerts.$unref;
export const $set = puerts.$set;
export const $promise = puerts.$promise;
export const $generic = puerts.$generic;
export const $genericMethod = puerts.$genericMethod;
export const $typeof = puerts.$typeof;
export const $extension = puerts.$extension;
export const $reflectExtension = puerts.$reflectExtension;
export const on = puerts.on;
export const off = puerts.off;
export const emit = puerts.emit;
export const searchModule = puerts.searchModule;
export const genRequire = puerts.genRequire;
export const getModuleBySID = puerts.getModuleBySID;
export const registerBuildinModule = puerts.registerBuildinModule;
export const require = puerts.require;
