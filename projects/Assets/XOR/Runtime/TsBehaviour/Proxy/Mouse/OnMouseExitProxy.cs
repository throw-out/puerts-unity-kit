namespace XOR
{
    public class OnMouseExitProxy : ProxyAction
    {
        void OnMouseExit()
        {
            callback?.Invoke();
        }
    }
}
