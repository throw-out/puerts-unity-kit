import * as csharp from "csharp";
declare const CLOSE_EVENT = "__e_close__";
declare class ThreadWorkerImpl {
    private readonly mainThread;
    private readonly worker;
    private readonly events;
    private _postIndex;
    get isAlive(): boolean;
    get isInitialized(): boolean;
    get source(): csharp.XOR.ThreadWorker;
    constructor(loader: csharp.Puerts.ILoader, options?: csharp.XOR.ThreadOptions);
    start(filepath: string): void;
    stop(): void;
    post<TResult = void>(eventName: string, data?: any, notResult?: true): Promise<TResult>;
    postSync<TResult = any>(eventName: string, data?: any, throwOnError?: boolean): TResult;
    eval(chunk: string, chunkName?: string): void;
    on(eventName: typeof CLOSE_EVENT, fn: () => void | false): this;
    on<T = any, TResult = void>(eventName: string, fn: (data?: T) => TResult): this;
    once(eventName: string, fn: Function): this;
    remove(eventName: string, fn: Function): void;
    removeAll(eventName?: string): void;
    private _on;
    private _emit;
    private register;
    private registerRemoteProxy;
    private executeRemoteResolver;
    private pack;
    private unpack;
    private _packByRefs;
    private _unpackByRefs;
    private _validate;
    private _getResultId;
    private _isResultId;
}
declare global {
    namespace xor {
        class ThreadWorker extends ThreadWorkerImpl {
        }
        const globalWorker: ThreadWorkerImpl;
    }
}
export {};
