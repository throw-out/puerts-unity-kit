import * as CS from "csharp";
import { GameObject, Transform } from "csharp.UnityEngine";
import { ButtonAlias } from "./types";
import * as t from "testModule";

type TransformAlias = Transform;
import GameObjectAlias = CS.UnityEngine.GameObject;

export class AnalyzeTest1 {
    private _prop1: GameObjectAlias;
    private _prop4: TransformAlias;
    private _prop5: ButtonAlias;
    private _prop2: number;
    private _prop3: Map<number, string>;
}
export class AnalyzeTest2 {
    private _prop1: GameObject;
    private _prop2: CS.UnityEngine.GameObject;
}
export class AnalyzeTest3 {
    private _prop1: GameObject;
    private _prop2: t.TestClass1;
    private _prop3: t.TestNamespace.TestClass2;
    private _prop4: TestClass3;
}
export class AnalyzeTest4 {
    private _prop1: GameObject;
    private _prop2: GameObject[];
}

declare global {
    class TestClass3 {

    }
}