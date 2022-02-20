using NinjaDiscordSalesBot;

var bot = new NinjaBot(new NinjaBotOptions
{
    DiscordBotToken = "<discord_bot_token>",
    DiscordChannelId = "<discord_channel_id>",
    CollectionContractAddress = "<collection_contract_address>",
    InfuraApiKey = "<infura_api_key>",
});

await bot.StartAsync();

Thread.Sleep(-1);
