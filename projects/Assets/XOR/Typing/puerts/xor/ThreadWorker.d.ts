import * as csharp from "csharp";
declare const CLOSE_EVENT_NAME = "__e_close";
declare class ThreadWorkerImpl {
    private readonly mainThread;
    private readonly worker;
    private readonly events;
    constructor(loader: csharp.Puerts.ILoader);
    start(filepath: string): void;
    stop(): void;
    post(eventName: string, data?: any): void;
    postSync<T = any>(eventName: string, data?: any, throwOnError?: boolean): T;
    eval(chunk: string, chunkName?: string): void;
    on(eventName: typeof CLOSE_EVENT_NAME, fn: () => void | false): void;
    on<T = any, TResult = void>(eventName: string, fn: (data?: T) => TResult): void;
    remove(eventName: string, fn: Function): void;
    removeAll(eventName?: string): void;
    private register;
    private pack;
    private unpack;
    private _packByRefs;
    private _unpackByRefs;
    private _validate;
}
declare global {
    namespace XOR {
        class ThreadWorker extends ThreadWorkerImpl {
        }
        const globalWorker: ThreadWorkerImpl;
    }
}
export {};
