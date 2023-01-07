namespace XOR
{
    public class OnBecameVisibleProxy : ProxyAction<bool>
    {
        void OnBecameVisible()
        {
            callback?.Invoke(true);
        }
        void OnBecameInvisible()
        {
            callback?.Invoke(false);
        }
    }
}
