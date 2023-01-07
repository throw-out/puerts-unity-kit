namespace XOR
{
    public class UpdateProxy : ProxyAction
    {
        private void Update()
        {
            callback?.Invoke();
        }
    }
}