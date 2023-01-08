using System;
using System.Linq;
using Puerts;

namespace XOR
{
    public static class JsEnvExtension
    {
        internal static void GlobalListenerQuit(this JsEnv env)
        {
            env.Eval(
    @"(function(){
    let listener = (globalThis ?? global ?? this)['globalListener'];
    if(listener && listener.quit){
        listener.quit.invoke();
    }
})();");
        }

        internal static void TryAutoUsing(this JsEnv env)
        {
            const string typeName = "PuertsStaticWrap.AutoStaticCodeUsing";
            var type = (from _assembly in AppDomain.CurrentDomain.GetAssemblies()
                        let _type = _assembly.GetType(typeName, false)
                        where _type != null
                        select _type).FirstOrDefault();
            if (type != null)
            {
                type.GetMethod("AutoUsing").Invoke(null, new object[] { env });
            }
        }
    }
}