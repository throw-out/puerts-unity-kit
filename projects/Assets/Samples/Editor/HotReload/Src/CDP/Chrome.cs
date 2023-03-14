using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace CDP
{
    using WebSocketSharp;
    using WebSocket = WebSocketSharp.WebSocket;
    using WebSocketState = WebSocketSharp.WebSocketState;

    using JSON = Newtonsoft.Json.JsonConvert;
    using JSONSettings = Newtonsoft.Json.JsonSerializerSettings;
    using ChromeParameters = System.Collections.Generic.Dictionary<string, object>;
    using ChromeCallback = System.Action<string, System.Collections.Generic.Dictionary<string, object>>;

    public class Chrome : EventEmitter, IDisposable
    {
        private WebSocket ws;
        private string url;
        private ulong commandId = 1;
        private Dictionary<ulong, ChromeCallback> commandCallbacks =
            new Dictionary<ulong, ChromeCallback>();

        private JSONSettings jsonSettings = new JSONSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore };

        public bool IsOpen => ws != null && ws.ReadyState == WebSocketState.Open;
        public WebSocketState State => ws != null ? ws.ReadyState : WebSocketState.Closed;

        public Chrome() : this("localhost", 9222) { }
        public Chrome(string host, ushort port)
        {
            this.url = port > 0 ? $"ws://{host}:{port}" : $"ws://{host}";
        }
        public Chrome(string url)
        {
            this.url = url;
        }

        public void Start()
        {
            if (this.ws != null)
                throw new InvalidOperationException();
            try
            {
                var context = System.Threading.SynchronizationContext.Current;
                if (context == null)
                {
                    context = new System.Threading.SynchronizationContext();
                }

                this.ws = new WebSocket(this.url);

                this.ws.OnOpen += (sender, arg) => context.Post(this.HandleOpen, arg); ;
                this.ws.OnClose += (sender, arg) => context.Post(this.HandleClose, arg); ;
                this.ws.OnError += (sender, arg) => context.Post(this.HandleError, arg);
                this.ws.OnMessage += (sender, arg) => context.Post(this.HandleMessage, arg);
                this.ws.ConnectAsync();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                this.Close();
            }
        }
        public void Close()
        {
            if (this.ws == null)
                return;
            WebSocket ws = this.ws;
            this.ws = null;
            this.commandId = 1;
            this.commandCallbacks.Clear();

            ws.CloseAsync();
        }

        public async Task<ChromeParameters> Send(string method, object parameters, string sessionId)
        {
            ChromeParameters result = await new Promise<ChromeParameters>((resolve, reject) =>
            {
                Send(method, parameters, sessionId, (error, data) =>
                {
                    if (string.IsNullOrEmpty(error))
                    {
                        resolve(data as ChromeParameters);
                    }
                    else
                    {
                        reject(new Exception(error));
                    }
                });
            });
            return result;
        }
        public void Send(string method, object parameters, string sessionId, ChromeCallback callback)
        {
            ulong id = this.commandId++;
            CommandRequest msg = new CommandRequest()
            {
                id = id,
                method = method,
                sessionId = sessionId,
                @params = parameters ?? new object()
            };

            this.ws.SendAsync(JSON.SerializeObject(msg, jsonSettings), success =>
            {
                if (success) this.commandCallbacks[id] = callback;
            });
        }

        private void HandleOpen(object args)
        {
            if (!(args is EventArgs))
                return;
            this.Emit("open");
        }
        private void HandleClose(object args)
        {
            if (!(args is CloseEventArgs))
                return;
            var data = args as CloseEventArgs;
            this.Emit("close", data.Code, data.Reason);
        }
        private void HandleError(object args)
        {
            if (!(args is ErrorEventArgs))
                return;
            var data = args as ErrorEventArgs;
            this.Emit("error", data.Message);
        }
        private void HandleMessage(object args)
        {
            if (!(args is MessageEventArgs))
                return;
            var data = args as MessageEventArgs;

            this.Emit("message", data.Data);

            CommandResponse msg = JSON.DeserializeObject<CommandResponse>(data.Data);
            if (msg.id > 0)
            {
                ChromeCallback callback;
                this.commandCallbacks.TryGetValue(msg.id, out callback);
                if (callback == null)
                {
                    return;
                }
                this.commandCallbacks.Remove(msg.id);

                if (msg.error == null)
                {
                    callback(null, msg.result ?? new ChromeParameters());
                }
                else
                {
                    callback(msg.error.message, null);
                }

                if (this.commandCallbacks.Count == 0)
                {
                    this.Emit("ready");
                }
            }
            else if (!string.IsNullOrEmpty(msg.method))
            {
                this.Emit("event", msg);
                this.Emit(msg.method, msg.@params, msg.sessionId);
                this.Emit($"{msg.method}.{msg.sessionId}", msg.@params, msg.sessionId);
            }
        }

        public void Dispose()
        {
            this.Close();
        }

        class CommandRequest
        {
            public ulong id;
            public string method;
            public string sessionId;
            public object @params;
        }
        class CommandResponse
        {
            public ulong id;
            public string method;
            public string sessionId;
            public ChromeError error;
            public ChromeParameters result;
            public ChromeParameters @params;
        }
        class ChromeError
        {
            public int code;
            public string message;
        }
    }
}
