using System.Collections.Generic;
using System.IO;

namespace XOR
{
    public static class ScriptUtil
    {
        public static byte[] Pack(Dictionary<string, string> scripts)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(scripts.Count);
            foreach (var a in scripts)
            {
                writer.Write(a.Key);
                writer.Write(a.Value);
            }
            return stream.ToArray();
        }

        public static Dictionary<string, string> Unpack(byte[] data)
        {
            Dictionary<string, string> scripts = new Dictionary<string, string>();

            return scripts;
        }
    }
}
