import fs from "fs";
import path from "path";
import * as url from "url";
import webpack from "webpack";

/**默认webpack编译配置
 * @type webpack.Configuration */
export const webpackConfigure = {
    target: "node",
    module: {
        rules: [{
            test: /\.tsx?$/,
            use: "ts-loader",
            exclude: /node_modules/,
        }]
    },
    resolve: {
        extensions: [".ts", "..."]
    },
    /** 忽略编辑的第三方库 */
    externals: {
        csharp: "commonjs2 csharp",
        puerts: "commonjs2 puerts",
        os: "commonjs2 os",
        stream: "commonjs2 stream",
        fs: "commonjs2 fs",
        path: "commonjs2 path",
        util: "commonjs2 util",
        perf_hooks: "commonjs2 perf_hooks",

        "puerts/console-track": "commonjs2 puerts/console-track",
        "puerts/puerts-source-map-support": "commonjs2 puerts/puerts-source-map-support",
    }
};

/**执行webpack编译
 * @param {webpack.Configuration} configure 
 * @param {boolean} isWatch 
 */
export async function webpackCompile(configure, isWatch) {
    if (!configure) {
        throw new Error(`invaild webpack configure: ${JSON.stringify(configure)}`);
    }
    try {
        const complier = webpack(configure);
        if (isWatch) {
            const watching = complier.watch({}, (err, stats) => {
                console.clear();
                console.log(`webpack complie: ${stats.endTime - stats.startTime}ms`);
                if (stats.hasErrors()) {
                    console.log(stats.toString());
                }
            });
            //watching.close();
        } else {
            await new Promise((resolve, reject) => {
                complier.run((err, stats) => {
                    if (err) {
                        reject(err);
                        return;
                    }
                    resolve();
                    console.log(`webpack complie: ${stats.endTime - stats.startTime}ms`);
                    if (stats.hasErrors()) {
                        console.log(stats.toString());
                    }
                });
            });
            //complier.close();
        }
    } catch (e) {
        throw new Error(`webpack complie failure: ${e}`);
    }
}

/**以递归方式复制文件或文件夹
 * @param {string} sourcePath 
 * @param {string} targetPath 
 * @param {object} options
 * @param {(name:string)=>boolean} options.filter 
 * @param {(name:string)=>string} options.resolve 
 */
export function copySync(sourcePath, targetPath, options) {
    let stat = fs.statSync(sourcePath);
    if (stat.isFile()) {
        let { filter, resolve } = options;
        if (filter && !filter(sourcePath))
            return;

        let dirpath = path.dirname(targetPath);
        if (!fs.existsSync(dirpath)) {
            fs.mkdirSync(dirpath, { recursive: true });
        }

        fs.copyFileSync(sourcePath, resolve ? resolve(targetPath) : targetPath);
        return;
    }
    if (stat.isDirectory()) {
        for (let name of fs.readdirSync(sourcePath)) {
            let _sourcePath = path.join(sourcePath, name),
                _targetPath = path.join(targetPath, name);
            copySync(_sourcePath, _targetPath, options);
        }
    }
}

const rootPath = path.resolve(path.dirname(url.fileURLToPath(import.meta.url)), "../");
/**获取typescript项目根路径 */
export function getRootPath() {
    return rootPath;
}



