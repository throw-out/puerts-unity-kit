using System;
using System.Collections.Generic;
using System.IO;
using Puerts;
using XOR.Services;

namespace XOR
{
    internal static class EditorApplicationUtil
    {
        public static IProgram _cached;
        public static bool IsRunning() => EditorApplication.Instance != null;
        public static bool IsInitializing() => IsRunning() && EditorApplication.Instance.IsInitializing();
        public static bool IsWorkerRunning() => IsRunning() && EditorApplication.Instance.IsWorkerRunning();
        public static bool IsAvailable()
        {
            return IsRunning() && EditorApplication.Instance.IsWorkerRunning() && !EditorApplication.Instance.IsInitializing();
        }
        public static string GetStatus()
        {
            Program program = EditorApplication.Instance?.Program;
            if (program != null)
                return Enum.GetName(typeof(ProgramState), program.state);
            if (IsRunning())
                return "Initializing";
            if (GetCached() != null)
                return "Cached";
            return "UNKNOWN"; ;
        }
        public static string GetCompileStatus()
        {
            if (!IsRunning())
                return GetCached() != null ? "Cached" : "UNKNOWN";
            Program program = EditorApplication.Instance?.Program;
            if (program == null || string.IsNullOrEmpty(program.compile))
                return "-";
            return program.compile;
        }
        public static string GetScripts()
        {
            EditorApplication app = EditorApplication.Instance;
            if (app == null || app.Program == null)
                return "UNKNOWN";
            return $"{app.Program.scripts}";
        }
        public static string GetErrors()
        {
            EditorApplication app = EditorApplication.Instance;
            if (app == null || app.Program == null)
                return "UNKNOWN";
            return $"{app.Program.errors}";
        }
        public static int GetErrorsCount()
        {
            EditorApplication app = EditorApplication.Instance;
            if (app == null || app.Program == null)
                return 0;
            return app.Program.errors;
        }
        public static string GetTypeCount()
        {
            IProgram program = GetProgram();
            if (program == null)
                return "UNKNOWN";
            return $"{program.Statements.Count}";
        }
        public static IProgram GetProgram()
        {
            EditorApplication app = EditorApplication.Instance;
            if (app == null || app.Program == null)
                return GetCached();
            return app.Program;
        }
        public static Statement GetStatement(string guid)
        {
            EditorApplication app = EditorApplication.Instance;
            if (app == null || app.Program == null)
                return null;
            Statement statement = null;
            if (guid != null)
            {
                app.Program.Statements.TryGetValue(guid, out statement);
            }
            return statement;
        }
        public static IProgram GetCached()
        {
            if (_cached == null && Settings.Load().cached)
            {
                _cached = TsServicesCached.CreateProgramFormRoot();
            }
            return _cached;
        }
        public static void DeleteCached()
        {
            _cached = null;
        }

        public static void Start()
        {
            Prefs.ASTEnable.SetValue(true);
            try
            {
                string projectConfig = PathUtil.GetFullPath(Settings.Load().project);
                bool useNodejsWatch = Settings.Load().watchType == Settings.WacthType.Nodejs && PuertsUtil.IsSupportNodejs();

                if (!File.Exists(projectConfig))
                {
                    string newPath = GUIUtil.RenderSelectProject(projectConfig);
                    if (string.IsNullOrEmpty(newPath))
                    {
                        Prefs.ASTEnable.SetValue(false);
                        return;
                    }
                    projectConfig = PathUtil.GetFullPath(newPath);
                }
                if (!ValidateProjectEnv(projectConfig))
                    return;

                Logger.Log($"<b>XOR.{nameof(EditorApplication)}: <color=green>Executing</color></b>");

                //创建EditorApplication实例
                EditorApplication app = EditorApplication.GetInstance();
                //create interfaces
                CSharpInterfaces ci = new CSharpInterfaces
                {
                    SetWorker = app.SetWorker,
                    SetProgram = (program) =>
                    {
                        app.SetProgram(program);

                        var cached = GetCached();
                        if (cached != null)
                        {
                            foreach (var statement in program.Statements)
                            {
                                cached.AddStatement(statement.Value);
                            }
                            program.SetCahced(cached);
                        }
                    }
                };
                //init application
                if (Prefs.DeveloperMode)
                {
                    string editorProjectOutput = Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath), "TsEditorProject/output");
                    var loader = new FileLoader(editorProjectOutput, Path.GetDirectoryName(editorProjectOutput));
                    app.Loader.AddLoader(loader);
                    app.Env.Load("webpack-editor/main");

                    var init = GetGlobal<Func<CSharpInterfaces, ILoader, TSInterfaces>>(app.Env, "init");
                    app.SetInterfaces(init(ci, loader));
                    app.Interfaces.Start(projectConfig, "webpack-editor/child");
                }
                else
                {
                    //app.Env.Eval("require('puerts/xor-editor/main')");
                    app.Env.Load("puerts/xor-editor/main");

                    var init = GetGlobal<Func<CSharpInterfaces, ILoader, TSInterfaces>>(app.Env, "init");
                    app.SetInterfaces(init(ci, null));
                    app.Interfaces.Start(projectConfig, "puerts/xor-editor/child");
                }

                //监听文件修改
                if (useNodejsWatch)
                {
                    app.Interfaces.Watch(projectConfig);
                }
                EditorFileWatcher.ReleaseInstance();
                if (!useNodejsWatch && Settings.Load().watchType != Settings.WacthType.None)
                {
                    string dirpath = Path.GetDirectoryName(projectConfig);
                    EditorFileWatcher watcher = EditorFileWatcher.GetInstance();
                    watcher.AddWatcher(dirpath, "*.ts");
                    watcher.AddWatcher(dirpath, "*.tsx");
                    watcher.OnChanged((path, type) =>
                    {
                        if (app == null || app.IsDestroyed || app.Interfaces == null)
                            return;
                        app.Interfaces.FileChanged(path);
                    });
                    watcher.Start(true);
                    Logger.Log($"<b>XOR.{nameof(EditorFileWatcher)}:</b> {dirpath}");
                }

                Logger.Log($"<b>XOR.{nameof(EditorApplication)}: <color=green>Started</color>.</b>");
            }
            catch (System.Exception e)
            {
                Prefs.ASTEnable.SetValue(false);
                EditorApplication.ReleaseInstance();
                EditorFileWatcher.ReleaseInstance();
                Logger.Log($"<b>XOR.{nameof(EditorApplication)}: <color=red>Exception</color>:</b>\n{e}");
            }
        }
        public static void Stop(bool print = true)
        {
            Prefs.ASTEnable.SetValue(false);
            EditorApplication.ReleaseInstance();
            EditorFileWatcher.ReleaseInstance();
            if (!UnityEngine.Application.isPlaying)
            {
                ThreadWorker.ReleaseAllInstances();
            }

            if (print) Logger.Log($"<b>XOR.{nameof(EditorApplication)}: <color=red>Stoped</color>.</b>");
        }

        /// <summary>
        /// 验证TsProject环境是否已安装
        /// </summary>
        /// <param name="tsconfigPath"></param>
        /// <returns></returns>
        static bool ValidateProjectEnv(string tsconfigPath)
        {
            //check dependents install
            string packagePath = Path.Combine(Path.GetDirectoryName(tsconfigPath), "package.json");
            if (!File.Exists(packagePath))
            {
                GUIUtil.RenderInvailTsconfig(tsconfigPath);
                return false;
            }
            string[] uninstallModules = NPMUtil.CheckDependentsInstall(packagePath);
            if (uninstallModules != null && uninstallModules.Length > 0)
            {
                bool open = GUIUtil.RenderDependentsUninstall(tsconfigPath, uninstallModules);
                if (open) UnityEditor.EditorUtility.RevealInFinder(tsconfigPath);
                return false;
            }
            //check compile output
            return true;
        }
        static T GetGlobal<T>(JsEnv env, string key)
        {
            return env.Eval<T>($@"
(function(){{
    let _g = global || globalThis || this;
    return _g['{key}'];
}})();");
        }
    }
}