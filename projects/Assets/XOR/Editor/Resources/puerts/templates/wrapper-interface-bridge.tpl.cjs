/**
 * this template file is write for generating the wrapper register code
 * 
 * @param {TypeInfo[]} infos
 * @returns 
 */
module.exports = function AutoRegTemplate(infos) {
    infos = toJsArray(infos);
    return `
namespace PuertsBridge
{
    using XOR;
    ${infos.map(info => {
        var events = toJsArray(info.Events);
        var methods = toJsArray(info.Methods);
        var properties = toJsArray(info.Properties);

        return `
    public class ${info.Name} : XOR.JsBase, ${info.InterfaceTypeName} 
    {
        public ${info.Name}(Puerts.JSObject target) : base(target)
        {
        }
        ${//Events
            events.map(event => {
                return `
        public event ${event.TypeName} ${event.Name};`;
            }).join('')
            }
        ${//Methods
            methods.map(method => {
                let parameters = toJsArray(method.Parameters);
                let hasReturn = method.ReturnTypeName !== 'void',
                    hasParams = parameters.length > 0,
                    support = !parameters.find(p => p.IsByRef);
                return `
        public ${method.ReturnTypeName} ${method.Name}(${parameters.map(p => formatParameter(p)).join(', ')})
        {
            ${!support ? `throw new System.NotSupportedException()` :
                        hasReturn ? `return target.Call<${method.ReturnTypeName}>("${method.Name}"${hasParams ? ', ' : ''}${parameters.map(p => p.Name).join(', ')})` :
                            `target.Call("${method.Name}"${hasParams ? ', ' : ''}${parameters.map(p => p.Name).join(', ')})`
                    };
        }`;
            }).join('')
            }
        ${//Properties
            properties.map(prop => {
                let str
                if (prop.Parameters) {
                    let parameters = toJsArray(prop.Parameters),
                        support = !parameters.find(p => p.IsByRef);;
                    str = `
        public ${prop.TypeName} this[${parameters.map(p => formatParameter(p)).join(', ')}]
        {`;
                    if (prop.CanRead) {
                        str += `
            get
            {
                ${support ? `return target.Call<${prop.TypeName}>("get_Item", ${parameters.map(p => p.Name).join(', ')})` :
                                `throw new System.NotSupportedException()`
                            };
            }`;
                    }
                    if (prop.CanWrite) {
                        str += `
            set
            {
                ${support ? `target.Call("set_Item", ${parameters.map(p => p.Name).join(', ')}, value)` :
                                `throw new System.NotSupportedException()`
                            };
            }`;
                    }
                    str += `
        }`;
                } else {
                    str = `
        public ${prop.TypeName} ${prop.Name} 
        {${prop.CanRead ? `
            get
            {
                return target.Get<string, ${prop.TypeName}>("${prop.Name}");
            }` : ''}${prop.CanWrite ? `
            set
            {
                target.Set<string, ${prop.TypeName}>("${prop.Name}", value);
            }` : ''} 
        }`;
                }
                return str;
            }).join('')
            }   
        public static XOR.JsBase __Create(Puerts.JSObject target)
        {
            return new ${info.Name}(target);
        }
    }
    `;
    }).join('')}
}
namespace PuertsStaticWrap
{
    using JsEnv = Puerts.JsEnv;
    using XOR;

    public static class AutoStaticCodeInterfaceBridge
    {
        public static void Register(this JsEnv jsEnv)
        {
${infos.map(info => {
        return `
            jsEnv.AddInterfaceBridgeCreator(typeof(${info.InterfaceTypeName}), PuertsBridge.${info.Name}.__Create);`;
    }).join('')}
        }
    }
}

    `.trim();
};

function toJsArray(csArr) {
    let arr = [];
    for (var i = 0; i < csArr.Length; i++) {
        arr.push(csArr.get_Item(i));
    }
    return arr;
}
/**格式化参数
 * @param {ParameterGenInfo} parameter 
 * @returns 
 */
function formatParameter(parameter) {
    return (!parameter.IsByRef ? '' : parameter.IsIn ? 'in ' : parameter.IsOut ? 'out ' : 'ref ') + `${parameter.TypeName} ${parameter.Name}`;
}
