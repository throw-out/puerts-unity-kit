using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using JSON = Newtonsoft.Json.JsonConvert;

namespace XOR.Services
{
    public class ProgramCached : IProgram
    {
        public Dictionary<string, Statement> Statements { get; private set; }
        public ProgramCached()
        {
            Statements = new Dictionary<string, Statement>();
        }
        public Statement GetStatement(string guid)
        {
            if (guid == null)
                return null;
            Statement statement;
            this.Statements.TryGetValue(guid, out statement);
            return statement;
        }
        public void AddStatement(Statement statement)
        {
            if (Statements.TryGetValue(statement.guid, out var old) && statement.version == old.version)
            {
                //return;
            }
            var copySelf = statement.Copy();
            copySelf.path = copySelf.GetLocalPath();
            copySelf.module = copySelf.GetLocalModule();
            copySelf.parent = this;

            this.RemoveStatement(copySelf.guid);
            this.Statements.Add(copySelf.guid, copySelf);
            //写入缓存
            if (!WriteCacheToRoot(copySelf))
            {
                this.RemoveStatement(copySelf.guid);
            }
        }
        public void RemoveStatement(string guid)
        {
            this.Statements.Remove(guid);
        }
        public string GetLocalPath(string path)
        {
            return path;
        }


        private static string _cacheRoot;
        private static string CacheRoot
        {
            get
            {
                if (string.IsNullOrEmpty(_cacheRoot))
                {
                    _cacheRoot = Path.Combine(UnityEngine.Application.dataPath, "../Library/XORCache");

                }
                return _cacheRoot;
            }
        }

        /// <summary>
        /// 创建缓存目录
        /// </summary>
        /// <param name="force">是否强制重新创建目录</param>
        public static void CreateRoot(bool force = false)
        {
            if (Directory.Exists(CacheRoot))
            {
                if (!force)
                    return;
                Directory.Delete(CacheRoot, true);
            }
            Directory.CreateDirectory(CacheRoot);
        }
        /// <summary>
        /// 删除缓存目录
        /// </summary>
        public static void DeleteRoot()
        {
            if (!Directory.Exists(CacheRoot))
                return;
            Directory.Delete(CacheRoot, true);
        }
        /// <summary>
        /// 从缓存中创建实例对象
        /// </summary>
        /// <returns></returns>
        public static ProgramCached CreateProgramFormRoot()
        {
            ProgramCached result = new ProgramCached();
            if (!Directory.Exists(CacheRoot))
                return result;

            foreach (var file in Directory.GetFiles(CacheRoot))
            {
                if (ToStatement(File.ReadAllBytes(file), out var statement))
                {
                    statement.parent = result;
                    result.Statements[statement.guid] = statement;
                }
            }
            return result;
        }
        private static bool WriteCacheToRoot(Statement statement)
        {
            CreateRoot();
            try
            {
                File.WriteAllBytes(
                    Path.Combine(CacheRoot, statement.guid),
                    FromStatement(statement)
                );
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError("{0}:{1}", statement.guid, e);
            }
            return false;
        }
        private static byte[] FromStatement(Statement statement)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                if (statement is TypeDeclaration td)
                {
                    writer.Write((byte)1);
                    writer.Write(JSON.SerializeObject(TypeDeclarationCached.From(td)));
                }
                else
                {
                    writer.Write((byte)0);
                    writer.Write(JSON.SerializeObject(statement));
                }
                return stream.ToArray();
            }
        }
        private static bool ToStatement(byte[] data, out Statement statement)
        {
            using (var stream = new MemoryStream(data))
            {
                var reader = new BinaryReader(stream);

                try
                {
                    byte type = reader.ReadByte();
                    string json = reader.ReadString();
                    if (type == 1)
                    {
                        var cached = JSON.DeserializeObject<TypeDeclarationCached>(json);
                        statement = cached != null ? TypeDeclarationCached.From(cached) : null;
                    }
                    else
                    {
                        statement = JSON.DeserializeObject<EnumDeclaration>(json);
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
                statement = null;
                return false;
            }
        }
    }

    public class TypeDeclarationCached
    {
        public string guid;
        public string source;
        public string version;
        public string name;
        public string module;
        public string path;
        public int line;
        public string route;
        public Dictionary<string, PropertyDeclarationCached> Properties { get; private set; }
        public Dictionary<string, List<MethodDeclarationCached>> Methods { get; private set; }
        public TypeDeclarationCached()
        {
            this.Properties = new Dictionary<string, PropertyDeclarationCached>();
            this.Methods = new Dictionary<string, List<MethodDeclarationCached>>();
        }

        public static TypeDeclarationCached From(TypeDeclaration declaration)
        {
            var result = new TypeDeclarationCached()
            {
                guid = declaration.guid,
                source = declaration.source,
                version = declaration.version,
                name = declaration.name,
                module = declaration.module,
                path = declaration.path,
                line = declaration.line,
                route = declaration.route,
            };
            if (declaration.Properties != null)
            {
                foreach (var property in declaration.Properties)
                {
                    result.Properties[property.Key] = PropertyDeclarationCached.From(property.Value);
                }
            }
            if (declaration.Methods != null)
            {
                foreach (var method in declaration.Methods)
                {
                    result.Methods[method.Key] = method.Value.Select(m => MethodDeclarationCached.From(m)).ToList();
                }
            }
            return result;
        }
        public static TypeDeclaration From(TypeDeclarationCached cached)
        {
            var result = new TypeDeclaration()
            {
                guid = cached.guid,
                source = cached.source,
                version = cached.version,
                name = cached.name,
                module = cached.module,
                path = cached.path,
                line = cached.line,
                route = cached.route,
            };
            if (cached.Properties != null)
            {
                foreach (var property in cached.Properties)
                {
                    var declaration = PropertyDeclarationCached.From(property.Value);
                    if (declaration != null)
                    {
                        result.Properties[property.Key] = declaration;
                    }
                }
            }
            if (cached.Methods != null)
            {
                foreach (var method in cached.Methods)
                {
                    var list = method.Value.Select(m => MethodDeclarationCached.From(m)).Where(d => d != null).ToList();
                    if (list.Count > 0)
                    {
                        result.Methods[method.Key] = list;
                    }
                }
            }
            return result;
        }
    }
    public class PropertyDeclarationCached
    {
        public string name;
        public string valueType;
        public ObjectSerialize defaultValue;
        public Tuple<float, float> valueRange;
        public Dictionary<string, ObjectSerialize> valueEnum;
        public Dictionary<string, string> valueReferences;
        public static PropertyDeclarationCached From(PropertyDeclaration declaration)
        {
            var defaultValue = ObjectSerialize.FromObject(declaration.defaultValue);
            Dictionary<string, ObjectSerialize> valueEnum = null;
            if (declaration.valueEnum != null)
            {
                valueEnum = new Dictionary<string, ObjectSerialize>();
                foreach (var pair in declaration.valueEnum)
                {
                    valueEnum[pair.Key] = ObjectSerialize.FromObject(pair.Value);
                }
            }
            return new PropertyDeclarationCached()
            {
                name = declaration.name,
                valueType = declaration.valueType.FullName,
                defaultValue = defaultValue,
                valueRange = declaration.valueRange,
                valueEnum = valueEnum,
                valueReferences = declaration.valueReferences,
            };
        }
        public static PropertyDeclaration From(PropertyDeclarationCached cached)
        {
            Type valueType = Type.GetType(cached.valueType, false);
            if (valueType == null)
                return null;
            var defaultValue = cached.defaultValue?.ToObject();
            Dictionary<string, object> valueEnum = null;
            if (cached.valueEnum != null)
            {
                valueEnum = new Dictionary<string, object>();
                foreach (var pair in cached.valueEnum)
                {
                    valueEnum[pair.Key] = pair.Value?.ToObject();
                }
            }

            return new PropertyDeclaration()
            {
                name = cached.name,
                valueType = valueType,
                defaultValue = defaultValue,
                valueRange = cached.valueRange,
                valueEnum = valueEnum,
                valueReferences = cached.valueReferences,
            };
        }
    }
    public class MethodDeclarationCached
    {
        public string name;
        public string returnType;
        public string[] parameterTypes;

        public static MethodDeclarationCached From(MethodDeclaration declaration)
        {
            return new MethodDeclarationCached()
            {
                name = declaration.name,
                returnType = declaration.returnType?.FullName,
                parameterTypes = declaration.parameterTypes?.Select(t => t.FullName)?.ToArray(),
            };
        }
        public static MethodDeclaration From(MethodDeclarationCached cached)
        {
            Type returnType = !string.IsNullOrEmpty(cached.returnType) ? Type.GetType(cached.returnType, false) : null;
            IEnumerable<Type> parameterTypes = cached.parameterTypes?.Select(n => Type.GetType(n, false));
            if (parameterTypes != null && parameterTypes.Any(t => t == null))
                return null;
            return new MethodDeclaration()
            {
                name = cached.name,
                returnType = returnType,
                parameterTypes = parameterTypes?.ToArray(),
            };
        }
    }

    public enum ObjectType
    {
        Default,
        Vector2,
        Vector3,
        Color,
        Color32,
        Array,
    }

    public class ObjectSerialize
    {
        public ObjectType type;
        public string valueType;
        public string value;

        public object ToObject()
        {
            if (string.IsNullOrEmpty(value))
                return null;
            object result = null;
            switch (type)
            {
                case ObjectType.Vector2:
                    result = Serializer.ToData<Vector2>(value);
                    break;
                case ObjectType.Vector3:
                    result = Serializer.ToData<Vector3>(value);
                    break;
                case ObjectType.Color:
                    result = Serializer.ToData<Color>(value);
                    break;
                case ObjectType.Color32:
                    result = Serializer.ToData<Color32>(value);
                    break;
                case ObjectType.Array:
                    {
                        var list = JSON.DeserializeObject<List<ObjectSerialize>>(value);
                        var firstValue = list.FirstOrDefault(v => v != null)?.ToObject();
                        if (firstValue != null)
                        {
                            Array array = Array.CreateInstance(firstValue.GetType(), list.Count);
                            for (int i = 0; i < array.Length; i++)
                            {
                                array.SetValue(list[i]?.ToObject(), i);
                            }
                        }
                    }
                    break;
                default:
                    Type _t = !string.IsNullOrEmpty(valueType) ? Type.GetType(valueType, false) : null;
                    if (_t != null)
                    {
                        result = JSON.DeserializeObject(value, _t);
                    }
                    else
                    {
                        result = JSON.DeserializeObject<object>(value);
                    }
                    break;
            }
            return result;
        }
        public static ObjectSerialize FromObject(object obj)
        {
            if (obj == null)
                return null;
            ObjectType type = ObjectType.Default;
            string value = null;
            string valueType = null;
            if (obj is Vector2 vec2)
            {
                type = ObjectType.Vector2;
                value = Serializer.ToString(vec2);
            }
            else if (obj is Vector3 vec3)
            {
                type = ObjectType.Vector3;
                value = Serializer.ToString(vec3);
            }
            else if (obj is Color color)
            {
                type = ObjectType.Color;
                value = Serializer.ToString(color);
            }
            else if (obj is Color32 color32)
            {
                type = ObjectType.Color32;
                value = Serializer.ToString(color32);
            }
            else if (obj is Array array)
            {
                List<ObjectSerialize> list = new List<ObjectSerialize>();
                for (int i = 0; i < array.Length; i++)
                {
                    list.Add(ObjectSerialize.FromObject(array.GetValue(i)));
                }
                type = ObjectType.Array;
                value = JSON.SerializeObject(list);
            }
            else
            {
                valueType = obj?.GetType().FullName;
                value = JSON.SerializeObject(obj);
            }
            if (string.IsNullOrEmpty(value))
                return null;
            return new ObjectSerialize()
            {
                type = type,
                value = value,
                valueType = valueType,
            };
        }
    }
}