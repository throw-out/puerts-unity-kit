namespace XOR
{
    public class OnMouseEnterProxy : ProxyAction
    {
        void OnMouseEnter()
        {
            callback?.Invoke();
        }
    }
}
