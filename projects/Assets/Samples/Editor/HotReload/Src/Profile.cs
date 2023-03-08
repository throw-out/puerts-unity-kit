namespace HR
{
    internal class Profile
    {
        public string name;
        public string path;
        public string host;
        public ushort port;

        /// <summary>忽略路径大小写  </summary>
        public bool ignoreCase;
        /// <summary>追踪信息  </summary>
        public bool trace;
        /// <summary>自动重连  </summary>
        public bool reconnect;
        /// <summary>重连延迟  </summary>
        public ulong reconnectDelay;
        /// <summary>启动时检查脚本更新  </summary>
        public bool startupCheck;

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