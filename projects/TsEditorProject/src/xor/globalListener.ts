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
    _g.XOR = _g.XOR || {};
    _g.XOR["globalListener"] = _g.XOR.globalListener ?? {
        quit: new Listener()
    };
}
register();

export { }
/**
 * 接口声明
 */
declare global {
    namespace XOR {
        /**全局监听器 */
        const globalListener: {
            readonly quit: Listener
        }
    }
}
