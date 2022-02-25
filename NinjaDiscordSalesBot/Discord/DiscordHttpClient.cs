using Newtonsoft.Json;
using System.Text;

namespace NinjaDiscordSalesBot
{
    public class DiscordHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly NinjaBotOptions _options;

        public DiscordHttpClient(NinjaBotOptions options, HttpClient? httpClient = null)
        {
            _options = options;
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task SendMessageAsync(DiscordMessage message, string channelId)
        {
            HttpRequestMessage request;

            if (!string.IsNullOrEmpty(_options.DiscordWebhookUrl))
            {
                request = BuildRequest(message, _options.DiscordWebhookUrl);
            }
            else
            {
                var url = $"https://discordapp.com/api/channels/{channelId}/messages";

                request = BuildRequest(message, url);

                request.Headers.Add("Authorization", $"Bot {_options.DiscordBotToken}");
            }

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        private static HttpRequestMessage BuildRequest(DiscordMessage message, string url)
        {
            return new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(message),
                    Encoding.UTF8,
                    "application/json")
            };
        }
    }
}
