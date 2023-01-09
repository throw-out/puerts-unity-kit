import * as csharp from "csharp";
import * as ts from "typescript";
import { XOR } from "./Program";

const { Path } = csharp.System.IO;

setTimeout(() => {
    let p: any = csharp.UnityEngine.Application.dataPath;
    console.log(typeof p);
    console.log(p instanceof csharp.System.Object ? p.GetType().FullName : 'null');
    console.log(p);
    let tsconfigPath = Path.GetFullPath(Path.Combine(
        Path.GetDirectoryName(p),
        "TsProject/tsconfig.json"
    ));
    let program = new XOR.Program(tsconfigPath);
    //program.print();
    program.print(statement =>
        statement.kind === ts.SyntaxKind.ClassDeclaration &&
        (<ts.ClassDeclaration>statement).name.getText().includes("AnalyzeTest")
    );
}, 2000);

setInterval(() => console.log("child thread active:"), 1000);