const path = require("path");
const webpack = require('webpack');

/**
 * @type webpack.Configuration
 */
module.exports = {
    entry: {
        main: path.resolve(__dirname, "./src/main.ts"),
    },
    /**@type {"production" | "development"} */
    mode: "development",
    /**@type {false | "source-map" | "inline-source-map"}  */
    devtool: "source-map",
    
    module: {   // new add +
        rules: [{
            test: /\.tsx?$/,
            use: "ts-loader",
            exclude: /node_modules/,
        }]
    },
    resolve: {
        extensions: [".ts", ".js"]
    },
    output: {
        filename: "[name].bundle.js",
        path: path.resolve(__dirname, "./output")
    },
    /** 忽略编辑的第三方库 */
    externals: {
        csharp: "commonjs2 csharp",
        puerts: "commonjs2 puerts",
        fs: "commonjs2 fs",
        path: "commonjs2 path"
    }
};