import * as csharp from "csharp";

import "./xor/threadWorker";
import "./xor/globalListener";

const { Path } = csharp.System.IO;
const { Application } = csharp.UnityEngine;

const loader = new csharp.XOR.MergeLoader();

let projectRoot = Path.Combine(Path.GetDirectoryName(Application.dataPath), "TsEditorProject");
let outputRoot = Path.Combine(projectRoot, "output");
loader.AddLoader(new csharp.XOR.FileLoader(outputRoot, projectRoot));

const worker = new ThreadWorker(loader);
worker.start("./child/main");


globalListener.quit.add(() => worker.stop());

