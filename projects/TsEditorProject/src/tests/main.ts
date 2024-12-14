import { System } from "csharp";
import { ITest } from "./behaviour/_base";

import * as ApplicationTests from "./behaviour/ApplicationTests";
import * as BaseEventsTests from "./behaviour/BaseEventsTests";
import * as EditTests from "./behaviour/EditTests";
import * as LogicTests from "./behaviour/LogicTests";
import * as MouseTests from "./behaviour/MouseTests";
import * as PhysicsCollider2DTests from "./behaviour/PhysicsCollider2DTests";
import * as PhysicsColliderTests from "./behaviour/PhysicsColliderTests";
import * as PhysicsCollision2DTests from "./behaviour/PhysicsCollision2DTests";
import * as PhysicsCollisionTests from "./behaviour/PhysicsCollisionTests";
import * as PointerEventsTests from "./behaviour/PointerEventsTests";
import * as RendererTests from "./behaviour/RendererTests";

import GameObject = CS.UnityEngine.GameObject;
function createTest<T extends typeof xor.TsBehaviour & { new(...args): ITest }>(ctor: T) {
    let go = new GameObject(ctor.name);
    return new ctor(go);
}


async function runTests(tests: Array<typeof xor.TsBehaviour & { new(...args): ITest }>) {
    for (let ctor of tests) {
        let test: ITest, error: string
        try {
            test = createTest(ctor)
            let now = Date.now()
            await test.run()
            let duration = Date.now() - now
            if (duration < 100) {
                await new Promise<void>(resolve => setTimeout(resolve, duration - 100))
            }
        } catch (e) {
            error = `${e}`
            console.error(e)
        } finally {
            test?.destroy()
        }
        if (!test) {
            console.error(`${ctor.name}: \tconstructor failure!`);
        }
        else if (test.result) {
            console.log(`${ctor.name}: \t<color=green>pass</color>`);
        } else {
            console.log(`${ctor.name}: \t<color=red>failure</color>\n${error}`);
        }
    }
}
export async function run(next: object) {
    let tests = [
        ...LogicTests.all(),
        ...ApplicationTests.all(),
        ...RendererTests.all(),
        ...EditTests.all(),
        ...MouseTests.all(),
        ...BaseEventsTests.all(),
        ...PointerEventsTests.all(),
        ...PhysicsColliderTests.all(),
        ...PhysicsCollider2DTests.all(),
        ...PhysicsCollisionTests.all(),
        ...PhysicsCollision2DTests.all(),
    ]
    console.log("========================================");
    console.log("==默认Callback调用=======================");
    console.log("========================================");

    await runTests(tests);

    console.log("========================================");
    console.log("==全局Invoker调用========================");
    console.log("========================================");

    xor.TsBehaviour.setGlobalInvoker(true);
    await runTests(tests);
    xor.TsBehaviour.setGlobalInvoker(false);

    if (!next)
        return
    if (typeof (next) === "function") {
        next();
    }
    else if (next instanceof System.Action) {
        next.Invoke();
    }
}