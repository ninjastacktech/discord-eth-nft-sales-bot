using Nethereum.ABI.Decoders;
using Nethereum.Hex.HexConvertors.Extensions;

namespace NinjaDiscordSalesBot
{
    public class ERC1155LogDecoder : ITokenLogDecoder
    {
        // Event Logs (Name: "TransferSingle")
        // topics[0] = signature
        // topics[1] = inputs[0] = operator[:address]
        // topics[2] = inputs[1] = from[:address]
        // topics[3] = inputs[2] = to[:address]
        //
        // Data:
        // id[:uint256]
        // value[:uint256]

        public string Signature { get; } = "0xc3d58168c5ae7397731d063d5bbf3d657854427343f4c083240f7aacaa2d0f62";

        public string Name { get; } = "ERC-1155";

        public TokenMetadata? GetTokenMetadata(TransactionReceiptLog log)
        {
            if (log.Data == null)
            {
                return null;
            }

            if (log.Topics.Length < 4)
            {
                return null;
            }

            try
            {
                var bytes = log.Data.HexToByteArray();
                var idBytes = bytes.Take(32).ToArray();

                var addressDecoder = new AddressTypeDecoder();

                return new TokenMetadata
                {
                    TokenId = new IntTypeDecoder().Decode<int>(idBytes),
                    Seller = addressDecoder.Decode<string>((string)log.Topics[2]),
                    Buyer = addressDecoder.Decode<string>((string)log.Topics[3]),
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding tokenId: {ex.Message}");

                return null;
            }
        }

        public bool IsTransferEvent(string topic)
        {
            return Signature == topic.ToLower();
        }
    }
}
