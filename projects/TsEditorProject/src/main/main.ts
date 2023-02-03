import * as nodefs from "fs";       //仅用于导入声明, 切勿直接使用
import * as csharp from "csharp";
import { WorkerEvent } from "../common/event";

type ReturnType<T extends Function> = T extends (...args: any[]) => infer R ? R : never;

const { Path } = csharp.System.IO;

require("puerts/console-track");
//require("puerts/puerts-source-map-support");

class Workflow {
    private _worker: xor.ThreadWorker;
    private _watcher: ReturnType<typeof this._createWatcher>;
    private readonly ci: csharp.XOR.Services.CSharpInterfaces;

    constructor(ci: csharp.XOR.Services.CSharpInterfaces) {
        this.ci = ci;
    }
    public start(editorProject: string, project: string, useWatcher: boolean) {
        if (this._worker && this._worker.isAlive)
            throw new Error("invalid operation");
        //console.log(`workflow start: \neditorProject: ${editorProject}\nproject: ${project}`);

        const worker = this._createWorker(editorProject);

        const program = new csharp.XOR.Services.Program();
        program.root = Path.GetDirectoryName(project);
        program.Reset();
        //请求子线程, 开始解析工程
        worker.post(WorkerEvent.StartProgream, { project, program }, true);

        this._worker = worker;
        this.ci.SetWorker.Invoke(worker.source);
        this.ci.SetProgram.Invoke(program);

        if (useWatcher) {
            try {
                let dirpath = Path.GetDirectoryName(project);
                this._watcher = this._createWatcher(dirpath);
                csharp.XOR.Logger.Log(`<b>nodejs.FSWacther:</b> ${dirpath}`);
            } catch (e) {
                console.error(e);
            }
        }
    }
    public stop() {
        if (this._watcher) this._watcher.close();
        this._watcher = null;
        if (this._worker) this._worker.stop();
        this._worker = null;

        this.ci.SetWorker.Invoke(null);
        this.ci.SetProgram.Invoke(null);
    }
    public change(path: string) {
        if (!this._worker || !this._worker.isAlive || !this._worker.isInitialized) {
            console.warn(`worker is not alive or initializing:`);
            return;
        }
        this._worker.post(WorkerEvent.FileChanged, path, true);
    }

    public bind(): csharp.XOR.Services.TSInterfaces {
        let ti = new csharp.XOR.Services.TSInterfaces();
        ti.Stop = () => this.stop();
        ti.Start = (ep, p, useWatcher) => this.start(ep, p, useWatcher);
        ti.FileChanged = (path) => this.change(path);
        return ti;
    }

    private _createWorker(editorProject: string) {
        const debug = new csharp.XOR.ThreadDebuggerOptions();
        debug.port = 9090;
        debug.wait = false;
        const options = new csharp.XOR.ThreadOptions();
        options.remote = false;
        options.isEditor = true;
        options.debugger = debug;

        const loader = new csharp.XOR.MergeLoader();
        loader.AddLoader(new csharp.XOR.FileLoader(editorProject, Path.GetDirectoryName(editorProject)));
        const worker = new xor.ThreadWorker(loader, options);
        worker.start("./child/main");

        xor.globalListener.quit.add(() => worker.stop());

        return worker;
    }
    private _createWatcher(path: string) {
        const fs: typeof nodefs = require("fs");
        const extNames = [".ts", ".tsx"];
        const watcher = fs.watch(path, {
            encoding: "utf8",
            persistent: true,
            recursive: true,
        });
        watcher.on("change", (event, filename) => {
            if (typeof (filename) !== "string" || !extNames.includes(Path.GetExtension(filename)))
                return;
            this.change(Path.GetFullPath(Path.Combine(path, filename)));
        });

        xor.globalListener.quit.add(() => watcher.close());
        return watcher;
    }
}

export function init(ci: csharp.XOR.Services.CSharpInterfaces) {
    return new Workflow(ci).bind();
}