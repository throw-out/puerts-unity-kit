using UnityEngine.EventSystems;

namespace XOR
{
    public class OnPointerClickProxy : ProxyAction<PointerEventData>, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            callback?.Invoke(eventData);
        }
    }
}
