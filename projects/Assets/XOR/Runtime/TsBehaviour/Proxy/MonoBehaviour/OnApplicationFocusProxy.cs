namespace XOR
{
    public class OnApplicationFocusProxy : ProxyAction<bool>
    {
        private void OnApplicationFocus(bool focus)
        {
            callback?.Invoke(focus);
        }
    }
}


