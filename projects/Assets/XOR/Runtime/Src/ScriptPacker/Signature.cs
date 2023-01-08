namespace XOR
{
    public interface ISignature
    {
        byte[] Sign(byte[] sourceData);
        bool Verify(byte[] sourceData, byte[] signData);
    }

    public class RsaSignature : ISignature
    {
        public readonly int KeyLength;
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }

        public RsaSignature(int keyLength)
        {
            this.KeyLength = keyLength;
        }

        public byte[] Sign(byte[] sourceData)
        {
            RSA ras = new RSA(KeyLength);
            ras.PrivateKey = PrivateKey;
            return ras.SignData(sourceData);
        }

        public bool Verify(byte[] sourceData, byte[] signData)
        {
            RSA ras = new RSA(KeyLength);
            ras.PublicKey = PublicKey;
            return ras.VerifySign(sourceData, signData);
        }
    }
}