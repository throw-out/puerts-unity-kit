import * as csharp from "csharp";
import { Component, GameObject, Transform } from "csharp.UnityEngine";
import { ButtonAlias } from "./types";

type TransformAlias = Transform;
import GameObjectAlias = csharp.UnityEngine.GameObject;

/**
 * TestInfo
 */
@xor.guid("82a04714-b73f-48cb-b608-3818d8539791")
export class AnalyzeTest111 extends xor.TsComponent {
    declare private _prop100: string;

    @xor.field({ type: csharp.System.Byte, value: 123 })
    private _prop101: number;
    @xor.field(csharp.System.Byte)
    private _prop102: number[];

    @xor.field({
        type: csharp.System.Int32,
        range: [0, 50],
        value: 0
    })
    private _prop103: number;

    declare private _prop1021111110opll: bigint;

    declare private _prop2: Transform;
    declare private _prop3: GameObject;
    declare private _prop4: Transform[];
    protected _prop5: GameObject[];
}
@xor.guid("ada747b2-5e4d-48a9-909a-c261b56d54bf")
export class AnalyzeTest222 extends xor.TsComponent {
    protected _prop3: GameObject;
    protected _prop4: GameObject[];
}

export enum AnalyzeTest8 {

}
export namespace T1 {
    @xor.guid("51499546-407d-430d-a80d-9bc789eb6408")
    export class AnalyzeTest333 extends xor.TsComponent {
        protected _prop999: GameObject;
    }
}