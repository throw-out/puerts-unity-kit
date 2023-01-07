namespace XOR
{
    public class OnMouseDownProxy : ProxyAction
    {
        void OnMouseDown()
        {
            callback?.Invoke();
        }
    }
}
