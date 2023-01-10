import * as fs from "fs";
import * as path from "path";
import { program } from "commander";
import minimist from "minimist";

/**
 * @type type T= {a:string, b}
 */

const commands = new class {
    /**复制文件夹
     * @param {minimist.ParsedArgs} options 
     */
    copydir(options) {
        let { source, target, append = '' } = options;
        if (!source || !target)
            throw new Error("必须传入--source和--target参数");

        let __dirname = path.dirname(import.meta.url.replace("file:///", ""));
        if (source.startsWith(".")) {
            source = path.join(__dirname, source);
        }
        if (target.startsWith(".")) {
            target = path.join(__dirname, target);
        }

        //是否递归处理文件
        let recursion = !("recursion" in options) || !!options.recursion;
        /**@type {string[]} 选中的文件扩展名 */
        let extnames = ("filter" in options) ? options.filter.split("/") : null;
        //如何处理文件
        let resolve = (path) => `${path}${append}`;

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
}

/**
 * 
 * @param {minimist.ParsedArgs} options 
 */
function copydir(options) {
    program.requiredOption("--source <source>", "缺少必要的参数: 源文件路径");
    program.requiredOption("--target <target>", "缺少必要的参数: 目标文件路径");
    program.option("--filter <filter>", "指定文件后缀名", ".js|.mjs");
    program.option("--recursion <recursion>", "递归文件夹", "true");
    program.option("--append <append>", "追加文件名");

    program.parse(process.argv);
    //const options = program.opts();

    let { source, target, filter, recursion, append = '' } = options;

    /**@type {string[]} */
    let filters = filter.split("|");
    /**是否过滤文件
     * @param {string} extname 
     */
    function isFilters(extname) {
        if (filters && filters.length > 0 && !filters.includes(extname)) {
            return false;
        }
        return true;
    }
    /**
     * @param {string} source 
     * @param {string} target 
     * @param {boolean} recursion 
     */
    function copy(source, target, recursion) {
        let stat = fs.statSync(source);
        if (stat.isFile()) {
            if (!isFilters(path.extname(source)))
                return;
            let dirpath = path.dirname(target);
            if (!fs.existsSync(dirpath)) {
                fs.mkdirSync(dirpath, { recursive: true });
            }
            fs.copyFileSync(source, `${target}${append}`);
        } else if (stat.isDirectory()) {
            for (let dirname of fs.readdirSync(source)) {
                copy(
                    path.join(source, dirname),
                    path.join(target, dirname),
                    recursion
                );
            }
        }

    }
    let dirname = path.dirname(import.meta.url.replace("file:///", ""));
    copy(
        path.join(dirname, source),
        path.join(dirname, target),
        recursion !== "false"
    );
    console.log("complete.");
}

const options = minimist(process.argv.slice(2));
const method = options["_"].find(m => m in commands);
if (method) {
    commands[method](options);
    console.log(`command completed.`);
} else {
    console.error("无效的命令参数");
}