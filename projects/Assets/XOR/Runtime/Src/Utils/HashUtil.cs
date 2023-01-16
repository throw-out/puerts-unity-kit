using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace XOR
{
    public static class HashUtil
    {
        public static string MD5(string data, bool lower = false)
        {
            return MD5(Encoding.UTF8.GetBytes(data), lower);
        }
        public static string SHA1(string data, bool lower = false)
        {
            return SHA1(Encoding.UTF8.GetBytes(data), lower);
        }
        public static string SHA256(string data, bool lower = false)
        {
            return SHA256(Encoding.UTF8.GetBytes(data), lower);
        }
        public static string SHA384(string data, bool lower = false)
        {
            return SHA384(Encoding.UTF8.GetBytes(data), lower);
        }
        public static string SHA512(string data, bool lower = false)
        {
            return SHA512(Encoding.UTF8.GetBytes(data), lower);
        }

        public static string MD5File(string path, bool lower = false, bool exclusive = false)
        {
            FileStream stream = null;
            try
            {
                FileShare share = !exclusive ? FileShare.Read : (FileShare.ReadWrite | FileShare.Inheritable | FileShare.Delete);
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, share);
                return MD5(stream, lower);
            }
            finally
            {
                if (stream != null) stream.Close();
            }
        }
        public static string SHA1File(string path, bool lower = false, bool exclusive = false)
        {
            FileStream stream = null;
            try
            {
                FileShare share = !exclusive ? FileShare.Read : (FileShare.ReadWrite | FileShare.Inheritable | FileShare.Delete);
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, share);
                return SHA1(stream, lower);
            }
            finally
            {
                if (stream != null) stream.Close();
            }
        }
        public static string SHA256File(string path, bool lower = false, bool exclusive = false)
        {
            FileStream stream = null;
            try
            {
                FileShare share = !exclusive ? FileShare.Read : (FileShare.ReadWrite | FileShare.Inheritable | FileShare.Delete);
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, share);
                return SHA256(stream, lower);
            }
            finally
            {
                if (stream != null) stream.Close();
            }
        }
        public static string SHA384File(string path, bool lower = false, bool exclusive = false)
        {
            FileStream stream = null;
            try
            {
                FileShare share = !exclusive ? FileShare.Read : (FileShare.ReadWrite | FileShare.Inheritable | FileShare.Delete);
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, share);
                return SHA384(stream, lower);
            }
            finally
            {
                if (stream != null) stream.Close();
            }
        }
        public static string SHA512File(string path, bool lower = false, bool exclusive = false)
        {
            FileStream stream = null;
            try
            {
                FileShare share = !exclusive ? FileShare.Read : (FileShare.ReadWrite | FileShare.Inheritable | FileShare.Delete);
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, share);
                return SHA512(stream, lower);
            }
            finally
            {
                if (stream != null) stream.Close();
            }
        }

        public static string MD5(byte[] data, bool lower = false)
        {
            return Hash(MD5CryptoServiceProvider.Create(), data, lower);
        }
        public static string SHA1(byte[] data, bool lower = false)
        {
            return Hash(SHA1CryptoServiceProvider.Create(), data, lower);
        }
        public static string SHA256(byte[] data, bool lower = false)
        {
            return Hash(SHA256CryptoServiceProvider.Create(), data, lower);
        }
        public static string SHA384(byte[] data, bool lower = false)
        {
            return Hash(SHA384CryptoServiceProvider.Create(), data, lower);
        }
        public static string SHA512(byte[] data, bool lower = false)
        {
            return Hash(SHA512CryptoServiceProvider.Create(), data, lower);
        }

        public static string MD5(Stream inputStream, bool lower = false)
        {
            return Hash(MD5CryptoServiceProvider.Create(), inputStream, lower);
        }
        public static string SHA1(Stream inputStream, bool lower = false)
        {
            return Hash(SHA1CryptoServiceProvider.Create(), inputStream, lower);
        }
        public static string SHA256(Stream inputStream, bool lower = false)
        {
            return Hash(SHA256CryptoServiceProvider.Create(), inputStream, lower);
        }
        public static string SHA384(Stream inputStream, bool lower = false)
        {
            return Hash(SHA384CryptoServiceProvider.Create(), inputStream, lower);
        }
        public static string SHA512(Stream inputStream, bool lower = false)
        {
            return Hash(SHA512CryptoServiceProvider.Create(), inputStream, lower);
        }


        static string Hash(HashAlgorithm hash, byte[] data, bool lower)
        {
            try
            {
                return ToString(hash.ComputeHash(data), lower);
            }
            finally
            {
                if (hash != null) hash.Clear();
            }
        }
        static string Hash(HashAlgorithm hash, Stream inputStream, bool lower)
        {
            try
            {
                return ToString(hash.ComputeHash(inputStream), lower);
            }
            finally
            {
                if (hash != null) hash.Clear();
            }
        }
        static string ToString(byte[] hashData, bool lower)
        {
            int charIndex = (lower ? 0x61 : 0x41) - 10;

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashData.Length; i++)
            {
                int index = hashData[i] / 16;
                if (index > 9)
                {
                    builder.Append((char)(index + charIndex));
                }
                else
                {
                    builder.Append((char)(index + 0x30));
                }
                index = hashData[i] % 16;
                if (index > 9)
                {
                    builder.Append((char)(index + charIndex));
                }
                else
                {
                    builder.Append((char)(index + 0x30));
                }
            }
            return builder.ToString();
        }
    }
}