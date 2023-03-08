using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

using JSON = Newtonsoft.Json.JsonConvert;

namespace CDP
{
    public static class GenerateProtocol
    {
        [MenuItem("Tools/CDP/Protocol/Update")]
        static void Update()
        {
            string path = GetProtocolPath();
            if (string.IsNullOrEmpty(path))
                throw new NullReferenceException();

            //browser_protocol.json | js_protocol.json
            string url = "https://raw.githubusercontent.com/ChromeDevTools/devtools-protocol/master/json/";
            int selected = EditorUtility.DisplayDialogComplex(
                "Tip",
                $"Current protocol storage path:\n{path}\n\nPlease download the file in the browser:\n{url}...",
                "Cancel",
                "js_protocol.json",
                "browser_protocol.json"
            );
            if (selected <= 0)
                return;
            Application.OpenURL(url + (selected == 1 ? "js_protocol.json" : "browser_protocol.json"));
        }
        [MenuItem("Tools/CDP/Protocol/Generate")]
        static void GenerateCode()
        {
            string protocolContent = GetProtocolContent();
            string protocolPath = GetProtocolPath();
            if (string.IsNullOrEmpty(protocolContent))
                throw new NullReferenceException();

            Protocol protocol = JSON.DeserializeObject<Protocol>(protocolContent);
            if (protocol == null || protocol.domains == null || protocol.domains.Length == 0)
                throw new InvalidDataException();

            using (StreamWriter textWriter = new StreamWriter(Path.GetDirectoryName(protocolPath) + "/Protocol.cs", false, Encoding.UTF8))
            {
                textWriter.Write(GenerateTemplateCode(protocol));
                textWriter.Flush();
                textWriter.Close();

                AssetDatabase.Refresh();
            }
        }

        static string GetProtocolPath()
        {
            var config = ScriptableObject.CreateInstance<Configure>();
            if (config.m_Protocol == null)
                return string.Empty;
            return AssetDatabase.GetAssetPath(config.m_Protocol);
        }
        static string GetProtocolContent()
        {
            var config = ScriptableObject.CreateInstance<Configure>();
            if (config.m_Protocol == null)
                return string.Empty;
            return config.m_Protocol.text;
        }
        static string GenerateTemplateCode(Protocol protocol)
        {
            ProtocolGenertor genertor = new ProtocolGenertor(protocol);

            return $@"
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CDP;
using JSON = Newtonsoft.Json.JsonConvert;

namespace CDP.Domains
{{
    public class DomainBase
    {{
        protected readonly CDP.Chrome chrome;

        public DomainBase(CDP.Chrome chrome)
        {{
            this.chrome = chrome;
        }}
        protected static T Convert<T>(Dictionary<string, object> data)
        {{
            if (data == null)
            {{
                return default(T);
            }}
            return JSON.DeserializeObject<T>(JSON.SerializeObject(data));
        }}
    }}

    {string.Join("", protocol.domains.Select(domain => $@"
    public class {domain.domain} : DomainBase
    {{
        public {domain.domain}(CDP.Chrome chrome) : base(chrome) {{ }}

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        {string.Join("", domain.events == null ? new string[0] : domain.events.Select(e =>
        {
            string commandName = $"{domain.domain}.{e.name}";
            string code;
            if (e.parameters != null && e.parameters.Length > 0)
            {
                string parameterCode = $"<On{ProtocolGenertor.FormatCapitalizeFirstCharacter(e.name)}Parameters>";
                code = $@"
        /// <summary> {ProtocolGenertor.FormatDescription(e.description)} </summary>
        /// <returns> remove handler </returns>
        public Action On{ProtocolGenertor.FormatCapitalizeFirstCharacter(e.name)}(Action{parameterCode} handler, string sessionId = default)
        {{
            Action<Dictionary<string, object>> _handler = (d) =>
            {{
                handler(Convert{parameterCode}(d));
            }};
            string eventName = string.IsNullOrEmpty(sessionId) ? ""{commandName}"" : $""{commandName}.{{sessionId}}"";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }}
        ";
            }
            else
            {
                code = $@"
        /// <summary> {ProtocolGenertor.FormatDescription(e.description)} </summary>
        public Action On{ProtocolGenertor.FormatCapitalizeFirstCharacter(e.name)}(Action handler, string sessionId = default)
        {{
            string eventName = string.IsNullOrEmpty(sessionId) ? ""{commandName}"" : $""{commandName}.{{sessionId}}"";
            this.chrome.On(eventName, handler);
            return () => this.chrome.Remove(eventName, handler);
        }}
        ";
            }
            return code;
        }))}

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        {string.Join("", domain.commands.Select(command =>
        {
            string commandName = $"{domain.domain}.{command.name}";
            //parameters
            bool hasParameters = command.parameters != null && command.parameters.Length > 0;
            string parameterDeclare = hasParameters ? $"{ProtocolGenertor.FormatCapitalizeFirstCharacter(command.name)}Parameters parameters, " : "",
                parameterCode = hasParameters ? $"parameters" : "null";

            //return
            bool hasReturn = command.returns != null && command.returns.Length > 0;
            string returnType = hasReturn ? $"Task<{ProtocolGenertor.FormatCapitalizeFirstCharacter(command.name)}Return>" : "Task",
                returnCode = hasReturn ? $"Convert<{ProtocolGenertor.FormatCapitalizeFirstCharacter(command.name)}Return>(___r)" : "";

            string code = $@"
        /// <summary> 
        /// {ProtocolGenertor.FormatDescription(command.description)} 
        /// </summary>{string.Join("", command.parameters == null ? new string[0] : command.parameters.Select(p => $@"
        /// <param name=""{ProtocolGenertor.FormatVariable(p.name)}"">{ProtocolGenertor.FormatDescription(p.description)}</param>"))}
        public async {returnType} {ProtocolGenertor.FormatCapitalizeFirstCharacter(command.name)}({parameterDeclare}string sessionId = default)
        {{
            var ___r = await this.chrome.Send(""{commandName}"", {parameterCode}, sessionId);
            return {returnCode};
        }}";
            return code;
        }))}

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        {string.Join("", domain.types == null ? new string[0] : domain.types.Select(type => type.type != "object" ? "" : $@"
        public class {genertor.FormatType(type, domain, false)}
        {{
            {string.Join("", type.properties == null ? new string[0] : type.properties.Select(p => $@"
            /// <summary> {ProtocolGenertor.FormatDescription(p.description)} </summary>
            public {genertor.FormatReference(p, domain)} {ProtocolGenertor.FormatVariable(p.name)};"))}
        }}"))}

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        {string.Join("", domain.events == null ? new string[0] : domain.events.Select(@event =>
        {
            string code = string.Empty;
            if (@event.parameters != null && @event.parameters.Length > 0)
            {
                code = $@"
        public class On{ProtocolGenertor.FormatCapitalizeFirstCharacter(@event.name)}Parameters
        {{
            {string.Join("", @event.parameters.Select(p => $@"
            /// <summary> [{ProtocolGenertor.FormatRequire(!p.optional)}] {ProtocolGenertor.FormatDescription(p.description)} </summary>
            public {genertor.FormatReference(p, domain)} {ProtocolGenertor.FormatVariable(p.name)};"))}
        }}";
            }
            return code;
        }))}

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        {string.Join("", domain.commands == null ? new string[0] : domain.commands.Select(command =>
        {
            string code = string.Empty;
            if (command.parameters != null && command.parameters.Length > 0)
            {
                code = $@"
        public class {ProtocolGenertor.FormatCapitalizeFirstCharacter(command.name)}Parameters
        {{
            {string.Join("", command.parameters.Select(p => $@"
            /// <summary> [{ProtocolGenertor.FormatRequire(!p.optional)}] {ProtocolGenertor.FormatDescription(p.description)} </summary>
            public {genertor.FormatReference(p, domain)} {ProtocolGenertor.FormatVariable(p.name)};"))}
        }}";
            }
            return code;
        }))}

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        {string.Join("", domain.commands == null ? new string[0] : domain.commands.Select(command =>
        {
            string code = string.Empty;
            if (command.returns != null && command.returns.Length > 0)
            {
                code = $@"
        public class {ProtocolGenertor.FormatCapitalizeFirstCharacter(command.name)}Return
        {{
            {string.Join("", command.returns.Select(r => $@"
            /// <summary> {ProtocolGenertor.FormatDescription(r.description)} </summary>
            public {genertor.FormatReference(r, domain)} {ProtocolGenertor.FormatVariable(r.name)};"))}
        }}";
            }
            return code;
        }))}
    }}
    "))}
}}
";
        }

        class ProtocolGenertor
        {
            private readonly Protocol protocol;
            private Dictionary<string, string> typeIndexes;
            public ProtocolGenertor(Protocol protocol)
            {
                this.protocol = protocol;
                this.typeIndexes = new Dictionary<string, string>();
                foreach (var domain in protocol.domains ?? new ProtocolDomain[0])
                {
                    if (domain.types == null)
                        continue;
                    foreach (var type in domain.types)
                    {
                        this.typeIndexes.Add($"{domain.domain}.{type.id}", FormatType(type, domain, true));
                    }
                }
            }

            public string FormatReference(ProtocolReference reference, ProtocolDomain domain)
            {
                string typeName;

                string @ref = reference.@ref;
                if (!string.IsNullOrEmpty(@ref) && (
                    this.typeIndexes.TryGetValue(@ref, out typeName) ||
                    !@ref.Contains(".") && this.typeIndexes.TryGetValue($"{domain.domain}.{@ref}", out typeName
                )))
                {
                    return typeName;
                }
                switch (reference.type)
                {
                    case "array":
                        typeName = reference.item != null ? $"{FormatReference(reference.item, domain)}[]" : "object[]";
                        break;
                    default:
                        typeName = FormatMapping(reference.type);
                        break;
                }
                return typeName;
            }
            public string FormatType(ProtocolType type, ProtocolDomain domain, bool fullName)
            {
                string typeName;
                switch (type.type)
                {
                    case "object":
                        typeName = fullName ? $"{domain.domain}.{type.id}Type" : $"{type.id}Type";
                        break;
                    case "array":
                        typeName = type.item != null ? $"{FormatReference(type.item, domain)}[]" : "object[]"; ;
                        break;
                    default:
                        typeName = FormatMapping(type.type);
                        break;
                }
                return typeName;
            }

            static readonly Dictionary<string, string> typeMappings = new Dictionary<string, string>()
            {
                {"number", "double"},
                {"integer", "int"},
                {"bigint", "long"},
                {"boolean", "bool"},
                {"any", "object"},
            };
            public static string FormatMapping(string name)
            {
                if (name == null || name.Length == 0)
                    return "object";
                if (typeMappings.TryGetValue(name, out var newName))
                {
                    return newName;
                }
                return name;
            }

            static readonly string[] systemKeywords = new string[]{
                "as","base","bool","break","byte","case","catch","char","checked","class","const",
                "continue","decimal","default","delegate","do","double","else","enum","event",
                "explicit","extern","false","finally","fixed","float","for","foreach","goto",
                "if","implicit","in","int","interface","internal","is","lock","long","namespace",
                "new","null","object","operator","out","override","params","private","protected",
                "public","readonly","ref","return","sbyte","sealed","short","sizeof","stackalloc",
                "static","string","struct","switch","this","throw","true","try","typeof","uint",
                "ulong","unchecked","unsafe","ushort","using","virtual","void","volatile","while"
            };
            public static string FormatVariable(string name)
            {
                if (systemKeywords.Contains(name))
                {
                    return $"@{name}";
                }
                return name;
            }

            public static string FormatCapitalizeFirstCharacter(string name)
            {
                if (name == null || name.Length == 0)
                {
                    return string.Empty;
                }
                char first = name[0];
                if (first < 97 || first > 122)
                {
                    return name;
                }
                StringBuilder builder = new StringBuilder();
                builder.Append((char)(first - 32));
                for (int i = 1; i < name.Length; i++)
                {
                    builder.Append(name[i]);
                }
                return builder.ToString();
            }
            public static string FormatDescription(string description)
            {
                if (description == null)
                {
                    return string.Empty;
                }
                return description.Replace("\r\n", "").Replace("\n", "");
            }
            public static string FormatRequire(bool require)
            {
                return require ? "Require" : "Optional";
            }
        }

        class Protocol
        {
            public ProtocolVersion version;
            public ProtocolDomain[] domains;
        }
        class ProtocolVersion
        {
            public string major;
            public string minor;
        }
        class ProtocolDomain
        {
            public string domain;
            public bool experimental;
            public string[] dependencies;
            public ProtocolType[] types;
            public ProtocolCommand[] commands;
            public ProtocolEvent[] events;
        }
        class ProtocolType
        {
            public string id;
            public string description;
            public string type;
            public string[] @enum;
            public ProtocolReference item;
            public ProtocolReference[] properties;
        }
        class ProtocolCommand
        {
            public string name;
            public string description;
            public ProtocolReference[] parameters;
            public ProtocolReference[] returns;
        }
        class ProtocolEvent
        {
            public string name;
            public string description;
            public bool experimental;
            public ProtocolReference[] parameters;
        }

        class ProtocolReference
        {
            public string name;
            public string description;
            public string type;
            public bool optional;
            [JsonProperty(PropertyName = "$ref")]
            public string @ref;
            public ProtocolReference item;
        }

        class NonValidateCertificate : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                return true;
            }
        }
    }
}
