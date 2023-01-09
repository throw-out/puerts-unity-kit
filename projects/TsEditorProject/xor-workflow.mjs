import * as fs from "fs";
import * as path from "path";
import { program } from "commander";




function copydir() {
    program.requiredOption("--source <source>", "缺少必要的参数: 源文件路径");
    program.requiredOption("--target <target>", "缺少必要的参数: 目标文件路径");
    program.option("--filter <filter>", "指定文件后缀名", ".js|.mjs");
    program.option("--recursion <recursion>", "递归文件夹", "true");
    program.option("--append <append>", "追加文件名");

    program.parse(process.argv);
    const options = program.opts();

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

const commandList = {
    ["copydir"]: copydir
}

const exc = commandList[process.argv[2]];
if (exc) {
    exc();
} else {
    console.error("无效的命令参数");
}