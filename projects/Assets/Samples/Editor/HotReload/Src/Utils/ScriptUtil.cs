namespace HR
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using JSON = Newtonsoft.Json.JsonConvert;
    public static class ScriptUtil
    {
        public static void PrintScriptInfo(string file)
        {
            bool fileExists = File.Exists(file);
            bool sourceMap = false;
            string source = "<null>";
            bool sourceExists = false;

            if (fileExists && IsSourceMap(ReadFileLastLine(file)))
            {
                var mapData = GetSourceMapData(file, ReadFileLastLine(file));
                sourceMap = mapData != null;
                if (mapData != null && mapData.sources != null && mapData.sources.Length > 0)
                {
                    source = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), mapData.sources[0]));
                    sourceExists = File.Exists(source);
                }
            }

            Debug.Log($@"<b>File</b>:  {file}
<b>FileExists</b>: {fileExists}
<b>SourceMap</b>: {sourceMap}
<b>TSFile</b>: {source}
<b>TSExists</b>: {sourceExists}
");
        }

        public static void UpdateScript(Debugger debugger, string filepath)
        {
            try
            {
                string dirpath = Path.GetDirectoryName(filepath);
                if (!Directory.Exists(dirpath))
                {
                    dirpath = Path.GetDirectoryName(UnityEngine.Application.dataPath);
                }
                string path = UnityEditor.EditorUtility.OpenFilePanel(
                    $"Update: {filepath}",
                    dirpath,
                    null
                );
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    return;

                debugger.Update(filepath, File.ReadAllText(path));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        public static async void SaveScript(Debugger debugger, string filepath)
        {
            EditorUtility.DisplayProgressBar("Reading", string.Empty, 0);
            try
            {
                string dirpath = Path.GetDirectoryName(filepath);
                if (!Directory.Exists(dirpath))
                {
                    dirpath = Path.GetDirectoryName(UnityEngine.Application.dataPath);
                }

                var scriptSource = await debugger.GetScriptSource(filepath);
                if (string.IsNullOrEmpty(scriptSource))
                {
                    Debug.LogWarning($"ScriptSource is empty: {filepath}");
                    return;
                }
                EditorUtility.ClearProgressBar();
                string path = UnityEditor.EditorUtility.SaveFilePanel(
                    $"Save: {filepath}",
                    dirpath,
                    null,
                    null
                );
                if (string.IsNullOrEmpty(path))
                    return;
                dirpath = Path.GetDirectoryName(path);
                if (!Directory.Exists(dirpath)) Directory.CreateDirectory(dirpath);
                File.WriteAllText(path, scriptSource);
                EditorUtility.RevealInFinder(path);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        static string ReadFileLastLine(string file)
        {
            var lines = File.ReadAllLines(file);
            if (lines != null && lines.Length > 0)
            {
                return lines[lines.Length - 1];
            }
            return string.Empty;
        }
        static string SourceMapHeader = "//# sourceMappingURL=";
        static string InlineSourceMapHeader = $"{SourceMapHeader}data:application/json;base64,";
        static bool IsSourceMap(string line)
        {
            return line.StartsWith(SourceMapHeader);
        }
        static SourceMapData GetSourceMapData(string file, string lastLine)
        {
            try
            {
                string json = null;
                if (lastLine.StartsWith(InlineSourceMapHeader))
                {
                    json = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(
                        lastLine.Substring(InlineSourceMapHeader.Length)
                    ));
                }
                else
                {
                    file = Path.Combine(Path.GetDirectoryName(file), lastLine.Substring(SourceMapHeader.Length));
                    if (File.Exists(file))
                    {
                        json = File.ReadAllText(file);
                    }
                }
                if (!string.IsNullOrEmpty(json))
                {
                    return JSON.DeserializeObject<SourceMapData>(json);
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
            return null;
        }

        class SourceMapData
        {
            public int version;
            public string file;
            public string sourceRoot;
            public string[] sources;
            public string mappings;
        }
    }
}