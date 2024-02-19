using Puerts;

namespace MiniLinkXml
{
    /// <summary>
    /// 某些版本的Unity中, Puerts.DefaultLoader似乎不能正常工作???
    /// </summary>
    class ProxyAssertLoader : ILoader
    {
        private readonly ILoader loader;
        public ProxyAssertLoader() : this(new DefaultLoader())
        {
        }
        public ProxyAssertLoader(ILoader loader)
        {
            this.loader = loader;
        }
        public bool FileExists(string filepath)
        {
            if (!this.loader.FileExists(filepath))
                return false;
            if (loader is Puerts.DefaultLoader)
            {
                string script = loader.ReadFile(filepath, out string debugpath);
                return !string.IsNullOrEmpty(script);
            }
            return true;
        }

        public string ReadFile(string filepath, out string debugpath)
        {
            return this.loader.ReadFile(filepath, out debugpath);
        }
    }
}