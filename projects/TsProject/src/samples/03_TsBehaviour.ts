import GameObject = CS.UnityEngine.GameObject;
import Time = CS.UnityEngine.Time;

//注册全局生命周期回调, 避免在xor.TsBehaviour中创建多个Delegate实例
xor.TsBehaviour.registerGlobalInvoker();

//一个简单的TsBehaviour示例:
class Sample01 extends xor.TsBehaviour {
    private _time: number = 0;

    protected Awake(): void {
        console.log(`TsBehaviour ${Sample01.name}: Awake`);
    }
    //Update将由xor.TsBehaviour统一调用, 你可以使用`standalone`装饰器告知xor.TsBehaviour使用单独的Update调用组件
    //@xor.standalone()
    protected Update(deltaTime?: number): void {
        this._time += (deltaTime ?? Time.deltaTime);
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
