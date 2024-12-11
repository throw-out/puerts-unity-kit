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
            if (IsRunning())
            {
                return EditorApplication.Instance.IsWorkerRunning() && !EditorApplication.Instance.IsInitializing();
            }
            return GetProgramCached() != null;
        }
        public static string GetStatus()
        {
            if (!IsRunning())
            {
                return GetProgramCached() != null ? "Cached" : "UNKNOWN";
            }
            Program program = EditorApplication.Instance.Program;
            if (program == null)
                return "Initializing";
            return Enum.GetName(typeof(ProgramState), program.state);
        }
        public static string GetCompileStatus()
        {
            if (!IsRunning())
            {
                return GetProgramCached() != null ? "-" : "UNKNOWN";
            }
            Program program = EditorApplication.Instance.Program;
            if (program == null || string.IsNullOrEmpty(program.compile))
                return "-";
            return program.compile;
        }
        public static string GetScripts()
        {
            if (!IsRunning())
            {
                return GetProgramCached() != null ? "-" : "UNKNOWN";
            }
            EditorApplication app = EditorApplication.Instance;
            if (app == null || app.Program == null)
                return "UNKNOWN";
            return $"{app.Program.scripts}";
        }
        public static string GetErrors()
        {
            if (!IsRunning())
            {
                return GetProgramCached() != null ? "-" : "UNKNOWN";
            }
            EditorApplication app = EditorApplication.Instance;
            if (app == null || app.Program == null)
                return "UNKNOWN";
            return $"{app.Program.errors}";
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
            if (app == null)
                return GetProgramCached();
            return app.Program;
        }
        public static Statement GetStatement(string guid)
        {
            if (guid == null)
                return null;
            IProgram program = GetProgram();
            if (program == null)
                return null;
            program.Statements.TryGetValue(guid, out Statement statement);
            return statement;
        }
        public static IProgram GetProgramCached()
        {
            if (_cached == null && Settings.GetInstance().cached)
            {
                _cached = ProgramCached.CreateProgramFormRoot();
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
                string projectConfig = PathUtil.GetFullPath(Settings.GetInstance().project);
                bool useNodejsWatch = Settings.GetInstance().watchType == Settings.WacthType.Nodejs && PuertsUtil.IsSupportNodejs();

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
                        var cached = GetProgramCached();
                        if (cached != null)
                        {
                            foreach (var statement in program.Statements)
                            {
                                cached.AddStatement(statement.Value);
                            }
                            program.SetCahced(cached);
                        }

                        app.SetProgram(program);
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
                if (!useNodejsWatch && Settings.GetInstance().watchType != Settings.WacthType.None)
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