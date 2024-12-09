import child_process from "child_process";
import * as fs from "fs";
import minimist from "minimist";
import * as path from "path";
import { copySync, getRootPath } from "./_common.mjs";

//获取typescript项目根路径
const rootPath = getRootPath();
//编译输出目录
const outputPath = path.resolve(rootPath, "./output/xor");
//脚本发布路径
const publishPaths = {
    //运行时路径
    runtime: path.resolve(rootPath, "../Assets/XOR/Runtime/Resources/puerts/xor"),
    //声明文件路径
    declaration: path.resolve(rootPath, "../Assets/XOR/Typing/puerts/xor"),
};

(async () => {
    //读取命令行配置
    const options = minimist(process.argv.slice(2));

    const isPublish = !!options['publish'];     //是否为发布模式
    const isESM = !!(options['esm'] ?? options['esmodule']); //是否为esm模块

    console.clear();
    console.log(`
╔════RUNTIME COMPILE═════════════════════════════════════╗
║mode: isPublish=${isPublish}, isESM=${isESM}
║rootPath: ${rootPath}
║outputPath: ${outputPath}
║publishPaths: 
║   ${publishPaths.runtime}
║   ${publishPaths.declaration}
╚════════════════════════════════════════════════════════╝
`.trim());

    //执行tsc编译命令
    try {
        const stdio = child_process.execSync(`npx tsc -p tsconfig.json --module ${isESM ? 'ES6' : 'CommonJS'}`, {
            cwd: rootPath,
            encoding: 'utf8',
            stdio: 'pipe',
            env: {
                ...process.env,
            }
        });
        if (stdio) {
            console.log(stdio);
        }
    } catch (e) {
        throw e;
    }
    if (!isPublish)
        return;

    //清空目标路径
    if (fs.existsSync(publishPaths.runtime)) {
        fs.rmdirSync(publishPaths.runtime, { recursive: true });
    }
    if (fs.existsSync(publishPaths.declaration)) {
        fs.rmdirSync(publishPaths.declaration, { recursive: true });
    }
    //复制文件至目标路径
    const extname = isESM ? '.mjs' : '.cjs';
    copySync(
        outputPath,
        publishPaths.runtime,
        {
            filter: (name) => name.endsWith('.js') || name.endsWith('.cjs') || name.endsWith('.mjs'),
            resolve: (name) => name.endsWith(".js") ? `${name.substring(0, name.length - 3)}${extname}` : name,
        }
    );
    copySync(
        outputPath,
        publishPaths.declaration,
        {
            filter: (name) => name.endsWith('.d.ts'),
        }
    );
})();