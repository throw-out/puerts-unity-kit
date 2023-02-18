## 介绍
> 提供`跨线程`&`跨Puerts.JsEnv实例`交互(基于C#类型传递数据);  
> 允许子线程通过remote方式调用UnityEngine Api;

## 已知缺陷
> 在C#脚本编译或进入Play模式时将会进行AppDomain Unload操作, 此时AppDomain将强制**跨线程**调用JsEnv.Dispose从而导致Unity Crash!
>
> 解决方案如下:
> - 在`Player Settings` - `Other Settings` - `Scripting Define Symbols`中添加`THREAD_SAFE`定义;
>
>ThreadWorker本身是线程安全的, 目前只有AppDomain Unload的情况是例外, 它只会在Editor环境下出现.

## 定义
> [`C#`]继承: [XOR.ThreadWorker](../projects/Assets/XOR/Runtime/Src/Thread/ThreadWorker.cs) → 无  

> [`ts`]继承: [xor.ThreadWorker](../projects/TsEditorProject/src/xor/worker.ts) → 无

<details>
<summary>接口详情</summary>

| 成员  | 描述  |
| ------------ | ------------ |
| `get isAlive(): boolean` | 线程是否正在工作中 |
| `get isInitialized(): boolean` | 线程是否已初始化完成 |
| `get source(): XOR.ThreadWorker` |  |

| 方法  | 描述  |
| ------------ | ------------ |
| `start(string, boolean): void` |  开始实例并指定startup脚本  |
| `stop(): void` |  停止实例(如果在子线程调用将发送事件给主线程确认)  |
| `post(string, any, [boolena]): Promise<any>` | 发送异步事件并获取结果 |
| `postSync(string, any, [boolena]): any` | 发送同步事件并获取结果(在初始化阶段不可用) |
| `eval(string, [string]): any` | 执行一段代码, 只能由主线程调用 |
| `remote<TConstruct>(TConstruct): TConstruct` | 创建一个remote类型, 用于在子线程中使用UnityApi(仅限子线程) |
| `remote<T>(T): T` | 创建一个remote对象, 用于在子线程中使用UnityApi(仅限子线程) |
| `local<T>(T): T` | 从remote对象上获取原始对象(仅限子线程) |
| `on("close", () => void \| false): this` | 监听停止事件, 如handler返回false将阻止实例停止(仅限主线程) |
| `on(string, Function): this` | 注册一个监听事件 |
| `once(string, Function): this` | 注册一个监听事件(回调一次后自动移除) |
| `remove(string, Function): void` | 移除指定监听handler |
| `removeAll(string): void` | 移除所有监听handlers |
</details>

## 示例演示
> 示例场景:[projects/Assets/Samples/04_ThreadWorker](../projects/Assets/Samples/04_ThreadWorker)  
> 示例typescript代码: [projects/TsProject/src/samples/04_ThreadWorker](../projects/TsProject/src/samples/04_ThreadWorker)  

- 主线程代码
```typescript
const ThreadId: number = CS.System.Threading["Thread"]["CurrentThread"]["ManagedThreadId"];

export async function init(loader: CS.Puerts.ILoader) {
    const worker = new xor.ThreadWorker(loader);
    
    const module = "./samples/04_ThreadWorker/child";
    worker.start(module, loader["IsESM"] ? loader["IsESM"](module) : false);
    xor.globalListener.quit.add(() => worker.stop());

    worker.on("main_test1", (msg) => {
        console.log(`thread(${ThreadId}) receive: ${msg}`);
        return "ok";
    });
    //wait worker initialize completed.
    while (!worker.isInitialized) {
        await new Promise<void>(resovle => setTimeout(resovle, 1));
    }

    //send async message
    worker.post<number>("child_test1", "main_thread_event_1").then((ret) => {
        console.log(`result value: ${ret}`);
    });
    //send sync message
    let ret = worker.postSync<number>("child_test2", "main_thread_event_2");
    console.log(`result value: ${ret}`);
}
```
- 子线程代码
```typescript
const ThreadId: number = CS.System.Threading["Thread"]["CurrentThread"]["ManagedThreadId"];

xor.globalWorker.on("child_test1", (msg) => {
    console.log(`thread(${ThreadId}) receive: ${msg}`);
    return 1;
});
xor.globalWorker.on("child_test2", (msg) => {
    console.log(`thread(${ThreadId}) receive: ${msg}`);
    return 2;
});

setTimeout(() => {
    xor.globalWorker.post("main_test1", "child_thread_event");
}, 2000);
```
> 运行后将输出以下语句:  
> ![image](https://user-images.githubusercontent.com/45587825/217461927-9e8a13fe-0195-4490-bc3e-7448a06c8ad9.png)
