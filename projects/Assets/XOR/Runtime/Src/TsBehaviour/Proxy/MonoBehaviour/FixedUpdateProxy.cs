namespace XOR
{
    public class FixedUpdateProxy : ProxyAction
    {
        private void FixedUpdate()
        {
            callback?.Invoke();
        }
    }
}