namespace XOR
{
    public class OnMouseDragProxy : ProxyAction
    {
        void OnMouseDrag()
        {
            callback?.Invoke();
        }
    }
}
