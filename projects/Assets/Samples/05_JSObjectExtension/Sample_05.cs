using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XOR;

/// <summary>
/// 请悉知: JsObjectExtension中包含大量装箱丶拆箱操作, 必需谨慎使用;
/// Puerts.JSObject本身不提供getter丶setter成员的操作, JsObjectExtension所做的是在js侧实现操作方法并映射和缓存到C# Delegate, 后续使用时无需重复创建C# Delegate;
/// js侧的操作方法需要同时传入JSObject对象, 因此写UsingAction/UsingFunc时, 第一个参数必然是JSObejct.
/// </summary>
public class Sample_05 : MonoBehaviour
{
    void Start()
    {
        using (var env = new Puerts.JsEnv())
        {
            env.UsingFunc<Puerts.JSObject, int>();
            env.UsingFunc<Puerts.JSObject, string, int>();
            env.UsingFunc<Puerts.JSObject, string, bool>();
            env.UsingAction<Puerts.JSObject, string>();
            env.UsingAction<Puerts.JSObject, string, int>();

            var obj = env.Eval<Puerts.JSObject>(@"
(function(){
    return {
        a: 111,
        b: '222',
        c: function(){
            console.log('call member method: '+ this?.a );
        },
        d:[1, 2, 3],
        e:{
            value: 'this is e member'
        }
    };
})();");
            //============================================================
            Debug.Log("===============Getter=============================");
            Debug.Log("a= " + obj.Get<int>("a"));
            Debug.Log("b= " + obj.Get<string>("b"));

            var c = obj.Get<Action>("c");
            c();
            obj.Call("c");

            Debug.Log("d.length= " + obj.Get<Puerts.JSObject>("d").Length());
            Debug.Log("e.value= " + obj.GetInPath<string>("e.value"));

            Debug.Log("keys= " + string.Join(",", obj.GetKeys()));

            //=============================================================
            Debug.Log("===============Setter=============================");
            obj.Set("a", 1111);
            Debug.Log("a= " + obj.Get<int>("a"));

            obj.Set("b", 222);
            Debug.Log("b= " + obj.Get<int>("b"));

            obj.Set("f", 1);
            obj.RemoveKey("a");
            Debug.Log("keys= " + string.Join(",", obj.GetKeys()));

            //=============================================================
            Debug.Log("===============ForEach=============================");

            Debug.Log("foreach(string, object)");
            obj.ForEach((k, v) =>
            {
                Debug.Log($"{k}= {v}");
            });
            Debug.Log("foreach(string, int)");
            obj.ForEach<int>((k, v) =>
            {
                Debug.Log($"{k}= {v}");
            });
        }
    }
}
