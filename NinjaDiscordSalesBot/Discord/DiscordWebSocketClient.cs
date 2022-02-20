using Newtonsoft.Json.Linq;
using Ninja.WebSocketClient;
using System.Buffers;
using System.Text;

namespace NinjaDiscordSalesBot
{
    public class DiscordWebSocketClient
    {
        private NinjaWebSocket? _ws;
        private volatile int _seq = 0;
        private readonly string _botToken;

        public DiscordWebSocketClient(string botToken)
        {
            _botToken = botToken;
        }

        public async Task StartAsync()
        {
            _ws = new NinjaWebSocket("wss://gateway.discord.gg/?v=9&encoding=json")
                .SetAutomaticReconnect(intervalMilliseconds: 10000);

            _ws.OnConnected += async () =>
            {
                Console.WriteLine("Discord Bot connected to websocket. Logging in...");

                await _ws.SendAsync(AuthenticatePayload(_botToken));
            };

            _ws.OnReceived += data =>
            {
                var message = Encoding.UTF8.GetString(data!.Value.ToArray());

                var jo = JObject.Parse(message);
                var opCode = jo?.SelectToken("op")?.ToObject<int?>();
                var t = jo?.SelectToken("t")?.ToObject<string?>();

                switch (opCode)
                {
                    case 0:
                        switch (t)
                        {
                            case "READY":
                                Console.WriteLine("Discord Bot logged in successfully.");
                                break;
                            case "MESSAGE_CREATE":
                                var d = jo?.SelectToken("d");
                                var embeds = d?.SelectToken("embeds")?.ToArray();
                                var title = embeds != null && embeds.Count() > 0 ? embeds[0]?.SelectToken("title")?.ToObject<string?>() : string.Empty;
                                Console.WriteLine($"Discord Bot posted a new message: {title}");
                                break;
                        }
                        break;
                    case 10: //Hello
                        var heartbeatInterval = jo?.SelectToken("d")?.SelectToken("heartbeat_interval")?.ToObject<int?>();
                        if (heartbeatInterval.HasValue)
                        {
                            _ws.SetKeepAlive(() => HeartbeatPayload, intervalMilliseconds: heartbeatInterval.Value);
                        }
                        break;
                    case 11: //Heartbeat ack
                        break;
                }

                var seq = jo?.SelectToken("s")?.ToObject<int?>();

                if (seq.HasValue)
                {
                    _seq = seq.Value;
                }

                return Task.CompletedTask;
            };

            _ws.OnKeepAlive += () =>
            {
                return Task.CompletedTask;
            };

            _ws.OnReconnecting += (ex) =>
            {
                Console.WriteLine($"Discord Bot reconnecting to websocket: {ex?.Message}");
                return Task.CompletedTask;
            };

            _ws.OnClosed += (ex) =>
            {
                Console.WriteLine($"Discord Bot connection closed: {ex?.Message}");
                return Task.CompletedTask;
            };

            await _ws.StartAsync();
        }

        public async Task StopAsync()
        {
            await (_ws?.StopAsync() ?? Task.CompletedTask);
        }

        ArraySegment<byte> HeartbeatPayload => GetPayload($"{{\"op\": 1, \"d\": {(_seq == 0 ? "null" : _seq)}}}");

        static ArraySegment<byte> AuthenticatePayload(string botToken) => GetPayload(
                @$"{{
                    ""op"": 2,
                    ""d"": {{
                    ""token"": ""{botToken}"",
                    ""intents"": 513,
                    ""properties"": {{ 
                        ""$os"": ""linux"",
                        ""$browser"": ""ninja-websocket-net"",
                        ""$device"": ""ninja-websocket-net""
                    }}}}}}");

        static ArraySegment<byte> GetPayload(string json)
        {
            var encoded = Encoding.UTF8.GetBytes(json);
            var bufferSend = new ArraySegment<byte>(encoded, 0, encoded.Length);

            return bufferSend;
        }
    }
}
