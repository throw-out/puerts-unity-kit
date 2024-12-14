import * as csharp from "csharp";
import * as nodefs from "fs"; //仅用于导入声明, 切勿直接使用
import { WorkerEvent } from "../common/event";

type ReturnType<T extends Function> = T extends (...args: any[]) => infer R ? R : never;

const { Path } = csharp.System.IO;

const BatchProcess = true,      //批量处理file change事件
    BatchProcessDelay = 100;    //批量处理file change事件的延迟

//require("puerts/console-track");
//require("puerts/puerts-source-map-support");

class Workflow {
    private readonly ci: csharp.XOR.Services.CSharpInterfaces;
    private readonly dev: csharp.Puerts.ILoader;
    private _worker: xor.ThreadWorker;
    private _watcher: ReturnType<typeof this._createWatcher>;
    private _changeEvents: Set<string>;

    constructor(ci: csharp.XOR.Services.CSharpInterfaces, dev: csharp.Puerts.ILoader) {
        this.ci = ci;
        this.dev = dev;
    }
    public start(project: string, childStartup: string) {
        if (this._worker && this._worker.isAlive)
            throw new Error("invalid operation");
        //console.log(`workflow start: \neditorProject: ${editorProject}\nproject: ${project}`);

        const worker = this._createWorker(childStartup);

        const program = new csharp.XOR.Services.Program();
        program.root = Path.GetDirectoryName(project);
        program.Reset();
        //请求子线程, 开始解析工程
        worker.post(WorkerEvent.StartProgream, { project, program }, true);

        this._worker = worker;
        this.ci.SetWorker.Invoke(worker.source);
        this.ci.SetProgram.Invoke(program);
    }
    public watch(project: string) {
        try {
            let dirpath = Path.GetDirectoryName(project);
            this._watcher = this._createWatcher(dirpath);
            if (csharp.XOR.Logger.Verbose) {
                csharp.XOR.Logger.Log(`<b>nodejs.FSWacther:</b> ${dirpath}`);
            }
        } catch (e) {
            console.error(e);
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
    public bind(): csharp.XOR.Services.TSInterfaces {
        let ti = new csharp.XOR.Services.TSInterfaces();
        ti.Stop = () => this.stop();
        ti.Start = (p, s) => this.start(p, s);
        ti.Watch = (path) => this.watch(path);
        ti.FileChanged = (path) => this.change(path);
        return ti;
    }

    private change(path: string) {
        if (!BatchProcess) {
            this.sendChangeEvent([path]);
            return;
        }
        if (this._changeEvents) {
            this._changeEvents.add(path);
        } else {
            this._changeEvents = new Set();
            this._changeEvents.add(path);
            setTimeout(() => {
                this.sendChangeEvent([...this._changeEvents]);
                this._changeEvents = null;
            }, BatchProcessDelay);
        }
    }
    private sendChangeEvent(paths: string[]) {
        if (!this._worker || !this._worker.isAlive || !this._worker.isInitialized) {
            console.warn(`worker is not alive or initializing:`);
            return;
        }
        this._worker.post(WorkerEvent.FileChanged, paths, true);
    }

    private _createWorker(startup: string) {
        const debug = new csharp.XOR.ThreadDebuggerOptions();
        //debug.port = 9090;
        //debug.wait = true;
        const options = new csharp.XOR.ThreadOptions();
        options.remote = false;
        options.isEditor = true;
        options.debugger = debug;

        const loader = new csharp.XOR.MixerLoader();
        if (this.dev) loader.AddLoader(this.dev);
        const worker = new xor.ThreadWorker(loader, options);
        worker.start(startup);

        xor.globalListener.quit.add(() => worker.stop());

        return worker;
    }
    private _createWatcher(path: string) {
        const fs: typeof nodefs = require("fs");
        const extNames = [".ts", ".tsx"], listenr = (event: "change" | "rename", filename: string) => {
            if (typeof (filename) !== "string" || !extNames.includes(Path.GetExtension(filename)))
                return;
            this.change(Path.GetFullPath(Path.Combine(path, filename)));
        };
        const watcher = fs.watch(path, {
            encoding: "utf8",
            persistent: true,
            recursive: true,
        }, listenr);

        xor.globalListener.quit.add(() => watcher.close());
        return watcher;
    }
}

function init(ci: csharp.XOR.Services.CSharpInterfaces, devLoader: csharp.Puerts.ILoader) {
    return new Workflow(ci, devLoader)
        .bind();
}
(function () {
    var _g = global || globalThis || this;
    _g.init = init;
})();