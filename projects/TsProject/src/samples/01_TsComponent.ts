/////////////////////////////////////////////////////////////////////
//基础组件演示
//Sample01: 基础类型演示
//Sample02: 数组类型演示
//Sample03: 指定RawType丶指定DefaultValue和指定range的演示
//Sample04: enum类型演示
/////////////////////////////////////////////////////////////////////
import * as csharp from "csharp";
import { GameObject, Transform, Vector2, Vector3 } from "csharp.UnityEngine";

@xor.guid("e1cb6fde-b4dd-4fba-b08d-aed2f2bea1f2")
export class Sample01 extends xor.TsComponent {
    declare private _prop1: string;
    declare private _prop2: number;
    declare private _prop3: boolean;
    declare private _prop4: bigint;

    declare private _prop5: Vector2;
    declare private _prop6: Vector3;
    declare private _prop7: GameObject;
    declare private _prop8: Transform;
}
@xor.guid("a071b597-ff11-420a-b7d5-cd586ee4b1a1")
export class Sample02 extends xor.TsComponent {
    declare private _prop1: string[];
    declare private _prop2: number[];
    declare private _prop3: boolean[];
    declare private _prop4: bigint[];

    declare private _prop5: Vector2[];
    declare private _prop6: Vector3[];
    declare private _prop7: GameObject[];
    declare private _prop8: Transform[];
}
@xor.guid("d2ea683a-0e55-41a2-b837-b7e7c2a1fee4")
export class Sample03 extends xor.TsComponent {
    @xor.field(csharp.System.Byte)
    declare private _prop1: number;
    @xor.field(csharp.System.Int16)
    declare private _prop2: number;

    @xor.field({ range: [0, 100], value: 50 })
    declare private _prop3: number;
    @xor.field({ value: "string default value" })
    declare private _prop4: string;

    @xor.field({ type: csharp.System.Int16, value: [1, 15] })
    declare private _prop5: number[];
    @xor.field({ range: [0, 100], value: [1, 33.333, 67.67] })
    declare private _prop6: number[];
}
@xor.guid("70292285-10d5-44e0-9031-e12b14a05d15")
export class Sample04 extends xor.TsComponent {

}