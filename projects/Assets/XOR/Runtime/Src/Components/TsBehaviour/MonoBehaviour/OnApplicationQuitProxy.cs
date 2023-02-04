namespace XOR
{
    public class OnApplicationQuitProxy : ProxyAction
    {
        private void OnApplicationQuit()
        {
            callback?.Invoke();
        }
    }
}
