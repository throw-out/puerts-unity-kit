using System;
using System.Security.Cryptography;
using System.Text;

namespace XOR
{
    public class AES
    {
        public readonly int keySize;
        public string Key { get; set; }
        public string IV { get; set; }
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public AES() : this(16) { }
        public AES(int keySize)
        {
            this.keySize = keySize;
        }
        public void GenerateKeyPair()
        {
            this.IV = GenerateRandomeString(keySize);
            this.Key = GenerateRandomeString(keySize);
        }

        public string Encrypt(string source) => Convert.ToBase64String(Encrypt(Encoding.GetBytes(source)));
        public byte[] Encrypt(byte[] source)
        {
            if (Key != null && IV != null && Key.Length == keySize && IV.Length == keySize)
            {
                return Operation(source, keySize, Key, IV, true);
            }
            throw new Exception("Invalid key");
        }
        public string Decrypt(string source) => Encoding.GetString(Decrypt(Convert.FromBase64String(source)));
        public byte[] Decrypt(byte[] source)
        {
            if (Key != null && IV != null && Key.Length == keySize && IV.Length == keySize)
            {
                return Operation(source, keySize, Key, IV, false);
            }
            throw new Exception("Invalid key");
        }

        protected static byte[] Operation(byte[] src, int keySize, string key, string iv, bool isEncrypt)
        {
            byte[] keyRaw = Encoding.UTF8.GetBytes(key);
            byte[] ivRaw = Encoding.UTF8.GetBytes(iv);
            if (keyRaw.Length > keySize)
            {
                byte[] temp = new byte[keySize];
                Array.Copy(keyRaw, temp, keySize);
                keyRaw = temp;
            }
            RijndaelManaged cipher = new RijndaelManaged();
            cipher.Mode = CipherMode.CBC;
            cipher.Padding = PaddingMode.Zeros;
            cipher.KeySize = keySize * 8;
            cipher.BlockSize = keySize * 8;
            cipher.Key = keyRaw;
            cipher.IV = ivRaw;
            ICryptoTransform cTransform = isEncrypt ? cipher.CreateEncryptor() : cipher.CreateDecryptor();
            byte[] result = cTransform.TransformFinalBlock(src, 0, src.Length);
            cTransform.Dispose();
            cipher.Dispose();
            return result;
        }

        //随机字符串源字符串
        protected const string RANDOME_SOURCE_STRING = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        //生成随机字符串
        protected static string GenerateRandomeString(int length)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                builder.Append(RANDOME_SOURCE_STRING[UnityEngine.Random.Range(0, RANDOME_SOURCE_STRING.Length)]);
            }
            return builder.ToString();
        }
    }
}