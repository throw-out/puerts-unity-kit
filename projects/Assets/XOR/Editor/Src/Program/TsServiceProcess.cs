using System.IO;
using Puerts;

namespace XOR
{
    public class TsServiceProcess : Singleton<TsServiceProcess>
    {
        internal JsEnv Env { get; private set; }
        internal MergeLoader Loader { get; private set; }

        ~TsServiceProcess()
        {
            this.Release();
        }

        public override void Init()
        {
            base.Init();

            bool isESM = Settings.Load().IsESM;

            Loader = new MergeLoader();
            Loader.AddLoader(new DefaultLoader(), int.MaxValue);

            string projectRoot = Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath), "TsEditorProject");
            string outputRoot = Path.Combine(projectRoot, "output");
            Loader.AddLoader(new FileLoader(outputRoot, projectRoot));

            Env = new JsEnv(Loader);
            Env.TryAutoUsing();
            Env.SupportCommonJS();
            Env.RequireXORModules(isESM);
        }
        public override void Release()
        {
            base.Release();

            if (Env != null)
            {
                //Env.GlobalListenerQuit();
                Env.Tick();
                Env.Dispose();
                Env = null;
            }
            if (Loader != null)
            {
                Loader.Dispose();
                Loader = null;
            }
        }
        public void Tick()
        {
            Env?.Tick();
        }
    }
}
