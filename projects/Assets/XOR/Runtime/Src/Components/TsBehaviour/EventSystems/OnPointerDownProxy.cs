using UnityEngine.EventSystems;

namespace XOR
{
    public class OnPointerDownProxy : ProxyAction<PointerEventData>, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            callback?.Invoke(eventData);
        }
    }
}
