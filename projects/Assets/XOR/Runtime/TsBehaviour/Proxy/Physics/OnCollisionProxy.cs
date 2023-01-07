using System;
using UnityEngine;

namespace XOR
{
    public class OnCollisionProxy : Proxy
    {
        public bool stayFrame = false;
        public Action<Collision> enter { get; set; }
        public Action<Collision> stay { get; set; }
        public Action<Collision> exit { get; set; }
        private void OnCollisionEnter(Collision collision)
        {
            enter?.Invoke(collision);
        }
        private void OnCollisionStay(Collision collision)
        {
            if (stayFrame)
                stay?.Invoke(collision);
        }
        private void OnCollisionExit(Collision collision)
        {
            exit?.Invoke(collision);
        }

        public override void Release()
        {
            base.Release();
            enter = null;
            stay = null;
            exit = null;
        }
    }
}