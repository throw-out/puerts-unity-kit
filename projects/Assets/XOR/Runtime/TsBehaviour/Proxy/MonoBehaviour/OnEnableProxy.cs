namespace XOR
{
    public class OnEnableProxy : ProxyAction
    {
        private void OnEnable()
        {
            callback?.Invoke();
        }
    }
}