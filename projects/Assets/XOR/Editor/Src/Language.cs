using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XOR
{
    public class Language
    {
        internal static Env Type = Env.EN_US;
        internal static readonly Language Default = new Language("i18n/default.json");

        private Language(string path)
        {
            this.path = path;
        }
        private string path;
        private Dictionary<string, I18nData> i18n;

        public string Get(string key)
        {
            if (i18n == null)
            {
                i18n = Read() ?? new Dictionary<string, I18nData>();
            }

            I18nData data;
            if (i18n.TryGetValue(key, out data))
            {
                switch (Type)
                {
                    case Env.EN_US:
                        return data.en_us;
                    //break;
                    case Env.ZH_CN:
                        return data.zh_cn;
                        //break;
                }
            }
            return string.Empty;
        }

        Dictionary<string, I18nData> Read()
        {
            TextAsset asset = Resources.Load<TextAsset>(path);
            if (asset == null || string.IsNullOrEmpty(asset.text))
                return null;
            return JsonUtility.FromJson<Dictionary<string, I18nData>>(asset.text);
        }


        [System.Serializable]
        internal struct I18nData
        {
            public string zh_cn;
            public string en_us;
        }
        internal enum Env
        {
            EN_US = 0,
            ZH_CN,
        }
    }
}
