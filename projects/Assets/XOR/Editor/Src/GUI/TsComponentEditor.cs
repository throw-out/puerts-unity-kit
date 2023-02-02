using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using XOR.Serializables;
using XOR.Services;

namespace XOR
{
    [CustomEditor(typeof(TsComponent))]
    internal class TsComponentEditor : Editor
    {
        private const int ModuleHeaderWidth = 60;

        private static bool servicesStatusFoldout = true;
        private static bool moduleFoldout = true;
        private static bool memberFoldout = true;

        private TsComponent component;
        private Program program;
        private Statement statement;

        private Rect moduleSelectorRect;

        private SerializedObjectWrap root;
        private List<NodeWrap> properties;
        private XOR.Serializables.TsComponent.Display display;

        void OnEnable()
        {
            component = target as TsComponent;
            if (!EditorApplicationUtil.IsRunning())
            {
                servicesStatusFoldout = true;
            }
            //初始化节点信息
            root = SerializedObjectWrap.Create(serializedObject, typeof(TsComponent));
            properties = new List<NodeWrap>();
            display = XOR.Serializables.TsComponent.Display.Create();
        }
        void OnDisable()
        {
            component = null;
        }

        public override void OnInspectorGUI()
        {
            if (component == null)
            {
                return;
            }
            program = EditorApplicationUtil.GetProgram();
            statement = program?.GetStatement(TsComponentHelper.GetGuid(component));
            if (statement != null && statement.version != TsComponentHelper.GetVersion(component))
            {
                TsComponentHelper.RebuildProperties(component, statement);
            }
            TsComponentHelper.RebuildNodes(root, properties, statement);

            EditorGUILayout.BeginVertical();

            RenderServicesStatus();
            RenderModule();
            RenderMembers();

            EditorGUILayout.EndVertical();
        }

        void RenderServicesStatus()
        {
            if (GUIUtil.RenderStatusHeader())
            {
                servicesStatusFoldout = !servicesStatusFoldout;
            }
            if (servicesStatusFoldout)
            {
                GUIUtil.RenderStatusContent();
            }
            else
            {
                GUILayout.Space(Skin.LineSpace);
            }
        }
        void RenderModule()
        {
            if (GUIUtil.RenderHeader("模块"))
            {
                moduleFoldout = !moduleFoldout;
            }
            if (moduleFoldout)
            {
                using (new EditorGUI.DisabledScope(!EditorApplicationUtil.IsAvailable()))
                {
                    GUIUtil.RenderGroup(
                        _RenderModule,
                        _RenderModuleSelector
                    );
                }
            }
            else
            {
                GUILayout.Space(Skin.LineSpace);
            }
        }
        void RenderMembers()
        {
            if (GUIUtil.RenderHeader("成员属性"))
            {
                memberFoldout = !memberFoldout;
            }
            if (memberFoldout)
            {
                using (new EditorGUI.DisabledScope(statement == null))
                {
                    GUIUtil.RenderGroup(_RenderPropertiesNodes);
                }
            }
            else
            {
                GUILayout.Space(Skin.LineSpace);
            }
        }

        void _RenderModule()
        {
            string guid = component != null ? TsComponentHelper.GetGuid(component) : string.Empty;
            string route = component != null ? TsComponentHelper.GetRoute(component) : string.Empty;
            string version = component != null ? TsComponentHelper.GetVersion(component) : string.Empty;

            using (new EditorGUI.DisabledScope(true))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("GUID:", GUILayout.Width(ModuleHeaderWidth));
                GUILayout.Label(guid, Skin.label);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Type:", GUILayout.Width(ModuleHeaderWidth));
                if (statement != null) GUILayout.Label(new GUIContent(statement.name, statement.name), Skin.label);
                else GUILayout.Label("module missing", Skin.label);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Module:", GUILayout.Width(ModuleHeaderWidth));
                if (statement != null) GUILayout.Label(new GUIContent(
                    statement.module.Contains("\\") || statement.module.Contains("/") ? PathUtil.GetLocalPath(statement.module, program.root) : statement.module, statement.module
                ), Skin.label);
                else GUILayout.Label("module missing", Skin.label);
                GUILayout.EndHorizontal();
            }
        }
        void _RenderModuleSelector()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("模块"))
            {
                ModuleSelector.Show(EditorApplicationUtil.GetProgram(), OnSelectorCallback, moduleSelectorRect);
            }
            if (Event.current.type == EventType.Repaint)
            {
                moduleSelectorRect = GUILayoutUtility.GetLastRect();
            }
            using (new EditorGUI.DisabledScope(statement == null))
            {
                if (GUILayout.Button("重置"))
                {
                    TsComponentHelper.ClearProperties(component);
                    TsComponentHelper.RebuildProperties(component, statement);
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                    TsComponentHelper.RebuildNodes(root, properties, statement);
                }
                if (GUILayout.Button("编辑") && File.Exists(statement.path))
                {
                    FileUtil.OpenFileInIDE(statement.path, statement.line);
                }
            }
            GUILayout.EndHorizontal();
        }
        void _RenderPropertiesNodes()
        {
            if (display == null || properties == null || properties.Count == 0)
            {
                GUILayout.Label("Empty");
                return;
            }
            bool dirty = false;
            foreach (var node in properties)
            {
                dirty |= display.Render(node);
            }
            if (dirty)
            {
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
                TsComponentHelper.SetDirty(component);
            }
        }

        void OnSelectorCallback(string guid)
        {
            if (component == null)
                return;
            TsComponentHelper.ClearProperties(component);
            TsComponentHelper.SetGuid(component, guid);
            TsComponentHelper.SetDirty(component);
        }

    }
    internal static class TsComponentHelper
    {
        const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        static FieldInfo _guidField;
        static FieldInfo _routeField;
        static FieldInfo _versionField;
        static FieldInfo guidField
        {
            get
            {
                if (_guidField == null) _guidField = typeof(TsComponent).GetField("guid", Flags);
                return _guidField;
            }
        }
        static FieldInfo routeField
        {
            get
            {
                if (_routeField == null) _routeField = typeof(TsComponent).GetField("route", Flags);
                return _routeField;
            }
        }
        static FieldInfo versionField
        {
            get
            {
                if (_versionField == null) _versionField = typeof(TsComponent).GetField("version", Flags);
                return _versionField;
            }
        }

        public static string GetGuid(TsComponent component)
        {
            return guidField.GetValue(component) as string;
        }
        public static string GetRoute(TsComponent component)
        {
            return routeField.GetValue(component) as string;
        }
        public static string GetVersion(TsComponent component)
        {
            return versionField.GetValue(component) as string;
        }
        public static void SetGuid(TsComponent component, string value)
        {
            guidField.SetValue(component, value);
        }
        public static void SetRoute(TsComponent component, string value)
        {
            routeField.SetValue(component, value);
        }
        public static void SetVersion(TsComponent component, string value)
        {
            versionField.SetValue(component, value);
        }

        public static void RebuildNodes(SerializedObjectWrap root, List<NodeWrap> outputNodes, Statement statement)
        {
            if (outputNodes == null)
                return;
            outputNodes.Clear();
            //创建当前需要渲染的节点信息
            foreach (var info in root.FieldMapping)
            {
                SerializedProperty arrayParent = root.GetProperty(info.Key);
                for (int i = 0; i < arrayParent.arraySize; i++)
                {
                    var nw = new NodeWrap(arrayParent.GetArrayElementAtIndex(i), info.Value.Element.Type, i, arrayParent);
                    if (statement != null && statement is TypeDeclaration type && type.Properties.TryGetValue(nw.Key, out var property))
                    {
                        nw.ExplicitValueType = property.valueType;
                        nw.ExplicitValueRange = property.valueRange;
                        nw.ExplicitValueEnum = property.valueEnum;
                        nw.Tooltip = property.BuildTooltip();
                    }
                    outputNodes.Add(nw);
                }
            }
            outputNodes.Sort((n1, n2) => n1.Index < n2.Index ? -1 : n1.Index < n2.Index ? 1 : 0);
        }
        public static void RebuildProperties(TsComponent component, Statement statement)
        {
            if (statement == null || !(statement is TypeDeclaration))
                return;
            TypeDeclaration type = (TypeDeclaration)statement;

            ComponentWrap<TsComponent> cw = ComponentWrap<TsComponent>.Create();
            //移除无效的节点
            Dictionary<string, IPair> properties = cw.GetProperties(component)?.ToDictionary(pair => pair.Key);
            if (properties != null)
            {
                foreach (string key in properties.Keys.ToArray())
                {
                    if (!type.Properties.ContainsKey(key) ||
                        !cw.IsExplicitPropertyValue(component, key, type.Properties[key].valueType)    //检测当前值兼容性
                    )
                    {
                        properties.Remove(key);
                        cw.RemoveProperty(component, key);
                    }
                }
            }
            //添加未序列化的节点
            int index = 0;
            foreach (var property in type.Properties.Values)
            {
                IPair current = null;
                properties?.TryGetValue(property.name, out current);
                if (current == null)
                {
                    if (cw.AddProperty(component, property.valueType, property.name, index))
                    {
                        if (property.defaultValue != null) cw.SetPropertyValue(component, property.name, property.defaultValue);
                    }
                    else
                    {
                        Debug.LogWarning($"{nameof(XOR.TsComponent)} unsupport type: {property.valueType.FullName}");
                    }
                }
                else if (current.Index != index)
                {
                    cw.SetPropertyIndex(component, property.name, index);
                }
                index++;
            }

            TsComponentHelper.SetVersion(component, statement.version);
            TsComponentHelper.SetDirty(component);
        }
        public static void ClearProperties(TsComponent component)
        {
            ComponentWrap<TsComponent> cw = ComponentWrap<TsComponent>.Create();
            cw.ClearProperties(component);

            TsComponentHelper.SetVersion(component, default);
            TsComponentHelper.SetDirty(component);
        }

        public static void SetDirty(UnityEngine.Object obj)
        {
            if (obj == null)
                return;
            EditorUtility.SetDirty(obj);
        }

        public static void SyncAssetsComponents(bool isForce)
        {
            if (UnityEngine.Application.isPlaying)
            {
                EditorUtility.DisplayDialog("提示", "你必需退出Play模式才能继续操作.", "确定");
                return;
            }
            XOR.Services.Program program = EditorApplicationUtil.GetProgram();
            if (program == null || program.state != XOR.Services.ProgramState.Completed)
            {
                bool startup = EditorUtility.DisplayDialog("提示", "你必需启动XOR服务并等待其初始化完成.", "启动", "取消");
                if (startup && !EditorApplicationUtil.IsRunning())
                    EditorApplicationUtil.Start();
                return;
            }
            string currentScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
            HashSet<string> unknwonGuids = new HashSet<string>(),
                resolveAssets = new HashSet<string>();

            string[] assetPaths = AssetDatabase.GetAllAssetPaths().Where(p => p.StartsWith("Assets")).ToArray();
            try
            {
                for (int i = 0; i < assetPaths.Length; i++)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("遍历资源中", assetPaths[i], (i) / assetPaths.Length))
                        break;
                    var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPaths[i]);
                    if (obj is GameObject gameObject)
                    {
                        bool dirty = SyncGameObjectComponents(program, gameObject, unknwonGuids, isForce);
                        if (dirty)
                        {
                            resolveAssets.Add(assetPaths[i]);
                            AssetDatabase.SaveAssets();
                        }
                    }
                    else if (obj is SceneAsset)
                    {
                        var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(assetPaths[i]);
                        bool dirty = SyncSceneComponents(program, scene, unknwonGuids, isForce);
                        if (dirty)
                        {
                            resolveAssets.Add(assetPaths[i]);
                            AssetDatabase.SaveAssets();
                        }
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                //打开最开始的场景
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(currentScene);
                //打印信息
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("Sync Asset {0}, unknown guid {1}", resolveAssets.Count, unknwonGuids.Count);
                if (unknwonGuids.Count > 0)
                {
                    builder.AppendLine();
                    builder.Append("<b>Unknown Guids:</b>");
                    foreach (var guid in unknwonGuids)
                    {
                        builder.AppendLine();
                        builder.Append(guid);
                    }
                }
                if (resolveAssets.Count > 0)
                {
                    builder.AppendLine();
                    builder.Append("<b>Sync Assets:</b>");
                    foreach (var asset in resolveAssets)
                    {
                        builder.AppendLine();
                        builder.Append(asset);
                    }
                }
                Debug.Log(builder.ToString());
            }
        }
        static bool SyncGameObjectComponents(XOR.Services.Program program, GameObject gameObject, HashSet<string> unknwonGuids, bool isForce)
        {
            var dirty = false;
            var components = gameObject.GetComponentsInChildren<XOR.TsComponent>();
            for (int i = 0; i < components.Length; i++)
            {
                var guid = TsComponentHelper.GetGuid(components[i]);
                if (string.IsNullOrEmpty(guid))
                    continue;
                var statement = program.GetStatement(guid);
                if (statement == null)
                {
                    if (unknwonGuids != null) unknwonGuids.Add(guid);
                    continue;
                }
                if (isForce || TsComponentHelper.GetVersion(components[i]) != statement.version)
                {
                    TsComponentHelper.RebuildProperties(components[i], statement);
                    dirty |= true;
                }
            }
            if (dirty)
            {
                SetDirty(gameObject);
            }
            return dirty;
        }
        static bool SyncSceneComponents(XOR.Services.Program program, UnityEngine.SceneManagement.Scene sceen, HashSet<string> unknwonGuids, bool isForce)
        {
            var dirty = false;
            foreach (GameObject gameObject in sceen.GetRootGameObjects())
            {
                dirty |= SyncGameObjectComponents(program, gameObject, unknwonGuids, isForce);
            }
            return dirty;
        }
    }
}
