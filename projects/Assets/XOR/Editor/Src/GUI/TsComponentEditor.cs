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

        private RootWrap root;
        private List<NodeWrap> nodes;
        private XOR.Serializables.TsComponent.Display display;

        void OnEnable()
        {
            component = target as TsComponent;
            if (!EditorApplicationUtil.IsRunning())
            {
                servicesStatusFoldout = true;
            }
            //初始化节点信息
            root = RootWrap.Create(serializedObject, typeof(TsComponent));
            nodes = new List<NodeWrap>();
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
            statement = program?.GetStatement(TsComponentHelper.Guid.Get(component));
            if (statement != null && !TsComponentHelper.IsSynchronized(component, statement))
            {
                TsComponentHelper.RebuildProperties(component, statement);
            }
            TsComponentHelper.RebuildNodes(root, nodes, statement);

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
            if (GUIUtil.RenderHeader(Language.Component.Get("module")))
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
            if (GUIUtil.RenderHeader(Language.Component.Get("properties")))
            {
                memberFoldout = !memberFoldout;
            }
            if (memberFoldout)
            {
                using (new EditorGUI.DisabledScope(statement == null))
                {
                    GUIUtil.RenderGroup(_RenderNodes);
                }
            }
            else
            {
                GUILayout.Space(Skin.LineSpace);
            }
        }

        void _RenderModule()
        {
            string guid = component != null ? TsComponentHelper.Guid.Get(component) : string.Empty;
            string route = component != null ? TsComponentHelper.Route.Get(component) : string.Empty;
            string version = component != null ? TsComponentHelper.Version.Get(component) : string.Empty;

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
                if (statement != null) GUILayout.Label(new GUIContent(statement.GetLocalModule(), statement.module), Skin.label);
                else GUILayout.Label("module missing", Skin.label);
                GUILayout.EndHorizontal();
            }
        }
        void _RenderModuleSelector()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Language.Component.Get("module")))
            {
                ModuleSelector.Show(EditorApplicationUtil.GetProgram(), OnSelectorCallback, moduleSelectorRect);
            }
            if (Event.current.type == EventType.Repaint)
            {
                moduleSelectorRect = GUILayoutUtility.GetLastRect();
            }
            using (new EditorGUI.DisabledScope(statement == null))
            {
                if (GUILayout.Button(Language.Default.Get("reset")))
                {
                    TsComponentHelper.ClearProperties(component);
                    TsComponentHelper.RebuildProperties(component, statement);
                    root.ApplyModifiedProperties();
                    root.Update();
                    TsComponentHelper.RebuildNodes(root, nodes, statement);
                }
                if (GUILayout.Button(Language.Default.Get("edit")) && File.Exists(statement.path))
                {
                    FileUtil.OpenFileInIDE(statement.path, statement.line);
                }
            }
            GUILayout.EndHorizontal();
        }
        void _RenderNodes()
        {
            if (display == null || nodes == null || nodes.Count == 0)
            {
                GUILayout.Label("Empty");
                return;
            }
            bool dirty = false;
            foreach (var node in nodes)
            {
                bool _d = display.Render(node);
                dirty |= _d;
                if (_d)
                {
                    TsComponentHelper.ChangePropertyEvent(root, component, node.Key);
                }
            }
            if (dirty)
            {
                root.ApplyModifiedProperties();
                root.Update();
                TsComponentHelper.SetDirty(component);
            }
        }

        void OnSelectorCallback(string guid)
        {
            if (component == null)
                return;
            TsComponentHelper.ClearProperties(component);
            TsComponentHelper.Guid.Set(component, guid);
            TsComponentHelper.SetDirty(component);
        }

    }
    internal static class TsComponentHelper
    {
        public static readonly PropertyAccessor<TsComponent, string> Guid = new PropertyAccessor<TsComponent, string>("guid");
        public static readonly PropertyAccessor<TsComponent, string> Path = new PropertyAccessor<TsComponent, string>("path");
        public static readonly PropertyAccessor<TsComponent, string> Route = new PropertyAccessor<TsComponent, string>("route");
        public static readonly PropertyAccessor<TsComponent, string> Version = new PropertyAccessor<TsComponent, string>("version");

        static readonly PropertyAccessor<TsComponent, Action<string, object>> OnPropertyChange = new PropertyAccessor<TsComponent, Action<string, object>>("onPropertyChange");

        public static void ChangePropertyEvent(RootWrap root, TsComponent component, string key)
        {
            Action<string, object> func = OnPropertyChange.Get(component);
            if (func == null)
            {
                return;
            }
            root.ApplyModifiedProperties();
            root.Update();
            ComponentWrap<TsComponent> cw = ComponentWrap<TsComponent>.Create();
            IPair pair = cw.GetProperty(component, key);
            if (pair == null)
            {
                return;
            }
            func(pair.Key, pair.Value);
        }
        public static string GetComponentPath(Statement statement)
        {
            if (!Settings.Load().autoLoadScript)
            {
                return string.Empty;
            }
            string path = statement.GetLocalPath();
            if (string.IsNullOrEmpty(path))
                return string.Empty;
            if (path.StartsWith("src/"))
            {
                path = path.Substring(4);
            }
            int extnameIndex = path.LastIndexOf(".");
            if (extnameIndex > 0)
            {
                path = path.Substring(0, extnameIndex);
            }
            return path;
        }
        public static bool IsSynchronized(TsComponent component, Statement statement)
        {
            return statement.version == TsComponentHelper.Version.Get(component) &&
                TsComponentHelper.GetComponentPath(statement) == TsComponentHelper.Path.Get(component);
        }

        public static void RebuildNodes(RootWrap root, List<NodeWrap> outputNodes, Statement statement)
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
                        nw.ExplicitValueReferences = property.valueReferences;
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
                    if (!cw.AddProperty(component, property.valueType, property.name, index))
                    {
                        Debug.LogWarning($"{nameof(XOR.TsComponent)} unsupport type: {property.valueType.FullName}");
                        continue;
                    }
                    if (property.defaultValue != null)
                    {
                        cw.SetPropertyValue(component, property.name, property.defaultValue);
                    }
                    current = cw.GetProperty(component, property.name);
                }
                else if (current.Index != index)
                {
                    cw.SetPropertyIndex(component, property.name, index);
                }
                //检查Range
                var valueType = property.valueType;
                if (property.valueRange != null && XOR.Serializables.Utility.IsSingleType(valueType.IsArray ? valueType.GetElementType() : valueType))
                {
                    Tuple<float, float> range = property.valueRange;
                    if (valueType.IsArray)
                    {
                        double[] array = current.Value as double[];
                        XOR.Serializables.Utility.SetArrayRange(range, array);
                        cw.SetPropertyValue(component, property.name, array);
                    }
                    else
                    {
                        double value = (double)current.Value;
                        XOR.Serializables.Utility.SetRange(range, ref value);
                        cw.SetPropertyValue(component, property.name, value);
                    }
                }
                //integer值取值范围
                if (XOR.Serializables.Utility.IsIntegerType(valueType.IsArray ? valueType.GetElementType() : valueType))
                {
                    if (valueType.IsArray)
                    {
                        Array array = current.Value as Array;
                        if (XOR.Serializables.Utility.SetIntegerArrayRange(valueType, array))
                        {
                            cw.SetPropertyValue(component, property.name, array);
                        }
                    }
                    else
                    {
                        int value = (int)(double)current.Value;
                        if (XOR.Serializables.Utility.SetIntegerRange(valueType, ref value))
                        {
                            cw.SetPropertyValue(component, property.name, value);
                        }
                    }
                }
                index++;
            }

            TsComponentHelper.Path.Set(component, TsComponentHelper.GetComponentPath(statement));
            TsComponentHelper.Version.Set(component, statement.version);
            TsComponentHelper.SetDirty(component);
        }
        public static void ClearProperties(TsComponent component)
        {
            ComponentWrap<TsComponent> cw = ComponentWrap<TsComponent>.Create();
            cw.ClearProperties(component);

            TsComponentHelper.Version.Set(component, default);
            TsComponentHelper.SetDirty(component);
        }
        public static bool RebuildTsReferenceProperties(TsComponent component, Statement statement)
        {
            if (statement == null || !(statement is TypeDeclaration))
                return false;

            ComponentWrap<TsComponent> cw = ComponentWrap<TsComponent>.Create();
            Dictionary<string, IPair> properties = cw.GetProperties(component)?.ToDictionary(pair => pair.Key);
            if (properties == null)
                return false;

            bool dirty = false;
            TypeDeclaration type = (TypeDeclaration)statement;
            foreach (var property in type.Properties.Values)
            {
                var valueReferences = property.valueReferences;
                if (valueReferences == null || valueReferences.Count == 0)
                    continue;
                var valueType = property.valueType;
                if (!XOR.Serializables.Utility.IsTsReferenceType(valueType.IsArray ? valueType.GetElementType() : valueType))
                    continue;
                IPair current = null;
                if (!properties.TryGetValue(property.name, out current) || current.Value == null)
                    continue;

                if (valueType.IsArray)
                {
                    var array = current.Value as XOR.TsComponent[];
                    if (array == null)
                        continue;
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] == null || valueReferences.ContainsKey(array[i].Guid))
                            continue;
                        dirty |= true;
                        array[i] = null;
                    }
                }
                else
                {
                    var value = current.Value as XOR.TsComponent;
                    if (value == null || valueReferences.ContainsKey(value.Guid))
                        continue;
                    dirty |= true;
                    cw.SetPropertyValue(component, property.name, default);
                }
            }
            if (dirty) TsComponentHelper.SetDirty(component);
            return dirty;
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
                EditorUtility.DisplayDialog(
                    Language.Default.Get("tip"),
                    Language.Default.Get("must_exit_playing"),
                    Language.Default.Get("confirm")
                );
                return;
            }
            XOR.Services.Program program = EditorApplicationUtil.GetProgram();
            if (program == null || program.state != XOR.Services.ProgramState.Completed)
            {
                GUIUtil.RenderMustLaunchServices();
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
                    if (EditorUtility.DisplayCancelableProgressBar(Language.Default.Get("scanning_resources"), assetPaths[i], (i) / assetPaths.Length))
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
                            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
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
                builder.AppendFormat("Sync Asset {0}, Unknown Guid {1}", resolveAssets.Count, unknwonGuids.Count);
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
                var guid = TsComponentHelper.Guid.Get(components[i]);
                if (string.IsNullOrEmpty(guid))
                    continue;
                var statement = program.GetStatement(guid);
                if (statement == null)
                {
                    if (unknwonGuids != null) unknwonGuids.Add(guid);
                    continue;
                }
                //检查属性是否已同步
                if (isForce || !TsComponentHelper.IsSynchronized(components[i], statement))
                {
                    TsComponentHelper.RebuildProperties(components[i], statement);
                    dirty |= true;
                }
                //检查TsReference类型是否相符
                dirty = RebuildTsReferenceProperties(components[i], statement) || dirty;
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
