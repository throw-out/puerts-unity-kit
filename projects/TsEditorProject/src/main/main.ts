import * as csharp from "csharp";
import { WorkerEvent } from "../common/event";

require("puerts/console-track");
//require("puerts/puerts-source-map-support");

const { Path } = csharp.System.IO;

class Workflow {
    private worker: xor.ThreadWorker;
    private readonly ci: csharp.XOR.Services.CSharpInterfaces;

    constructor(ci: csharp.XOR.Services.CSharpInterfaces) {
        this.ci = ci;
    }
    public start(editorProject: string, project: string) {
        if (this.worker && this.worker.isAlive)
            throw new Error("invalid operation");
        //console.log(`workflow start: \neditorProject: ${editorProject}\nproject: ${project}`);

        const worker = this._createWorker(editorProject);

        const program = new csharp.XOR.Services.Program();
        program.Reset();
        //请求子线程, 开始解析工程
        worker.post<boolean>(WorkerEvent.StartProgream, { project, program });

        this.worker = worker;
        this.ci.SetWorker.Invoke(worker.source);
        this.ci.SetProgram.Invoke(program);
    }
    public stop() {
        if (this.worker) this.worker.stop();
        this.worker = null;

        this.ci.SetWorker.Invoke(null);
        this.ci.SetProgram.Invoke(null);
    }
    public change(path: string) {
        this.worker.post<boolean>(WorkerEvent.FileChanged, path, true);
    }

    public bind(): csharp.XOR.Services.TSInterfaces {
        let ti = new csharp.XOR.Services.TSInterfaces();
        ti.Stop = () => this.stop();
        ti.Start = (ep, p) => this.start(ep, p);
        ti.FileChanged = (path) => this.change(path);
        return ti;
    }

    private _createWorker(editorProject: string) {
        const debug = new csharp.XOR.ThreadDebuggerOptions();
        debug.port = 9090;
        debug.wait = false;
        const options = new csharp.XOR.ThreadOptions();
        options.remote = true;
        options.isEditor = true;
        options.debugger = debug;

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