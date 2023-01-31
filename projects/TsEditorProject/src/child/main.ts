import * as csharp from "csharp";
import { WorkerEvent } from "../common/event";
import * as services from "./services";

require("puerts/console-track");
require("puerts/puerts-source-map-support");

let program: services.Program;

xor.globalWorker.on(WorkerEvent.StartProgream, (data: { project: string, program: csharp.XOR.Services.Program }) => {
    data.program.state = csharp.XOR.Services.ProgramState.Scanning;
    data.program.stateMessage = '';

    const timer = new Timer();

    let pcl = services.parseConfigFile(data.project);
    let rootNames = (pcl.fileNames ?? []).filter(p => !p.includes("node_modules"));
    //console.log(`scanning files duration ${timer.duration()}ms, total ${rootNames.length} files:\n${rootNames.join("\n")}`);

    timer.reset();
    program = new services.Program(data.program, rootNames, pcl.options);
    //console.log(`program parse duration ${timer.duration()}ms`);
});
xor.globalWorker.on(WorkerEvent.FileChanged, (path: string) => {
    program?.change(path);
});

class Timer {
    private _start: number;
    constructor() {
        this.reset();
    }
    public reset() {
        this._start = new Date().valueOf();
    }
    public duration() {
        return new Date().valueOf() - this._start;
    }
}