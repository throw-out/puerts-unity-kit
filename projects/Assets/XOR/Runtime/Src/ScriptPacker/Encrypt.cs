using System.IO;
using System.IO.Compression;

namespace XOR
{
    public interface IEncrypt
    {
        byte[] Encode(byte[] sourceData);
        byte[] Decode(byte[] encodeData);
    }
    public class GzipEncrypt : IEncrypt
    {
        const int BUFFER_SIZE = 1024 * 1024;

        public byte[] Encode(byte[] sourceData)
        {
            MemoryStream reader = null, writer = null;
            try
            {
                reader = new MemoryStream(sourceData);
                writer = new MemoryStream();
                Compress(reader, writer);

                return writer.ToArray();
            }
            finally
            {
                if (reader != null) reader.Dispose();
                if (writer != null) writer.Dispose();
            }
        }
        public byte[] Decode(byte[] encodeData)
        {
            MemoryStream reader = null, writer = null;
            try
            {
                reader = new MemoryStream(encodeData);
                writer = new MemoryStream();
                Decompress(reader, writer);

                return writer.ToArray();
            }
            finally
            {
                if (reader != null) reader.Dispose();
                if (writer != null) writer.Dispose();
            }
        }

        private static void Compress(Stream reader, Stream writer)
        {
            using (GZipStream gzip = new GZipStream(writer, CompressionMode.Compress))
            {
                byte[] buffer = new byte[BUFFER_SIZE];//缓冲区

                int length = 0;
                while ((length = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    gzip.Write(buffer, 0, length);
                }
            }
        }
        private static void Decompress(Stream reader, Stream writer)
        {
            using (GZipStream gzip = new GZipStream(reader, CompressionMode.Decompress))
            {
                byte[] buffer = new byte[BUFFER_SIZE];//缓冲区

                int length = 0;
                while ((length = gzip.Read(buffer, 0, buffer.Length)) > 0)
                {
                    writer.Write(buffer, 0, length);
                }
            }
        }
    }
}