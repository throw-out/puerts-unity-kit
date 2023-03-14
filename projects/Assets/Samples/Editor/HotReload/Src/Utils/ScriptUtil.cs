namespace HR
{
    using System.IO;
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