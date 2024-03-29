﻿using System.Net;

namespace NinjaDiscordSalesBot
{
    public class NinjaBot
    {
        private readonly NinjaBotOptions _options;
        private readonly DiscordHttpClient _discordHttpClient;
        private readonly DiscordWebSocketClient? _discordWebSocketClient;
        private readonly InfuraWebSocketClient _infuraWebSocketClient;
        private readonly InfuraHttpClient _infuraHttpClient;

        public NinjaBot(NinjaBotOptions options)
        {
            _options = options;

            _infuraWebSocketClient = new InfuraWebSocketClient(apiKey: _options.InfuraApiKey, collectionContractAddress: _options.CollectionContractAddress);
            _infuraHttpClient = new InfuraHttpClient(apiKey: _options.InfuraApiKey);

            _discordHttpClient = new DiscordHttpClient(options: _options);

            if (string.IsNullOrEmpty(_options.DiscordWebhookUrl))
            {
                _discordWebSocketClient = new DiscordWebSocketClient(botToken: _options.DiscordBotToken);
            }
        }

        public async Task StartAsync()
        {
            await (_discordWebSocketClient?.StartAsync() ?? Task.CompletedTask);

            _infuraWebSocketClient.OnTokenTransfer += async (transactionHash, tokenDecoder) =>
            {
                TokenMetadata? tokenMetadata = null;

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

                    var transferLogs = txReceipt.Logs
                        .Where(x => tokenDecoder.IsTransferEvent((string)x.Topics[0]))
                        .Where(x => x.Address.ToLower() == _options.CollectionContractAddress.ToLower())
                        .ToList();

                    if (!transferLogs.Any())
                    {
                        return;
                    }

                    tokenMetadata = new TokenMetadata
                    {
                        TokenIds = new List<int>(),
                    };

                    foreach (var transferLog in transferLogs)
                    {
                        var tokenTransferMetadata = tokenDecoder.GetTokenMetadata(transferLog);

                        if (tokenTransferMetadata?.TokenId == null)
                        {
                            continue;
                        }

                        tokenMetadata.Buyer = tokenTransferMetadata.Buyer;
                        tokenMetadata.Seller = tokenTransferMetadata.Seller;
                        tokenMetadata.TokenIds.Add(tokenTransferMetadata.TokenId.Value);
                    }

                    tokenMetadata.TokenStandard = tokenDecoder.Name;

                    var marketLog = txReceipt.Logs.FirstOrDefault(x => marketDecoder.IsOrderEventLog(x));

                    if (marketLog == null)
                    {
                        return;
                    }

                    var marketInfo = marketDecoder.GetTransactionInfo(marketLog);

                    if (marketInfo != null)
                    {
                        tokenMetadata.Marketplace = marketDecoder.Name;
                        tokenMetadata.TotalPriceEth = marketInfo.Price;
                        tokenMetadata.Amount = marketInfo.Amount;
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
            await (_discordWebSocketClient?.StopAsync() ?? Task.CompletedTask);

            await _infuraWebSocketClient.StopAsync();
        }

        private static DiscordMessage BuildDiscordMessage(TokenMetadata tokenMetadata) => new DiscordMessageBuilder()
            .SetTitle(tokenMetadata.Name ?? (tokenMetadata.TokenIds?.Any() == true ? string.Join(",", tokenMetadata.TokenIds) : "Unknown"))
            .SetDescription($"Sold on {tokenMetadata.Marketplace}.")
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
