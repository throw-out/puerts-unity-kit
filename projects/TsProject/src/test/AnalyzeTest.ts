import * as csharp from "csharp";
import { GameObject, Transform } from "csharp.UnityEngine";
import { ButtonAlias } from "./types";

type TransformAlias = Transform;
import GameObjectAlias = csharp.UnityEngine.GameObject;

export class AnalyzeTest6 extends xor.TsComponent {
    protected _prop3: GameObject;
    protected _prop4: GameObject[];
}

@xor.guid("12345678888")
export class AnalyzeTest7 extends xor.TsComponent {
    protected _prop3: GameObject;
    protected _prop4: GameObject[];
}

export enum AnalyzeTest8 {

}

declare global {
    class TestClass3 {

    }
}