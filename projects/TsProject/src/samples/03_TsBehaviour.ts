import { GameObject, Time } from "csharp.UnityEngine";

//一个简单的TsBehaviour示例:
class Sample01 extends xor.TsBehaviour {
    private _time: number = 0;

    protected Awake(): void {
        console.log(`TsBehaviour ${Sample01.name}: Awake`);
    }
    //Update将由xor.TsBehaviour统一调用, 你可以使用`standalone`装饰器告知xor.TsBehaviour使用单独的Update调用组件
    //@xor.TsBehaviour.standalone()
    protected Update(deltaTime?: number): void {
        this._time += (deltaTime ?? Time.time);
        if (this._time > 1) {
            this._time -= 1;
            console.log(`TsBehaviour ${Sample01.name}: Update`);
        }
    }
    protected OnEnable(): void {
        console.log(`TsBehaviour ${Sample01.name}: OnEnable`);
    }
    protected OnDisable(): void {
        console.log(`TsBehaviour ${Sample01.name}: OnDisable`);
    }
}

export function init() {
    let gameObject = new GameObject(Sample01.name);
    return new Sample01(gameObject);
}
