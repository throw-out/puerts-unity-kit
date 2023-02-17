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

//test remote call
function testRemote() {
    console.log(`<b>==THREAD_REMOTE(${ThreadId}) BEGINE=============</b>`);
    function remoteCall(title: string, fn: () => any) {
        try {
            let result = fn();
            console.log(`${title}: success ->${result}`);
        } catch (e) {
            console.error(`${title}: failure ->\n${e}`);
        }
    }
    let gameObject: CS.UnityEngine.GameObject;
    //test static call
    remoteCall("static getter", () => CS.UnityEngine.Application.dataPath);
    remoteCall("static setter", () => CS.UnityEngine.Application.targetFrameRate = 60);
    remoteCall("static construct", () => {
        gameObject = new CS.UnityEngine.GameObject("RemoteConstruct");
        return gameObject?.GetType().FullName;
    });
    remoteCall("static apply", () => CS.UnityEngine.GameObject.Find("RemoteConstruct")?.GetType().FullName);

    if (gameObject) {
        //test instance call
        gameObject = xor.globalWorker.remote(gameObject);
        gameObject = xor.globalWorker.remote(gameObject);       //duplicate creation remote object, but return the same instance.
        remoteCall("instance getter", () => gameObject.name);
        remoteCall("instance setter", () => gameObject.name += "(1)");
        remoteCall("instance apply", () => {
            gameObject.SetActive(false);
            return "ok";
        });

        //test instance restore
        gameObject = xor.globalWorker.local(gameObject);
        remoteCall("restore getter", () => gameObject.name);
    }
    console.log(`<b>==THREAD_REMOTE(${ThreadId}) END=============</b>`);
}
setTimeout(testRemote, 3000);

export { };