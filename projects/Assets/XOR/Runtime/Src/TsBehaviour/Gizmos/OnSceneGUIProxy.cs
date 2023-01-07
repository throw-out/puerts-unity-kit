namespace XOR
{
    public class OnSceneGUIProxy : ProxyAction
    {
        void OnSceneGUI()
        {
            callback?.Invoke();
        }
    }
}
