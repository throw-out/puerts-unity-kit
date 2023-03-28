import { Color, Color32, GameObject, Transform, Vector2, Vector3 } from "csharp.UnityEngine";

/**
 * 演示如何将方法绑定到实例对象上
 */
@xor.guid("276b5325-5f85-4631-ab46-3107005e3c3d")
export class Sample01 extends xor.TsComponent {
    declare private _prop1: string;
    declare private _prop2: number;

    //无参方法
    public method1() {
        console.log(`invoke ${this.method1.name} `);
    }
    //带参数的方法
    public method2(value: boolean) {
        console.log(`invoke ${this.method2?.name}: ${value}`);
    }
    public method3(value: number) {
        console.log(`invoke ${this.method3?.name}: ${value}`);
    }
    public method4(value: string) {
        console.log(`invoke ${this.method4?.name}: ${value}`);
    }
    public method5(value: GameObject) {
        console.log(`invoke ${this.method5?.name}: ${value?.GetType().FullName}`);
    }
    //重载参数类型的方法
    public method6(value: Transform) {
        console.log(`invoke ${this.method6?.name}: ${value?.GetType().FullName}`);
    }
    public method7(value: Color) {
        console.log(`invoke ${this.method7?.name}: ${value}`);
    }
    public method8(value: Color32) {
        console.log(`invoke ${this.method8?.name}: ${value}`);
    }
    public method9(value: Vector2) {
        console.log(`invoke ${this.method9?.name}: ${value}`);
    }
    public method10(value: Vector3) {
        console.log(`invoke ${this.method10?.name}: ${value}`);
    }

    //多个参数: 不支持
    public method_nsupport(p1: string, p2: number) {
        console.log(`invoke ${this.method3.name}: non-support`);
    }
}