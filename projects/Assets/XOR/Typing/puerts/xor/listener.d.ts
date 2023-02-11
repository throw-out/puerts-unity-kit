declare class Listener {
    private handlers;
    add(fn: Function): void;
    remove(fn: Function): void;
    removeAll(): void;
    invoke(...args: any[]): void;
}
/**
 * 接口声明
 */
declare global {
    namespace xor {
        /**全局监听器 */
        const globalListener: {
            readonly quit: Listener;
        };
    }
}
export declare function quit(): void;
export {};
