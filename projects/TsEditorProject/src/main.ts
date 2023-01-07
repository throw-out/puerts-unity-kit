import * as CS from "csharp";
import * as ts from "typescript";
import { Analyze } from "./ASTAnalyze";

function showKindName(node: ts.Node) {
    node["kindName"] = ts.SyntaxKind[node.kind];
    for (let key of Object.keys(node)) {
        let value = node[key];
        if (Array.isArray(value)) {
            for (let o of value) {
                if (o && o.kind) {
                    showKindName(o);
                }
            }
        }
    }
}

const root = CS.System.IO.Path.GetDirectoryName(CS.UnityEngine.Application.dataPath);

const syntaxOutput = CS.System.IO.Path.Combine(
    root,
    "TsProject/src/test/ts/syntax/"
);
if (!CS.System.IO.Directory.Exists(syntaxOutput)) {
    CS.System.IO.Directory.CreateDirectory(syntaxOutput);
}

function tryAST() {
    let paths: string[] = [
        CS.System.IO.Path.Combine(
            root,
            "TsProject/typeing/csharp.modules.d.ts"
        ),
        CS.System.IO.Path.Combine(
            root,
            "TsProject/typeing/type.d.ts"
        ),
        CS.System.IO.Path.Combine(
            root,
            "TsProject/src/framework/TsBehaviour.ts"
        ),
        CS.System.IO.Path.Combine(
            root,
            "TsProject/src/framework/controllers/Process.ts"
        )
    ];

    for (let path of paths) {
        let fileName = CS.System.IO.Path.GetFileName(path),
            fileSource = CS.System.IO.File.ReadAllText(path);
        let source2 = ts.createSourceFile(fileName, fileSource, ts.ScriptTarget.ES2020);
        showKindName(source2);

        let ast = new Analyze.File(source2);
        console.log(ast);

        CS.System.IO.File.WriteAllText(CS.System.IO.Path.Combine(syntaxOutput, fileName.replace(".ts", ".json")), JSON.stringify(ast));
    }

    let program = ts.createProgram({
        rootNames: [],
        options: {
            target: ts.ScriptTarget.ES5,
            module: ts.ModuleKind.CommonJS,
        }
    });
    let checker = program.getTypeChecker();

    const getFunctionTypeInfoFromSignature = (signature: ts.Signature, checker: ts.TypeChecker) => {
        // 获取参数类型
        const paramTypeStrList = signature.parameters.map((parameter) => {
            return checker.typeToString(checker.getTypeOfSymbolAtLocation(parameter, parameter.valueDeclaration));
        });

        // 获取返回值类型
        const returnTypeStr = checker.typeToString(signature.getReturnType());

        return {
            paramTypes: paramTypeStrList,
            returnType: returnTypeStr,
        };
    };
}

import "./Analyze";