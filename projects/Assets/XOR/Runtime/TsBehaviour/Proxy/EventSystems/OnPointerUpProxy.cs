using UnityEngine.EventSystems;

namespace XOR
{
    public class OnPointerUpProxy : ProxyAction<PointerEventData>, IPointerUpHandler
    {

        public void OnPointerUp(PointerEventData eventData)
        {
            callback?.Invoke(eventData);
        }
    }
}
