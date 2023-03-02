using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XOR
{
    public class JsBase
    {
        protected readonly Puerts.JSObject target;
        public JsBase(Puerts.JSObject target)
        {
            this.target = target;
        }
    }
}
