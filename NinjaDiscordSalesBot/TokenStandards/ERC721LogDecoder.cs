using Nethereum.ABI.Decoders;
using Nethereum.Hex.HexConvertors.Extensions;

namespace NinjaDiscordSalesBot
{
    public class ERC721LogDecoder : ITokenLogDecoder
    {
        // Event Logs (Name: "Transfer")
        // topics[0] = signature
        // topics[1] = inputs[0] = from[:address]
        // topics[2] = inputs[1] = to[:address]
        // topics[3] = inputs[2] = tokenId[:uint256]

        public string Signature { get; } = "0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef";

        public string Name { get; } = "ERC-721";

        public TokenTransferMetadata? GetTokenMetadata(TransactionReceiptLog log)
        {
            if (log.Topics.Length < 4)
            {
                return null;
            }

            var from = (string)log.Topics[1];
            var to = (string)log.Topics[2];
            var tokenIdTopic = (string)log.Topics[3];

            var addressTypeDecoder = new AddressTypeDecoder();

            return new TokenTransferMetadata()
            {
                TokenId = new IntTypeDecoder().Decode<int>(tokenIdTopic.HexToByteArray()),
                Seller = addressTypeDecoder.Decode<string>(from.HexToByteArray()),
                Buyer = addressTypeDecoder.Decode<string>(to.HexToByteArray()),
            };
        }

        public bool IsTransferEvent(string topic)
        {
            return Signature == topic.ToLower();
        }
    }
}
