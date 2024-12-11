import GameObject = CS.UnityEngine.GameObject;
import { ITest, TestBase, name } from "./_base";

class Awake extends TestBase {
    protected Awake(): void {
        this.result = true
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
        this.enabled = false
        await this.waitTime(1)
        this.enabled = true
        await this.waitTime(1)

        this.result = this.tick == 2
    }
}
class OnDisable extends TestBase {
    private tick: number
    protected OnDisable(): void {
        this.tick = (this.tick ?? 0) + 1
    }
    public async run() {
        await super.run()
        this.enabled = false
        await this.waitTime(1)
        this.enabled = true
        await this.waitTime(1)
        this.enabled = false
        await this.waitTime(1)

        this.result = this.tick == 2
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

class Update extends TestBase {
    private tick: number;
    @xor.standalone()
    protected Update(): void {
        this.tick = (this.tick ?? 0) + 1
    }
    public async run() {
        await super.run()

        await this.waitTime(100)
        this.enabled = false
        await this.waitTime(1)
        let t = this.tick
        await this.waitTime(100)

        this.result = this.tick > 0 && this.tick == t;
    }
}
class LateUpdate extends TestBase {
    private tick: number;
    protected LateUpdate(): void {
        this.tick = (this.tick ?? 0) + 1
    }
    @xor.standalone()
    public async run() {
        await super.run()

        await this.waitTime(100)
        this.enabled = false
        await this.waitTime(1)
        let t = this.tick
        await this.waitTime(100)

        this.result = this.tick > 0 && this.tick == t;
    }
}
class FixedUpdate extends TestBase {
    private tick: number;
    @xor.standalone()
    protected FixedUpdate(): void {
        this.tick = (this.tick ?? 0) + 1
    }
    public async run() {
        await super.run()

        await this.waitTime(100)
        this.enabled = false
        await this.waitTime(1)
        let t = this.tick
        await this.waitTime(100)

        this.result = this.tick > 0 && this.tick == t;
    }
}

@name("Update(Batch)")
class UpdateBatch extends TestBase {
    private tick: number;
    protected Update(): void {
        this.tick = (this.tick ?? 0) + 1
    }
    public async run() {
        await super.run()

        await this.waitTime(100)
        this.enabled = false
        await this.waitTime(1)
        let t = this.tick
        await this.waitTime(100)

        this.result = this.tick > 0 && this.tick == t;
    }
}
@name("LateUpdate(Batch)")
class LateUpdateBatch extends TestBase {
    private tick: number;
    protected LateUpdate(): void {
        this.tick = (this.tick ?? 0) + 1
    }
    public async run() {
        await super.run()

        await this.waitTime(100)
        this.enabled = false
        await this.waitTime(1)
        let t = this.tick
        await this.waitTime(100)

        this.result = this.tick > 0 && this.tick == t;
    }
}
@name("FixedUpdate(Batch)")
class FixedUpdateBatch extends TestBase {
    private tick: number;
    protected FixedUpdate(): void {
        this.tick = (this.tick ?? 0) + 1
    }
    public async run() {
        await super.run()

        await this.waitTime(100)
        this.enabled = false
        await this.waitTime(1)
        let t = this.tick
        await this.waitTime(100)

        this.result = this.tick > 0 && this.tick == t;
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