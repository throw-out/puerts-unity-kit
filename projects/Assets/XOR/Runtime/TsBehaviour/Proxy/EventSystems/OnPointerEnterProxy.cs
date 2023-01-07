using UnityEngine.EventSystems;

namespace XOR
{
    public class OnPointerEnterProxy : ProxyAction<PointerEventData>, IPointerEnterHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            callback?.Invoke(eventData);
        }
    }
}