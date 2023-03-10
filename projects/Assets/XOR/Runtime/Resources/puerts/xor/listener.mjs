class Listener {
    constructor() {
        this.handlers = [];
    }
    add(fn) {
        this.handlers.push(fn);
    }
    remove(fn) {
        let index = this.handlers.indexOf(fn);
        if (index >= 0) {
            for (let i = index; i < this.handlers.length - 1; i++) {
                this.handlers[i] = this.handlers[i + 1];
            }
            this.handlers.pop();
        }
    }
    removeAll() {
        this.handlers = [];
    }
    invoke(...args) {
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
//export to csharp
export function quit() {
    let _g = global ?? globalThis ?? this;
    let listener = _g?.xor?.globalListener;
    if (listener && listener.quit) {
        listener.quit.invoke();
    }
}
//# sourceMappingURL=listener.js.map