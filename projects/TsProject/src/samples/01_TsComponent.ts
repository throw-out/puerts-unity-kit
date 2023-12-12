import Color = CS.UnityEngine.Color;
import GameObject = CS.UnityEngine.GameObject;
import GUILayout = CS.UnityEngine.GUILayout;
import Transform = CS.UnityEngine.Transform;
import Vector2 = CS.UnityEngine.Vector2;
import Vector3 = CS.UnityEngine.Vector3;

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

    public get prop1() { return this._prop1; }
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

    public get prop1() { return this._prop1; }
}
/**
 * 指定RawType丶DefaultValue和Range的演示
 */
@xor.guid("d2ea683a-0e55-41a2-b837-b7e7c2a1fee4")
export class Sample03 extends xor.TsComponent {
    @xor.field(CS.System.Byte)
    declare private _prop1: number;
    @xor.field(CS.System.Int16)
    declare private _prop2: number;

    @xor.field({ range: [0, 100], value: 50 })
    declare private _prop3: number;
    @xor.field({ value: "string default value" })
    declare private _prop4: string;
    @xor.field({ value: new Vector2(1, 10) })
    declare private _prop5: Vector2;

    @xor.field({ type: CS.System.Int16, value: [1, 15] })
    declare private _prop6: number[];
    @xor.field({ range: [0, 100], value: [1, 33.333, 67.67] })
    declare private _prop7: number[];

    public get prop1() { return this._prop1; }
}

namespace Types {
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
/**
 * enum类型演示:
 * 枚举值只包含Number时, 以System.Int32作为序列化类型
 * 枚举值包含String时, 以System.String作为序列化类型
 */
@xor.guid("70292285-10d5-44e0-9031-e12b14a05d15")
export class Sample04 extends xor.TsComponent {
    @xor.field({ value: 3 })
    declare private _prop1: 1 | 3 | 5 | 6 | 7;
    @xor.field({ value: "string2" })
    declare private _prop2: "string1" | "string2" | "string3";
    @xor.field({ value: Types.Type1.P1 })
    declare private _prop3: Types.Type1;
    declare private _prop4: Types.Type2;
    declare private _prop5: Types.Type3;

    @xor.field({ value: [1] })
    declare private _prop6: Array<1 | 3 | 5>;
    @xor.field({ value: [Types.Type1.P1, Types.Type1.P2] })
    declare private _prop7: Array<Types.Type1>;
    @xor.field({ value: [Types.Type3.P6] })
    declare private _prop8: Array<Types.Type3>;

    public get prop1() { return this._prop1; }
}

/**
 * 自定义序列化类型: Color和CS.MyData不属于XOR内置类型
 */
@xor.guid("36102130-2db9-4d62-9221-98c51c3c138c")
export class Sample05 extends xor.TsComponent {
    @xor.field({ value: Color.white })
    declare private _prop1: Color;
    declare private _prop2: CS.MyData;

    @xor.field({ value: [Color.white] })
    declare private _prop3: Color[];
    declare private _prop4: CS.MyData[];
}

/**
 * 双向绑定例子
 * 在OnGUI中修改this._value将会同步到Inspector, 反之在Inspector中修改_value字段亦会同步到this._value上
 */
@xor.guid("7984a4b8-de57-4af6-a572-6c159bc6dce3")
export class Sample06 extends xor.TsComponent {
    declare private _value: number;

    protected Awake(): void {
        console.log(`${Sample06.name}: AWake`);
    }
    protected OnGUI(): void {
        GUILayout.BeginHorizontal();
        GUILayout.Label(`${this._value}`);
        if (GUILayout.Button("-")) {
            this._value--;
        }
        if (GUILayout.Button("+")) {
            this._value++;
        }
        GUILayout.EndHorizontal();
    }
}

/**
 * ts类型引用例子
 */
@xor.guid("30641f2f-bdeb-436e-8586-349ee2323a15")
export class Sample07 extends xor.TsComponent {
    declare private _sample01: Sample01;
    declare private _sample02: Sample02;
    declare private _sample03: Sample03;
    declare private _sample04: Sample01 | Sample03;

    declare private _sample10: xor.TsComponent;
    declare private _sample11: CS.XOR.TsComponent;
    declare private _list1: xor.TsComponent[];
    declare private _list2: Array<Sample01>;
    declare private _list3: Array<Sample01 | Sample03>;

    protected Awake(): void {
        console.log(`=================${Sample07.name}====================`);
        console.log(`this._sample01._prop1 = ${this._sample01?.prop1}`);
        console.log(`this._sample02._prop1.length = ${this._sample02?.prop1?.length}`);
        console.log(`this._sample03._prop1 = ${this._sample03?.prop1}`);
        console.log(`this._sample04._prop1 = ${this._sample04?.prop1}`);
        //这里的_sample01,_sample03和_sample04是Proxy对象, 不可直接通过'==='进行判断.
        //调用xxx.valueOf() 获取原始js对象
        console.log(`this._sample04: is sample01 = ${this._sample04?.valueOf() === this._sample01?.valueOf()},  is sample03 = ${this._sample04?.valueOf() === this._sample03?.valueOf()}`);

        console.log(`this._sample10.name = ${this.getName(this._list1)}`);
        console.log(`this._sample11.name = ${this.getName(this._sample11)}`);
        console.log(`this._list1.length = ${this._list1?.length}, memebrs = ${this._list1?.map(s => this.getName(s)).join(", ")}`);
        console.log(`this._list2.length = ${this._list2?.length}, memebrs = ${this._list2?.map(s => this.getName(s)).join(", ")}`);
        console.log(`this._list3.length = ${this._list3?.length}, memebrs = ${this._list3?.map(s => this.getName(s)).join(", ")}`);
    }

    private getName(obj: object) {
        if (obj instanceof xor.TsComponent) {
            return this.getName(obj.gameObject);
        }
        if (obj instanceof CS.UnityEngine.Object && !obj.Equals(null)) {
            return obj.name;
        }
        return null;
    }
}

/**
 * GameObject.GetCompoment和GameObject.AddCompoment示例
 */
class Sample10 extends xor.TsComponent {
    public value: number;
}
console.log(`=================${Sample10.name}====================`);
let sample7GO = new GameObject(Sample10.name);
console.log(`GetComponent: ${Sample10.name}.value = ${sample7GO.GetComponent(Sample10)?.value}`);

console.log(`AddComponent: ${Sample10.name}`);
let sample7 = sample7GO.AddComponent(Sample10);
sample7.value = 10;
console.log(`GetComponent: ${Sample10.name}.value = ${sample7GO.GetComponent(Sample10)?.value}`);
sample7.value = 20;
console.log(`GetComponent: ${Sample10.name}.value = ${sample7GO.GetComponent(Sample10)?.value}`);
sample7.value = 30;
console.log(`GetComponent: ${Sample10.name}.value = ${sample7GO.GetComponent(Sample10)?.value}`);