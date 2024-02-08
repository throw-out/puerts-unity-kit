fixModuleSearch();

import * as csharp from "csharp";
import { WorkerEvent } from "../common/event";
import * as services from "./services-morph";

//require("puerts/console-track");
//require("puerts/puerts-source-map-support");

let program: services.Program;
//let program: services.Program;

xor.globalWorker.on(WorkerEvent.StartProgream, (data: { project: string, program: csharp.XOR.Services.Program }) => {
    data.program.state = csharp.XOR.Services.ProgramState.Scanning;
    data.program.compile = '';


    program = new services.Program(data.program, data.project);
    //program = new services.Program(data.program, rootNames, pcl.options);
    //console.log(`program parse duration ${timer.duration()}ms`);
});
xor.globalWorker.on(WorkerEvent.FileChanged, (path: string | string[]) => {
    program?.change(Array.isArray(path) ? path : [path]);
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
//修复模块名字中带"."的模块匹配问题
function fixModuleSearch() {
    try {
        let _g = this || global || globalThis;
        let puerts = _g.puerts;

        function getFileExtension(filepath) {
            let last = filepath.split('/').pop();
            let frags = last.split('.');
            if (frags.length > 1) {
                return frags.pop();
            }
        }
        let searchModule: (dir: string, requiredModule: string) => string = puerts.searchModule;
        puerts.searchModule = function (dir: string, requiredModule: string) {
            if (getFileExtension(requiredModule)) {
                return searchModule(dir, requiredModule) ||
                    searchModule(dir, requiredModule + "/index.js") ||
                    searchModule(dir, requiredModule + "/package.json");
            }
            return searchModule(dir, requiredModule);
        }
    } catch (e) {
        console.warn(e);
    }
}