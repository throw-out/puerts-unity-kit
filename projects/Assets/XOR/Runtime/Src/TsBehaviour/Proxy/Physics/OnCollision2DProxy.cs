using System;
using UnityEngine;

namespace XOR
{
    public class OnCollision2DProxy : Proxy
    {
        public bool stayFrame = false;
        public Action<Collision2D> enter { get; set; }
        public Action<Collision2D> stay { get; set; }
        public Action<Collision2D> exit { get; set; }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            enter?.Invoke(collision);
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (stayFrame)
                stay?.Invoke(collision);
        }
        private void OnCollisionExit2D(Collision2D collision)
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
