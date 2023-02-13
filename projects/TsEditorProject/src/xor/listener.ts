class Listener {
    private handlers: Function[] = [];
    public add(fn: Function) {
        this.handlers.push(fn);
    }
    public remove(fn: Function) {
        let index = this.handlers.indexOf(fn);
        if (index >= 0) {
            for (let i = index; i < this.handlers.length - 1; i++) {
                this.handlers[i] = this.handlers[i + 1];
            }
            this.handlers.pop();
        }
    }
    public removeAll() {
        this.handlers = [];
    }
    public invoke(...args: any[]) {
        for (let func of this.handlers) {
            func(...args);
        }
    }
}

function register() {
    let _g = (global ?? globalThis ?? this);
    _g.xor = _g.xor || {};
    _g.xor.globalListener = _g.xor.globalListener ?? {
        quit: new Listener()
    };
}
register();

/**
 * 接口声明
 */
declare global {
    namespace xor {
        /**全局监听器 */
        const globalListener: {
            readonly quit: Listener;
        }
    }
}

//export to csharp
export function quit() {
    let _g = global ?? globalThis ?? this;
    let listener: typeof xor.globalListener = _g?.xor?.globalListener;
    if (listener && listener.quit) {
        listener.quit.invoke();
    }
}
