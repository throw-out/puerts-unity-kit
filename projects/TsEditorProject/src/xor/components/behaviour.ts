import XOR = CS.XOR;
import Transform = CS.UnityEngine.Transform;
import GameObject = CS.UnityEngine.GameObject;
import RectTransform = CS.UnityEngine.RectTransform;
import Application = CS.UnityEngine.Application;
import PointerEventData = CS.UnityEngine.EventSystems.PointerEventData;
import Collision = CS.UnityEngine.Collision;
import Collision2D = CS.UnityEngine.Collision2D;
import Collider = CS.UnityEngine.Collider;
import Collider2D = CS.UnityEngine.Collider2D;
import Time = CS.UnityEngine.Time;
import CSObject = CS.System.Object;

const { File, Path } = CS.System.IO;
const isEditor = Application.isEditor;

type AccessorType = CS.UnityEngine.Component & CS.XOR.Serializables.IAccessor;
type AccessorUnionType = AccessorType | AccessorType[] | CS.System.Array$1<AccessorType>;

type ConstructorEvent = <T extends TsBehaviourConstructor = any>(this: T) => void;
type ConstructorOptions = {
    accessor?: AccessorUnionType | boolean;
    /**传递给onConstructor的参数 */
    args?: any[];
    /**在绑定Unity生命周期之前调用 */
    before?: ConstructorEvent;
    /**在绑定Unity生命周期之后调用 */
    after?: ConstructorEvent;
};

/**
 * 详情参阅: https://docs.unity3d.com/cn/current/ScriptReference/MonoBehaviour.html
 */
abstract class IBehaviour {
    /**
     * 创建实例时被调用 
     */
    protected onConstructor?(...args: any[]): void;

    /**
     * Awake在加载脚本实例时调用。
     * (如果游戏对象在启动期间处于非活动状态，则在激活之后才会调用 Awake。)
     */
    protected Awake?(): void;
    /**
     * 仅当启用脚本实例后，才会在第一次帧更新之前调用 Start。 
     */
    protected Start?(): void;
    /**
     * (仅在对象处于激活状态时调用)在启用对象后立即调用此函数。
     * 在创建 MonoBehaviour 实例时（例如加载关卡或实例化具有脚本组件的游戏对象时）会执行此调用。
     */
    protected OnEnable?(): void;
    /**
     * 行为被禁用或处于非活动状态时，调用此函数。 
     */
    protected OnDisable?(): void;
    /**
     * 对象存在的最后一帧完成所有帧更新之后，调用此函数（可能应 Object.Destroy 要求或在场景关闭时销毁该对象）。 
     */
    protected OnDestroy?(): void;


    /**
     * 每帧调用一次 Update。这是用于帧更新的主要函数。 
     * @param deltaTime 批量调用时将传参此值
     */
    protected Update?(deltaTime?: number): void;
    /**
     * 用于物理计算且独立于帧率的 MonoBehaviour.FixedUpdate 消息。
     * 调用 FixedUpdate 的频度常常超过 Update。如果帧率很低，可以每帧调用该函数多次；
     * 如果帧率很高，可能在帧之间完全不调用该函数。在 FixedUpdate 之后将立即进行所有物理计算和更新。在 FixedUpdate 内应用运动计算时，无需将值乘以 Time.deltaTime。这是因为 FixedUpdate 的调用基于可靠的计时器（独立于帧率）。
     * @param deltaTime 批量调用时将传参此值
     */
    protected FixedUpdate?(deltaTime?: number): void;
    /**
     * 每帧调用一次 LateUpdate（在 Update完成后）。
     * LateUpdate 开始时，在 Update 中执行的所有计算便已完成。LateUpdate 的常见用途是跟随第三人称摄像机。如果在 Update 内让角色移动和转向，可以在 LateUpdate 中执行所有摄像机移动和旋转计算。这样可以确保角色在摄像机跟踪其位置之前已完全移动。
     * @param deltaTime 批量调用时将传参此值
     */
    protected LateUpdate?(deltaTime?: number): void;


    /**
     * 每帧调用多次以响应 GUI 事件。首先处理布局和重新绘制事件，然后为每个输入事件处理布局和键盘/鼠标事件。
     */
    protected OnGUI?(): void;
    /**
     * 在退出应用程序之前在所有游戏对象上调用此函数。在编辑器中，用户停止播放模式时，调用函数。
     */
    protected OnApplicationQuit?(): void;
    /**
     * 
     * @param focus 
     */
    protected OnApplicationFocus?(focus: boolean): void;
    /**
     * 在帧的结尾处调用此函数（在正常帧更新之间有效检测到暂停）。
     * 在调用 OnApplicationPause 之后，将发出一个额外帧，从而允许游戏显示图形来指示暂停状态。
     * @param pause 
     */
    protected OnApplicationPause?(pause: boolean): void;
}
abstract class IGizmos {
    /**
     * (仅Editor可用)
     * Gizmos 类允许您将线条、球体、立方体、图标、纹理和网格绘制到 Scene 视图中，在开发项目时用作调试、设置的辅助手段或工具。
     * Handles 类似于 Gizmos，但在交互性和操作方面提供了更多功能。Unity 本身提供的用于在 Scene 视图中操作项目的 3D 控件是 Gizmos 和 Handles 的组合。内置的 Handle GUI 有很多，如通过变换组件定位、缩放和旋转对象等熟悉的工具。不过，您可以自行定义 Handle GUI，以与自定义组件编辑器结合使用。此类 GUI 对于编辑以程序方式生成的场景内容、“不可见”项和相关对象的组（如路径点和位置标记）非常实用。
     */
    protected OnDrawGizmosSelected?(): void;
    /**(仅Editor可用)*/
    protected OnSceneGUI?(): void;
}
abstract class IOnPointerHandler {
    /**
     * 实现C#接口: UnityEngine.EventSystems.IPointerClickHandler
     * @see CS.UnityEngine.EventSystems.IPointerClickHandler
     * @param eventData 
     */
    protected OnPointerClick?(eventData: PointerEventData): void;
    /**
     * 实现C#接口: UnityEngine.EventSystems.IPointerDownHandler
     * @see CS.UnityEngine.EventSystems.IPointerDownHandler
     * @param eventData 
     */
    protected OnPointerDown?(eventData: PointerEventData): void;
    /**
     * 实现C#接口: UnityEngine.EventSystems.IPointerUpHandler
     * @see CS.UnityEngine.EventSystems.IPointerUpHandler
     * @param eventData 
     */
    protected OnPointerUp?(eventData: PointerEventData): void;
    /**
     * 实现C#接口: UnityEngine.EventSystems.IPointerEnterHandler
     * @see CS.UnityEngine.EventSystems.IPointerEnterHandler
     * @param eventData 
     */
    protected OnPointerEnter?(eventData: PointerEventData): void;
    /**
     * 实现C#接口: UnityEngine.EventSystems.IPointerExitHandler
     * @see CS.UnityEngine.EventSystems.IPointerExitHandler
     * @param eventData 
     */
    protected OnPointerExit?(eventData: PointerEventData): void;
}
abstract class IOnDragHandler {
    /**
     * 实现C#接口: UnityEngine.EventSystems.IBeginDragHandler
     * @see CS.UnityEngine.EventSystems.IBeginDragHandler
     * @param eventData 
     */
    protected OnBeginDrag?(eventData: PointerEventData): void;
    /**
     * 实现C#接口: UnityEngine.EventSystems.IDragHandler
     * @see CS.UnityEngine.EventSystems.IDragHandler
     * @param eventData 
     */
    protected OnDrag?(eventData: PointerEventData): void;
    /**
     * 实现C#接口: UnityEngine.EventSystems.IEndDragHandler
     * @see CS.UnityEngine.EventSystems.IEndDragHandler
     * @param eventData 
     */
    protected OnEndDrag?(eventData: PointerEventData): void;

}
abstract class IOnCollision {
    /**
     * Collision组件事件回调(Enter)
     * @param collision 
     */
    protected OnCollisionEnter?(collision: Collision): void;
    /**
     * Collision组件事件回调(Stay)
     * @param collision 
     */
    protected OnCollisionStay?(collision: Collision): void;
    /**
     * Collision组件事件回调(Exit)
     * @param collision 
     */
    protected OnCollisionExit?(collision: Collision): void;

}
abstract class IOnCollision2D {
    /**
     * Collision2D组件事件回调(Enter)
     * @param collision 
     */
    protected OnCollisionEnter2D?(collision: Collision2D): void;
    /**
     * Collision2D组件事件回调(Stay)
     * @param collision 
     */
    protected OnCollisionStay2D?(collision: Collision2D): void;
    /**
     * Collision2D组件事件回调(Exit)
     * @param collision 
     */
    protected OnCollisionExit2D?(collision: Collision2D): void;

}
abstract class IOnTrigger {
    /**
     * Collider组件事件回调(Enter)
     * @param collider 
     */
    protected OnTriggerEnter?(collider: Collider): void;
    /**
     * Collider组件事件回调(Stay)
     * @param collider 
     */
    protected OnTriggerStay?(collider: Collider): void;
    /**
     * Collider组件事件回调(Exit)
     * @param collider 
     */
    protected OnTriggerExit?(collider: Collider): void;
}
abstract class IOnTrigger2D {
    /**
     * Collider2D组件事件回调(Enter)
     * @param collider 
     */
    protected OnTriggerEnter2D?(collider: Collider2D): void;
    /**
     * Collider2D组件事件回调(Stay)
     * @param collider 
     */
    protected OnTriggerStay2D?(collider: Collider2D): void;
    /**
     * Collider2D组件事件回调(Exit)
     * @param collider 
     */
    protected OnTriggerExit2D?(collider: Collider2D): void;
}
abstract class IOnMouse {
    /**
     * 当用户在 Collider 上按下鼠标按钮时，将调用 OnMouseDown。
     */
    protected OnMouseDown?(): void;
    /**
     * 当用户单击 Collider 并仍然按住鼠标时，将调用 OnMouseDrag。在按住鼠标按钮的情况下，每帧调用一次 OnMouseDrag。
     */
    protected OnMouseDrag?(): void;
    /**
     * 当鼠标进入 Collider 时调用。
     * 当鼠标停留在对象上时，调用相应的 OnMouseOver 函数； 当鼠标移开时，调用 OnMouseExit。
     */
    protected OnMouseEnter?(): void;
    /**
     * 当鼠标不再处于 Collider 上方时调用。OnMouseExit 调用跟随在相应的 OnMouseEnter 和 OnMouseOver 调用之后。
     * 在属于 Ignore Raycast 层的对象上，不调用该函数。
     * 当且仅当 Physics.queriesHitTriggers 为 true 时，才在标记为触发器的碰撞体上调用该函数。
     * 如果在函数中的某处添加 yield 语句，则可以将 OnMouseExit 用作协程。 此事件将发送至附加到 Collider 的所有脚本。
     */
    protected OnMouseExit?(): void;
    /**
     * 当鼠标悬停在 Collider 上时，每帧调用一次。
     * OnMouseEnter 在鼠标处于该对象上方的第一帧调用。 然后，每帧都调用 OnMouseOver，直到移开鼠标 - 此时，调用 OnMouseExit。
     * 在属于 Ignore Raycast 层的对象上，不调用该函数。
     * 当且仅当 Physics.queriesHitTriggers 为 true 时，才在标记为触发器的碰撞体上调用该函数。
     * OnMouseOver 可以是协程，在函数中只是使用 yield 语句。 此事件将发送至附加到 Collider 的所有脚本。
     */
    protected OnMouseOver?(): void;
    /**
     * 当用户松开鼠标按钮时，将调用 OnMouseUp。 
     * 请注意，即使鼠标不在按下鼠标时所在的 Collider 上，也会调用 OnMouseUp。 有关类似于按钮的行为，请参阅：OnMouseUpAsButton。
     */
    protected OnMouseUp?(): void;
    /**
     * 松开鼠标时，仅当鼠标在按下时所在的 Collider 上时，才调用 OnMouseUpAsButton。
     */
    protected OnMouseUpAsButton?(): void;
}

/**
 * 沿用C# MonoBehaviour习惯, 将OnEnable丶Update丶OnEnable等方法绑定到C#对象上, Unity将在生命周期内调用
 * 
 * 注: 为避免多次跨语言调用, Update丶FixedUpdate丶LateUpdate方法将由BatchProxy统一管理(并非绑定到各自的GameObject上)
 * @see standalone 如果需要绑定独立的组件, 在对应方法上添加此标注
 */
abstract class BehaviourConstructor {
    private __listeners__: Map<string, Function[]>;
    private __listenerProxy__: CS.XOR.TsMessages;
    private __updateElement__: UpdateManager.Element;
    private __componentID__: number;
    //--------------------------------------------------------
    //协程
    public StartCoroutine(routine: ((...args: any[]) => Generator) | Generator, ...args: any[]): CS.UnityEngine.Coroutine {
        //传入了js Generator方法, 转为C#迭代器对象
        var iterator = inner.cs_generator(routine, ...args);
        return this.component.StartCoroutine(iterator);
    }
    public StopCoroutine(routine: CS.UnityEngine.Coroutine) {
        this.component.StopCoroutine(routine);
    }
    public StopAllCoroutines() {
        this.component.StopAllCoroutines();
    }

    /**添加Unity Message listener
     * @param eventName 
     * @param fn 
     */
    protected addListener(eventName: string, fn: Function) {
        //create message proxy
        if (!this.__listenerProxy__ || this.__listenerProxy__.Equals(null)) {
            this.__listenerProxy__ = (
                this.gameObject.GetComponent(puerts.$typeof(CS.XOR.TsMessages)) ??
                this.gameObject.AddComponent(puerts.$typeof(CS.XOR.TsMessages))
            ) as CS.XOR.TsMessages;
            this.__listenerProxy__.emptyCallback = () => this._invokeListeners('');
            this.__listenerProxy__.callback = (name, args) => this._invokeListeners(name, args);
        }
        //add listeners
        if (!this.__listeners__) {
            this.__listeners__ = new Map();
        }
        if (eventName === undefined || eventName === null || eventName === void 0)
            eventName = '';
        let functions = this.__listeners__.get(eventName);
        if (!functions) {
            functions = [];
            this.__listeners__.set(eventName, functions);
        }
        functions.push(fn);
    }
    /**移除Unity Message listener
     * @param eventName 
     * @param fn 
     */
    protected removeListener(eventName: string, fn: Function) {
        if (!this.__listeners__)
            return;
        if (eventName === undefined || eventName === null || eventName === void 0)
            eventName = '';
        let functions = this.__listeners__.get(eventName);
        if (!functions)
            return;
        functions = functions.filter(f => f !== fn);
        if (functions.length > 0) {
            this.__listeners__.set(eventName, functions);
        } else {
            this.__listeners__.delete(eventName);
            if (this.__listeners__.size === 0)
                this.clearListeners();
        }
    }
    /**移除所有Unity Message listener
     * @param eventName 
     */
    protected removeAllListeners(eventName: string) {
        if (!this.__listeners__)
            return;
        if (eventName === undefined || eventName === null || eventName === void 0)
            eventName = '';
        this.__listeners__.delete(eventName);
        if (this.__listeners__.size === 0)
            this.clearListeners();
    }
    /**清空Unity Message listener */
    protected clearListeners() {
        if (!this.__listeners__)
            return;
        this.__listeners__ = null;
        this.__listenerProxy__.callback = null;
        this.__listenerProxy__.emptyCallback = null;
    }
    private _invokeListeners(eventName: string, args?: Array<any> | CS.System.Array$1<any>) {
        if (!this.__listeners__) {
            console.warn(`invail invoke: ${eventName}`);
            return;
        }
        let functions = this.__listeners__.get(eventName);
        if (!functions)
            return;

        if (args instanceof CS.System.Array) {
            let _args = new Array<any>();
            for (let i = 0; i < args.Length; i++) {
                _args.push(args.get_Item(i));
            }
            args = _args;
        }
        functions.forEach(fn => fn.apply(undefined, args));
    }

    //protected
    protected disponse() {
        if (this.__componentID__) {
            GlobalManager.unregister(this.__componentID__)
        }
        if (this.__updateElement__) {
            UpdateManager.unregister(this.__updateElement__)
        }
    }
    //绑定生命周期方法
    protected bindLifecycle() {
        const proto = Object.getPrototypeOf(this);
        const isGlobalInvoker = !!CS.XOR.Behaviour.Invoker.Default;
        if (isGlobalInvoker) {
            this.__componentID__ = this.component.GetObjectID()
            GlobalManager.register(this.__componentID__, this)
        }

        //注册Mono事件
        let methodFlags = 0
        let specificFlags = CS.XOR.Behaviour.Args.Mono.Awake |
            CS.XOR.Behaviour.Args.Mono.Start |
            CS.XOR.Behaviour.Args.Mono.OnDestroy |
            CS.XOR.Behaviour.Args.Mono.OnEnable |
            CS.XOR.Behaviour.Args.Mono.OnDisable
        let updateFlags = CS.XOR.Behaviour.Args.Mono.Update |
            CS.XOR.Behaviour.Args.Mono.FixedUpdate |
            CS.XOR.Behaviour.Args.Mono.LateUpdate
        for (let funcname in CS.XOR.Behaviour.Args.Mono) {
            let v = CS.XOR.Behaviour.Args.Mono[funcname]
            if (typeof (v) != "number")
                continue;
            let hasFunc = typeof (this[funcname]) == "function"
            if ((specificFlags & v) > 0) {
                if (hasFunc)
                    continue;
                specificFlags ^= v
            }
            else if ((updateFlags & v) > 0) {
                if (!hasFunc) {
                    updateFlags ^= v
                    continue;
                }
                if (metadata.isDefine(proto, funcname, utils.standalone)) {
                    updateFlags ^= v
                    methodFlags |= v
                }
            }
            else {
                if (!hasFunc)
                    continue;
                methodFlags |= v
            }
        }
        if (specificFlags > 0) {  //注册Awake丶Start丶OnDestroy事件
            this.component.CreateMono(specificFlags, isGlobalInvoker ? undefined : (method) => {
                let funcname = CS.XOR.Behaviour.Args.Mono[method]
                switch (method) {
                    case CS.XOR.Behaviour.Args.Mono.Awake:
                    case CS.XOR.Behaviour.Args.Mono.Start:
                    case CS.XOR.Behaviour.Args.Mono.OnDestroy:
                        inner.invoke(this, funcname, true)
                        break;
                    default:
                        inner.invoke(this, funcname, false)
                        break;
                }
            })
        }
        if (methodFlags > 0) {  //注册剩余Mono事件
            this.component.CreateMono(methodFlags, isGlobalInvoker ? undefined : (method) => this[CS.XOR.Behaviour.Args.Mono[method]]())
        }
        if (updateFlags > 0) {
            let element = new UpdateManager.Element(updateFlags, this);
            this.__updateElement__ = element;
            UpdateManager.register(element)
            if (isGlobalInvoker) {
                element.enabled = !this.component.IsDestroyed && this.component.IsEnable;
            }
            else {
                //生命周期管理
                this.component.CreateMono(CS.XOR.Behaviour.Args.Mono.OnEnable | CS.XOR.Behaviour.Args.Mono.OnDisable | CS.XOR.Behaviour.Args.Mono.OnDestroy, (method) => {
                    switch (method) {
                        case CS.XOR.Behaviour.Args.Mono.OnEnable:
                            element.enabled = true;
                            break;
                        case CS.XOR.Behaviour.Args.Mono.OnDisable:
                            element.enabled = false;
                            break;
                        case CS.XOR.Behaviour.Args.Mono.OnDestroy:
                            element.enabled = false;
                            UpdateManager.unregister(element)
                            break;
                    }
                })
            }
        }

        //注册Gizmos事件
        if (isEditor) {
            methodFlags = inner.getFunctionFlags(this, CS.XOR.Behaviour.Args.Gizmos)
            if (methodFlags > 0) {
                this.component.CreateGizmos(methodFlags, isGlobalInvoker ? undefined : (method) => this[CS.XOR.Behaviour.Args.Gizmos[method]]())
            }
        }
        //注册Mouse事件
        methodFlags = inner.getFunctionFlags(this, CS.XOR.Behaviour.Args.Mouse)
        if (methodFlags > 0) {
            this.component.CreateMouse(methodFlags, isGlobalInvoker ? undefined : (method) => this[CS.XOR.Behaviour.Args.Mouse[method]]())
        }
        //注册MonoBoolean事件
        methodFlags = inner.getFunctionFlags(this, CS.XOR.Behaviour.Args.MonoBoolean)
        if (methodFlags > 0) {
            this.component.CreateMonoBoolean(methodFlags, isGlobalInvoker ? undefined : (method, data) => this[CS.XOR.Behaviour.Args.MonoBoolean[method]](data))
        }
        //注册EventSystems事件
        methodFlags = inner.getFunctionFlags(this, CS.XOR.Behaviour.Args.EventSystems)
        if (methodFlags > 0) {
            this.component.CreateEventSystems(methodFlags, isGlobalInvoker ? undefined : (method, data) => this[CS.XOR.Behaviour.Args.EventSystems[method]](data))
        }
        //注册Physics事件
        methodFlags = inner.getFunctionFlags(this, CS.XOR.Behaviour.Args.PhysicsCollider)
        if (methodFlags > 0) {
            this.component.CreatePhysicsCollider(methodFlags, isGlobalInvoker ? undefined : (method, data) => this[CS.XOR.Behaviour.Args.PhysicsCollider[method]](data))
        }
        methodFlags = inner.getFunctionFlags(this, CS.XOR.Behaviour.Args.PhysicsCollider2D)
        if (methodFlags > 0) {
            this.component.CreatePhysicsCollider2D(methodFlags, isGlobalInvoker ? undefined : (method, data) => this[CS.XOR.Behaviour.Args.PhysicsCollider2D[method]](data))
        }
        methodFlags = inner.getFunctionFlags(this, CS.XOR.Behaviour.Args.PhysicsCollision)
        if (methodFlags > 0) {
            this.component.CreatePhysicsCollision(methodFlags, isGlobalInvoker ? undefined : (method, data) => this[CS.XOR.Behaviour.Args.PhysicsCollision[method]](data))
        }
        methodFlags = inner.getFunctionFlags(this, CS.XOR.Behaviour.Args.PhysicsCollision2D)
        if (methodFlags > 0) {
            this.component.CreatePhysicsCollision2D(methodFlags, isGlobalInvoker ? undefined : (method, data) => this[CS.XOR.Behaviour.Args.PhysicsCollision2D[method]](data))
        }
    }
    protected bindListeners() {
        const proto = Object.getPrototypeOf(this);
        for (let funcname of metadata.getKeys(proto)) {
            let eventName = metadata.getDefineData(proto, funcname, utils.listener);
            if (!eventName)
                continue;
            let waitAsyncComplete = metadata.getDefineData(proto, funcname, utils.throttle, false);
            let func: CS.System.Action = inner.bind(this, funcname, waitAsyncComplete);
            if (!func)
                return undefined;
            this.addListener(eventName, func);
        }
    }
    //绑定脚本内容
    protected bindModuleInEditor() {
        if (!isEditor || !this.gameObject || this.gameObject.Equals(null))
            return;
        //堆栈信息
        let stack = new Error().stack
            .replace(/\r\n/g, "\n")
            .split('\n')
            .slice(2)
            .join("\n");
        //class名称
        let className = Object.getPrototypeOf(this).constructor.name;

        let moduleName: string, modulePath: string, moduleLine: number, moduleColumn: number;
        //匹配new构造函数
        //let regex = /at [a-zA-z0-9#$._ ]+ \(([a-zA-Z0-9:/\\._ ]+(.js|.ts))\:([0-9]+)\:([0-9]+)\)/g;
        let regex = /at [a-zA-z0-9#$._ ]+ \(([^\n\r\*\"\|\<\>]+(.js|.cjs|.mjs|.ts|.mts))\:([0-9]+)\:([0-9]+)\)/g;
        let match: RegExpExecArray;
        while (match = regex.exec(stack)) {
            let isConstructor = match[0].includes("at new ");   //是否调用构造对象函数
            if (isConstructor) {
                let path = match[1].replace(/\\/g, "/");
                let line = match[3], column = match[4];
                if (path.endsWith(".js") || path.endsWith(".cjs") || path.endsWith(".mjs")) {
                    //class在声明变量时赋初值, 构造函数中sourceMap解析会失败: 故此处尝试读取js.map文件手动解析
                    try {
                        let mapPath = path + ".map", tsPath: string;
                        let sourceMap = File.Exists(mapPath) ? JSON.parse(File.ReadAllText(mapPath)) : null;
                        if (sourceMap && Array.isArray(sourceMap.sources) && sourceMap.sources.length == 1) {
                            tsPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), sourceMap.sources[0]));
                        }
                        if (File.Exists(tsPath)) {
                            path = tsPath;
                            line = column = "0";
                            //尝试寻常class信息
                            let lines = File.ReadAllLines(tsPath);
                            for (let i = 0; i < lines.Length; i++) {
                                let content = lines.get_Item(i);
                                if (content.indexOf(`class ${className}`) >= 0 || content.indexOf(`function ${className}`) >= 0) {
                                    line = (i + 1).toString();
                                    break;
                                }
                            }
                        }
                    } catch (e) { console.warn(e); }
                }

                modulePath = path;
                moduleName = path.substring(path.lastIndexOf("/") + 1);
                moduleLine = parseInt(line ?? "0");
                moduleColumn = parseInt(column ?? "0");
            } else if (modulePath) {
                break;
            }
        }
        let module = new CS.XOR.ModuleInfo();
        module.className = className;
        if (modulePath) {
            module.moduleName = moduleName;
            module.modulePath = modulePath;
            module.line = moduleLine;
            module.column = moduleColumn;
            module.stack = stack;
        } else {
            console.warn(`Unresolved Module: ${className}\n${stack}`);
        }
        this.component["Module"] = module;
    }

    //Getter 丶 Setter
    public abstract get transform(): Transform;
    public abstract get gameObject(): GameObject;
    protected abstract get component(): XOR.TsBehaviour;
    public get enabled() { return this.component.enabled; }
    public set enabled(value: boolean) { this.component.enabled = value; }
    public get isActiveAndEnabled() { return this.component.isActiveAndEnabled; }
    public get tag() { return this.gameObject.tag; }
    public set tag(value: string) { this.gameObject.tag = value; }
    public get name() { return this.gameObject.name; }
    public set name(value: string) { this.gameObject.name = value; }
    public get rectTransform() {
        if (!this.transform)
            return undefined;
        if (!("__rectTransform__" in this)) {
            this["__rectTransform__"] = this.transform instanceof RectTransform ? this.transform : null;
        }
        return this["__rectTransform__"] as RectTransform;
    }
}
//无实际意义: 仅作为继承子类提示接口名称用
interface BehaviourConstructor extends IBehaviour, IGizmos, IOnPointerHandler, IOnDragHandler, IOnMouse, IOnCollision, IOnCollision2D, IOnTrigger, IOnTrigger2D {
}

class TsBehaviourConstructor extends BehaviourConstructor {
    private __transform__: Transform;
    private __gameObject__: GameObject;
    private __component__: CS.XOR.TsBehaviour;
    //--------------------------------------------------------
    public get transform(): Transform {
        return this.__transform__;
    }
    public get gameObject(): GameObject {
        return this.__gameObject__;
    }
    protected get component(): XOR.TsBehaviour {
        if (!this.__component__ || this.__component__.Equals(null)) {
            this.__component__ = this.__gameObject__.GetComponent(puerts.$typeof(CS.XOR.TsBehaviour)) as CS.XOR.TsBehaviour;
            if (!this.__component__ || this.__component__.Equals(null)) {
                this.__component__ = this.__gameObject__.AddComponent(puerts.$typeof(CS.XOR.TsBehaviour)) as CS.XOR.TsBehaviour;
            }
        }
        return this.__component__;
    }

    public constructor(object: GameObject | Transform | CS.XOR.TsBehaviour, accessor?: AccessorUnionType | boolean);
    public constructor(object: GameObject | Transform | CS.XOR.TsBehaviour, options?: ConstructorOptions);
    public constructor() {
        super();
        let object: GameObject | Transform | CS.XOR.TsBehaviour = arguments[0],
            accessor: AccessorUnionType | boolean,
            args: any[],
            before: ConstructorEvent,
            after: ConstructorEvent;
        let p2 = arguments[1];
        switch (typeof (p2)) {
            case "object":
                if (p2 === null) { }
                else if (p2 instanceof CSObject || Array.isArray(p2)) {
                    accessor = <any>p2;
                } else {
                    accessor = (<ConstructorOptions>p2).accessor;
                    args = (<ConstructorOptions>p2).args;
                    before = (<ConstructorOptions>p2).before;
                    after = (<ConstructorOptions>p2).after;
                }
                break;
            case "boolean":
                accessor = <any>p2;
                break;
        }

        let gameObject: GameObject;
        if (object instanceof CS.XOR.TsBehaviour) {
            gameObject = object.gameObject;
            this.__component__ = object;
        }
        else if (object instanceof Transform) {
            gameObject = object.gameObject;
        }
        else {
            gameObject = object;
        }
        this.__gameObject__ = gameObject;
        this.__transform__ = gameObject.transform;
        //call before callback
        if (before) inner.invoke(this, before, true);

        //bind properties
        if (accessor === undefined || accessor === true) {
            utils.bindAccessor(
                this,
                object.GetComponents(puerts.$typeof(CS.XOR.TsProperties)) as CS.System.Array$1<CS.XOR.TsProperties>,
                true
            );
        }
        else if (accessor) {
            utils.bindAccessor(this, accessor, true);
        }
        //call constructor
        let onctor: Function = this["onConstructor"];
        if (onctor && typeof (onctor) === "function") {
            if (args && args.length > 0) {
                inner.invoke(this, onctor, true, ...args);
            } else {
                inner.invoke(this, onctor, true);
            }
        }
        //bind methods
        this.bindLifecycle();
        this.bindListeners();
        this.bindModuleInEditor();

        //call after callback
        if (after) inner.invoke(this, after, true);
    }

    protected disponse() {
        this.__gameObject__ = undefined;
        this.__transform__ = undefined;
        this.__component__ = undefined;
    }

    /**注册全局生命周期回调, 每个TsBehaviour实例不再单独创建多个生命周期回调绑定 */
    public static registerGlobalInvoker() {
        GlobalManager.init();
    }
}

/**全局对象管理 */
class GlobalManager {
    private static readonly objects = new Map<number, BehaviourConstructor>();
    public static register(objectID: number, obj: BehaviourConstructor) {
        this.objects.set(objectID, obj);
    }
    public static unregister(objectID: number) {
        this.objects.delete(objectID);
    }
    public static init() {
        let invoker = new CS.XOR.Behaviour.Invoker();
        invoker.mono = (objectID, method) => {
            switch (method) {
                case CS.XOR.Behaviour.Args.Mono.Awake:
                case CS.XOR.Behaviour.Args.Mono.Start:
                    this.invoke(objectID, CS.XOR.Behaviour.Args.Mono[method], true)
                    break;
                case CS.XOR.Behaviour.Args.Mono.OnDestroy:
                    this.unregister(objectID)
                    this.setUpdateElement(objectID, false)
                    this.invoke(objectID, CS.XOR.Behaviour.Args.Mono[method], true)
                    break;
                case CS.XOR.Behaviour.Args.Mono.OnEnable:
                    this.setUpdateElement(objectID, true)
                    this.invoke(objectID, CS.XOR.Behaviour.Args.Mono[method], false)
                    break;
                case CS.XOR.Behaviour.Args.Mono.OnDisable:
                    this.setUpdateElement(objectID, false)
                    this.invoke(objectID, CS.XOR.Behaviour.Args.Mono[method], false)
                    break;
                default:
                    this.invoke(objectID, CS.XOR.Behaviour.Args.Mono[method], false)
                    break;
            }
        }
        invoker.monoBoolean = (objectID, method, data) => this.invoke(objectID, CS.XOR.Behaviour.Args.MonoBoolean[method], false, data)
        invoker.gizmos = (objectID, method) => this.invoke(objectID, CS.XOR.Behaviour.Args.Gizmos[method], false)
        invoker.mouse = (objectID, method) => this.invoke(objectID, CS.XOR.Behaviour.Args.Mouse[method], false)
        invoker.eventSystems = (objectID, method, data) => this.invoke(objectID, CS.XOR.Behaviour.Args.EventSystems[method], false, data)
        invoker.collision = (objectID, method, data) => this.invoke(objectID, CS.XOR.Behaviour.Args.PhysicsCollider[method], false, data)
        invoker.collision2D = (objectID, method, data) => this.invoke(objectID, CS.XOR.Behaviour.Args.PhysicsCollider2D[method], false, data)
        invoker.collision = (objectID, method, data) => this.invoke(objectID, CS.XOR.Behaviour.Args.PhysicsCollision[method], false, data)
        invoker.collision2D = (objectID, method, data) => this.invoke(objectID, CS.XOR.Behaviour.Args.PhysicsCollision2D[method], false, data)
        invoker.destroy = (objectID) => this.unregister(objectID)
        CS.XOR.Behaviour.Invoker.Default = invoker
    }

    private static invoke(objectID: number, funcname: string, catchExecption: boolean, ...args: any) {
        let obj = this.objects.get(objectID);
        if (!obj)
            return;
        inner.invoke(obj, funcname, catchExecption, ...args)
    }
    private static setUpdateElement(objectID: number, enabled: boolean) {
        let obj = this.objects.get(objectID);
        if (!obj || !obj["__updateElement__"])
            return;
        obj["__updateElement__"].enabled = enabled;
    }
}
/**Update批量调用管理 */
class UpdateManager {
    private static Elements = class {
        private readonly every: UpdateManager.Element[] = [];
        private readonly frame: Map<number, {
            tick: number,
            dt: number,
            elements: UpdateManager.Element[]
        }> = new Map();

        /**调用类型:
         * 0: Update
         * 1: LateUpdate
         * 2: FixedUpdate
         */
        private readonly lifecycle: CS.XOR.Behaviour.Args.Mono;

        constructor(lifecycle: CS.XOR.Behaviour.Args.Mono) {
            this.lifecycle = lifecycle;
        }

        public add(element: UpdateManager.Element, skip: number) {
            if (skip <= 0) {
                this.every.push(element);
                return;
            }
            let state = this.frame.get(skip)
            if (!state) {
                state = { tick: 0, dt: 0, elements: [] }
                this.frame.set(skip, state)
            }
            state.elements.push(element);
        }
        public remove(element: UpdateManager.Element, skip: number) {
            const units = skip > 0 ? this.frame.get(skip)?.elements : this.every;
            if (!units || units.length <= 0)
                return;
            const index = units.indexOf(element);
            if (index >= 0) {
                units.splice(index, 1)
            }
        }
        public tick(dt: number) {
            //每帧调用
            if (this.every.length > 0) {
                for (let element of this.every) {
                    element.invoke(this.lifecycle, dt)
                }
            }
            //跨帧调用
            for (const [t, state] of this.frame) {
                state.dt += dt;
                if ((--state.tick) > 0)
                    continue;
                if (state.elements.length > 0) {
                    for (let element of state.elements) {
                        element.invoke(this.lifecycle, state.dt)
                    }
                }
                state.tick = t;
                state.dt = 0;
            }
        }
    }

    private static _init: boolean;

    private static readonly update = new UpdateManager.Elements(CS.XOR.Behaviour.Args.Mono.Update);
    private static readonly lateUpdate = new UpdateManager.Elements(CS.XOR.Behaviour.Args.Mono.LateUpdate);
    private static readonly fixedUpdate = new UpdateManager.Elements(CS.XOR.Behaviour.Args.Mono.FixedUpdate);
    public static register(element: UpdateManager.Element) {
        if (element.isUpdate) {
            this.update.add(element, element.updateSkipFrame);
        }
        if (element.isLateUpdate) {
            this.lateUpdate.add(element, element.lateUpdateSkipFrame);
        }
        if (element.isFixedUpdate) {
            this.fixedUpdate.add(element, element.fixedUpdateSkipFrame);
        }
        if (!this._init) {
            this._init = true;
            this.init();
        }
    }
    public static unregister(element: UpdateManager.Element) {
        if (element.isUpdate) {
            this.update.remove(element, element.updateSkipFrame);
        }
        if (element.isLateUpdate) {
            this.lateUpdate.remove(element, element.lateUpdateSkipFrame);
        }
        if (element.isFixedUpdate) {
            this.fixedUpdate.remove(element, element.fixedUpdateSkipFrame);
        }
    }
    public static init() {
        let go = new CS.UnityEngine.GameObject("[UpdateBatchManager]");
        go.transform.SetParent((CS.XOR.Application.GetInstance() as CS.XOR.Application).transform);
        (go.AddComponent(puerts.$typeof(CS.XOR.Behaviour.Default.UpdateBehaviour)) as CS.XOR.Behaviour.Default.UpdateBehaviour).Callback = () => {
            this.update.tick(Time.deltaTime);
        };
        (go.AddComponent(puerts.$typeof(CS.XOR.Behaviour.Default.LateUpdateBehaviour)) as CS.XOR.Behaviour.Default.LateUpdateBehaviour).Callback = () => {
            this.lateUpdate.tick(Time.deltaTime);
        };
        (go.AddComponent(puerts.$typeof(CS.XOR.Behaviour.Default.FixedUpdateBehaviour)) as CS.XOR.Behaviour.Default.FixedUpdateBehaviour).Callback = () => {
            this.fixedUpdate.tick(Time.fixedDeltaTime);
        };
    }
}
namespace UpdateManager {
    export class Element {
        private target: BehaviourConstructor;
        /**是否启用当前组件 */
        public enabled: boolean;

        public readonly isUpdate: boolean;
        public readonly isLateUpdate: boolean;
        public readonly isFixedUpdate: boolean;

        public readonly updateSkipFrame: number;
        public readonly lateUpdateSkipFrame: number;
        public readonly fixedUpdateSkipFrame: number;

        constructor(methods: CS.XOR.Behaviour.Args.Mono, target: BehaviourConstructor) {
            this.target = target;
            this.enabled = false;
            this.isUpdate = (methods & CS.XOR.Behaviour.Args.Mono.Update) > 0;
            this.isLateUpdate = (methods & CS.XOR.Behaviour.Args.Mono.LateUpdate) > 0;
            this.isFixedUpdate = (methods & CS.XOR.Behaviour.Args.Mono.FixedUpdate) > 0;

            const proto = Object.getPrototypeOf(this);
            this.updateSkipFrame = this.isUpdate ? metadata.getDefineData(
                proto,
                CS.XOR.Behaviour.Args.Mono[CS.XOR.Behaviour.Args.Mono.Update],
                utils.frameskip,
                0
            ) : 0
            this.lateUpdateSkipFrame = this.isLateUpdate ? metadata.getDefineData(
                proto,
                CS.XOR.Behaviour.Args.Mono[CS.XOR.Behaviour.Args.Mono.LateUpdate],
                utils.frameskip,
                0
            ) : 0
            this.fixedUpdateSkipFrame = this.isFixedUpdate ? metadata.getDefineData(
                proto,
                CS.XOR.Behaviour.Args.Mono[CS.XOR.Behaviour.Args.Mono.FixedUpdate],
                utils.frameskip,
                0
            ) : 0
        }
        public invoke(lifecycle: CS.XOR.Behaviour.Args.Mono, dt?: number) {
            if (!this.enabled)
                return;
            inner.invoke(
                this.target,
                CS.XOR.Behaviour.Args.Mono[lifecycle],
                false,
                dt
            )
        }
    }
}

namespace inner {

    /**将对象与方法绑定
     * @param thisArg 
     * @param funcname 
     * @param waitAsyncComplete 
     * @returns 
     */
    export function bind(thisArg: object, funcname: string | Function, waitAsyncComplete?: boolean): any {
        const func = typeof (funcname) === "string" ? (thisArg[funcname] as Function) : funcname;
        if (func !== undefined && typeof (func) === "function") {
            //return (...args: any[]) => func.call(thisArg, ...srcArgs, ...args);
            if (waitAsyncComplete) {
                let executing = false;
                return function (...args: any[]) {
                    if (executing)
                        return;
                    let result = func.call(thisArg, ...args);
                    if (result instanceof Promise) {
                        executing = true;       //wait async function finish
                        result.finally(() => executing = false);
                    }
                    return result;
                };
            }
            return function (...args: any[]) {
                return func.call(thisArg, ...args);
            };
        }
        return undefined;
    }
    /**调用方法
     * @param thisArg 
     * @param funcname 
     * @param catchException 是否捕获异常
     * @param args 
     */
    export function invoke(thisArg: any, funcname: string | Function, catchException: boolean, ...args: any[]) {
        const func = typeof (funcname) === "string" ? (thisArg[funcname] as Function) : funcname;
        if (!catchException) {
            return func.apply(thisArg, args);
        }

        //带try catch捕获异常的调用
        try {
            return func.apply(thisArg, args);
        }
        catch (e) {
            console.error(e.message + "\n" + e.stack);
        }
        return undefined;
    }
    /**创建C#迭代器
     * @param func 
     * @param args 
     * @returns 
     */
    export function cs_generator(func: ((...args: any[]) => Generator) | Generator, ...args: any[]): CS.System.Collections.IEnumerator {
        let generator: Generator = undefined;
        if (typeof (func) === "function") {
            generator = func(...args);
            if (generator === null || generator === undefined || generator === void 0)
                throw new Error("Function '" + func?.name + "' no return Generator");
        }
        else {
            generator = func;
        }

        return CS.XOR.IEnumeratorUtil.Generator(function () {
            let tick: CS.XOR.IEnumeratorUtil.Tick;
            try {
                let next = generator.next();
                tick = new CS.XOR.IEnumeratorUtil.Tick(next.value, next.done);
            } catch (e) {
                tick = new CS.XOR.IEnumeratorUtil.Tick(null, true);
                console.error(e.message + "\n" + e.stack);
            }
            return tick;
        });
    }

    /**导出方法枚举映射值
     * @param obj 
     * @param types 
     * @returns 
     */
    export function getFunctionFlags(obj: object, types:
        typeof CS.XOR.Behaviour.Args.Mono |
        typeof CS.XOR.Behaviour.Args.MonoBoolean |
        typeof CS.XOR.Behaviour.Args.Gizmos |
        typeof CS.XOR.Behaviour.Args.Mouse |
        typeof CS.XOR.Behaviour.Args.EventSystems |
        typeof CS.XOR.Behaviour.Args.PhysicsCollider |
        typeof CS.XOR.Behaviour.Args.PhysicsCollider2D |
        typeof CS.XOR.Behaviour.Args.PhysicsCollision |
        typeof CS.XOR.Behaviour.Args.PhysicsCollision2D
    ) {
        let results = 0
        for (let funcname in types) {
            let v = types[funcname]
            if (typeof (v) != "number")
                continue;
            if (typeof (obj[funcname]) != "function")
                continue;
            results |= v
        }
        return results;
    }
}

namespace metadata {
    const MATEDATA_INFO = Symbol("__MATEDATA_INFO__");

    export function define(proto: object, key: string, attribute: Function, data?: any) {
        let matedatas: { [key: string]: Array<{ attribute: Function, data?: any }> } = proto[MATEDATA_INFO];
        if (!matedatas) {
            matedatas = proto[MATEDATA_INFO] = {};
        }
        let attributes = matedatas[key];
        if (!attributes) {
            attributes = matedatas[key] = [];
        }
        attributes.push({ attribute, data });
    }
    export function getKeys(proto: object) {
        let matedatas: { [key: string]: Array<{ attribute: Function, data?: any }> } = proto[MATEDATA_INFO];
        return matedatas ? Object.keys(matedatas) : [];
    }
    export function isDefine(proto: object, key: string, attribute: Function) {
        let matedatas: { [key: string]: Array<{ attribute: Function, data?: any }> } = proto[MATEDATA_INFO];
        if (!matedatas) {
            return false;
        }
        let attributes = matedatas[key];
        if (!attributes) {
            return false;
        }
        return !!attributes.find(define => define.attribute === attribute);
    }
    export function getDefineData<T = any>(proto: object, key: string, attribute: Function, defaultValue?: T): T {
        let matedatas: { [key: string]: Array<{ attribute: Function, data?: any }> } = proto[MATEDATA_INFO];
        if (!matedatas) {
            return defaultValue;
        }
        let attributes = matedatas[key];
        if (!attributes) {
            return defaultValue;
        }
        return attributes.find(define => define.attribute === attribute)?.data ?? defaultValue;
    }
}

namespace utils {
    function toCSharpArray<T>(array: Array<T>, checkMemberType: boolean = true): CS.System.Array$1<T> {
        if (!array || array.length === 0)
            return null;
        let firstIndex = array.findIndex(m => m !== undefined && m !== null && m !== void 0) ?? -1;
        if (firstIndex < 0)
            return null;
        let first = array[firstIndex];
        let results: CS.System.Array,
            type = typeof first, memberType: CS.System.Type;
        switch (type) {
            case "bigint":
                results = CS.System.Array.CreateInstance(puerts.$typeof(CS.System.Int64), array.length);
                break;
            case "number":
                results = CS.System.Array.CreateInstance(puerts.$typeof(CS.System.Double), array.length);
                break;
            case "string":
                results = CS.System.Array.CreateInstance(puerts.$typeof(CS.System.String), array.length);
                break;
            case "boolean":
                results = CS.System.Array.CreateInstance(puerts.$typeof(CS.System.Boolean), array.length);
                break;
            case "object":
                if (first instanceof CS.System.Object) {
                    results = CS.System.Array.CreateInstance(first.GetType(), array.length);
                }
                break;
        }
        if (results) {
            for (let i = 0; i < array.length; i++) {
                let value = array[i];
                if (checkMemberType) {
                    if (!memberType && typeof (value) !== type) {
                        continue;
                    }
                    if (memberType && (typeof (value) !== "object" ||
                        !(value instanceof CS.System.Object) ||
                        !memberType.IsAssignableFrom(value.GetType())
                    )) {
                        continue;
                    }
                }
                results.SetValue(value, i);
            }
        }
        return results as CS.System.Array$1<T>;
    }
    function toArray<T>(array: CS.System.Array$1<T>): T[];
    function toArray(array: CS.System.Array): any[];
    function toArray() {
        let array: CS.System.Array = arguments[0];
        if (!array)
            return null;
        let results = new Array<any>();
        for (let i = 0; i < array.Length; i++) {
            results.push(array.GetValue(i));
        }
        return results;
    }

    const WatchFlag = Symbol("--watch--");
    const WatchFunctions: typeof Array.prototype = ["pop", "push", "reverse", "shift", "sort", "splice", "unshift"];
    function watch<T>(obj: T, change: () => void) {
        if (!obj || !Array.isArray(obj))
            return obj;
        let functions: { [property: string | symbol]: Function } = {};

        Object.defineProperty(obj, WatchFlag, {
            value: change,
            configurable: true,
            enumerable: false,
            writable: false,
        });
        return new Proxy(obj, {
            get: function (target, property) {
                if (WatchFlag in target && WatchFunctions.includes(property)) {
                    if (!(property in functions)) {
                        functions[property] = new Proxy(<Function>Array.prototype[property], {
                            apply: function (target, thisArg, argArray) {
                                let result = target.apply(thisArg, argArray);
                                if (WatchFlag in thisArg) {
                                    (<Function>thisArg[WatchFlag])();
                                }
                                return result;
                            }
                        });
                    }
                    return functions[property];
                }
                return target[property];
            },
            set: function (target, property, newValue) {
                target[property] = newValue;
                if (WatchFlag in target) {
                    (<Function>target[WatchFlag])();
                }
                return true;
            }
        });
    }
    function unwatch<T>(obj: T) {
        if (!obj || !Array.isArray(obj))
            return;
        delete obj[WatchFlag];
    }

    const OriginFlag = Symbol("--origin--"), ProxyFlag = Symbol("--proxy--");
    function convertToJsObejctProxy(component: CS.XOR.TsComponent): object {
        if (!component || !(component instanceof CS.XOR.TsComponent))
            return component;
        if (!component.Guid)
            return undefined;
        let proxy: object = component[ProxyFlag];
        if (proxy === undefined || proxy === null || proxy === void 0) {
            let target: any;
            function getter() {
                if (!component)
                    return;
                if (component.Equals(null)) {
                    component = null;
                    return;
                }
                if (!CS.XOR.TsComponent.IsRegistered())
                    throw new Error("XOR.TsComponet.Register is required.");
                if (component.IsPending)
                    CS.XOR.TsComponent.Resolve(component, true);
                target = component.JSObject;
                component = null;
            };
            proxy = new Proxy({}, {
                apply: function (_, thisArg, argArray) {
                    getter();
                    target.apply(thisArg, argArray);
                },
                construct: function (_, argArray, newTarget) {
                    getter();
                    return new target(...argArray);
                },
                get: function (_, property) {
                    getter();
                    if (property === OriginFlag && target)
                        return target;
                    return target[property];
                },
                set: function (_, property, newValue) {
                    getter();
                    target[property] = newValue;
                    return true;
                },
                defineProperty: function (_, property, attributes) {
                    getter();
                    Object.defineProperty(target, property, attributes);
                    return true;
                },
                deleteProperty: function (_, property) {
                    getter();
                    delete target[property];
                    return true;
                },
                getOwnPropertyDescriptor: function (_, property) {
                    getter();
                    return Object.getOwnPropertyDescriptor(target, property);
                },
                getPrototypeOf: function (_) {
                    getter();
                    return Object.getPrototypeOf(target);
                },
                setPrototypeOf: function (_, newValue) {
                    getter();
                    Object.setPrototypeOf(target, newValue);
                    return true;
                },
                has: function (_, property) {
                    getter();
                    return property in target;
                },
                isExtensible: function (_) {
                    getter();
                    return Object.isExtensible(target);
                },
                ownKeys: function (_) {
                    getter();
                    return Reflect.ownKeys(target)?.filter(key => Object.getOwnPropertyDescriptor(target, key)?.configurable);
                },
                preventExtensions: function (_) {
                    getter();
                    Object.preventExtensions(target);
                    return true;
                },
            });
            Object.defineProperty(component, ProxyFlag, {
                get: () => proxy,
                enumerable: false,
                configurable: true,
            });
        }
        return proxy;
    }
    function createJsObjectProxies(properties: { [key: string]: any }) {
        let c2jsKeys = [];
        for (let key of Object.keys(properties)) {
            let val = properties[key];
            if (val && val instanceof CS.XOR.TsComponent) {
                properties[key] = convertToJsObejctProxy(val);
                c2jsKeys.push(key);
            }
            else if (val && Array.isArray(val)) {
                for (let i = 0; i < val.length; i++) {
                    if (val[i] && val[i] instanceof CS.XOR.TsComponent) {
                        val[i] = convertToJsObejctProxy(val[i]);
                    }
                }
            }
        }
        return c2jsKeys;
    }

    export function getAccessorProperties(accessor: AccessorType) {
        let results: { [key: string]: any } = {};

        let properties = accessor.GetProperties();
        if (properties && properties.Length > 0) {
            for (let i = 0; i < properties.Length; i++) {
                let { key, value } = properties.get_Item(i);
                if (value && value instanceof CS.System.Array) {
                    value = toArray(value);
                }
                results[key] = value;
            }
        }
        return results;
    }
    export function bindAccessor(object: object, accessor: AccessorUnionType, bind?: boolean): void;
    export function bindAccessor(object: object, accessor: AccessorUnionType, options?: {
        /**双向绑定key-value */
        bind?: boolean;
        /**XOR.TsComponent自动转js object */
        convertToJsObejct?: boolean;
    }): void;
    export function bindAccessor() {
        let object: object = arguments[0], accessor: AccessorUnionType = arguments[1], bind: boolean, c2js: boolean;
        if (!accessor)
            return;
        switch (typeof (arguments[2])) {
            case "boolean":
                bind = arguments[2];
                break;
            case "object":
                bind = arguments[2]?.bind;
                c2js = arguments[2]?.convertToJsObejct;
                break;
        }

        let list: AccessorType[] = accessor instanceof CS.System.Array ? toArray(accessor) : Array.isArray(accessor) ? accessor : [accessor];
        for (let accessor of list) {
            if (!accessor || accessor.Equals(null))
                continue;
            let properties = getAccessorProperties(accessor),
                keys = Object.keys(properties);
            if (keys.length === 0)
                continue;

            let c2jsKeys = c2js ? createJsObjectProxies(properties) : null;
            if (isEditor && bind) {
                let setValue = (key: string, newValue: any) => {
                    unwatch(properties[key]);
                    properties[key] = watch(newValue, () => {
                        accessor.SetProperty(key, Array.isArray(newValue) ? toCSharpArray(newValue) : newValue);
                    });
                };
                accessor.SetPropertyListener((key, newValue) => {
                    if (newValue && newValue instanceof CS.System.Array) {
                        newValue = toArray(newValue);
                    }
                    setValue(key, newValue);
                });
                for (let key of keys) {
                    if (key in object) {
                        console.warn(`Object ${object}(${object["name"]}) already exists prop '${key}' ---> ${object[key]}`);
                    }
                    if (!c2jsKeys || !c2jsKeys.includes(key)) {
                        setValue(key, properties[key]);
                    }
                    Object.defineProperty(object, key, {
                        get: () => properties[key],
                        set: (newValue) => {
                            setValue(key, newValue);
                            accessor.SetProperty(key, Array.isArray(newValue) ? toCSharpArray(newValue) : newValue);
                        },
                        configurable: true,
                        enumerable: true,
                    });
                }
            }
            else {
                Object.assign(object, properties);
            }
        }
    }
    export function getAccessorPropertyOrigin(val: object) {
        if (!val || typeof (val) !== "object")
            return val;
        return val[OriginFlag] ?? val;
    }

    export function standalone(): PropertyDecorator {
        return (target, key: string) => {
            let proto = target.constructor.prototype;
            if (!(proto instanceof TsBehaviourConstructor)) {
                console.warn(`${target.constructor.name}: invaild decorator ${standalone.name}`);
                return;
            }
            metadata.define(proto, key, standalone);
        };
    }
    export function frameskip(value: number): PropertyDecorator {
        return (target, key: string) => {
            let proto = target.constructor.prototype;
            if (!(proto instanceof TsBehaviourConstructor)) {
                console.warn(`${target.constructor.name}: invaild decorator ${frameskip.name}`);
                return;
            }
            if (!Number.isInteger(value) || value <= 1) {
                console.warn(`${target.constructor.name}: invaild decorator parameter ${value} for ${frameskip.name}`);
                return;
            }
            metadata.define(proto, key, frameskip, value);
        };
    }
    export function throttle(enable: boolean): PropertyDecorator {
        return (target, key: string) => {
            let proto = target.constructor.prototype;
            if (!(proto instanceof TsBehaviourConstructor)) {
                console.warn(`${target.constructor.name}: invaild decorator ${throttle.name}`);
                return;
            }
            metadata.define(proto, key, throttle, !!enable);
        };
    }
    export function listener(eventName?: string): PropertyDecorator {
        return (target, key: string) => {
            let proto = target.constructor.prototype;
            if (!(proto instanceof TsBehaviourConstructor)) {
                console.warn(`${target.constructor.name}: invaild decorator ${listener.name}`);
                return;
            }
            metadata.define(proto, key, listener, eventName ?? key);
        };
    }
}

function register() {
    let _g = (global ?? globalThis ?? this);
    _g.xor = _g.xor || {};
    Object.assign(_g.xor, utils);
    Object.assign(TsBehaviourConstructor, utils);
    _g.xor.Behaviour = BehaviourConstructor;
    _g.xor.TsBehaviour = TsBehaviourConstructor;
}
register();

/**接口声明 */
declare global {
    namespace xor {
        abstract class Behaviour extends BehaviourConstructor { }

        class TsBehaviour extends TsBehaviourConstructor { }
        /**兼容以前的声明 */
        namespace TsBehaviour {
            /**@deprecated xor.TsBehaviour.getAccessorProperties has been deprecated.Use xor.getAccessorProperties instead.  */
            const getAccessorProperties: typeof utils.getAccessorProperties;
            /**@deprecated xor.TsBehaviour.bindAccessor has been deprecated.Use xor.bindAccessor instead.  */
            const bindAccessor: typeof utils.bindAccessor;
            /**@deprecated xor.TsBehaviour.standalone has been deprecated.Use xor.standalone instead.  */
            const standalone: typeof utils.standalone;
            /**@deprecated xor.TsBehaviour.frameskip has been deprecated.Use xor.frameskip instead.  */
            const frameskip: typeof utils.frameskip;
            /**@deprecated xor.TsBehaviour.throttle has been deprecated.Use xor.throttle instead.  */
            const throttle: typeof utils.throttle;
            /**@deprecated xor.TsBehaviour.listener has been deprecated.Use xor.listener instead.  */
            const listener: typeof utils.listener;
        }

        /**获取IAccessor中的属性 */
        const getAccessorProperties: typeof utils.getAccessorProperties;
        /**将C# IAccessor中的属性绑定到obj对象上
         * @param object 
         * @param accessor 
         * @param bind       运行时绑定
         */
        const bindAccessor: typeof utils.bindAccessor;
        /**获取序列化Ts类型的原始对象 */
        const getAccessorPropertyOrigin: typeof utils.getAccessorPropertyOrigin;
        /**以独立组件的方式调用
         * 适用于Update丶LateUpdate和FixedUpdate方法, 默认以BatchProxy管理调用以满足更高性能要求
         * @returns 
         */
        const standalone: typeof utils.standalone;
        /**跨帧调用(全局共用/非单独的frameskip分区)
         * 适用于Update丶LateUpdate和FixedUpdate方法, 仅允许BatchProxy管理调用(与standalone组件冲突)
         * (如你需要处理Input等事件, 那么就不应该使用它)
         * @param value  每n帧调用一次(<不包含>大于1时才有效)
         * @returns 
         */
        const frameskip: typeof utils.frameskip;
        /**节流方法
         * 适用于async/Promise方法, 在上一次调用完成后才会再次调用(Awake丶Update丶FixedUpdate...)
         * @param enable 
         * @returns 
         */
        const throttle: typeof utils.throttle;
        /**注册侦听器
         * 适用于@see CS.XOR.TsMessages 回调
         * @param eventName 
         * @returns 
         */
        const listener: typeof utils.listener;
    }
}
export { };

