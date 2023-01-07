using Puerts;

public static class JsEnvExtends
{
    internal static void GlobalListenerQuit(this JsEnv jsEnv)
    {
        jsEnv.Eval(
@"(function(){
    let listener = (globalThis ?? global ?? this)['globalListener'];
    if(listener && listener.quit){
        listener.quit.invoke();
    }
})();");
    }
}