using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XOR.Services;

namespace XOR
{
    internal class ModuleSelector : PopupWindowContent
    {
        internal static ModuleSelector GetWindow()
        {
            ModuleSelector selector = new ModuleSelector();
            PopupWindow.Show(GUILayoutUtility.GetLastRect(), selector);
            return selector;
        }

        private string searchText = string.Empty;
        private Program program;

        public void SetProgram(Program program)
        {
            this.program = program;
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
                RenderStructured();
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
        void ss()
        {

        }

        void RenderSearch()
        {
            GUILayout.Label("Search", "HeaderButton", GUILayout.Height(20f));
        }
        void RenderStructured()
        {
            GUILayout.Label("Component", "HeaderButton", GUILayout.Height(20f));
        }

        class Path
        {
            public string name;
            public string path;
            public Path parent;
            public Dictionary<string, Path> childs;
            public Dictionary<string, Statement> members;

            public Path()
            {
                this.name = string.Empty;
                this.path = string.Empty;
                this.childs = new Dictionary<string, Path>();
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
                    string name = statement.module.Substring(this.path.Length).Split('/')[0];
                    string path = this.path + "/" + name;

                    Path child;
                    if (!childs.TryGetValue(name, out child))
                    {
                        child = new Path();
                        child.parent = this;
                        child.name = name;
                        child.path = path;
                        childs.Add(name, child);
                    }

                    child.AddStatement(statement);
                }
            }
        }
    }
}
