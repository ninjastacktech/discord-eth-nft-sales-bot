using Newtonsoft.Json.Linq;
using System.Text;

namespace NinjaDiscordSalesBot
{
    public class InfuraHttpClient
    {
        private readonly HttpClient _client;
        private readonly string _apiKey;

        public InfuraHttpClient(string apiKey, HttpClient? client = null)
        {
            _client = client ?? new HttpClient();
            _apiKey = apiKey;
        }

        public async Task<Transaction?> GetTransactionByHashAsync(string transactionHash, CancellationToken ct = default)
        {
            var body = @$"{{""jsonrpc"":""2.0"",""method"":""eth_getTransactionByHash"",""params"": [""{transactionHash}""],""id"":1}}";

            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await RequestAsync($"https://mainnet.infura.io/v3/{_apiKey}", HttpMethod.Post, content, ct: ct);

            var jo = JObject.Parse(response);

            return jo?.SelectToken("result")?.ToObject<Transaction>();
        }

        public async Task<TransactionReceipt?> GetTransactionReceiptAsync(string transactionHash, CancellationToken ct = default)
        {
            var body = @$"{{""jsonrpc"":""2.0"",""method"":""eth_getTransactionReceipt"",""params"": [""{transactionHash}""],""id"":1}}";

            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await RequestAsync($"https://mainnet.infura.io/v3/{_apiKey}", HttpMethod.Post, content, ct: ct);

            var jo = JObject.Parse(response);

            return jo?.SelectToken("result")?.ToObject<TransactionReceipt>();
        }

        public async Task<string?> CallAsync(string from, string to, string data, CancellationToken ct = default)
        {
            var body = @$"{{""jsonrpc"":""2.0"",""method"":""eth_call"",""params"": [{{""from"": ""{from}"",""to"": ""{to}"",""data"": ""{data}""}}, ""latest""],""id"":1}}";

            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await RequestAsync($"https://mainnet.infura.io/v3/{_apiKey}", HttpMethod.Post, content, ct: ct);

            var jo = JObject.Parse(response);

            return jo?.SelectToken("result")?.ToObject<string>();
        }

        protected async Task<string> RequestAsync(
            string url,
            HttpMethod method,
            HttpContent? content = null,
            CancellationToken ct = default)
        {
            var uri = new Uri(url);

            using var request = new HttpRequestMessage(method, uri);

            if (content != null)
            {
                request.Content = content;
            }

            using var response = await _client.SendAsync(request, ct);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(ct);
        }
    }
}
