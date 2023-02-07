using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace XOR
{
    internal static class ScriptingDefineSymbols
    {
        private static HashSet<string> symbols;
        private static Dictionary<BuildTargetGroup, HashSet<string>> groupSymbols;
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
            foreach (var group in groupSymbols)
            {
                group.Value.Add(symbol);
            }
            change = true;
        }
        public static void RemoveSymbol(string symbol)
        {
            if (symbols == null) Read();
            symbols.Remove(symbol);
            foreach (var group in groupSymbols)
            {
                group.Value.Remove(symbol);
            }
            change = true;
        }
        public static bool HasSymbol(string symbol)
        {
            if (symbols == null) Read();
            return symbols.Contains(symbol);
        }

        public static void Save()
        {
            if (groupSymbols == null)
                return;
            change = false;
            foreach (var group in groupSymbols)
            {
                UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(group.Key, Serialize(group.Value));
            }
        }
        public static void Read()
        {
            change = false;
            symbols = new HashSet<string>();
            groupSymbols = new Dictionary<BuildTargetGroup, HashSet<string>>();
            foreach (var target in Enum.GetValues(typeof(BuildTargetGroup)))
            {
                var _target = (BuildTargetGroup)target;
                if (_target == BuildTargetGroup.Unknown || groupSymbols.ContainsKey(_target))
                    continue;
                var ss = Deserialize(UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(_target));
                foreach (var s in ss) symbols.Add(s);
                groupSymbols.Add(_target, new HashSet<string>(ss));
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