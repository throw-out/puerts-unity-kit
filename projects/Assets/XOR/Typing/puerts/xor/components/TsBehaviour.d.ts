import * as csharp from "csharp";
import Transform = csharp.UnityEngine.Transform;
import GameObject = csharp.UnityEngine.GameObject;
import RectTransform = csharp.UnityEngine.RectTransform;
import PointerEventData = csharp.UnityEngine.EventSystems.PointerEventData;
import Collision = csharp.UnityEngine.Collision;
import Collision2D = csharp.UnityEngine.Collision2D;
import Collider = csharp.UnityEngine.Collider;
import Collider2D = csharp.UnityEngine.Collider2D;
declare abstract class IBehaviour {
    protected onConstructor?(): void;
    protected Awake?(): void;
    protected Start?(): void;
    protected OnEnable?(): void;
    protected OnDisable?(): void;
    protected OnDestroy?(): void;
    protected Update?(deltaTime?: number): void;
    protected FixedUpdate?(deltaTime?: number): void;
    protected LateUpdate?(deltaTime?: number): void;
    protected OnGUI?(): void;
    protected OnApplicationQuit?(): void;
    protected OnApplicationFocus?(focus: boolean): void;
    protected OnApplicationPause?(pause: boolean): void;
}
declare abstract class IGizmos {
    protected OnDrawGizmosSelected?(): void;
    protected OnSceneGUI?(): void;
}
declare abstract class IOnPointerHandler {
    protected OnPointerClick?(eventData: PointerEventData): void;
    protected OnPointerDown?(eventData: PointerEventData): void;
    protected OnPointerUp?(eventData: PointerEventData): void;
    protected OnPointerEnter?(eventData: PointerEventData): void;
    protected OnPointerExit?(eventData: PointerEventData): void;
}
declare abstract class IOnDragHandler {
    protected OnBeginDrag?(eventData: PointerEventData): void;
    protected OnDrag?(eventData: PointerEventData): void;
    protected OnEndDrag?(eventData: PointerEventData): void;
}
declare abstract class IOnCollision {
    protected OnCollisionEnter?(collision: Collision): void;
    protected OnCollisionStay?(collision: Collision): void;
    protected OnCollisionExit?(collision: Collision): void;
}
declare abstract class IOnCollision2D {
    protected OnCollisionEnter2D?(collision: Collision2D): void;
    protected OnCollisionStay2D?(collision: Collision2D): void;
    protected OnCollisionExit2D?(collision: Collision2D): void;
}
declare abstract class IOnTrigger {
    protected OnTriggerEnter?(collider: Collider): void;
    protected OnTriggerStay?(collider: Collider): void;
    protected OnTriggerExit?(collider: Collider): void;
}
declare abstract class IOnTrigger2D {
    protected OnTriggerEnter2D?(collider: Collider2D): void;
    protected OnTriggerStay2D?(collider: Collider2D): void;
    protected OnTriggerExit2D?(collider: Collider2D): void;
}
declare abstract class IOnMouse {
    protected OnMouseDown?(): void;
    protected OnMouseDrag?(): void;
    protected OnMouseEnter?(): void;
    protected OnMouseExit?(): void;
    protected OnMouseOver?(): void;
    protected OnMouseUp?(): void;
    protected OnMouseUpAsButton?(): void;
}
declare class TsBehaviourImpl {
    private __transform__;
    private __gameObject__;
    private __component__;
    private __listeners__;
    private __listenerProxy__;
    constructor(trf: Transform | GameObject, refs?: boolean | csharp.XOR.TsPropertys | csharp.XOR.TsPropertys[]);
    StartCoroutine(routine: ((...args: any[]) => Generator) | Generator, ...args: any[]): csharp.UnityEngine.Coroutine;
    StopCoroutine(routine: csharp.UnityEngine.Coroutine): void;
    StopAllCoroutines(): void;
    protected addListener(eventName: string, fn: Function): void;
    protected removeListener(eventName: string, fn: Function): void;
    protected removeAllListeners(eventName: string): void;
    protected clearListeners(): void;
    private _invokeListeners;
    protected disponse(): void;
    private _bindProxies;
    private _bindUpdateProxies;
    private _bindListeners;
    private _bindModuleForEditor;
    get transform(): Transform;
    get gameObject(): GameObject;
    get enabled(): boolean;
    set enabled(value: boolean);
    get isActiveAndEnabled(): boolean;
    get tag(): string;
    set tag(value: string);
    get name(): string;
    set name(value: string);
    get rectTransform(): RectTransform;
    protected get component(): csharp.XOR.TsBehaviour;
}
interface TsBehaviourImpl extends IBehaviour, IGizmos, IOnPointerHandler, IOnDragHandler, IOnMouse, IOnCollision, IOnCollision2D, IOnTrigger, IOnTrigger2D {
}
declare namespace TsBehaviourImpl {
    function bindPropertys(instance: object, refs: csharp.XOR.TsPropertys | csharp.XOR.TsPropertys[] | csharp.System.Array$1<csharp.XOR.TsPropertys>, destroy?: boolean): void;
    function Standalone(): PropertyDecorator;
    function Frameskip(value: number): PropertyDecorator;
    function Throttle(enable: boolean): PropertyDecorator;
    function Listener(eventName?: string): PropertyDecorator;
}
declare global {
    namespace XOR {
        class TsBehaviour extends TsBehaviourImpl {
        }
    }
}
export {};
