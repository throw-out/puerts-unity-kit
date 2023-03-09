using UnityEditor;

namespace HR
{
    using JSON = Newtonsoft.Json.JsonConvert;

    internal class Prefs
    {
        public static readonly Prefs<bool> SaveExecute = new Prefs<bool>("Editor.HRSaveExecute");
        public static readonly Prefs<Profile[]> Profiles = new Prefs<Profile[]>("Editor.HRProfiles");

    }
    internal class Prefs<T>
    {
        private readonly string key;
        public Prefs(string key)
        {
            this.key = key;
        }
        public T Read()
        {
            var json = EditorPrefs.GetString(this.key);
            return string.IsNullOrEmpty(json) ? default(T) : JSON.DeserializeObject<T>(json);
        }
        public void Save(T profiles)
        {
            if (profiles != null)
            {
                EditorPrefs.SetString(this.key, JSON.SerializeObject(profiles));
            }
            else
            {
                EditorPrefs.DeleteKey(this.key);
            }
        }
    }

    internal class Profile
    {
        public string name;
        public string path;
        public string host = "localhost";
        public ushort port = 9222;
        public bool execute;

        /// <summary>忽略路径大小写  </summary>
        public bool ignoreCase;
        /// <summary>追踪信息  </summary>
        public bool trace;
        /// <summary>自动重连  </summary>
        public bool reconnect = true;
        /// <summary>重连延迟  </summary>
        public ulong reconnectDelay = 1000;
        /// <summary>启动时检查脚本更新  </summary>
        public bool startupCheck = true;

        /// <summary>监听的文件类型 </summary>
        public FileType file = FileType.All;
    }
    internal enum FileType
    {
        None = 0,
        All = js | js_txt | jsx | cjs | mjs | json,
        js = 1,
        js_txt = 2,
        jsx = 4,
        cjs = 8,
        mjs = 16,
        json = 32,
    }
}