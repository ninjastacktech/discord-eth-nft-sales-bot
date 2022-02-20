# Ethereum NFT sales bot for Discord

Instead of polling the OpenSea API, this bot connects to an Infura websocket channel, subscribing to the collection's contract address events.

It filters those events based on standard `ERC-721`, `ERC-1155` event log transfer signatures.

If the transaction's recipient matches the contract address of one of the popular NFT marketplaces on Ethereum (OpenSea, LooksRare, x2y2), it will extract the token's metadata and post it to a Discord channel.

Built with 💙 and:
- [NinjaWebSocket](https://github.com/ninjastacktech/ninja-websocket-net) - Lightweight, user-friendly WebSocket APIs
- [OpenSeaHttpClient](https://github.com/ninjastacktech/opensea-net) - SDK for the OpenSea marketplace API
- [Netherum.ABI](https://github.com/Nethereum/Nethereum) - Encode/Decode event log topics & data

## Usage
```C#
var bot = new NinjaBot(new NinjaBotOptions
{
    DiscordBotToken = "<discord_bot_token>",
    DiscordChannelId = "<discord_channel_id>",
    CollectionContractAddress = "<collection_contract_address>",
    InfuraApiKey = "<infura_api_key>"
});

await bot.StartAsync();
```

## Options

---

### MIT License
