using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace HR
{
    using JSON = Newtonsoft.Json.JsonConvert;
    using Ping = System.Net.NetworkInformation.Ping;

    public class InspectorScanner
    {
        const ushort THREAD_SLEEP = 5;
        const ushort NETWORK_TIMEOUT = 120;

        public static IPEndPoint[] GetActiveTcpListeners()
        {
            return IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
        }
        public static IPEndPoint[] GetActiveUdpListeners()
        {
            return IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners();
        }

        public static async Task<int[]> GetActiveInspector(Action<CancellationToken, int, int> status = null)
        {
            var listeners = GetActiveTcpListeners().ToArray();
            var token = new CancellationToken();

            var results = new List<int>();
            for (int i = 0, count = listeners.Length; i < count && !token.IsCancelled; i++)
            {
                if (status != null)
                {
                    status(token, i + 1, count);
                }
                var protocols = await GetProtocolAsync($"http://localhost:{listeners[i].Port}/json", NETWORK_TIMEOUT);
                if (protocols == null || protocols.Length == 0)
                    continue;
                if (protocols.FirstOrDefault(p => p.IsValid()) == null)
                    continue;
                results.Add(listeners[i].Port);
            }
            if (token.IsCancelled)
            {
                return null;
            }
            return new HashSet<int>(results).ToArray();
        }

        static async Task<Protocol[]> GetProtocolAsync(string url, int timeout = -1)
        {
            try
            {
                var http = new HttpClient();
                http.Timeout = TimeSpan.FromMilliseconds(timeout > 0 ? timeout : NETWORK_TIMEOUT);

                var response = await http.GetAsync(url).ConfigureAwait(true);
                if (response == null || response.StatusCode != HttpStatusCode.OK ||
                    response.Content == null)
                {
                    return null;
                }
                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                if (string.IsNullOrEmpty(data))
                {
                    return null;
                }
                if (data.StartsWith("{"))
                {
                    try
                    {
                        var protocol = JSON.DeserializeObject<Protocol>(data);
                        return new Protocol[] { protocol };
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                    return null;
                }
                return JSON.DeserializeObject<Protocol[]>(data);
            }
            catch (Exception /*e*/)
            {
                return null;
            }
        }

        public class CancellationToken
        {
            public bool IsCancelled { get; private set; }
            public void Cancel()
            {
                this.IsCancelled = true;
            }
        }
        public class Protocol
        {
            public string title;
            public string description;
            public string id;
            public string type;
            public string webSocketDebuggerUrl;
            public bool IsValid()
            {
                return !(
                    string.IsNullOrEmpty(type) ||
                    string.IsNullOrEmpty(webSocketDebuggerUrl)
                );
            }
        }
    }
}