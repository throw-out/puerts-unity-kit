namespace XOR
{
    public class OnMouseOverProxy : ProxyAction
    {
        void OnMouseOver()
        {
            callback?.Invoke();
        }
    }
}
