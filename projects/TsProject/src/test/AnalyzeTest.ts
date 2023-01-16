import * as csharp from "csharp";
import { GameObject, Transform } from "csharp.UnityEngine";
import { ButtonAlias } from "./types";

type TransformAlias = Transform;
import GameObjectAlias = csharp.UnityEngine.GameObject;

/**
 * TestInfo
 */
@xor.guid("201fcfb8-f0e4-43d4-827f-034f8c48ed34")
export class AnalyzeTest6 extends xor.TsComponent {
    protected _prop3: GameObject;
    protected _prop4: GameObject[];
}

//
@xor.guid("3f967054-4762-45c4-b34a-c2e4f4ba77cb")
export class AnalyzeTest7 extends xor.TsComponent {
    protected _prop3: GameObject;
    protected _prop4: GameObject[];
}

export enum AnalyzeTest8 {

}
export namespace T1 {

    //
    @xor.guid("99533289-af50-4610-892a-12da37dd8e6d")
    export class TP extends xor.TsComponent {

    }
}