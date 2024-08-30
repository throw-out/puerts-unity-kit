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
                return;
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
            byte type = 0;
            object obj = statement;
            if (statement is TypeDeclaration td)
            {
                type = 1;
                obj = TypeDeclarationCached.From(td);
            }

            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write(type);
                writer.Write(JSON.SerializeObject(obj, JSONSettings.format));

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
                        var cached = JSON.DeserializeObject<TypeDeclarationCached>(json, JSONSettings.format);
                        statement = cached != null ? TypeDeclarationCached.From(cached) : null;
                    }
                    else
                    {
                        statement = JSON.DeserializeObject<EnumDeclaration>(json, JSONSettings.format);
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
        public ObjectSerializer defaultValue;
        public Tuple<float, float> valueRange;
        public Dictionary<string, ObjectSerializer> valueEnum;
        public Dictionary<string, string> valueReferences;
        public static PropertyDeclarationCached From(PropertyDeclaration declaration)
        {
            var defaultValue = ObjectSerializer.FromObject(declaration.defaultValue);
            Dictionary<string, ObjectSerializer> valueEnum = null;
            if (declaration.valueEnum != null)
            {
                valueEnum = new Dictionary<string, ObjectSerializer>();
                foreach (var pair in declaration.valueEnum)
                {
                    valueEnum[pair.Key] = ObjectSerializer.FromObject(pair.Value);
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
            Type valueType = ReflectionUtil.GetType(cached.valueType);
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
            Type returnType = ReflectionUtil.GetType(cached.returnType);
            IEnumerable<Type> parameterTypes = cached.parameterTypes?.Select(n => ReflectionUtil.GetType(n));
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

    public class ObjectSerializer
    {
        public string type;
        public string value;

        public object ToObject()
        {
            if (string.IsNullOrEmpty(value))
                return null;
            Type explicitType = ReflectionUtil.GetType(type);
            if (explicitType != null)
            {
                return JSON.DeserializeObject(value, explicitType, JSONSettings.converters);
            }
            return JSON.DeserializeObject<object>(value, JSONSettings.converters);
        }
        public static ObjectSerializer FromObject(object obj)
        {
            if (obj == null)
                return null;
            return new ObjectSerializer()
            {
                value = JSON.SerializeObject(obj, JSONSettings.converters),
                type = obj?.GetType().FullName,
            };
        }
    }

    internal static class JSONSettings
    {
        public static readonly JsonSerializerSettings format = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
        };
        public static readonly JsonSerializerSettings converters = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            //MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            //Formatting = Formatting.Indented,
            Converters = new List<JsonConverter>
            {
                new VectorConverter(),
                new ColorConverter(),
                new RectConverter(),
            },
        };

        abstract class BaseConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return WriterMethods.ContainsKey(objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (objectType == null || !ReaderMethods.TryGetValue(objectType, out var read))
                {
                    throw new Exception("Unexpected Error Occurred");
                }
                return read(reader, serializer);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (value == null || !WriterMethods.TryGetValue(value.GetType(), out var write))
                {
                    throw new Exception("Unexpected Error Occurred");
                }
                writer.WriteStartObject();
                write(writer, serializer, value);
                writer.WriteEndObject();
            }
            protected abstract Dictionary<Type, Action<JsonWriter, JsonSerializer, object>> WriterMethods { get; }
            protected abstract Dictionary<Type, Func<JsonReader, JsonSerializer, object>> ReaderMethods { get; }
        }

        class VectorConverter : BaseConverter
        {
            protected override Dictionary<Type, Func<JsonReader, JsonSerializer, object>> ReaderMethods { get; } = new Dictionary<Type, Func<JsonReader, JsonSerializer, object>>()
            {
                {typeof(Vector2), (reader, serializer) => JSON.DeserializeObject<Vector2>(serializer.Deserialize(reader).ToString())},
                {typeof(Vector3), (reader, serializer) => JSON.DeserializeObject<Vector3>(serializer.Deserialize(reader).ToString())},
                {typeof(Vector4), (reader, serializer) => JSON.DeserializeObject<Vector4>(serializer.Deserialize(reader).ToString())},
                {typeof(Vector2Int), (reader, serializer) => JSON.DeserializeObject<Vector2Int>(serializer.Deserialize(reader).ToString())},
                {typeof(Vector3Int), (reader, serializer) => JSON.DeserializeObject<Vector3Int>(serializer.Deserialize(reader).ToString())},
            };
            protected override Dictionary<Type, Action<JsonWriter, JsonSerializer, object>> WriterMethods { get; } = new Dictionary<Type, Action<JsonWriter, JsonSerializer, object>>()
            {
                { typeof(Vector2), (writer, serializer, value) => WriteProperties(writer, (Vector2)value)},
                { typeof(Vector3), (writer, serializer, value) => WriteProperties(writer, (Vector3)value)},
                { typeof(Vector4), (writer, serializer, value) => WriteProperties(writer, (Vector4)value)},
                { typeof(Vector2Int), (writer, serializer, value) => WriteProperties(writer, (Vector2Int)value)},
                { typeof(Vector3Int), (writer, serializer, value) => WriteProperties(writer, (Vector3Int)value)},
            };

            static void WriteProperties(JsonWriter writer, Vector2 value)
            {
                writer.WritePropertyName("x");
                writer.WriteValue(value.x);
                writer.WritePropertyName("y");
                writer.WriteValue(value.y);
            }
            static void WriteProperties(JsonWriter writer, Vector3 value)
            {
                writer.WritePropertyName("x");
                writer.WriteValue(value.x);
                writer.WritePropertyName("y");
                writer.WriteValue(value.y);
                writer.WritePropertyName("z");
                writer.WriteValue(value.z);
            }
            static void WriteProperties(JsonWriter writer, Vector4 value)
            {
                writer.WritePropertyName("x");
                writer.WriteValue(value.x);
                writer.WritePropertyName("y");
                writer.WriteValue(value.y);
                writer.WritePropertyName("z");
                writer.WriteValue(value.z);
                writer.WritePropertyName("w");
                writer.WriteValue(value.w);
            }
            static void WriteProperties(JsonWriter writer, Vector2Int value)
            {
                writer.WritePropertyName("x");
                writer.WriteValue(value.x);
                writer.WritePropertyName("y");
                writer.WriteValue(value.y);
            }
            static void WriteProperties(JsonWriter writer, Vector3Int value)
            {
                writer.WritePropertyName("x");
                writer.WriteValue(value.x);
                writer.WritePropertyName("y");
                writer.WriteValue(value.y);
                writer.WritePropertyName("z");
                writer.WriteValue(value.z);
            }
        }
        class ColorConverter : BaseConverter
        {
            protected override Dictionary<Type, Func<JsonReader, JsonSerializer, object>> ReaderMethods { get; } = new Dictionary<Type, Func<JsonReader, JsonSerializer, object>>()
            {
                {typeof(Color), (reader, serializer) => JSON.DeserializeObject<Color>(serializer.Deserialize(reader).ToString())},
                {typeof(Color32), (reader, serializer) => JSON.DeserializeObject<Color32>(serializer.Deserialize(reader).ToString())},
            };
            protected override Dictionary<Type, Action<JsonWriter, JsonSerializer, object>> WriterMethods { get; } = new Dictionary<Type, Action<JsonWriter, JsonSerializer, object>>()
            {
                { typeof(Color), (writer, serializer, value) => WriteProperties(writer, (Color)value)},
                { typeof(Color32), (writer, serializer, value) => WriteProperties(writer, (Color32)value)},
            };

            static void WriteProperties(JsonWriter writer, Color value)
            {
                writer.WritePropertyName("r");
                writer.WriteValue(value.r);
                writer.WritePropertyName("g");
                writer.WriteValue(value.g);
                writer.WritePropertyName("b");
                writer.WriteValue(value.b);
                writer.WritePropertyName("a");
                writer.WriteValue(value.a);
            }
            static void WriteProperties(JsonWriter writer, Color32 value)
            {
                writer.WritePropertyName("r");
                writer.WriteValue(value.r);
                writer.WritePropertyName("g");
                writer.WriteValue(value.g);
                writer.WritePropertyName("b");
                writer.WriteValue(value.b);
                writer.WritePropertyName("a");
                writer.WriteValue(value.a);
            }
        }
        class RectConverter : BaseConverter
        {
            protected override Dictionary<Type, Func<JsonReader, JsonSerializer, object>> ReaderMethods { get; } = new Dictionary<Type, Func<JsonReader, JsonSerializer, object>>()
            {
                {typeof(Rect), (reader, serializer) => JSON.DeserializeObject<Rect>(serializer.Deserialize(reader).ToString())},
                {typeof(RectInt), (reader, serializer) => JSON.DeserializeObject<RectInt>(serializer.Deserialize(reader).ToString())},
                {typeof(RectOffset), (reader, serializer) => JSON.DeserializeObject<RectOffset>(serializer.Deserialize(reader).ToString())},
            };
            protected override Dictionary<Type, Action<JsonWriter, JsonSerializer, object>> WriterMethods { get; } = new Dictionary<Type, Action<JsonWriter, JsonSerializer, object>>()
            {
                { typeof(Rect), (writer, serializer, value) => WriteProperties(writer, (Rect)value)},
                { typeof(RectInt), (writer, serializer, value) => WriteProperties(writer, (RectInt)value)},
                { typeof(RectOffset), (writer, serializer, value) => WriteProperties(writer, (RectOffset)value)},
            };

            static void WriteProperties(JsonWriter writer, Rect value)
            {
                writer.WritePropertyName("x");
                writer.WriteValue(value.x);
                writer.WritePropertyName("y");
                writer.WriteValue(value.y);
                writer.WritePropertyName("width");
                writer.WriteValue(value.width);
                writer.WritePropertyName("height");
                writer.WriteValue(value.height);
            }
            static void WriteProperties(JsonWriter writer, RectInt value)
            {
                writer.WritePropertyName("x");
                writer.WriteValue(value.x);
                writer.WritePropertyName("y");
                writer.WriteValue(value.y);
                writer.WritePropertyName("width");
                writer.WriteValue(value.width);
                writer.WritePropertyName("height");
                writer.WriteValue(value.height);
            }
            static void WriteProperties(JsonWriter writer, RectOffset value)
            {
                writer.WritePropertyName("left");
                writer.WriteValue(value.left);
                writer.WritePropertyName("right");
                writer.WriteValue(value.right);
                writer.WritePropertyName("top");
                writer.WriteValue(value.top);
                writer.WritePropertyName("bottom");
                writer.WriteValue(value.bottom);
                writer.WritePropertyName("horizontal");
                writer.WriteValue(value.horizontal);
                writer.WritePropertyName("vertical");
                writer.WriteValue(value.vertical);
            }
        }
    }
}