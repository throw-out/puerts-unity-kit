import $CS = CS;
declare const CLOSE_EVENT = "close";
type Construct<T = any> = Function & {
    new (...args: any[]): T;
};
/**
 * 跨JsEnv实例交互封装
 */
declare class ThreadWorkerConstructor {
    private readonly mainThread;
    private readonly worker;
    private readonly events;
    private _postIndex;
    private _remoteRegistered;
    /**线程是否正在工作 */
    get isAlive(): boolean;
    /**线程是否已初始化完成 */
    get isInitialized(): boolean;
    get source(): $CS.XOR.ThreadWorker;
    constructor(loader: $CS.Puerts.ILoader, options?: $CS.XOR.ThreadOptions);
    start(filepath: string, isESM?: boolean): void;
    stop(): void;
    /**异步调用事件, 无返回值
     * @param eventName
     * @param data
     * @param notResult 不获取返回值
     */
    post<TResult = void>(eventName: string, data?: any, notResult?: true): Promise<TResult>;
    /**同步调用事件, 并立即获取返回值
     * @param eventName
     * @param data
     * @param throwOnError
     * @returns
     */
    postSync<TResult = any>(eventName: string, data?: any, throwOnError?: boolean): TResult;
    /**执行一段代码, 只能由主线程调用
     * @param chunk
     * @param chunkName
     */
    eval(chunk: string, chunkName?: string): void;
    /**创建Type Construct的remote Proxy对象, 以便于在子线程中调用(仅)主线程方法(仅限受支持的C#类型, 仅限子线程调用)
     * @param construct
     */
    remote<T extends $CS.System.Object>(construct: Construct): Construct;
    /**创建C#对象的Remote Proxy对象, 以便于在子线程中调用(仅)主线程方法(仅限受支持的C#类型, 仅限子线程调用)
     * @param instance
     */
    remote<T extends $CS.System.Object>(instance: T): T;
    /**监听ThreadWorker close消息(从子线程中请求), 只能由主线程处理, 返回flase将阻止ThreadWorker实例销毁
     * @param eventName
     * @param fn
     */
    on(eventName: typeof CLOSE_EVENT, fn: () => void | false): this;
    /**监听事件信息
     * @param eventName
     * @param fn
     */
    on<T = any, TResult = void>(eventName: string, fn: (data?: T) => TResult): this;
    /**监听事件信息(仅回调一次后自动取消注册)
     * @param eventName
     * @param fn
     * @returns
     */
    once(eventName: string, fn: Function): this;
    /**移除指定监听事件 */
    remove(eventName: string, fn: Function): void;
    /**移除所有监听事件 */
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
    /**验证data数据
     * @param data
     * @returns 0:纯json数据, 1:引用UnityObject, 2:包含js functon/js symbol等参数
     */
    private _validate;
    /**postSync返回值接口事件名
     * @returns
     */
    private _getResultId;
    private _isResultId;
    /**remote proxy方法 */
    private _isProxyType;
    private _createTypeProxy;
    private _createMethodProxy;
    private _createInstanceProxy;
}
/**接口声明 */
declare global {
    namespace xor {
        class ThreadWorker extends ThreadWorkerConstructor {
        }
        /**
         * 只能在JsWorker内部访问, 与主线程交互的对象
         */
        const globalWorker: ThreadWorker;
    }
}
export declare function bind(worker: $CS.XOR.ThreadWorker): void;
export {};
