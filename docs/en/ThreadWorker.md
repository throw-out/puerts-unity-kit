*The current page is translated by a machine, information may be missing or inaccurate. If you encounter any problems or have questions, please submit an ISSUE.*

## Introduction
> Provides `cross-thread` & `cross-Puerts.JsEnv instance` interaction (based on C# type data transfer);  
> Allows sub-threads to invoke UnityEngine Api through the remote method;

## Known Defects
> When compiling C# scripts or entering Play mode, an AppDomain Unload operation will be performed, and at this time, AppDomain will forcibly **cross-thread** call JsEnv.Dispose, causing Unity Crash!
>
> Solution:
> - Add `THREAD_SAFE` definition in `Player Settings` - `Other Settings` - `Scripting Define Symbols`;
>
> ThreadWorker itself is thread-safe, and currently, the only exception is the AppDomain Unload case, which only occurs in the Editor environment.

## Definition
> [`C#`] Inherits: [XOR.ThreadWorker](../../projects/Assets/XOR/Runtime/Src/Thread/ThreadWorker.cs) → None  

> [`ts`] Inherits: [xor.ThreadWorker](../../projects/TsEditorProject/src/xor/worker.ts) → None

<details>
<summary>Interface Details</summary>

| Member  | Description  |
| ------------ | ------------ |
| `get isAlive(): boolean` | Indicates if the thread is currently working |
| `get isInitialized(): boolean` | Indicates if the thread has been initialized |
| `get source(): XOR.ThreadWorker` |  |

| Method  | Description  |
| ------------ | ------------ |
| `start(string, boolean): void` | Starts the instance and specifies the startup script |
| `stop(): void` | Stops the instance (if called from a sub-thread, an event will be sent to the main thread for confirmation) |
| `post(string, any, [boolean]): Promise<any>` | Sends an asynchronous event and retrieves the result |
| `postSync(string, any, [boolean]): any` | Sends a synchronous event and retrieves the result (not available during initialization) |
| `eval(string, [string]): any` | Executes a piece of code, can only be called from the main thread |
| `remote<TConstruct>(TConstruct): TConstruct` | Creates a remote type for using UnityApi in a sub-thread (limited to sub-threads) |
| `remote<T>(T): T` | Creates a remote object for using UnityApi in a sub-thread (limited to sub-threads) |
| `local<T>(T): T` | Gets the original object from a remote object (limited to sub-threads) |
| `on("close", () => void \| false): this` | Listens for the stop event, and if the handler returns false, it will prevent the instance from stopping (limited to the main thread) |
| `on(string, Function): this` | Registers a listener event |
| `once(string, Function): this` | Registers a listener event (automatically removes after one callback) |
| `remove(string, Function): void` | Removes the specified listener handler |
| `removeAll(string): void` | Removes all listener handlers |
</details>

## Example Demonstration
> Example Scene: [projects/Assets/Samples/04_ThreadWorker](../../projects/Assets/Samples/04_ThreadWorker)  
> Example TypeScript Code: [projects/TsProject/src/samples/04_ThreadWorker](../../projects/TsProject/src/samples/04_ThreadWorker)  

- Main Thread Code
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
- Sub-Thread Code
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
> After running, the following statements will be output:
> ![image](https://user-images.githubusercontent.com/45587825/217461927-9e8a13fe-0195-4490-bc3e-7448a06c8ad9.png)
