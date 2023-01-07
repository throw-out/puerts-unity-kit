using System;
using System.Collections;
using Puerts;

namespace XOR
{
    public static class IEnumeratorUtil
    {
        public static IEnumerator Generator(Func<object> next, Func<bool> isDone)
        {
            var done = false;
            while (!done)
            {
                yield return next();
                done = isDone();
            }
            next = null;
            isDone = null;
        }
        public static IEnumerator Generator(Func<Tick> next)
        {
            Tick tick = new Tick() { done = false };
            while (!tick.done)
            {
                tick = next();
                yield return tick.value;
            }
            next = null;
        }

        public static void UsingTick(this JsEnv jsEnv)
        {
            jsEnv.UsingFunc<Tick>();
        }
        public struct Tick
        {
            public bool done;
            public object value;
            public Tick(object value, bool done)
            {
                this.value = value;
                this.done = done;
            }
        }
    }
}