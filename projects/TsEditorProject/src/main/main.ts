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

const worker = new xor.ThreadWorker(loader, options);
worker.start("./child/main");

xor.globalListener.quit.add(() => worker.stop());

//console.log("main thread ready.");
//setInterval(() => console.log("main thread active:"), 1000);

worker.post('test1', 'testMessage1').then(r => {
    console.log('response: ' + r);
});
//let r = worker.postSync('test1', 'atestMessage2');
//console.log('response: ' + r)