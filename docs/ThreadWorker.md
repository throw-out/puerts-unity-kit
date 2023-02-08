## 介绍
> 提供跨线程&跨Puerts.JsEnv实例交互(基于C#类型传递数据);

## 已知缺陷
> 在C#脚本编译或进入Play模式时将会进行AppDomain Unload操作, 此时AppDomain将强制**跨线程**调用JsEnv.Dispose从而导致Unity Crash!
>
> 解决方案如下:
> - 在`Player Settings` - `Other Settings` - `Scripting Define Symbols`中添加`THREAD_SAFE`定义;
>
>ThreadWorker本身是线程安全的, 目前只有AppDomain Unload的情况是例外, 它只会在Editor环境下出现.

## 定义
> 继承: [XOR.ThreadWorker](../projects/Assets/XOR/Runtime/Src/Thread/ThreadWorker.cs) → 无

## 成员
<details>
<summary>查看详情</summary>

| 名称  | 描述  |
| ------------ | ------------ |
</details>

## 方法
<details>
<summary>查看详情</summary>

| 名称  | 描述  |
| ------------ | ------------ |
</details>

## 示例演示
> - 示例场景:[projects/Assets/Samples/04_ThreadWorker](../projects/Assets/Samples/04_ThreadWorker)  
> - 示例typescript代码: [projects/TsProject/src/samples/04_ThreadWorker](../projects/TsProject/src/samples/04_ThreadWorker)

- 主线程代码
```typescript
import * as csharp from "csharp";

const ThreadId: number = csharp.System.Threading["Thread"]["CurrentThread"]["ManagedThreadId"];

export async function init(loader: csharp.Puerts.ILoader) {
    const worker = new xor.ThreadWorker(loader);
    worker.start("./samples/04_ThreadWorker/child");
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
import * as csharp from "csharp";

const ThreadId: number = csharp.System.Threading["Thread"]["CurrentThread"]["ManagedThreadId"];

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
