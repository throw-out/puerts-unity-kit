namespace XOR
{
    public interface ISignature
    {
        byte[] Sign(byte[] sourceData);
        bool Verify(byte[] sourceData, byte[] signData);
    }

    public class RsaSignature : ISignature
    {
        public readonly int keySize;
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }

        public RsaSignature(int keySize)
        {
            this.keySize = keySize;
        }

        public byte[] Sign(byte[] sourceData)
        {
            RSA ras = new RSA(keySize);
            ras.PrivateKey = PrivateKey;
            return ras.SignData(sourceData);
        }

        public bool Verify(byte[] sourceData, byte[] signData)
        {
            RSA ras = new RSA(keySize);
            ras.PublicKey = PublicKey;
            return ras.VerifySign(sourceData, signData);
        }
    }
}