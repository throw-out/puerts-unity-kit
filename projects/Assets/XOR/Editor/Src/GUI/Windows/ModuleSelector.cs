using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XOR.Services;

namespace XOR
{
    internal class ModuleSelector : PopupWindowContent
    {
        const int IconWidth = 14;
        const int HeaderHeight = 20;

        internal static void Show(IProgram program, Action<string> callback)
        {
            Show(program, callback, GUILayoutUtility.GetLastRect());
        }
        internal static void Show(IProgram program, Action<string> callback, Rect activatorRect)
        {
            ModuleSelector selector = new ModuleSelector();
            selector.SetCallback(callback);
            selector.SetProgram(program);
            PopupWindow.Show(activatorRect, selector);
        }
        private StatementPath root;
        private StatementPath current;

        private string searchText = string.Empty;
        private List<Statement> searchReuslts;

        private Action<string> callback;

        public override void OnGUI(Rect rect)
        {
            string newContent = GUILayout.TextField(searchText, "SearchTextField");
            if (!searchText.Equals(newContent))
            {
                searchText = newContent;
                SearchStatements();
            }
            GUILayout.BeginVertical();
            if (string.IsNullOrEmpty(searchText))
            {
                RenderStatementPath();
            }
            else
            {
                RenderSearch();
            }
            GUILayout.EndVertical();
        }

        void RenderSearch()
        {
            GUILayout.Label("Search", "HeaderButton", GUILayout.Height(20f));
            if (this.searchReuslts != null)
            {
                foreach (Statement memeber in this.searchReuslts)
                {
                    if (RenderColumn(memeber.name, true, false, memeber.module))
                    {
                        Complete(memeber);
                    }
                }
            }
        }
        void RenderStatementPath()
        {
            bool toUpper = this.current != null && this.current.parent != null;
            if (RenderHeader(this.current != null && string.IsNullOrEmpty(this.current.path) ? "root" : this.current.name, toUpper, this.current?.path) && toUpper)
            {
                this.current = this.current.parent;
            }

            if (this.current == null)
            {
                return;
            }

            foreach (var child in this.current.childs.Values)
            {
                if (RenderColumn(child.name, false, true, child.path))
                {
                    this.current = child;
                }
            }
            foreach (var member in this.current.members.Values)
            {
                if (RenderColumn(member.name, true, false, member.module))
                {
                    Complete(member);
                }
            }
        }
        bool RenderHeader(string text, bool prefix, string tooltip = null)
        {
            bool click = false;
            GUILayout.BeginHorizontal("HeaderButton", GUILayout.Height(HeaderHeight));
            //click |= GUILayout.Button(prefix ? "◀" : string.Empty, Skin.label, GUILayout.Width(IconWidth));
            click |= GUILayout.Button(string.Empty, prefix ? Skin.arrawLeft : Skin.label, GUILayout.Width(IconWidth));
            if (tooltip != null)
            {
                click |= GUILayout.Button(new GUIContent(text, tooltip), Skin.label);
            }
            else
            {
                click |= GUILayout.Button(text, Skin.label);
            }
            GUILayout.EndHorizontal();
            return click;
        }
        bool RenderColumn(string text, bool prefix, bool suffix, string tooltip = null)
        {
            bool click = false;
            GUILayout.BeginHorizontal("SearchModeFilter", GUILayout.Height(HeaderHeight));
            click |= GUILayout.Button(prefix ? "#" : string.Empty, Skin.label, GUILayout.Width(IconWidth));
            if (tooltip != null)
            {
                click |= GUILayout.Button(new GUIContent(text, tooltip), Skin.label);
            }
            else
            {
                click |= GUILayout.Button(text, Skin.label);
            }
            //click |= GUILayout.Button(suffix ? "▶" : string.Empty, Skin.label, GUILayout.Width(IconWidth));
            click |= GUILayout.Button(string.Empty, suffix ? Skin.arrawRight : Skin.label, GUILayout.Width(IconWidth));
            GUILayout.EndHorizontal();
            return click;
        }

        void SetCallback(Action<string> callback)
        {
            this.callback = callback;
        }
        void SetProgram(IProgram program)
        {
            if (program != null)
            {
                this.current = this.root = new StatementPath();
                foreach (var statement in program.Statements)
                {
                    string path = statement.Value.module;
                    if (path.Contains("/") || path.Contains("\\"))
                    {
                        path = program.GetLocalPath(path);
                    }
                    this.root.AddStatement(path.Replace("\\", "/"), statement.Value);
                }
                this.root.ShortestPath();
            }
            else
            {
                this.root = null;
                this.current = null;
            }
        }
        void Complete(Statement statement)
        {
            if (callback != null) callback(statement.guid);
            EditorWindow.focusedWindow?.Close();
        }
        void SearchStatements()
        {
            this.current = this.root;
            if (string.IsNullOrEmpty(searchText) || this.root == null)
            {
                this.searchReuslts = null;
                return;
            }
            string content = searchText.ToLower();
            this.searchReuslts = new List<Statement>();
            this.root.Search((o) => o.name.ToLower().Contains(content) /**|| o.module.Contains(content)*/, this.searchReuslts);
        }

        class StatementPath
        {
            public string name;
            public string path;
            public StatementPath parent;
            public Dictionary<string, StatementPath> childs;
            public Dictionary<string, Statement> members;

            public StatementPath()
            {
                this.name = string.Empty;
                this.path = string.Empty;
                this.childs = new Dictionary<string, StatementPath>();
                this.members = new Dictionary<string, Statement>();
            }

            public void Search(Func<Statement, bool> match, List<Statement> results)
            {
                foreach (Statement member in this.members.Values)
                {
                    if (match(member))
                    {
                        results.Add(member);
                    }
                }
                foreach (StatementPath child in this.childs.Values)
                {
                    child.Search(match, results);
                }
            }
            public void ShortestPath()
            {
                while (childs.Count == 1 && members.Count == 0)
                {
                    StatementPath node = childs.Values.First();
                    this.name += $"/{node.name}";
                    this.path = node.path;
                    childs.Clear();
                    foreach (var nchild in node.childs)
                    {
                        nchild.Value.parent = this;
                        childs.Add(nchild.Key, nchild.Value);
                    }
                    foreach (var nmember in node.members)
                    {
                        members.Add(nmember.Key, nmember.Value);
                    }
                }
                foreach (var child in childs.Values.ToArray())
                {
                    string oldName = child.name;
                    child.ShortestPath();
                    if (!oldName.Equals(child.name))
                    {
                        childs.Remove(oldName);
                        childs.Add(child.name, child);
                    }
                }
            }
            public void AddStatement(string path, Statement statement)
            {
                if (string.IsNullOrEmpty(path) || this.path.Equals(path))
                {
                    this.members.Remove(statement.name);
                    this.members.Add(statement.name, statement);
                }
                else
                {
                    try
                    {
                        string name = path.Substring(this.path.Length).Split('/').First(str => str.Length > 0);

                        StatementPath child;
                        if (!childs.TryGetValue(name, out child))
                        {
                            child = new StatementPath();
                            child.parent = this;
                            child.name = name;
                            child.path = string.IsNullOrEmpty(this.path) ? name : (this.path + "/" + name);
                            childs.Add(name, child);
                        }

                        child.AddStatement(path, statement);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                }
            }
        }
    }
}
