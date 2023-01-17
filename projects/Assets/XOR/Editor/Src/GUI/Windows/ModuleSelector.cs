using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XOR.Services;

namespace XOR
{
    internal class ModuleSelector : PopupWindowContent
    {
        internal static void GetWindow(Program program)
        {
            ModuleSelector selector = new ModuleSelector();
            selector.SetProgram(program);
            PopupWindow.Show(GUILayoutUtility.GetLastRect(), selector);
        }

        private string searchText = string.Empty;
        private Program program;
        private StatementPath root;
        private StatementPath current;

        public void SetProgram(Program program)
        {
            this.program = program;
            if (program != null)
            {
                this.current = this.root = new StatementPath();
                this.root.path = Path.GetDirectoryName(UnityEngine.Application.dataPath).Replace("\\", "/");
                foreach (var statement in program.Statements)
                {
                    this.root.AddStatement(statement.Value);
                }
            }
            else
            {
                this.root = null;
                this.current = null;
            }
        }

        public override void OnGUI(Rect rect)
        {
            string newContent = GUILayout.TextField(searchText, "SearchTextField");
            if (!searchText.Equals(newContent))
            {
                searchText = newContent;
                SearchData();
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

        void SearchData()
        {
            if (program == null || string.IsNullOrEmpty(searchText))
            {
                return;
            }
        }

        void RenderSearch()
        {
            GUILayout.Label("Search", "HeaderButton", GUILayout.Height(20f));
        }
        void RenderStatementPath()
        {
            if (RenderHeader(this.current != null && string.IsNullOrEmpty(this.current.path) ? "Component" : this.current.path, this.current != null && this.current.parent != null) &&
                this.current != null && this.current.parent != null
            )
            {
                this.current = this.current.parent;
            }

            if (this.current == null)
            {
                return;
            }


            foreach (var child in this.current.childs)
            {
                if (RenderColumn(child.Key, true))
                {
                    this.current = child.Value;
                }
            }
            foreach (var member in this.current.members)
            {
                if (RenderColumn(member.Key, false))
                {

                }
            }
        }

        bool RenderHeader(string text, bool prefix)
        {
            bool click = false;
            GUILayout.BeginHorizontal("HeaderButton", GUILayout.Height(20f));
            click |= GUILayout.Button(prefix ? "〈" : string.Empty, Skin.label, GUILayout.Width(10f));
            click |= GUILayout.Button(text, Skin.label);
            GUILayout.EndHorizontal();
            return click;
        }
        bool RenderColumn(string text, bool suffix)
        {
            bool click = false;
            GUILayout.BeginHorizontal("SearchModeFilter", GUILayout.Height(20f));
            click |= GUILayout.Button(text, Skin.label);
            click |= GUILayout.Button(suffix ? "〉" : string.Empty, Skin.label, GUILayout.Width(10f));
            GUILayout.EndHorizontal();
            return click;
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

            public void AddStatement(Statement statement)
            {
                if (string.IsNullOrEmpty(statement.module) || statement.module.Equals(this.path))
                {
                    this.members.Remove(statement.name);
                    this.members.Add(statement.name, statement);
                }
                else
                {
                    try
                    {
                        string name = statement.module.Substring(this.path.Length).Split('/')[0];
                        string path = string.IsNullOrEmpty(this.path) ? name : (this.path + "/" + name);

                        StatementPath child;
                        if (!childs.TryGetValue(name, out child))
                        {
                            child = new StatementPath();
                            child.parent = this;
                            child.name = name;
                            child.path = path;
                            childs.Add(name, child);
                        }

                        child.AddStatement(statement);
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
