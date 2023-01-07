using System;
using UnityEngine.EventSystems;

namespace XOR
{
    public class OnDragProxy : Proxy, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public bool stayFrame = false;
        public Action<PointerEventData> enter { get; set; }
        public Action<PointerEventData> stay { get; set; }
        public Action<PointerEventData> exit { get; set; }

        public void OnBeginDrag(PointerEventData eventData)
        {
            enter?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (stayFrame)
                stay?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            exit?.Invoke(eventData);
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
