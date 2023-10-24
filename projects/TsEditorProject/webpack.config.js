const path = require("path");
const webpack = require('webpack');

/**
 * @type webpack.Configuration
 */
module.exports = {
    mode: "production",
    //mode: "development",
    //devtool: false, //"inline-source-map",
    target: "node",
    entry: {
        main: path.resolve(__dirname, "./src/main/main.ts"),
        child: path.resolve(__dirname, "./src/child/main.ts"),
    },
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
    output: {
        filename: "[name].js",
        path: path.resolve(__dirname, "./output/webpack")
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