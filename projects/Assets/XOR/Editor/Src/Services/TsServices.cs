using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XOR.Services
{
    public class CSharpInterfaces
    {
        public Action<Program> SetProgram;
        public Action<ThreadWorker> SetWorker;
    }
    public class TSInterfaces
    {
        public Action<string, string> Start;
        public Action Stop;

        /// <summary>文件更新: 修改/新增/删除 </summary>
        public Action<string> FileChanged;
    }

    public class Program
    {
        /// <summary>编译错误数量 </summary>
        public int error;
        /// <summary>编译源文件数量 </summary>
        public int source;
        /// <summary>当前状态  </summary>
        public ProgramState state;
        public Dictionary<string, Statement> Statements { get; private set; }

        public Program()
        {
            this.Statements = new Dictionary<string, Statement>();
        }

        public Statement GetStatement(string guid, bool create = true)
        {
            Statement statement;
            this.Statements.TryGetValue(guid, out statement);
            return statement;
        }
        public void AddStatement(Statement statement)
        {
            this.RemoveStatement(statement);
            this.Statements.Add(statement.guid, statement);
        }
        public void RemoveStatement(Statement statement) => RemoveStatement(statement.guid);
        public void RemoveStatement(string guid)
        {
            this.Statements.Remove(guid);
        }

        public void Reset()
        {
            this.error = 0;
            this.state = ProgramState.Pending;
            this.Statements.Clear();
        }
    }

    public enum ProgramState
    {
        Pending,
        Error,
        Compiling,
        Compiled,
    }

    public abstract class Statement
    {
        /// <summary>类Id(全局唯一标识符) </summary>
        public string guid;
        /// <summary>源文件路径 </summary>
        public string source;
        /// <summary>当前版本信息 </summary>
        public string version;
        /// <summary>声明类类名 </summary>
        public string name;
        /// <summary>声明类类名(全路径) </summary>
        public string fullName;
    }

    public class EnumDeclaration : Statement
    {
        public Dictionary<string, EnumPropertyDeclaration> Properties { get; private set; }
        public EnumDeclaration()
        {
            this.Properties = new Dictionary<string, EnumPropertyDeclaration>();
        }
        public string[] GetNames()
        {
            return Properties.Keys.ToArray();
        }
        public EnumPropertyDeclaration[] GetProperties()
        {
            return Properties.Values.ToArray();
        }
        public EnumPropertyDeclaration GetProperty(string propertyName)
        {
            EnumPropertyDeclaration property;
            this.Properties.TryGetValue(propertyName, out property);
            return property;
        }
        public void AddProperty(EnumPropertyDeclaration property)
        {
            this.RemoveProperty(property);
            this.Properties.Add(property.name, property);
        }
        public void RemoveProperty(EnumPropertyDeclaration property) => RemoveProperty(property.name);
        public void RemoveProperty(string propertyName)
        {
            this.Properties.Remove(propertyName);
        }
    }
    public class TypeDeclaration : Statement
    {
        /// <summary>类路由值 </summary>
        public string route;

        /// <summary>成员信息 </summary>
        public Dictionary<string, PropertyDeclaration> Properties { get; private set; }

        public TypeDeclaration()
        {
            this.Properties = new Dictionary<string, PropertyDeclaration>();
        }
        public string[] GetNames()
        {
            return Properties.Keys.ToArray();
        }
        public PropertyDeclaration[] GetProperties()
        {
            return Properties.Values.ToArray();
        }

        public PropertyDeclaration GetProperty(string propertyName)
        {
            PropertyDeclaration property;
            this.Properties.TryGetValue(propertyName, out property);
            return property;
        }
        public void AddProperty(PropertyDeclaration property)
        {
            this.RemoveProperty(property);
            this.Properties.Add(property.name, property);
        }
        public void RemoveProperty(PropertyDeclaration property) => RemoveProperty(property.name);
        public void RemoveProperty(string propertyName)
        {
            this.Properties.Remove(propertyName);
        }
    }
    public class PropertyDeclaration
    {
        /// <summary>字段名 </summary>
        public string name;
        /// <summary>字段值类型  </summary>
        public Type valueType;
        /// <summary>字段默认值  </summary>
        public object defaultValue;
    }
    public class EnumPropertyDeclaration
    {
        /// <summary>字段名 </summary>
        public string name;
        /// <summary>字段值 </summary>
        public string value;
        /// <summary>此属性是否可用 </summary>
        public bool active;
    }
}
