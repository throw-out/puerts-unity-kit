using Puerts;
using System;

namespace XOR
{
    public static class BufferUtil
    {
        public static byte[] ToBytes(ArrayBuffer buffer)
        {
            return buffer.Bytes;
        }
        public static ArrayBuffer ToBuffer(byte[] bytes)
        {
            return new ArrayBuffer(bytes);
        }
        public static byte[] Copy(byte[] bytes, int offset, int length)
        {
            var result = new byte[length];
            Array.Copy(bytes, offset, result, 0, length);
            return result;
        }
        public static ArrayBuffer Copy(ArrayBuffer buffer, int offset, int length)
        {
            return ToBuffer(Copy(ToBytes(buffer), offset, length));
        }
        public static byte[] Create(int size)
        {
            return new byte[size];
        }
        public static byte[] Connect(byte[] first, byte[] second)
        {
            var result = new byte[first.Length + second.Length];
            Array.Copy(first, 0, result, 0, first.Length);
            Array.Copy(second, 0, result, first.Length, second.Length);
            return result;
        }
    }
}
