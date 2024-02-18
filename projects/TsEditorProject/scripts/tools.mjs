import fs from "fs";
import minimist from "minimist";
import path from "path";
import { copySync, getRootPath, webpackCompile, webpackConfigure } from "./_common.mjs";

//获取typescript项目根路径
const rootPath = getRootPath();
//编译输出目录
const outputPath = path.resolve(rootPath, "./output/webpack-tools");

(async () => {
    //读取命令行配置
    const options = minimist(process.argv.slice(2));

    const method = options['method'];
    const isPublish = !!options['publish']; //是否为发布模式
    const isDevelopment = !isPublish && !!(options['dev'] ?? options['development']);    //是否为开发者模式
    const isWatch = !isPublish && !!options['watch'];       //是否为watch模式

    //目录信息
    const compileConfigure = { ...webpackConfigure };
    let publishPath = '';
    switch (method) {
        case 'link.xml':
        case 'mini.link.xml':
            compileConfigure.entry = {
                ['link.xml']: path.resolve(rootPath, "./src/tools/mini.link.xml/main.ts"),
            };
            publishPath = path.resolve(rootPath, "../Assets/Samples/Editor/MiniLinkXml/Resources/puerts/xor-tools");
            break;
    }
    if (!publishPath || !compileConfigure.entry) {
        throw new Error(`invaild operation method: ${method}`);
    }

    console.clear();
    console.log(`
╔════TOOLS COMPILE═══════════════════════════════════════╗
║mode: isPublish=${isPublish}, isDevelopment=${isDevelopment}, isWatch=${isWatch}
║method: ${method}
║rootPath: ${rootPath}
║outputPath: ${outputPath}
║publishPath: ${publishPath}
╚════════════════════════════════════════════════════════╝
`.trim());

    //执行webpack编译
    await webpackCompile({
        ...compileConfigure,
        watch: !!isDevelopment,
        mode: isDevelopment ? 'development' : 'production',
        devtool: isDevelopment ? 'inline-source-map' : false,
        output: {
            filename: "[name].js",
            path: outputPath,
        },
    }, isWatch);
    if (!isPublish)
        return;
    
    //删除旧文件
    if (fs.existsSync(publishPath)) {
        fs.rmdirSync(publishPath, { recursive: true });
    }
    //复制文件至目标路径
    const extname = '.cjs', fileName = Object.keys(compileConfigure.entry)[0];
    copySync(
        outputPath,
        publishPath,
        {
            filter: (name) => name && name.includes(fileName),
            resolve: (name) => name.endsWith(".js") ? `${name.substring(0, name.length - 3)}${extname}` : name,
        }
    );
})();