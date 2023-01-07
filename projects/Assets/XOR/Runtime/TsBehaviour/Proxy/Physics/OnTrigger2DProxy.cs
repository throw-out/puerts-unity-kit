using System;
using UnityEngine;

namespace XOR
{
    public class OnTrigger2DProxy : Proxy
    {
        public bool stayFrame = false;
        public Action<Collider2D> enter { get; set; }
        public Action<Collider2D> stay { get; set; }
        public Action<Collider2D> exit { get; set; }
        private void OnTriggerEnter2D(Collider2D other)
        {
            enter?.Invoke(other);
        }
        private void OnTriggerStay2D(Collider2D other)
        {
            if (stayFrame)
                stay?.Invoke(other);
        }
        private void OnTriggerExit2D(Collider2D other)
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
