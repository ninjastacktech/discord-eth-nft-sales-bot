using Newtonsoft.Json;
using System.Text;

namespace NinjaDiscordSalesBot
{
    public class DiscordHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _botToken;

        public DiscordHttpClient(string botToken, HttpClient? httpClient = null)
        {
            _botToken = botToken;
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task SendMessageAsync(DiscordMessage message, string channelId)
        {
            var url = $"https://discordapp.com/api/channels/{channelId}/messages";

            var request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Headers.Add("Authorization", $"Bot {_botToken}");

            request.Content = new StringContent(
                JsonConvert.SerializeObject(message),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }
    }
}
