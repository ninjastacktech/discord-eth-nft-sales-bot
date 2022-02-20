using Nethereum.ABI.Decoders;
using Nethereum.Hex.HexConvertors.Extensions;

namespace NinjaDiscordSalesBot
{
    public class ERC1155Token : IToken
    {
        // Event Logs (Name: "TransferSingle")
        // topics[0] = signature
        // topics[1] = inputs[0] = operator[:address]
        // topics[2] = inputs[0] = from[:address]
        // topics[3] = inputs[1] = to[:address]
        // data (id[:uint256], value[:uint256])

        public string Signature { get; } = "0xc3d58168c5ae7397731d063d5bbf3d657854427343f4c083240f7aacaa2d0f62";

        public int? GetTokenId(TransactionReceiptLog log)
        {
            if (log.Data == null)
            {
                return null;
            }

            var bytes = log.Data.HexToByteArray();
            var idBytes = bytes.Take(32).ToArray();

            try
            {
                return new IntTypeDecoder().Decode<int>(idBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding tokenId: {ex.Message}");

                return null;
            }
        }

        public bool IsTransferEvent(string topic)
        {
            return Signature == topic;
        }
    }
}
