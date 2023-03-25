import { GameObject } from "csharp.UnityEngine";

/**
 * 演示如何将方法绑定到实例对象上
 */
@xor.guid("276b5325-5f85-4631-ab46-3107005e3c3d")
export class Sample01 extends xor.TsComponent {
    declare private _prop1: string;
    declare private _prop2: number;

    public method1() {
        console.log(`invoke ${this.method1.name} `);
    }
    //带参数的方法
    public method2(on: boolean) {
        console.log(`invoke ${this.method2.name}: ${on}`);
    }
    //UnityEngine.Object类型的参数
    public method3(obj: GameObject) {
        console.log(`invoke ${this.method3.name}: ${obj?.GetType()?.FullName}`);
    }
}