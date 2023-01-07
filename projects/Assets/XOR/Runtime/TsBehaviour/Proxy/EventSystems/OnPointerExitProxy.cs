using UnityEngine.EventSystems;

namespace XOR
{
    public class OnPointerExitProxy : ProxyAction<PointerEventData>, IPointerExitHandler
    {
        public void OnPointerExit(PointerEventData eventData)
        {
            callback?.Invoke(eventData);
        }
    }
}
