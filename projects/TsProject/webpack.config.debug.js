const path = require('path');
const webpack = require('webpack');

/**
 * @type webpack.Configuration
 */
module.exports = {
    mode: "development",
    entry: {
        main: path.resolve(__dirname, "./src/main.ts"),
    },
    //devtool: "inline-source-map",
    devtool: "source-map",
    module: {   // new add +
        rules: [{
            test: /\.tsx?$/,
            use: "ts-loader",
            exclude: /node_modules/,
        }]
    },
    resolve: { // new add +
        extensions: [".tsx", ".ts", ".js"]
    },
    output: {
        filename: "[name].bundle.debug.js",
        path: path.resolve(__dirname, "./output")
    },
    /** 忽略编辑的第三方库 */
    externals: {
        csharp: "commonjs2 csharp",
        puerts: "commonjs2 puerts",
        fs: "commonjs2 fs",
        path: "commonjs2 path",
        "csharp.System": "commonjs2 csharp.System",
        "csharp.UnityEngine": "commonjs2 csharp.UnityEngine",
        "csharp.UnityEngine.UI": "commonjs2 csharp.UnityEngine.UI",

        "puerts/console-track": "commonjs2 puerts/console-track",
        "puerts/puerts-source-map-support": "commonjs2 puerts/puerts-source-map-support",
    }
};