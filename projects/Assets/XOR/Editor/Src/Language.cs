using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XOR
{
    internal class Language
    {
        internal static Env Type => (Env)Prefs.Language.GetValue();

        public static readonly Language Default = new Language("i18n/default");
        public static readonly Language Component = new Language("i18n/component");
        public static readonly Language Behaviour = new Language("i18n/behaviour");

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
                i18n = Load() ?? new Dictionary<string, I18nData>();
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

        private Dictionary<string, I18nData> Load()
        {
            TextAsset asset = Resources.Load<TextAsset>(path);
            if (asset == null || string.IsNullOrEmpty(asset.text))
                return null;
            return Unity.Plastic.Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, I18nData>>(asset.text);
        }

        public static void Reload()
        {
            Default.i18n = null;
            Component.i18n = null;
            Behaviour.i18n = null;
        }

        [System.Serializable]
        internal struct I18nData
        {
            public string zh_cn;
            public string en_us;
        }
        internal enum Env
        {
            ZH_CN,
            EN_US,
        }
    }
}
