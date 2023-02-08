using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace XOR
{
    internal static class ScriptingDefineSymbols
    {
        private static readonly string cscRsp = Path.Combine(UnityEngine.Application.dataPath, "csc.rsp");
        private static HashSet<string> symbols;
        private static bool change;
        public static bool Change => change;

        public static string[] GetSymbols()
        {
            if (symbols != null) Read();
            return symbols.ToArray();
        }
        public static void AddSymbol(string symbol)
        {
            if (symbols == null) Read();
            if (symbols.Contains(symbol))
                return;
            symbols.Add(symbol);
            change = true;
        }
        public static void RemoveSymbol(string symbol)
        {
            if (symbols == null) Read();
            symbols.Remove(symbol);
            change = true;
        }
        public static bool HasSymbol(string symbol)
        {
            if (symbols == null) Read();
            return symbols.Contains(symbol);
        }

        public static void Save()
        {
            if (symbols == null)
                return;
            if (symbols.Count > 0)
            {
                File.WriteAllText(cscRsp, string.Format("-define:{0}", Serialize(symbols)));
            }
            else if (File.Exists(cscRsp))
            {
                File.Delete(cscRsp);
                var meta = cscRsp + ".meta";
                if (File.Exists(meta)) File.Delete(meta);
            }
            change = false;
            AssetDatabase.Refresh();
            FileUtil.SyncIDE();
            //UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Unknown, "");
            //UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Unknown)
        }
        public static void Read()
        {
            change = false;
            symbols = new HashSet<string>();
            if (File.Exists(cscRsp))
            {
                var lines = File.ReadAllText(cscRsp).Replace("\r\n", "\n").Split('\n');
                foreach (var line in lines)
                {
                    if (!line.StartsWith("-define:") || line.Length == 8)
                        continue;
                    foreach (var symbol in Deserialize(line.Substring(8)))
                    {
                        symbols.Add(symbol);
                    }
                }
            }
        }

        static IEnumerable<string> Deserialize(string content)
        {
            return content.Split(';')
                .Select(symbol => symbol.Trim())
                .Where(line => !string.IsNullOrEmpty(line));
        }
        static string Serialize(IEnumerable<string> symbols)
        {
            return string.Join(";", symbols);
        }
    }
}