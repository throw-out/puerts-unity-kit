using System;
using UnityEngine;

namespace XOR
{
    public class OnTriggerProxy : Proxy
    {
        public bool stayFrame = false;
        public Action<Collider> enter { get; set; }
        public Action<Collider> stay { get; set; }
        public Action<Collider> exit { get; set; }
        private void OnTriggerEnter(Collider other)
        {
            enter?.Invoke(other);
        }
        private void OnTriggerStay(Collider other)
        {
            if (stayFrame)
                stay?.Invoke(other);
        }
        private void OnTriggerExit(Collider other)
        {
            exit?.Invoke(other);
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

