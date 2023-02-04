namespace XOR
{
    public class OnApplicationPauseProxy : ProxyAction<bool>
    {
        private void OnApplicationPause(bool pause)
        {
            callback?.Invoke(pause);
        }
    }
}
