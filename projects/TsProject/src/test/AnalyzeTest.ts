import * as csharp from "csharp";
import { GameObject, Transform } from "csharp.UnityEngine";
import { ButtonAlias } from "./types";
import * as t from "testModule";

type TransformAlias = Transform;
import GameObjectAlias = csharp.UnityEngine.GameObject;

export class AnalyzeTest1 {
    private _prop1: GameObjectAlias;
    private _prop4: TransformAlias;
    private _prop5: ButtonAlias;
    private _prop2: number;
    private _prop3: Map<number, string>;
}
export class AnalyzeTest2 {
    private _prop1: GameObject;
    private _prop2: csharp.UnityEngine.GameObject;
}
export class AnalyzeTest3 {
    private _prop1: GameObject;
    private _prop2: t.TestClass1;
    private _prop3: t.TestNamespace.TestClass2;
    private _prop4: TestClass3;
}
export class AnalyzeTest4 {
    protected _prop1: GameObject;
    protected _prop2: GameObject[];
}
export class AnalyzeTest5 extends AnalyzeTest4 {
    protected _prop3: GameObject;
    protected _prop4: GameObject[];
}
export class AnalyzeTest6 extends xor.TsComponent {
    protected _prop3: GameObject;
    protected _prop4: GameObject[];
}

declare global {
    class TestClass3 {

    }
}