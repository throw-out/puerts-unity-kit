namespace XOR
{
    public static class PuertsUtil
    {
        public static bool IsSupportNodejs()
        {
            return Puerts.PuertsDLL.GetLibBackend() != 0;
        }
    }
}