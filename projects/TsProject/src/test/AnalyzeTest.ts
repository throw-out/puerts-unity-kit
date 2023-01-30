import * as csharp from "csharp";
import { GameObject, Transform } from "csharp.UnityEngine";
import { ButtonAlias } from "./types";

type TransformAlias = Transform;
import GameObjectAlias = csharp.UnityEngine.GameObject;

/**
 * TestInfo
 */
@xor.guid("201fcfb8-f0e4-43d4-827f-034f8c48ed34")
export class AnalyzeTest111 extends xor.TsComponent {
    declare private _prop100: string;
    declare private _prop101: number;
    declare private _prop1021111110opll: bigint;

    declare private _prop2: Transform;
    declare private _prop3: GameObject;
    declare private _prop4: GameObject[];
    protected _prop5: GameObject[];
}

//
@xor.guid("3f967054-4762-45c4-b34a-c2e4f4ba77cb")
export class AnalyzeTest222 extends xor.TsComponent {
    protected _prop3: GameObject;
    protected _prop4: GameObject[];
}

export enum AnalyzeTest8 {

}
export namespace T1 {
    //
    @xor.guid("99533289-af50-4610-892a-12da37dd8e6d")
    export class AnalyzeTest333 extends xor.TsComponent {
        protected _prop999: GameObject;
    }
}