using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XOR.Services
{
    public class TsServicesCached : IProgram
    {
        public Dictionary<string, Statement> Statements { get; private set; }
        public TsServicesCached()
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
        public void AddStatement(Statement statement, bool copy)
        {
            if (copy)
            {
                var copySelf = statement.Copy();
                copySelf.parent = this;
                statement = copySelf;
            }
            AddStatement(statement);
        }
        public void AddStatement(Statement statement)
        {
            if (Statements.TryGetValue(statement.guid, out var old) && statement.version == old.version)
            {
                return;
            }
            var copySelf = statement.Copy();
            copySelf.path = copySelf.GetLocalPath();
            copySelf.module = copySelf.GetLocalModule();
            copySelf.parent = this;

            this.RemoveStatement(copySelf.guid);
            this.Statements.Add(copySelf.guid, copySelf);
            //写入缓存数据
            WriteCacheToRoot(copySelf);
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
        public static TsServicesCached CreateProgramFormRoot()
        {
            TsServicesCached result = new TsServicesCached();
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
        private static void WriteCacheToRoot(Statement statement)
        {
            CreateRoot();
            File.WriteAllBytes(
                Path.Combine(CacheRoot, statement.guid),
                FromStatement(statement)
            );
        }
        private static byte[] FromStatement(Statement statement)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);

                int type = statement is TypeDeclaration ? 1 : 0;
                writer.Write((byte)type);
                writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(statement));

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
                        var cached = Newtonsoft.Json.JsonConvert.DeserializeObject<TypeDeclarationCache>(json);
                        statement = cached != null ? TypeDeclarationCache.From(cached) : null;
                    }
                    else
                    {
                        statement = Newtonsoft.Json.JsonConvert.DeserializeObject<EnumDeclaration>(json);
                    }
                    return true;
                }
                catch (Exception) { }
                statement = null;
                return false;
            }
        }
    }

    public class TypeDeclarationCache
    {
        public string guid;
        public string source;
        public string version;
        public string name;
        public string module;
        public string path;
        public int line;
        public string route;
        public Dictionary<string, PropertyDeclarationCache> Properties { get; private set; }
        public Dictionary<string, List<MethodDeclarationCache>> Methods { get; private set; }
        public TypeDeclarationCache()
        {
            this.Properties = new Dictionary<string, PropertyDeclarationCache>();
            this.Methods = new Dictionary<string, List<MethodDeclarationCache>>();
        }

        public static TypeDeclarationCache From(TypeDeclaration declaration)
        {
            var result = new TypeDeclarationCache()
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
                    result.Properties[property.Key] = PropertyDeclarationCache.From(property.Value);
                }
            }
            if (declaration.Methods != null)
            {
                foreach (var method in declaration.Methods)
                {
                    result.Methods[method.Key] = method.Value.Select(m => MethodDeclarationCache.From(m)).ToList();
                }
            }
            return result;
        }
        public static TypeDeclaration From(TypeDeclarationCache cached)
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
                    var declaration = PropertyDeclarationCache.From(property.Value);
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
                    var list = method.Value.Select(m => MethodDeclarationCache.From(m)).Where(d => d != null).ToList();
                    if (list.Count > 0)
                    {
                        result.Methods[method.Key] = list;
                    }
                }
            }
            return result;
        }
    }
    public class PropertyDeclarationCache
    {
        public string name;
        public string valueType;
        public object defaultValue;
        public Tuple<float, float> valueRange;
        public Dictionary<string, object> valueEnum;
        public Dictionary<string, string> valueReferences;
        public static PropertyDeclarationCache From(PropertyDeclaration declaration)
        {
            return new PropertyDeclarationCache()
            {
                name = declaration.name,
                valueType = declaration.valueType.FullName,
                valueRange = declaration.valueRange,
                valueEnum = declaration.valueEnum,
                valueReferences = declaration.valueReferences,
            };
        }
        public static PropertyDeclaration From(PropertyDeclarationCache cached)
        {
            Type valueType = Type.GetType(cached.valueType, false);
            if (valueType == null)
                return null;
            return new PropertyDeclaration()
            {
                name = cached.name,
                valueType = valueType,
                valueRange = cached.valueRange,
                valueEnum = cached.valueEnum,
                valueReferences = cached.valueReferences,
            };
        }
    }
    public class MethodDeclarationCache
    {
        public string name;
        public string returnType;
        public string[] parameterTypes;

        public static MethodDeclarationCache From(MethodDeclaration declaration)
        {
            return new MethodDeclarationCache()
            {
                name = declaration.name,
                returnType = declaration.returnType.FullName,
                parameterTypes = declaration.parameterTypes.Select(t => t.FullName).ToArray()
            };
        }
        public static MethodDeclaration From(MethodDeclarationCache cached)
        {
            Type returnType = Type.GetType(cached.returnType, false);
            if (returnType == null)
                return null;
            IEnumerable<Type> parameterTypes = cached.parameterTypes.Select(n => Type.GetType(n, false));
            if (parameterTypes.Any(t => t == null))
                return null;
            return new MethodDeclaration()
            {
                name = cached.name,
                returnType = returnType,
                parameterTypes = parameterTypes.ToArray(),
            };
        }
    }
}