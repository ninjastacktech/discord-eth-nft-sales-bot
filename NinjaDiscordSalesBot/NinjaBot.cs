using System.Net;

namespace NinjaDiscordSalesBot
{
    public class NinjaBot
    {
        private readonly NinjaBotOptions _options;
        private readonly DiscordHttpClient _discordHttpClient;
        private readonly DiscordWebSocketClient _discordWebSocketClient;
        private readonly InfuraWebSocketClient _infuraWebSocketClient;
        private readonly InfuraHttpClient _infuraHttpClient;

        public NinjaBot(NinjaBotOptions options)
        {
            _options = options;
            _discordHttpClient = new DiscordHttpClient(botToken: _options.DiscordBotToken);
            _discordWebSocketClient = new DiscordWebSocketClient(botToken: _options.DiscordBotToken);
            _infuraWebSocketClient = new InfuraWebSocketClient(apiKey: _options.InfuraApiKey, collectionContractAddress: _options.CollectionContractAddress);
            _infuraHttpClient = new InfuraHttpClient(apiKey: _options.InfuraApiKey);
        }

        public async Task StartAsync()
        {
            await _discordWebSocketClient.StartAsync();

            _infuraWebSocketClient.OnTokenTransfer += async (transactionHash, tokenDecoder) =>
            {
                TokenMetadata? tokenMetadata = new();

                try
                {
                    var txReceipt = await _infuraHttpClient.GetTransactionReceiptAsync(transactionHash);

                    if (txReceipt == null)
                    {
                        return;
                    }

                    var marketDecoder = MarketLogDecoderFactory.GetMarketDecoder(txReceipt.To);

                    if (marketDecoder == null)
                    {
                        return;
                    }

                    var transferLog = txReceipt.Logs.FirstOrDefault(x => tokenDecoder.IsTransferEvent((string)x.Topics[0]) && x.Address == _options.CollectionContractAddress);

                    if (transferLog == null)
                    {
                        return;
                    }

                    tokenMetadata.TokenId = tokenDecoder.GetTokenId(transferLog);

                    if (tokenMetadata.TokenId == null)
                    {
                        return;
                    }

                    tokenMetadata.TokenStandard = tokenDecoder.Name;

                    var marketplaceLog = txReceipt.Logs.FirstOrDefault(x => marketDecoder.IsOrderEventLog(x));

                    if (marketplaceLog == null)
                    {
                        return;
                    }

                    var marketInfo = marketDecoder.GetTransactionInfo(marketplaceLog);

                    if (marketInfo != null)
                    {
                        tokenMetadata.Marketplace = marketDecoder.Name;
                        tokenMetadata.TotalPriceEth = marketInfo.Price;
                        tokenMetadata.Seller = marketInfo.Seller;
                        tokenMetadata.Buyer = marketInfo.Buyer;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Discord Bot failed to get token metadata: {ex.Message}");
                }

                if (tokenMetadata == null)
                {
                    return;
                }

                try
                {
                    await _discordHttpClient.SendMessageAsync(BuildDiscordMessage(tokenMetadata), _options.DiscordChannelId);
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
                {
                    Console.WriteLine($"Make sure the Discord Bot is allowed to post messages to the specified channel! ex: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Discord Bot failed to post the message to channel: {ex.Message}");
                }
            };

            await _infuraWebSocketClient.StartAsync();
        }

        public async Task StopAsync()
        {
            await _discordWebSocketClient.StopAsync();

            await _infuraWebSocketClient.StopAsync();
        }

        private static DiscordMessage BuildDiscordMessage(TokenMetadata tokenMetadata) => new DiscordMessageBuilder()
            .SetTitle(tokenMetadata.Name ?? tokenMetadata.TokenId?.ToString() ?? "Unknown")
            .SetImageUrl(tokenMetadata.ImageUrl)
            .SetTimestamp(DateTime.UtcNow)
            .AddField("Sale Price", $"{tokenMetadata.TotalPriceEth?.ToString() ?? "Unknown"}Ξ", inline: true)
            .AddField("Seller", $"{TrimString(tokenMetadata.Seller) ?? "Unknown"}", inline: true)
            .AddField("Buyer", $"{TrimString(tokenMetadata.Buyer) ?? "Unknown"}", inline: true)
            .Build();

        private static string? TrimString(string? str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return new string(str.Take(10).ToArray());
        }
    }
}
