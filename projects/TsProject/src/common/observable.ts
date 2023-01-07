export class Observable<T, TData = never> {
    public get closed() { return this._closed; }
    public get count() { return this._closed ? 0 : this._subscribers.length; }
    private _closed: boolean = false;
    private _subscribers: Array<Subscriber<T>> = [];
    private _defaultValue: TData;
    private _subscribe?: (subscriber: Subscriber<T>, defaultValue?: TData) => void;

    constructor(subscribe?: (subscriber: Subscriber<T>, defaultValue?: TData) => void) {
        this._subscribe = subscribe;
    }
    public subscribe(callback: (val: T) => void, immediately: boolean = true): Subscription {
        if (this._closed)
            return undefined;
        let subscriber = new Subscriber<T>(callback);
        this._subscribers.push(subscriber);
        if (immediately) {
            this._subscribe?.call(this, subscriber, this._defaultValue);
        }
        return subscriber;
    }
    public next(value: T) {
        if (this._closed) return;
        let closed = 0;
        this._subscribers.forEach(subscriber => {
            if (subscriber.closed) closed++;
            else subscriber.next(value);
        });
        if (closed > 0) {
            this._subscribers = this._subscribers.filter(subscriber => !subscriber.closed);
        }
    }
    public unsubscribe() {
        if (this._closed) return;
        this._subscribers.forEach(subscriber => subscriber.unsubscribe());
        this._subscribers = [];
    }
    public dispose() {
        if (this._closed) return;
        this.unsubscribe();
        this._closed = true;
    }

    public setDefault(data: TData) {
        this._defaultValue = data;
    }
}
export class Subscription {
    public get closed() { return this._closed; }
    private _closed: boolean = false;
    public unsubscribe() {
        this._closed = true;
    }
}
export class Subscriber<T> extends Subscription {
    private _closedFn: () => void;
    private _callback: (val: T) => void;

    constructor(callback: (val: T) => void) {
        super();
        this._callback = callback;
    }
    public unsubscribe() {
        if (this.closed) return;
        super.unsubscribe();

        let cb = this._closedFn;
        this._callback = undefined;
        this._closedFn = undefined;

        if (cb) cb();
    }
    public onClosed(fn: () => void) {
        this._closedFn = fn;
    }
    public next(value: T) {
        if (this.closed) return;
        this._callback?.call(this, value);
    }
}
