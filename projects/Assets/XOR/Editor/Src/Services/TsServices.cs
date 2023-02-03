using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public Action<string, string, bool> Start;
        public Action Stop;

        /// <summary>文件更新: 修改/新增/删除 </summary>
        public Action<string> FileChanged;
    }

    public class Program
    {
        /// <summary>编译错误数量 </summary>
        public int errors;
        /// <summary>编译源文件数量 </summary>
        public int scripts;
        /// <summary>当前状态  </summary>
        public ProgramState state;
        /// <summary>编译状态值  </summary>
        public string compile;
        /// <summary>项目根路径 </summary>
        public string root;
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
            this.errors = 0;
            this.state = ProgramState.Pending;
            this.compile = string.Empty;
            this.Statements.Clear();
        }
    }

    public enum ProgramState
    {
        Pending,
        Scanning,
        Compiling,
        Analyzing,
        Allocating,
        Completed,
        Error,
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
        /// <summary>声明类所在模块/文件路径 </summary>
        public string module;

        public string path;
        public int line;
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

        /// <summary>
        /// 数字类型时, 此字段作为数字范围选项
        /// </summary>
        public Tuple<float, float> valueRange;
        /// <summary>
        /// 为enum类型时, 此字段作为所有可选值(实际类型为int或string)
        /// </summary>
        public Dictionary<string, object> valueEnum;

        public void SetRange(float left, float right)
        {
            this.valueRange = new Tuple<float, float>(left, right);
        }
        public void AddEnum(string key, object value)
        {
            if (this.valueEnum == null)
            {
                this.valueEnum = new Dictionary<string, object>();
            }
            else
            {
                this.valueEnum.Remove(key);
            }
            this.valueEnum.Add(key, value);
        }

        private string _tooltip;
        public string BuildTooltip(bool force = false)
        {
            if (force || this._tooltip == null)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("Name:\t{0}", this.name);
                builder.AppendLine();
                builder.AppendFormat("Type:\t{0}", this.valueType?.FullName ?? "NULL");
                if (this.valueRange != null)
                {
                    builder.AppendLine();
                    builder.AppendFormat("Range:\t[{0}, {1}]", this.valueRange.Item1, this.valueRange.Item2);
                }
                if (this.valueEnum != null && this.valueEnum.Count > 0)
                {
                    builder.AppendLine();
                    builder.Append("Enum:");
                    bool isValueEnum = this.valueEnum.Keys.FirstOrDefault(k => k != this.valueEnum[k]?.ToString()) == null;
                    if (isValueEnum)
                    {
                        foreach (var e in this.valueEnum)
                        {
                            builder.AppendLine();
                            builder.AppendFormat("\t[{0}]", e.Key);
                        }
                    }
                    else
                    {
                        foreach (var e in this.valueEnum)
                        {
                            builder.AppendLine();
                            builder.AppendFormat("\t[{0}, {1}]", e.Key, e.Value);
                        }
                    }
                }
                if (this.defaultValue != null)
                {
                    builder.AppendLine();
                    if (this.defaultValue is Array)
                    {
                        builder.Append("Default:\t[");
                        Array array = (Array)this.defaultValue;
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (i > 0) builder.Append(", ");
                            builder.Append(array.GetValue(i));
                        }
                        builder.Append("]");
                    }
                    else
                    {
                        builder.AppendFormat("Default:\t{0}", this.defaultValue);
                    }
                }
                this._tooltip = builder.ToString();
            }
            return this._tooltip;
        }
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
