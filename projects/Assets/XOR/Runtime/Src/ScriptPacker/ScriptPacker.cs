using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XOR
{
    public static class ScriptPacker
    {
        public static byte[] Pack(Dictionary<string, string> scripts, params object[] decorators)
        {
            ISignature auth = decorators.FirstOrDefault(d => d is ISignature) as ISignature;
            IEncrypt encrypt = decorators.FirstOrDefault(d => d is IEncrypt) as IEncrypt;

            byte[] data = PackScripts(scripts);

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);
                //write data
                writer.Write((bool)(encrypt != null));
                writer.WriteBlock(encrypt != null ? encrypt.Encode(data) : data);

                //write sign
                writer.Write((bool)(auth != null));
                if (auth != null)
                {
                    writer.WriteBlock(auth.Sign(data));
                }

                return stream.ToArray();
            }
        }

        public static Dictionary<string, string> Unpack(byte[] data, params object[] decorators)
        {
            return Unpack(data, true, decorators);
        }
        public static Dictionary<string, string> Unpack(byte[] data, bool throwFailure, params object[] decorators)
        {
            ISignature auth = decorators.FirstOrDefault(d => d is ISignature) as ISignature;
            IEncrypt encrypt = decorators.FirstOrDefault(d => d is IEncrypt) as IEncrypt;

            using (MemoryStream stream = new MemoryStream(data))
            {
                BinaryReader reader = new BinaryReader(stream);

                //read data
                bool isEncrypt = reader.ReadBoolean();
                byte[] _d = reader.ReadBlock();
                if (isEncrypt)
                {
                    if (encrypt == null)
                    {
                        if (throwFailure)
                            throw new ArgumentException($"{nameof(IEncrypt)} instance required.");
                        return null;
                    }
                    _d = encrypt.Decode(_d);
                }

                //verify sign
                bool isSign = reader.ReadBoolean();
                if (isSign)
                {
                    if (auth == null)
                    {
                        if (throwFailure)
                            throw new ArgumentException($"{nameof(ISignature)} instance required.");
                        return null;
                    }

                    byte[] signData = reader.ReadBlock();
                    if (!auth.Verify(_d, signData))
                    {
                        if (throwFailure)
                            throw new Exception($"{nameof(ISignature)} verify failure.");
                        return null;
                    }
                }
                return UnpackScripts(_d);
            }
        }

        /// <summary>
        /// 扫描指定目录下的所有js文件
        /// </summary>
        /// <param name="outputPath">js文件输出路径</param>
        /// <param name="fileExtNames">指定js文件后缀</param>
        /// <returns></returns>
        public static Dictionary<string, string> Scan(string outputPath, string[] fileExtNames = null)
        {
            if (fileExtNames == null || fileExtNames.Length == 0)
            {
                fileExtNames = new string[] { ".js", ".mjs", ".cjs" };
            }
            HashSet<string> extNames = new HashSet<string>(fileExtNames);

            Dictionary<string, string> scripts = new Dictionary<string, string>();

            string[] files = GetFiles(outputPath, extNames);
            if (files != null)
            {
                outputPath = outputPath.Replace("\\", "/");
                if (!outputPath.EndsWith("/")) outputPath += "/";

                foreach (string filePath in files)
                {
                    string localName = filePath.Replace("\\", "/").Replace(outputPath, "");
                    string content = File.ReadAllText(filePath);
                    scripts.Add(localName, content);
                }
            }
            return scripts;
        }
        /// <summary>
        /// 扫描指定项目路径下的node_modules
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="moduleNames"></param>
        /// <param name="fileExtNames"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ScanModule(string rootPath, string[] moduleNames, string[] fileExtNames = null)
        {
            if (fileExtNames == null || fileExtNames.Length == 0)
            {
                fileExtNames = new string[] { ".js", ".mjs", ".cjs", ".json" };
            }
            HashSet<string> extNames = new HashSet<string>(fileExtNames);

            rootPath = rootPath.Replace("\\", "/");
            if (!rootPath.EndsWith("/")) rootPath += "/";

            Dictionary<string, string> scripts = new Dictionary<string, string>();
            foreach (string moduleName in moduleNames)
            {
                string[] files = GetFiles(Path.Combine(rootPath, "node_modules", moduleName), extNames);
                if (files == null)
                {
                    UnityEngine.Debug.LogWarning($"node_modules {moduleName} is not installe, rootPath= {rootPath}");
                    continue;
                }
                foreach (string filePath in files)
                {
                    string localName = filePath.Replace("\\", "/").Replace(rootPath, "");
                    string content = File.ReadAllText(filePath);
                    scripts.Add(localName, content);
                }
            }
            return scripts;
        }

        static byte[] PackScripts(Dictionary<string, string> scripts)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);

                writer.Write((int)scripts.Count);
                foreach (var script in scripts)
                {
                    writer.Write((string)script.Key);
                    writer.Write((string)script.Value);
                }
                return stream.ToArray();
            }
        }
        static Dictionary<string, string> UnpackScripts(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                BinaryReader reader = new BinaryReader(stream);

                Dictionary<string, string> scripts = new Dictionary<string, string>();
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    string name = reader.ReadString();
                    string content = reader.ReadString();
                    scripts.Add(name, content);
                }

                return scripts;
            }
        }
        static string[] GetFiles(string path, HashSet<string> extNames = null)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists)
                return null;

            List<string> result = new List<string>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                if (extNames != null && !extNames.Contains(fileInfo.Extension))
                {
                    continue;
                }
                result.Add(fileInfo.FullName);
            }
            foreach (DirectoryInfo dirInfo in dir.GetDirectories())
            {
                string[] files = GetFiles(dirInfo.FullName, extNames);
                if (files != null)
                    result.AddRange(files);
            }
            if (result.Count > 0)
                return result.ToArray();
            return null;
        }

        static void WriteBlock(this BinaryWriter writer, byte[] data)
        {
            writer.Write((int)data.Length);
            writer.Write((byte[])data);
        }
        static byte[] ReadBlock(this BinaryReader reader)
        {
            int size = reader.ReadInt32();
            return reader.ReadBytes(size);
        }
    }
}
