const INVOKE_TICK = Symbol("INVOKE_TICK");

type EventName = string | number;

export class EventEmitter {
    private _events: Map<EventName, Array<Function>>;

    private _on(eventName: EventName, handler: Function) {
        if (!this._events)
            this._events = new Map();

        let functions = this._events.get(eventName);
        if (!functions) {
            functions = [];
            this._events.set(eventName, functions);
        }
        functions.push(handler);
    }
    public on(eventName: EventName, handler: Function): this | void {
        delete handler[INVOKE_TICK];
        this._on(eventName, handler);
        return this;
    }
    public once(eventName: EventName, handler: Function): this | void {
        handler[INVOKE_TICK] = 1;
        this._on(eventName, handler);
        return this;
    }
    public remove(eventName: EventName, handler: Function) {
        if (!this._events)
            return;
        let functions = this._events.get(eventName);
        if (!functions)
            return;
        let idx = functions.indexOf(handler);
        if (idx >= 0) {
            functions.splice(idx, 1);
        }
    }
    public removeAll(eventName: EventName) {
        if (!this._events)
            return;
        this._events.delete(eventName);
    }
    protected emit(eventName: EventName, ...args: any[]) {
        if (!this._events)
            return;
        let functions = this._events.get(eventName);
        if (!functions)
            return;
        let rmHandlers = new Array<Function>();
        functions.forEach(func => {
            func.apply(undefined, args);
            if (INVOKE_TICK in func && (--(<number>func[INVOKE_TICK])) <= 0) {
                rmHandlers.push(func);
            }
        });
        if (rmHandlers.length > 0) {
            this._events.set(eventName, functions.filter(func => !rmHandlers.includes(func)));
        }
    }
    protected clear() {
        this._events = undefined;
    }
}

/**混合类方法
 * @param target 
 * @param source 
 * @param force         defalut: true, 是否强制替换方法
 */
export function mixin(target: ConstructorType<any>, source: ConstructorType<any>, force?: boolean): void;
export function mixin(source: ConstructorType<any>, force?: boolean): ClassDecorator;
export function mixin() {
    let target: ConstructorType<any> = arguments[0],
        source: ConstructorType<any> = arguments[1],
        force: boolean = arguments[2];
    if (!source || typeof (source) === "boolean") {
        source = arguments[0];
        force = arguments[1];
        return function (target: ConstructorType<any>) {
            mixin(target, source, force);
        }
    } else {
        if (force === undefined) {
            force = true;
        }

        let descriptors = Object.getOwnPropertyDescriptors(source.prototype);
        for (let pKey of Object.keys(descriptors)) {
            if (pKey === "constructor" || !force && target.prototype[pKey])
                continue;
            Object.defineProperty(target.prototype, pKey, descriptors[pKey]);
        }
    }
}