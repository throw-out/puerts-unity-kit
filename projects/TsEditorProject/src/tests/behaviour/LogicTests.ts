import GameObject = CS.UnityEngine.GameObject;
import { ITest, TestBase, name } from "./_base";

class Awake extends TestBase {
    protected Awake(): void {
        this.result = true
    }
    public async run() {
        await super.run()
        await this.waitTime(100)
    }
}
class Start extends TestBase {
    protected Start(): void {
        this.result = true
    }
    public async run() {
        await super.run()
        await this.waitTime(100)
    }
}
class OnEnable extends TestBase {
    private tick: number;
    protected OnEnable(): void {
        this.tick = (this.tick ?? 0) + 1
    }
    public async run() {
        await super.run()

        //trigger 1th
        await this.waitTime(1)
        if (!this.tick || <number>this.tick != 1) {
            this.result = false
            return;
        }

        this.enabled = false
        await this.waitTime(1)

        //trigger 2th
        this.enabled = true
        await this.waitTime(1)
        if (!this.tick || <number>this.tick != 2) {
            this.result = false
            return;
        }

        this.result = true
    }
}
class OnDisable extends TestBase {
    private tick: number
    protected OnDisable(): void {
        this.tick = (this.tick ?? 0) + 1
    }
    public async run() {
        await super.run()

        //trigger 1th
        this.enabled = false
        await this.waitTime(1)
        if (!this.tick || <number>this.tick != 1) {
            this.result = false
            return;
        }

        this.enabled = true
        await this.waitTime(1)

        //trigger 2th
        this.enabled = false
        await this.waitTime(1)
        if (!this.tick || <number>this.tick != 2) {
            this.result = false
            return;
        }

        this.result = true
    }
}
class OnDestroy extends TestBase {
    protected OnDestroy(): void {
        this.result = true
    }
    public async run() {
        GameObject.Destroy(this.gameObject);

        await this.waitTime(100)
    }
}

class OnGUI extends TestBase {
    private tick: number;
    protected OnGUI(): void {
        this.tick = (this.tick ?? 0) + 1
    }
    public async run() {
        await super.run()

        await this.waitTime(100)
        let t = this.tick
        this.enabled = false
        await this.waitTime(100)

        this.result = this.tick > 0 && this.tick == t;
    }
}

abstract class UpdateBase extends TestBase {
    protected tick: number;
    public async run() {
        await super.run()

        //initialization
        await this.waitTime(100)
        if (!this.tick || this.tick <= 0) {
            this.result = false
            return
        }

        //disable component
        this.enabled = false
        await this.waitTime(1)
        let t = this.tick
        await this.waitTime(100)
        if (this.tick != t) {
            this.result = false
            return
        }

        //re-enable component
        this.enabled = true
        await this.waitTime(100)

        this.result = this.tick > t;
    }
}

class Update extends UpdateBase {
    @xor.standalone()
    protected Update(): void {
        this.tick = (this.tick ?? 0) + 1
    }
}
class LateUpdate extends UpdateBase {
    @xor.standalone()
    protected LateUpdate(): void {
        this.tick = (this.tick ?? 0) + 1
    }

}
class FixedUpdate extends UpdateBase {
    @xor.standalone()
    protected FixedUpdate(): void {
        this.tick = (this.tick ?? 0) + 1
    }
}

@name("Update(Batch)")
class UpdateBatch extends UpdateBase {
    protected Update(): void {
        this.tick = (this.tick ?? 0) + 1
    }
}
@name("LateUpdate(Batch)")
class LateUpdateBatch extends UpdateBase {
    protected LateUpdate(): void {
        this.tick = (this.tick ?? 0) + 1
    }
}
@name("FixedUpdate(Batch)")
class FixedUpdateBatch extends UpdateBase {
    protected FixedUpdate(): void {
        this.tick = (this.tick ?? 0) + 1
    }
}


export function all(): Array<typeof xor.TsBehaviour & { new(...args): ITest }> {
    return [
        Awake,
        Start,
        OnEnable,
        OnDisable,
        OnDestroy,
        OnGUI,

        Update,
        LateUpdate,
        FixedUpdate,
        UpdateBatch,
        LateUpdateBatch,
        FixedUpdateBatch,
    ]
}