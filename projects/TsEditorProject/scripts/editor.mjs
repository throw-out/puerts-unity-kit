import * as fs from "fs";
import minimist from "minimist";
import * as path from "path";
import { copySync, getRootPath, webpackCompile, webpackConfigure } from "./_common.mjs";

const rootPath = getRootPath();
//编译输出目录
const outputPath = path.resolve(rootPath, "./output/webpack-editor");
//脚本发布路径
const publishPath = path.resolve(rootPath, "../Assets/XOR/Editor/Resources/puerts/xor-editor");

(async () => {
    //读取命令行配置
    const options = minimist(process.argv.slice(2));

    const isPublish = !!options['publish']; //是否为发布模式
    const isDevelopment = !isPublish && !!(options['dev'] ?? options['development']);    //是否为开发者模式
    const isWatch = !isPublish && !!options['watch'];       //是否为watch模式

    console.clear();
    console.log(`
╔════EDITOR COMPILE══════════════════════════════════════╗
║mode: isPublish=${isPublish}, isDevelopment=${isDevelopment}, isWatch=${isWatch}
║rootPath: ${rootPath}
║outputPath: ${outputPath}
║publishPath: ${publishPath}
╚════════════════════════════════════════════════════════╝
`.trim());

    //执行webpack编译
    await webpackCompile({
        ...webpackConfigure,
        watch: !!isDevelopment,
        mode: isDevelopment ? 'development' : 'production',
        devtool: isDevelopment ? 'inline-source-map' : false,
        entry: {
            main: path.resolve(rootPath, "./src/editor/main/main.ts"),
            child: path.resolve(rootPath, "./src/editor/child/main.ts"),
        },
        output: {
            filename: "[name].js",
            path: outputPath,
        }
    }, isWatch);
    if (!isPublish)
        return;

    //清空目标路径
    if (fs.existsSync(publishPath)) {
        fs.rmdirSync(publishPath, { recursive: true });
    }
    //复制文件至目标路径
    const extname = '.cjs';
    copySync(
        outputPath,
        publishPath,
        {
            resolve: (name) => name.endsWith(".js") ? `${name.substring(0, name.length - 3)}${extname}` : name,
        }
    );
})();


