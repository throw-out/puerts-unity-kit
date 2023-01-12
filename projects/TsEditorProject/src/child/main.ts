import * as csharp from "csharp";
import { WorkerEvent } from "../common/event";
import { XOR } from "./Program";

require("puerts/console-track");

const workflow = new class {
    private cp: csharp.XOR.Services.Program;
    private program: XOR.Program;

    public setCSharpProgram(program: csharp.XOR.Services.Program) {
        this.cp = program;
    }
    public start(project: string) {
        this.program = new XOR.Program(project);
    }
}

xor.globalWorker.on(WorkerEvent.StartProgream, (data: { project: string, program: csharp.XOR.Services.Program }) => {
    //console.log(`start program: ${data.project}`);
    workflow.setCSharpProgram(data.program);
    workflow.start(data.project);
});
xor.globalWorker.on(WorkerEvent.FileChanged, (path: string) => {

});