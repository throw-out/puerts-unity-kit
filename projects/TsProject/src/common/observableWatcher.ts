import * as CS from "csharp";
import { Observable, Subscription } from "./observable";

import UnityObject = CS.UnityEngine.Object;
import Sprite = CS.UnityEngine.Sprite;
import Texture2D = CS.UnityEngine.Texture2D;

//UnityEngine.UI Component
import Text = CS.UnityEngine.UI.Text;
import Image = CS.UnityEngine.UI.Image;
import RawImage = CS.UnityEngine.UI.RawImage;
import Slider = CS.UnityEngine.UI.Slider;
import Toggle = CS.UnityEngine.UI.Toggle;
import Dropdown = CS.UnityEngine.UI.Dropdown;
import Scrollbar = CS.UnityEngine.UI.Scrollbar;
import InputField = CS.UnityEngine.UI.InputField;

type Descriptor<TInput, TOutput> = {
    setter?: (val: TInput) => TOutput,
    getter?: (val: TOutput) => TInput,
    /**auto unsubscribe when the component is destroyed (default: true)*/
    auto?: boolean,
};

const WATCHER = "__watcher__";
const WATCHER_VALUE = "__watcher_value__";
const WATCHER_TARGET = "__watcher_target__";

type NonFunctionKeys<T extends object> = { [K in keyof T]?: T[K] extends Function ? never : K }[keyof T];

type Share<T extends object, TKey extends NonFunctionKeys<T>> = {
    watcher: Watcher<T>,
    observable: Observable<T[TKey]>
};

/**数据监听*/
export class Watcher<T extends object> {
    protected data: T;
    protected disposed: boolean;

    protected keys: Array<NonFunctionKeys<T>>;
    protected fieldInfos: Map<string, Share<any, any>>;
    protected commonInfos: Map<string, Array<Share<any, any>>>;

    constructor(data: T) {
        this.data = data;
        this.keys = new Array();
        this.fieldInfos = new Map();
        this.commonInfos = data[WATCHER];
        if (!this.commonInfos) {
            this.commonInfos = data[WATCHER] = new Map();
        }
    }

    /**
     * watch field info. when it changes invoke
     * @param key           field name
     * @param callback      callback method
     * @param immediately   immediately invoke
     * @returns 
     */
    public subscribe<TKey extends NonFunctionKeys<T>>(key: TKey, callback: (val: T[TKey]) => void, immediately: boolean = true) {
        if (this.disposed) return undefined;
        let observable = this.getObservable(key);
        return observable.subscribe(callback, immediately);
    }

    /**
    * watch field info. when it changes invoke
    * @param key           field name
    * @param callback      callback method
    * @param immediately   immediately invoke
    * @returns 
    */
    public subscribeCustom(key: string, callback: (val: any) => void, immediately: boolean = true) {
        if (this.disposed) return undefined;
        let observable = this.getObservable(key as NonFunctionKeys<T>);
        return observable.subscribe(callback, immediately);
    }

    /**
     * unsubscribe
     * @param key           field name
     * @returns 
     */
    public unsubscribe<TKey extends NonFunctionKeys<T>>(key: TKey) {
        if (this.disposed) return;
        let observable = this.getObservable(key);
        observable.unsubscribe();
    }

    /**unsubscribe all fields and release instance */
    public dispose() {
        if (this.disposed) return;
        this.disposed = true;
        for (let key of this.keys) {
            let _key = this.getObservableKey(key);
            let share = this.fieldInfos.get(_key);
            share.observable.dispose();
            let shares = this.getShares(key);
            shares.splice(shares.indexOf(share), 1);
            if (shares.length === 0) {           //delete defineProperty
                this.commonInfos.delete(_key);
                let val = this.data[key];
                delete this.data[key];
                this.data[key] = val;
            }
        }
        delete this.commonInfos;
    }

    protected getObservable<TKey extends NonFunctionKeys<T>>(key: TKey) {
        let _key = this.getObservableKey(key);
        let share: Share<T, TKey> = this.fieldInfos.get(_key);
        if (!share) {
            let shares = this.getShares(key);
            share = {
                watcher: this,
                observable: new Observable(subscriber => subscriber.next(shares[WATCHER_VALUE]))
            }
            shares.push(share);

            this.keys.push(key);
            this.fieldInfos.set(_key, share);
        }
        return share.observable;
    }
    protected getShares<TKey extends NonFunctionKeys<T>>(key: TKey) {
        let _key = this.getObservableKey(key);
        let shares: Array<Share<T, TKey>> = this.commonInfos.get(_key);
        if (!shares) {
            shares = new Array();

            let value = this.data[key];
            if (value === undefined || value === void 0) {
                value = this.data[key] = null;
            }
            shares[WATCHER_VALUE] = value;
            Object.defineProperty(this.data, key, {
                get: function () {
                    return value;
                },
                set: function (val) {
                    if (value !== val) {
                        value = val;
                        shares[WATCHER_VALUE] = value;
                        [...shares].forEach(share => {
                            if (share.watcher.isDestroyed()) {
                                share.watcher.dispose();
                                return;
                            }
                            share.observable.next(val);
                        });
                    }
                }
            });

            this.commonInfos.set(_key, shares);
        }
        return shares;
    }
    protected getObservableKey<TKey extends NonFunctionKeys<T>>(key: TKey) {
        return `__${key as string}_observable__`;
    }
    protected isDestroyed() {
        return !!this.disposed;
    }

    /**
     * clear all subscriptions on data
     * @param data 
     */
    public static disposeData<T extends object>(data: T) {
        let commonInfos: Map<string, Array<Share<any, any>>> = data[WATCHER];
        if (commonInfos) {
            let defineKeys = new Array<string>();
            //dispose observables
            for (let shares of [...commonInfos.values()]) {
                for (let { observable, watcher: adapter } of shares) {
                    defineKeys.push(...adapter.keys.filter(k => defineKeys.indexOf(k) < 0));
                    observable.dispose();
                    delete adapter.commonInfos;
                }
            }
            //delete defineProperty
            for (let key of defineKeys) {
                let val = data[key];
                delete data[key];
                data[key] = val;
            }
            delete data[WATCHER];
        }
    }
}

export class WatcherObject<T extends object> extends Watcher<T>{

    protected target: UnityObject;

    /**
     * auto unsubscribe Subscription when the target is destroyed
     * @param target 
     * @param single  clear target Adapter if true (default: true)
     * @returns 
     */
    public bindObject(target: UnityObject, single: boolean = true): WatcherObject<T> {
        let adapters: WatcherObject<T> | Array<WatcherObject<T>>;
        if (this.target && this.target !== target) {
            adapters = this.target[WATCHER_TARGET];
            if (adapters instanceof WatcherObject)
                this.target[WATCHER_TARGET] = undefined;
            else if (Array.isArray(adapters))
                this.target[WATCHER_TARGET] = adapters.filter(o => o !== this);
        }
        this.target = target;

        if (target === undefined || target === null || target === void 0)
            return this;

        adapters = target[WATCHER_TARGET];
        if (!adapters)
            adapters = [];
        else if (!Array.isArray(adapters))
            adapters = [adapters];

        adapters = adapters.filter(o => o !== this);
        if (single) {
            adapters.filter(o => !o.disposed).forEach(o => o.dispose());
            target[WATCHER_TARGET] = this;
        } else {
            adapters.push(this);
            target[WATCHER_TARGET] = adapters;
        }
        return this;
    }

    /**bind the component to this key */
    public bind<TKey extends NonFunctionKeys<T>>(key: TKey, component: Text, descriptor?: Descriptor<T[TKey], string>): this;
    public bind<TKey extends NonFunctionKeys<T>>(key: TKey, component: Image, descriptor?: Descriptor<T[TKey], Sprite>): this;
    public bind<TKey extends NonFunctionKeys<T>>(key: TKey, component: RawImage, descriptor?: Descriptor<T[TKey], Texture2D>): this;
    public bind<TKey extends NonFunctionKeys<T>>(key: TKey, component: Toggle, descriptor?: Descriptor<T[TKey], boolean>): this;
    public bind<TKey extends NonFunctionKeys<T>>(key: TKey, component: InputField, descriptor?: Descriptor<T[TKey], string>): this;
    public bind<TKey extends NonFunctionKeys<T>>(key: TKey, component: Slider, descriptor?: Descriptor<T[TKey], number>): this;
    public bind<TKey extends NonFunctionKeys<T>>(key: TKey, component: Dropdown, descriptor?: Descriptor<T[TKey], number>): this;
    public bind<TKey extends NonFunctionKeys<T>>(key: TKey, component: Scrollbar, descriptor?: Descriptor<T[TKey], number>): this;
    public bind() {
        let key: NonFunctionKeys<T> = arguments[0], component: UnityObject = arguments[1], descriptor: Descriptor<any, any> = arguments[2];

        let { setter, getter, auto } = descriptor ?? {};
        if (!setter) setter = (val) => val;

        if (auto === undefined) auto = true;

        let subscription: Subscription;
        let set: (val: any) => void;

        if (component instanceof Text) {
            set = (val) => (<Text>component).text = val;
        }
        else if (component instanceof Image) {
            set = (val) => (<Image>component).sprite = val;
        }
        else if (component instanceof RawImage) {
            set = (val) => (<RawImage>component).texture = val;
        }
        else if (component instanceof Toggle) {
            set = (val) => (<Toggle>component).isOn = val;
            if (getter) {
                let listener = (val: boolean) => {
                    if (subscription.closed) {
                        (<Toggle>component).onValueChanged.RemoveListener(listener);
                        return;
                    }
                    this.data[key] = getter(val);
                }
                (<Toggle>component).onValueChanged.AddListener(listener);
            }
        }
        else if (component instanceof InputField) {
            set = (val) => (<InputField>component).text = val;
            if (getter) {
                let listener = (val: string) => {
                    if (subscription.closed) {
                        (<InputField>component).onValueChanged.RemoveListener(listener);
                        return;
                    }
                    this.data[key] = getter(val);
                }
                (<InputField>component).onValueChanged.AddListener(listener);
            }
        }
        else if (component instanceof Slider || component instanceof Dropdown || component instanceof Scrollbar) {
            set = (val) => (<Slider | Dropdown | Scrollbar>component).value = val;
            if (getter) {
                let listener = (val: number) => {
                    if (subscription.closed) {
                        (<Slider | Dropdown | Scrollbar>component).onValueChanged.RemoveListener(listener);
                        return;
                    }
                    this.data[key] = getter(val);
                }
                (<Slider | Dropdown | Scrollbar>component).onValueChanged.AddListener(listener);
            }
        }
        else {
            throw new Error("unsupport component: " + component)
        }
        subscription = this.subscribe(key, (val) => {
            if (auto && (!component || component.Equals(null))) {
                subscription.unsubscribe();
                return;
            }
            set(setter(val));
        }, true);

        return this;
    }
    protected isDestroyed() {
        return super.isDestroyed() || this.target && this.target.Equals(null);
    }

    /**
     * clear all subscriptions on object
     * @param target 
     */
    public static disposeObject(target: UnityObject) {
        let adapters: WatcherObject<any> | Array<WatcherObject<any>> = target[WATCHER_TARGET];
        if (adapters !== undefined && Array.isArray(adapters))
            adapters.filter(o => !o.disposed).forEach(o => o.dispose());
        else if (adapters instanceof WatcherObject && !adapters.disposed)
            adapters.dispose();

        delete target[WATCHER_TARGET];
    }
}