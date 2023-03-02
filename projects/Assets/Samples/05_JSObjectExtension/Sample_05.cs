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
            env.TryAutoInterfaceBridge();
            env.UsingFunc<Puerts.JSObject, int>();
            env.UsingFunc<Puerts.JSObject, string, int>();
            env.UsingFunc<Puerts.JSObject, string, bool>();
            env.UsingAction<Puerts.JSObject, string>();
            env.UsingAction<Puerts.JSObject, string, int>();

            var obj = env.Eval<Puerts.JSObject>(jsCode);
            TestGetter(obj);
            TestSetter(obj);
            TestForEach(obj);
            TestInterface(obj);
        }
    }
    void TestGetter(Puerts.JSObject obj)
    {
        Debug.Log("===============Getter Start=============================");

        Debug.Log("a= " + obj.Get<int>("a"));           //a= 111
        Debug.Log("b= " + obj.Get<string>("b"));        //b= 222

        var methodC = obj.Get<Action>("c");
        methodC();                                      //call member method: 111
        obj.Call("c");                                  //call member method: 111

        Debug.Log("d.length= " + obj.Get<Puerts.JSObject>("d").Length());   //d.length= 3
        Debug.Log("e.value= " + obj.GetInPath<string>("e.value"));          //e.value= this is e member

        Debug.Log("keys= " + string.Join(",", obj.GetKeys()));              //keys= a,b,c,d,e

        Debug.Log("===============Getter End=============================");
    }
    void TestSetter(Puerts.JSObject obj)
    {
        Debug.Log("===============Setter Start=============================");

        obj.Set("a", 1111);
        Debug.Log("a= " + obj.Get<int>("a"));           //a= 1111

        obj.Set("b", 222);
        Debug.Log("b= " + obj.Get<int>("b"));           //b= 222

        obj.Set("f", 1);
        obj.RemoveKey("a");
        Debug.Log("keys= " + string.Join(",", obj.GetKeys()));              //keys= b,c,d,e,f

        Debug.Log("===============Setter End=============================");
    }
    void TestForEach(Puerts.JSObject obj)
    {
        Debug.Log("===============ForEach Start=============================");

        Debug.Log("foreach(string, object)");
        obj.ForEach((k, v) =>
        {
            Debug.Log($"{k}= {v}");
            //b= 222
            //c= 2349589027088 [function object address]
            //d= Puerts.JSObject
            //e= Puerts.JSObject
            //f= 1
        });
        Debug.Log("foreach(string, int)");
        obj.ForEach<int>((k, v) =>
        {
            Debug.Log($"{k}= {v}");
            //b= 222
            //f= 1
        });
        Debug.Log("===============ForEach End=============================");
    }
    void TestInterface(Puerts.JSObject obj)
    {
        //env.TryAutoInterfaceBridge();

        Debug.Log("===============Interface Start=============================");

        var iobj = obj.Cast<ITest>();
        Debug.Log("a= " + iobj.a);          //a= -2147483648 [key has been deleted]
        Debug.Log("b= " + iobj.b);          //b= 222

        iobj.c();                           //call member method: 111

        Debug.Log("d.length= " + iobj.d.Length());                  //d.length= 3
        Debug.Log("e.value= " + iobj.e.Get<string>("value"));       //e.value= this is e member
        Debug.Log("f= " + iobj.f);                                  //f= 1

        Debug.Log("===============Interface End=============================");
    }
    const string jsCode = @"
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
})();";
    public interface ITest
    {
        int a { get; set; }
        string b { get; set; }
        string c();
        Puerts.JSObject d { get; set; }
        Puerts.JSObject e { get; set; }
        int f { get; set; }
    }
}
