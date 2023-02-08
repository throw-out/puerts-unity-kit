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