using Newtonsoft.Json.Linq;
using Ninja.WebSocketClient;
using System.Buffers;
using System.Text;

namespace NinjaDiscordSalesBot
{
    public class InfuraWebSocketClient
    {
        private NinjaWebSocket? _ws;
        private readonly string _apiKey;
        private readonly string _collectionContractAddress;

        public event Func<string, IToken, Task>? OnTokenTransfer;

        public InfuraWebSocketClient(string apiKey, string collectionContractAddress)
        {
            _apiKey = apiKey;
            _collectionContractAddress = collectionContractAddress;
        }

        public async Task StartAsync()
        {
            _ws = new NinjaWebSocket($"wss://mainnet.infura.io/ws/v3/{_apiKey}")
                .SetKeepAlive(intervalMilliseconds: 20000)
                .SetAutomaticReconnect(intervalMilliseconds: 20000);

            _ws.OnConnected += async () =>
            {
                Console.WriteLine("Discord Bot connected to Infura websocket.");

                // Subscribe to Ethereum wss endpoint.
                var subscription = GetContractAddressSubscription(_collectionContractAddress);

                await _ws.SendAsync(subscription);
            };

            _ws.OnReceived += async data =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(data!.Value.ToArray());

                    Console.WriteLine(message);

                    var jo = JObject.Parse(message);

                    var ev = jo?.SelectToken("params")?.SelectToken("result");

                    var topic = ev?.SelectToken("topics")?.ToArray()?.Select(x => x.ToString())?.First();

                    if (topic == null)
                    {
                        return;
                    }

                    // Match token standard (ERC-721, ERC-1155) by the Transfer method's signature
                    var tokenStandard = TokenFactory.GetToken(topic);

                    if (tokenStandard != null)
                    {
                        var txHash = ev?.SelectToken("transactionHash")?.ToString();

                        if (txHash == null)
                        {
                            return;
                        }

                        await (OnTokenTransfer?.Invoke(txHash, tokenStandard) ?? Task.CompletedTask);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            };

            _ws.OnKeepAlive += () =>
            {
                Console.WriteLine("Discord Bot ping Infura websocket.");

                return Task.CompletedTask;
            };

            _ws.OnReconnecting += (ex) =>
            {
                Console.WriteLine($"Discord Bot reconnecting to Infura websocket: {ex?.Message}");

                return Task.CompletedTask;
            };

            _ws.OnClosed += (ex) =>
            {
                Console.WriteLine($"Discord Bot Infura connection closed:: {ex?.Message}");

                return Task.CompletedTask;
            };

            await _ws.StartAsync();
        }

        public async Task StopAsync()
        {
            await (_ws?.StopAsync() ?? Task.CompletedTask);
        }

        static ArraySegment<byte> GetContractAddressSubscription(string collectionAddress)
        {
            var subscription = $"{{\"jsonrpc\":\"2.0\", \"id\": 1, \"method\": \"eth_subscribe\", \"params\": [\"logs\", {{\"address\": \"{collectionAddress}\"}}]}}";

            return GetPayload(subscription);
        }

        static ArraySegment<byte> GetPayload(string json)
        {
            var encoded = Encoding.UTF8.GetBytes(json);
            var bufferSend = new ArraySegment<byte>(encoded, 0, encoded.Length);

            return bufferSend;
        }
    }
}
