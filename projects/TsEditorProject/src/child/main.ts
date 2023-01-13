import * as csharp from "csharp";
import * as ts from "typescript";
import { WorkerEvent } from "../common/event";
import * as services from "./services";

require("puerts/console-track");
require("puerts/puerts-source-map-support");

let program: services.Program;

xor.globalWorker.on(WorkerEvent.StartProgream, (data: { project: string, program: csharp.XOR.Services.Program }) => {
    const timer = new Timer();

    console.log("==================================================");
    let pcl = services.parseConfigFile(data.project);
    let compilerOptions = pcl.options;
    let rootNames = (pcl.fileNames ?? []).filter(p => !p.includes("node_modules"));
    console.log(`scan files duration ${timer.duration()}ms, total ${rootNames.length} files:\n${rootNames.join("\n")}`);
    timer.reset();

    program = new services.Program(data.program, rootNames, compilerOptions);

    console.log(`program parse duration ${timer.duration()}ms`);
    //program.print();
});
xor.globalWorker.on(WorkerEvent.FileChanged, (path: string) => {

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