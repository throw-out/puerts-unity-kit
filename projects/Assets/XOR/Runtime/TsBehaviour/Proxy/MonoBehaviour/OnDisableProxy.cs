namespace XOR
{
    public class OnDisableProxy : ProxyAction
    {
        private void OnDisable()
        {
            callback?.Invoke();
        }
    }
}