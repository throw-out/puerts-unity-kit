namespace XOR
{
    public class OnDrawGizmosSelectedProxy : ProxyAction
    {
        void OnDrawGizmosSelected()
        {
            callback?.Invoke();
        }
    }
}
