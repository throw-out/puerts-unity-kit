import * as fs from "fs";
import * as url from "url";
import * as path from "path";
import minimist from "minimist";

/**获取文件绝对路径
 * @param {string} target 
 */
function fullname(target) {
    if (target.startsWith(".")) {
        let __dirname = path.dirname(url.fileURLToPath(import.meta.url));
        target = path.join(__dirname, target);
    }
    return target;
}

/**
 * @type type T= {a:string, b}
 */

const commands = new class {
    /**删除文件夹
     * @param {minimist.ParsedArgs} options 
     */
    rmdir(options) {
        let { target } = options;
        target = fullname(target);
        if (!fs.existsSync(target) || !fs.statSync(target).isDirectory())
            return;
        this.#_rmdir(target);
    }
    /**复制文件夹
     * @param {minimist.ParsedArgs} options 
     */
    copydir(options) {
        let { source, target, extname } = options;
        if (!source || !target)
            throw new Error("必须传入--source和--target参数");
        source = fullname(source);
        target = fullname(target);

        //是否递归处理文件
        let recursion = !("recursion" in options) || !!options.recursion;
        /**@type {string[]} 选中的文件扩展名 */
        let extnames = ("filter" in options) ? options.filter.split("/") : null;
        //如何处理文件
        let resolve = extname ? (_p) => _p.replace(path.extname(_p), extname) : (_p) => _p;

        this.#_copydir(source, target, {
            recursion,
            extnames,
            resolve,
        });
    }

    /**复制文件夹
     * @param {string} source 
     * @param {string} target 
     * @param { { 
     *      recursion?: boolean,         
     *      resolve?: (path:string)=>string,
     *      extnames?: string[]
     * }} options 
     */
    #_copydir(source, target, options) {
        let stat = fs.statSync(source);
        if (stat.isFile()) {
            if (options && options.extnames && !options.extnames.includes(path.extname(target))) {
                return;
            }
            let dirpath = path.dirname(target);
            if (!fs.existsSync(dirpath)) {
                fs.mkdirSync(dirpath, { recursive: true });
            }
            if (options && options.resolve) {
                fs.copyFileSync(source, options.resolve(target));
            } else {
                fs.copyFileSync(source, target);
            }
        } else if (stat.isDirectory()) {
            for (let name of fs.readdirSync(source)) {
                let _source = path.join(source, name);
                if (fs.statSync(_source).isDirectory() && (!options || !options.recursion)) {
                    continue;
                }
                let _target = path.join(target, name);
                this.#_copydir(_source, _target, options);
            }
        }
    }
    /**递归删除文件夹
     * @param {string} dirpath 
     */
    #_rmdir(dirpath) {
        for (let file of fs.readdirSync(dirpath)) {
            let _path = path.join(dirpath, file);
            if (fs.statSync(_path).isDirectory()) {
                this.#_rmdir(_path);
            } else {
                fs.unlinkSync(_path);
            }
        }
        fs.rmdirSync(dirpath);
    }
}

const options = minimist(process.argv.slice(2));
const method = options["_"].find(m => m in commands);
if (method) {
    commands[method](options);
    console.log(`command completed.`);
} else {
    console.error("无效的命令参数");
}