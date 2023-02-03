import * as csharp from "csharp";
import { GameObject, Transform, Vector2, Vector3 } from "csharp.UnityEngine";

/**
 * 基础类型演示
 */
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
    //declare private _prop9: Transform;
}
/**
 * 数组类型演示
 */
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
/**
 * 指定RawType丶DefaultValue和Range的演示
 */
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
    @xor.field({ value: new Vector2(1, 10) })
    declare private _prop5: Vector2;

    @xor.field({ type: csharp.System.Int16, value: [1, 15] })
    declare private _prop6: number[];
    @xor.field({ range: [0, 100], value: [1, 33.333, 67.67] })
    declare private _prop7: number[];
}
/**
 * enum类型演示:
 * 枚举值只包含Number时, 以System.Int32作为序列化类型
 * 枚举值包含String时, 以System.String作为序列化类型
 */
@xor.guid("70292285-10d5-44e0-9031-e12b14a05d15")
export class Sample04 extends xor.TsComponent {
    declare private _prop1: 1 | 3 | 5 | 6 | 7;
    declare private _prop2: "string1" | "string2" | "string3";
    declare private _prop3: Sample04.Type1;
    declare private _prop4: Sample04.Type2;
    declare private _prop5: Sample04.Type3;

    @xor.field({ value: [1] })
    declare private _prop6: Array<1 | 3 | 5>;
    @xor.field({ value: [Sample04.Type1.P1] })
    declare private _prop7: Array<Sample04.Type1>;
    @xor.field({ value: [Sample04.Type3.P6] })
    declare private _prop8: Array<Sample04.Type3>;
}
export namespace Sample04 {
    export enum Type1 {
        P1,
        P2,
        P3,
    }
    export enum Type2 {
        P4 = 4,
        P5 = "p5",
    }
    export enum Type3 {
        P6 = "p6",
        P7 = "p7",
    }
}