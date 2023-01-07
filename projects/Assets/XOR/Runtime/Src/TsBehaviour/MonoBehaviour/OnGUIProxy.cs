namespace XOR
{
    public class OnGUIProxy : ProxyAction
    {
        private void OnGUI()
        {
            callback?.Invoke();
        }
    }
}