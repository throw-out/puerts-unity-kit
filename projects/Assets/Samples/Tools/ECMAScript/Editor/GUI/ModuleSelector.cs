using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Puerts.Editor
{
    public class ModuleSelector : EditorWindow
    {
        public static ModuleSelector Get()
        {
            var window = EditorWindow.GetWindow<ModuleSelector>(true, nameof(ModuleSelector), true);
            window.ShowUtility();
            window.Focus();
            return window;
        }


        private Vector2 scrollPosition;

        private static readonly int[] pages = new int[] { 50, 100, 200 };
        private static readonly string[] pagesTexts = pages.Select(p => $"{p}").ToArray();

        private static string[] ModuleCache
        {
            get
            {
                string data = EditorPrefs.GetString("ECMAScript.moduleCache");
                return data != null ? data.Split('|') : null;
            }
            set
            {
                string data = value != null ? string.Join("|", value) : string.Empty;
                EditorPrefs.SetString("ECMAScript.moduleCache", data);
            }
        }

        private int page = pages[0];
        private int index = 0;
        private int maxIndex = 0;
        private string searchText;
        private bool isNested;

        private NamespaceInfo[] all;
        private NamespaceInfo[] current;

        private Action<IEnumerable<string>> callback;
        public void Once(Action<IEnumerable<string>> func)
        {
            this.callback = func;
        }
        private void OnEnable()
        {
            if (this.all == null)
            {
                var allTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                                from type in assembly.GetExportedTypes()
                                select type).Distinct();

                this.all = allTypes
                    .Select(type => new NamespaceInfo(type))
                    .Where(info => !string.IsNullOrEmpty(info.Name))
                    .GroupBy(info => info.Name)
                    .ToDictionary(group => group.Key, group => group.Cast<NamespaceInfo>())
                    .Select(pair => new NamespaceInfo(pair.Key, pair.Value.All(info => info.IsNested)))
                    .ToArray();
                this.ReadCache();
            }
            this.isNested = false;
            this.searchText = string.Empty;
            DOSearch();
        }
        private void OnDisable()
        {
            this.callback = null;
        }
        private void OnGUI()
        {
            RemderSearch();
            RenderPage();
            RenderNamespaceContent();

            if (GUILayout.Button("Confirm"))
            {
                SaveAndCallback();
            }
        }

        void RemderSearch()
        {
            GUILayout.BeginHorizontal("HelpBox");
            if (GUILayout.Toggle(this.isNested, "NestedType", GUILayout.Width(100f)) != this.isNested)
            {
                this.isNested = !this.isNested;
                DOSearch();
            }

            var newText = GUILayout.TextField(this.searchText, "SearchTextField");
            if (newText != this.searchText)
            {
                this.searchText = newText;
                DOSearch();
            }
            GUILayout.EndHorizontal();
        }
        void RenderPage()
        {
            GUILayout.BeginHorizontal("HelpBox");
            int selectCount = GetSelected(false);
            bool selectAll = selectCount > 0 && selectCount < this.current.Length ?
                GUILayout.Toggle(false, "Select", "ShurikenToggleMixed", GUILayout.Width(100f)) :
                GUILayout.Toggle(selectCount > 0, "Select", GUILayout.Width(100f));
            if (!selectAll && selectCount >= this.current.Length || selectAll && selectCount < this.current.Length)
            {
                for (int i = 0; i < this.current.Length; i++)
                {
                    this.current[i].Selected = selectAll;
                }
            }
            GUILayout.FlexibleSpace();

            using (new EditorGUI.DisabledScope(this.index <= 0))
            {
                if (GUILayout.Button("<<"))
                {
                    SetIndex(0);
                }
                if (GUILayout.Button("<"))
                {
                    SetIndex(this.index - 1);
                }
            }
            GUILayout.Label(this.maxIndex > 0 ? $"{this.index + 1}/{this.maxIndex}" : "0/0");
            using (new EditorGUI.DisabledScope(this.index >= this.maxIndex - 1))
            {
                if (GUILayout.Button(">"))
                {
                    SetIndex(this.index + 1);
                }
                if (GUILayout.Button(">>"))
                {
                    SetIndex(this.maxIndex - 1);
                }
            }
            GUILayout.Space(10f);
            int pIndex = Array.IndexOf(pages, this.page);
            int newPIndex = GUILayout.SelectionGrid(pIndex, pagesTexts, pages.Length);
            if (pIndex != newPIndex)
            {
                SetPage(pages[newPIndex]);
            }
            GUILayout.EndHorizontal();
        }
        void RenderNamespaceContent()
        {
            int startIndex = this.page * this.index,
                endIndex = Mathf.Min(this.current.Length, startIndex + this.page);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            for (int i = startIndex; i < endIndex && i >= 0; i++)
            {
                var info = this.current[i];
                if (GUILayout.Toggle(info.Selected, info.Name) != info.Selected)
                {
                    info.Selected = !info.Selected;
                }
            }
            GUILayout.EndScrollView();
        }

        void SetIndex(int index)
        {
            this.index = index;
            this.scrollPosition = Vector2.zero;
        }
        void SetPage(int page)
        {
            this.page = page;
            this.maxIndex = Mathf.CeilToInt(this.current.Length / (float)this.page);
            this.index = Mathf.Min(0, this.maxIndex - 1);
            this.scrollPosition = Vector2.zero;
        }
        void DOSearch()
        {
            if (string.IsNullOrEmpty(this.searchText))
            {
                this.current = this.all;
                if (!isNested) this.current = this.current
                    .Where(o => !o.IsNested)
                    .ToArray();
            }
            else
            {
                string text = this.searchText.ToLower();
                this.current = this.all
                    .Where(info => (this.isNested || !info.IsNested) && info.Name.ToLower().Contains(text))
                    .ToArray();
            }
            this.scrollPosition = Vector2.zero;
            this.maxIndex = Mathf.CeilToInt(this.current.Length / (float)this.page);
            this.index = 0;
        }
        int GetSelected(bool full)
        {
            int count = 0;
            for (int i = 0; i < this.current.Length; i++)
            {
                if (this.current[i].Selected) count++;
                else if (!full && count > 0) break;
            }
            return count;
        }

        void ReadCache()
        {
            string[] cache = ModuleCache;
            if (cache == null || cache.Length == 0)
                return;
            var set = new HashSet<string>(cache);
            foreach (var info in this.all)
            {
                info.Selected = set.Contains(info.Name);
            }
        }
        void SaveAndCallback()
        {
            var selected = this.all
                .Where(info => info.Selected)
                .Select(info => info.Name)
                .ToArray();

            int tipCount = 10;
            string tipContent = $"Selected Namespaces({selected.Length}):\n{string.Join("\n", selected.Length > tipCount ? selected.Take(tipCount) : selected)}";
            if (selected.Length > tipCount) tipContent += "\n......";
            bool confirm = EditorUtility.DisplayDialog("Tip", tipContent, "Confirm", "Cancel");
            if (!confirm)
            {
                return;
            }
            ModuleCache = selected;

            var func = this.callback;
            this.callback = null;
            if (func != null)
            {
                func(selected);
                this.Close();
            }
        }
    }
    class NamespaceInfo
    {
        public string Name { get; private set; }
        public bool Selected;
        public bool IsNested { get; private set; }

        public NamespaceInfo(Type type)
        {
            string fullName = type.FullName != null ?
                type.FullName.Replace("+", ".").Replace("`", "$") :
                string.Empty;
            int lastIndex = fullName.LastIndexOf(".");

            this.Name = lastIndex >= 0 ? fullName.Substring(0, lastIndex) : string.Empty;
            this.IsNested = type.IsNested;
        }
        public NamespaceInfo(string name, bool isNested)
        {
            this.Name = name;
            this.IsNested = isNested;
        }
    }
}