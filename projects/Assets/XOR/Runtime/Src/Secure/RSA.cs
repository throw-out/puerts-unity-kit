using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace XOR
{
    public class RSA
    {
        public readonly int KeyLength;
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        private int maxEncryptSize;
        private int maxDecryptSize;
        public RSA() : this(2048) { }
        public RSA(int keyLength)
        {
            this.KeyLength = keyLength;
            this.maxDecryptSize = keyLength / 8;
            this.maxEncryptSize = keyLength / 8 - 11;
        }
        public void GenerateKeyPair()
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(KeyLength);
            this.PublicKey = provider.ToXmlString(false);
            this.PrivateKey = provider.ToXmlString(true);
        }

        public string Encrypt(string source) => Convert.ToBase64String(Encrypt(Encoding.GetBytes(source)));
        public byte[] Encrypt(byte[] source)
        {
            if (!string.IsNullOrEmpty(PublicKey))
            {
                return EncryptOperation(source, PublicKey, maxEncryptSize);
            }
            throw new Exception("Invalid key");
        }
        public string Decrypt(string source) => Encoding.GetString(Decrypt(Convert.FromBase64String(source)));
        public byte[] Decrypt(byte[] source)
        {
            if (!string.IsNullOrEmpty(PrivateKey))
            {
                return DecryptOperation(source, PrivateKey, maxDecryptSize);
            }
            throw new Exception("Invalid key");
        }

        public string SignData(string source) => Convert.ToBase64String(SignData(Encoding.GetBytes(source)));
        public byte[] SignData(byte[] source)
        {
            if (!string.IsNullOrEmpty(PrivateKey))
            {
                return SignDataOperation(source, PrivateKey);
            }
            throw new Exception("Invalid key");
        }
        public bool VerifySign(string source, string signData) => VerifySign(Encoding.GetBytes(source), Convert.FromBase64String(signData));
        public bool VerifySign(byte[] source, byte[] signData)
        {
            if (!string.IsNullOrEmpty(PublicKey))
            {
                return VerifyDataOperation(source, PublicKey, signData);
            }
            throw new Exception("Invalid key");
        }

        protected static byte[] SignDataOperation(byte[] source, string key)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(key);
            return provider.SignData(source, new SHA1CryptoServiceProvider());
        }
        protected static bool VerifyDataOperation(byte[] source, string key, byte[] signData)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(key);
            return provider.VerifyData(source, new SHA1CryptoServiceProvider(), signData);
        }
        protected static byte[] EncryptOperation(byte[] src, string key, int blockSize)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(key);
            int offset = 0; //偏移值
            int length = src.Length;    //数据长度
            List<byte> data = new List<byte>();
            while (length - offset > 0)
            {
                byte[] cache;
                if (length - offset > blockSize)
                {
                    cache = provider.Encrypt(Select(src, offset, blockSize), false);
                }
                else
                {
                    cache = provider.Encrypt(Select(src, offset, length - offset), false);
                }
                data.AddRange(cache);
                offset += blockSize;
            }
            return data.ToArray();
        }
        protected static byte[] DecryptOperation(byte[] src, string key, int blockSize)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(key);
            int offset = 0; //偏移值
            int length = src.Length;    //数据长度
            List<byte> data = new List<byte>();
            while (length - offset > 0)
            {
                byte[] cache;
                if (length - offset > blockSize)
                {
                    cache = provider.Decrypt(Select(src, offset, blockSize), false);
                }
                else
                {
                    cache = provider.Decrypt(Select(src, offset, length - offset), false);
                }
                data.AddRange(cache);
                offset += blockSize;
            }
            return data.ToArray();
        }
        private static byte[] Select(byte[] src, int offset, int length)
        {
            byte[] result = new byte[length];
            Array.Copy(src, offset, result, 0, length);
            return result;
        }
    }
}