namespace XOR
{
    public class LateUpdateProxy : ProxyAction
    {
        private void LateUpdate()
        {
            callback?.Invoke();
        }
    }
}
