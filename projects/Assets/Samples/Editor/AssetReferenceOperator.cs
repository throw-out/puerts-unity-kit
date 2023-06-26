using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class AssetReferenceOperator
{
    [MenuItem("Assets/Reference/Migrate", true)]
    static bool ReferenceMigrateValidate()
    {
        var objs = Selection.objects;
        return objs != null && objs.Length >= 2;
    }
    /// <summary>
    /// 合并对多个文件的引用
    /// </summary>
    [MenuItem("Assets/Reference/Migrate")]
    static void ReferenceMigrate()
    {
        var objs = Selection.objects;
        if (objs == null || objs.Length < 2)
            return;
        Func<string, int, string> BuildDialogContent = (title, current) =>
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(title);
            for (int i = 0; i < objs.Length; i++)
            {
                builder.AppendLine();
                builder.Append(i == current ?
                    $"【{i}】 " :
                    $"〖{i}〗 "
                );
                builder.Append(AssetDatabase.GetAssetPath(objs[i]));
            }
            if (current >= 0 && current < objs.Length)
            {
                builder.AppendLine();
                builder.AppendLine();
                builder.Append("合并引用至资源:");
                builder.AppendLine();
                builder.Append(current < objs.Length ? AssetDatabase.GetAssetPath(objs[current]) : "<NULL>");
            }
            return builder.ToString();
        };

        //检查资源类型一致性
        var firstType = objs[0].GetType();
        if (objs.Any(obj => !obj.GetType().Equals(firstType)))
        {
            bool ok = EditorUtility.DisplayDialog("提示", BuildDialogContent("选中的资源类型不一致, 是否继续操作?", -1), "继续", "取消");
            if (!ok)
                return;
        }

        //检查图片SpriteImportMode
        bool hasMultipleSpriteImport = objs.Any(obj =>
        {
            if (!(obj is Texture) && !(obj is Sprite))
                return false;
            var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(obj)) as TextureImporter;
            if (importer == null)
                return false;
            return importer.spriteImportMode == SpriteImportMode.Multiple;
        });
        if (hasMultipleSpriteImport)
        {
            bool ok = EditorUtility.DisplayDialog("提示", BuildDialogContent("选中的Texture资源中存在MultipleSprite模式, 强制合并可能导致引用异常, 是否继续操作?", -1), "继续", "取消");
            if (!ok)
                return;
        }

        //选中保留的资源引用
        int selected = 0;
        while (true)
        {
            int menuIndex = EditorUtility.DisplayDialogComplex(
                "提示",
                BuildDialogContent("当前选中的资源: ", selected),
                "切换", "取消", "确定"
            );
            if (menuIndex == 0)
            {
                selected = selected < objs.Length - 1 ? selected + 1 : 0;
            }
            else if (menuIndex == 1)
            {
                return;
            }
            else
            {
                break;
            }
        }

        //合并资源
        string newGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(objs[selected]));
        string[] oldGuids = objs
            .Where((o, idx) => idx != selected)
            .Select(obj => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj))).ToArray();

        var rootPath = Application.dataPath.Replace("Assets", "");
        var assetPaths = AssetDatabase.GetAllAssetPaths();
        try
        {
            int count = 0;
            for (int i = 0; i < assetPaths.Length; i++)
            {
                string assetPath = assetPaths[i];
                bool cancel = EditorUtility.DisplayCancelableProgressBar($"处理中({i}/{assetPaths.Length})", assetPath, (float)i / assetPaths.Length);
                if (cancel)
                    break;

                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                if (obj == null)
                    continue;
                var isNative = AssetDatabase.IsNativeAsset(obj);     //本机资产(由Unity生成的序列化文件)
                var isForeign = AssetDatabase.IsForeignAsset(obj);   //外部资产(png等导入资产,首次导入将会生成序列化表示文件)
                var isMain = AssetDatabase.IsMainAsset(obj);         //资产是项目窗口中的主要资产
                if (isNative || isMain && (assetPath.EndsWith(".prefab") || assetPath.EndsWith(".unity")))
                {
                    //本机主要资产
                    var path = rootPath + assetPath;
                    var content = File.ReadAllText(path);
                    foreach (var oldGuid in oldGuids)
                    {
                        content = content.Replace(oldGuid, newGuid);
                    }
                    File.WriteAllText(path, content);
                    count++;
                }
            }
            Debug.Log($"本次共处理{count}个文件");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
        //AssetDatabase.Refresh();
    }


    [MenuItem("Assets/Reference/LookDependencies", true)]
    [MenuItem("Assets/Reference/LookReferences", true)]
    static bool LookValidate()
    {
        return Selection.activeObject != null;
    }
    /// <summary>查看资源依赖信息 </summary>
    [MenuItem("Assets/Reference/LookDependencies")]
    static void LookDependencies()
    {
        var path = Selection.activeObject != null ? AssetDatabase.GetAssetPath(Selection.activeObject) : null;
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return;

        var dependencies = AssetDatabase.GetDependencies(path);
        var dependenciesInfo = new StringBuilder();
        for (int i = 0; i < dependencies.Length; i++)
        {
            dependenciesInfo.AppendLine();
            dependenciesInfo.Append(i + 1);
            dependenciesInfo.Append('.');
            dependenciesInfo.Append(dependencies[i]);
        }
        Debug.Log($"<b>Path:</b>{path} \n<b>依赖:</b>{dependenciesInfo.ToString()}");
    }
    /// <summary>查看资源引用信息(其他资源对此资源依赖)</summary>
    [MenuItem("Assets/Reference/LookReferences")]
    static void LookReferences()
    {
        var path = Selection.activeObject != null ? AssetDatabase.GetAssetPath(Selection.activeObject) : null;
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return;

        //Dependencies
        var dependenciesInfo = new StringBuilder();
        var dependencies = AssetDatabase.GetDependencies(path);
        for (int i = 0; i < dependencies.Length; i++)
        {
            dependenciesInfo.AppendLine();
            dependenciesInfo.Append(i + 1);
            dependenciesInfo.Append('.');
            dependenciesInfo.Append(dependencies[i]);
        }
        //References
        var referencesInfo = new StringBuilder();
        var referencesIndex = 0;
        string[] paths = AssetDatabase.GetAllAssetPaths();
        for (int i = 0; i < paths.Length; i++)
        {
            try
            {
                var cancel = EditorUtility.DisplayCancelableProgressBar($"检查引用中({i}/{paths.Length})", paths[i], (float)i / paths.Length);
                if (cancel)
                {
                    EditorUtility.ClearProgressBar();
                    return;
                }
                if (paths[i].StartsWith("Assets/") && !AssetDatabase.IsValidFolder(paths[i]))
                {
                    dependencies = AssetDatabase.GetDependencies(paths[i])
                        .Where(p => p != paths[i] && p.Equals(path))
                        .ToArray();
                    if (dependencies.Length > 0)
                    {
                        referencesInfo.AppendLine();
                        referencesInfo.Append(++referencesIndex);
                        referencesInfo.Append('.');
                        referencesInfo.Append(paths[i]);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("路径异常:{0}\n错误详情:{1}", paths[i], e.Message));
            }
        }
        EditorUtility.ClearProgressBar();

        Debug.Log($"<b>Path:</b>{path} \n<b>依赖:</b>{dependenciesInfo.ToString()} \n<b>引用:</b>{referencesInfo.ToString()}");
    }
}
