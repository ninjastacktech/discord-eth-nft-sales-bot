using NinjaDiscordSalesBot;

// You can either configure:
// - channel id + bot token
// - webhook URL (recommended option: restricts the bot to post to the specified channel)
var bot = new NinjaBot(new NinjaBotOptions
{
    DiscordBotToken = "<discord_bot_token>",
    DiscordChannelId = "<discord_channel_id>",
    DiscordWebhookUrl = "<discord_webhook_url>",
    CollectionContractAddress = "<collection_contract_address>",
    InfuraApiKey = "<infura_api_key>"
});

await bot.StartAsync();

Thread.Sleep(-1);