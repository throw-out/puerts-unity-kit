namespace XOR
{
    public class OnMouseUpProxy : ProxyAction
    {
        void OnMouseUp()
        {
            callback?.Invoke();
        }
    }
}
