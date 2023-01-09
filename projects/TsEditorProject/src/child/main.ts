console.log("child thread started.");

import * as csharp from "csharp";
import * as ts from "typescript";
import { XOR } from "./Program";

const { Path } = csharp.System.IO;

setTimeout(() => {
    let tsconfigPath = Path.GetFullPath(Path.Combine(
        Path.GetDirectoryName(csharp.UnityEngine.Application.dataPath),
        "TsProject/tsconfig.json"
    ));
    let program = new XOR.Program(tsconfigPath);
    //program.print();
    program.print(statement =>
        statement.kind === ts.SyntaxKind.ClassDeclaration &&
        (<ts.ClassDeclaration>statement).name.getText().includes("AnalyzeTest")
    );
}, 2000);