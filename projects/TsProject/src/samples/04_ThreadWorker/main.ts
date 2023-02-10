const ThreadId: number = CS.System.Threading["Thread"]["CurrentThread"]["ManagedThreadId"];

export async function init(loader: CS.Puerts.ILoader) {
    const module = "./samples/04_ThreadWorker/child";
    const worker = new xor.ThreadWorker(loader);
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