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
    }

    public class Program
    {
        public int error;
        public bool compiling;
        public readonly Dictionary<string, Statement> statements;

        public Program()
        {
            this.statements = new Dictionary<string, Statement>();
        }

        public Statement GetStatement(string guid, bool create = true)
        {
            Statement statement;
            this.statements.TryGetValue(guid, out statement);
            return statement;
        }
        public void AddStatement(Statement statement)
        {
            this.RemoveStatement(statement);
            this.statements.Add(statement.guid, statement);
        }
        public void RemoveStatement(Statement statement) => RemoveStatement(statement.guid);
        public void RemoveStatement(string guid)
        {
            this.statements.Remove(guid);
        }

        public void r()
        {
            this.error = 0;
            this.compiling = false;
            this.statements.Clear();
        }
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
        public readonly Dictionary<string, EnumPropertyDeclaration> properties;
        public EnumDeclaration()
        {
            this.properties = new Dictionary<string, EnumPropertyDeclaration>();
        }
        public string[] GetNames()
        {
            return properties.Keys.ToArray();
        }
        public EnumPropertyDeclaration[] GetProperties()
        {
            return properties.Values.ToArray();
        }
        public EnumPropertyDeclaration GetProperty(string propertyName)
        {
            EnumPropertyDeclaration property;
            this.properties.TryGetValue(propertyName, out property);
            return property;
        }
        public void AddProperty(EnumPropertyDeclaration property)
        {
            this.RemoveProperty(property);
            this.properties.Add(property.name, property);
        }
        public void RemoveProperty(EnumPropertyDeclaration property) => RemoveProperty(property.name);
        public void RemoveProperty(string propertyName)
        {
            this.properties.Remove(propertyName);
        }
    }
    public class TypeDeclaration : Statement
    {
        /// <summary>类路由值 </summary>
        public string route;

        /// <summary>成员信息 </summary>
        public readonly Dictionary<string, PropertyDeclaration> properties;

        public TypeDeclaration()
        {
            this.properties = new Dictionary<string, PropertyDeclaration>();
        }
        public string[] GetNames()
        {
            return properties.Keys.ToArray();
        }
        public PropertyDeclaration[] GetProperties()
        {
            return properties.Values.ToArray();
        }

        public PropertyDeclaration GetProperty(string propertyName)
        {
            PropertyDeclaration property;
            this.properties.TryGetValue(propertyName, out property);
            return property;
        }
        public void AddProperty(PropertyDeclaration property)
        {
            this.RemoveProperty(property);
            this.properties.Add(property.name, property);
        }
        public void RemoveProperty(PropertyDeclaration property) => RemoveProperty(property.name);
        public void RemoveProperty(string propertyName)
        {
            this.properties.Remove(propertyName);
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
