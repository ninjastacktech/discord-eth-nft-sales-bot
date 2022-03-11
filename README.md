# Ethereum NFT sales bot for Discord

Instead of polling the OpenSea API, this bot connects to an Infura websocket channel, subscribing to the collection's contract address events.

It filters those events based on standard `ERC-721`, `ERC-1155` token transfer signature logs.

If the transaction's recipient matches the contract address of one of the popular NFT marketplaces on Ethereum, it will extract the token's metadata and post it to a Discord channel.

Currently supporting:
- [OpenSea](https://opensea.io/)
- [LooksRare](https://looksrare.org/)
- [x2y2](https://x2y2.io/)


Built with 💙 and:
- [NinjaWebSocket](https://github.com/ninjastacktech/ninja-websocket-net) - Lightweight, user-friendly WebSocket APIs
- [OpenSeaHttpClient](https://github.com/ninjastacktech/opensea-net) - SDK for the OpenSea marketplace API
- [Netherum.ABI](https://github.com/Nethereum/Nethereum) - Encode/Decode event log topics & data

## Usage
```C#
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
```


---

### MIT License
