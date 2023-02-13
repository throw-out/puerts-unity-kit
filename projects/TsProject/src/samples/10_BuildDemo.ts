import GameObject = CS.UnityEngine.GameObject;
import Time = CS.UnityEngine.Time;
import Vector3 = CS.UnityEngine.Vector3;

//一个简单的TsBehaviour示例:
class Sample01 extends xor.TsBehaviour {
    private _speed = 100;

    protected Update(deltaTime?: number): void {
        deltaTime = deltaTime ?? Time.deltaTime;

        this.transform.Rotate(new Vector3(0, this._speed * deltaTime, 0));
    }
}

export function init(target: GameObject) {
    return new Sample01(target);
}
