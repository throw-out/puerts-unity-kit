namespace XOR
{
    public class OnMouseUpAsButtonProxy : ProxyAction
    {
        void OnMouseUpAsButton()
        {
            callback?.Invoke();
        }
    }
}
