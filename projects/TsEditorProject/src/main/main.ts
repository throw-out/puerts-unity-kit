import * as csharp from "csharp";

require("puerts/console-track");
//require("puerts/puerts-source-map-support");

const { Path } = csharp.System.IO;
const { Application } = csharp.UnityEngine;

const loader = new csharp.XOR.MergeLoader();

let projectRoot = Path.Combine(Path.GetDirectoryName(Application.dataPath), "TsEditorProject");
let outputRoot = Path.Combine(projectRoot, "output");
loader.AddLoader(new csharp.XOR.FileLoader(outputRoot, projectRoot));

const options = new csharp.XOR.ThreadWorker.CreateOptions();
options.remote = true;
options.isEditor = true;

const worker = new xor.ThreadWorker(loader, options);
worker.start("./child/main");

xor.globalListener.quit.add(() => worker.stop());

//console.log("main thread ready.");
//setInterval(() => console.log("main thread active:"), 1000);

function start(currentOutputPath: string, targetProjectPath: string) {
    const options = new csharp.XOR.ThreadWorker.CreateOptions();
    options.remote = true;
    options.isEditor = true;

    const loader = new csharp.XOR.MergeLoader();
    loader.AddLoader(new csharp.XOR.FileLoader(currentOutputPath, Path.GetDirectoryName(currentOutputPath)));

    const worker = new xor.ThreadWorker(loader, options);
    worker.start("./child/main");

    xor.globalListener.quit.add(() => worker.stop());
}
const r = new class {
    private _worker: xor.ThreadWorker;

    public start(currentOutputPath: string, targetProjectPath: string) {
        this._worker = this._create(currentOutputPath);
    }

    private _create(curOutputPath: string) {
        const options = new csharp.XOR.ThreadWorker.CreateOptions();
        options.remote = true;
        options.isEditor = true;

        const loader = new csharp.XOR.MergeLoader();
        loader.AddLoader(new csharp.XOR.FileLoader(curOutputPath, Path.GetDirectoryName(curOutputPath)));

        const worker = new xor.ThreadWorker(loader, options);
        worker.start("./child/main");

        xor.globalListener.quit.add(() => worker.stop());

        return worker;
    }
}