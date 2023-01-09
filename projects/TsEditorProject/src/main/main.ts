console.log("main thread started.");

import * as csharp from "csharp";

const { Path } = csharp.System.IO;
const { Application } = csharp.UnityEngine;

const loader = new csharp.XOR.MergeLoader();

let projectRoot = Path.Combine(Path.GetDirectoryName(Application.dataPath), "TsEditorProject");
let outputRoot = Path.Combine(projectRoot, "output");
loader.AddLoader(new csharp.XOR.FileLoader(outputRoot, projectRoot));

const worker = new XOR.ThreadWorker(loader);
//worker.start("./child/main");

XOR.globalListener.quit.add(() => worker.stop());

