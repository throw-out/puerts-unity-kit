using System;
using System.Security.Cryptography;
using System.Text;

namespace XOR
{
    public interface IAuth
    {
        byte[] Sign(byte[] sourceData);
        bool Verify(byte[] sourceData, byte[] signData);
    }

    public class RsaAuth : IAuth
    {
        public readonly int keyLength;
        public string publicKey { get; set; }
        public string privateKey { get; set; }

        public RsaAuth(int keyLength)
        {
            this.keyLength = keyLength;
        }

        public byte[] Sign(byte[] sourceData)
        {
            return SignOperation(sourceData, privateKey);
        }

        public bool Verify(byte[] sourceData, byte[] signData)
        {
            return VerifyOperation(sourceData, publicKey, signData);
        }

        public void GenerateKeyPair()
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(keyLength);
            this.publicKey = provider.ToXmlString(false);
            this.privateKey = provider.ToXmlString(true);
        }

        private static byte[] SignOperation(byte[] source, string key)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(key);
            return provider.SignData(source, new SHA1CryptoServiceProvider());
        }
        private static bool VerifyOperation(byte[] source, string key, byte[] signData)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(key);
            return provider.VerifyData(source, new SHA1CryptoServiceProvider(), signData);
        }
    }
}