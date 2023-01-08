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

(function () {
    let _g = (global ?? globalThis ?? this);
    _g["globalListener"] = _g["globalListener"] ?? {
        quit: new Listener()
    };
})();

export { }
/**
 * 接口声明
 */
declare global {
    /**全局监听器 */
    const globalListener: {
        quit: Listener
    }
}
