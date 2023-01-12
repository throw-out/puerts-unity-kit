import * as csharp from "csharp";

require("puerts/console-track");
//require("puerts/puerts-source-map-support");

const { Path } = csharp.System.IO;
const { Application } = csharp.UnityEngine;

class Workflow {
    private worker: xor.ThreadWorker;
    private readonly ci: csharp.XOR.Services.CSharpInterfaces;

    constructor(ci: csharp.XOR.Services.CSharpInterfaces) {
        this.ci = ci;
    }
    public start(editorProject: string, project: string) {
        if (this.worker && this.worker.isAlive)
            throw new Error("invalid operation");

        const worker = this._createWorker(editorProject);
        xor.globalListener.quit.add(() => worker.stop());
        this.worker = worker;

        this.ci.SetWorker.Invoke(worker.source);
    }
    public stop() {
        if (this.worker) this.worker.stop();
        this.worker = null;
    }

    public bind(): csharp.XOR.Services.TSInterfaces {
        let ti = new csharp.XOR.Services.TSInterfaces();
        ti.Start = (ep, p) => this.start(ep, p);
        ti.Stop = () => this.stop();
        return ti;
    }

    private _createWorker(editorProject: string) {
        const options = new csharp.XOR.ThreadWorker.CreateOptions();
        options.remote = true;
        options.isEditor = true;
        const loader = new csharp.XOR.MergeLoader();
        loader.AddLoader(new csharp.XOR.FileLoader(editorProject, Path.GetDirectoryName(editorProject)));

        const worker = new xor.ThreadWorker(loader, options);
        worker.start("./child/main");

        xor.globalListener.quit.add(() => worker.stop());

        return worker;
    }
}

export function init(ci: csharp.XOR.Services.CSharpInterfaces) {
    return new Workflow(ci).bind();
}