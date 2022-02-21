using Nethereum.ABI.Decoders;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using System.Numerics;

namespace NinjaDiscordSalesBot
{
    public class LooksRareLogDecoder : IMarketLogDecoder
    {
        // Event Logs (Name: "TakerBid" / "TakerAsk"):
        // topics[0] = signature
        // topics[1] = inputs[2] = taker[:address]
        // topics[2] = inputs[3] = maker[:address]
        //
        // Data:
        // 1. orderHash[:bytes32]
        // 2. orderNonce[:uint256]
        // 3. currency [:address]
        // 4. collection [:address]
        // 5. tokenId [:uint256]
        // 6. amount [:uint256]
        // 7. price [:uint256]

        public string Name { get; } = "LooksRare";

        public string ContractAddress { get; } = "0x59728544b08ab483533076417fbbb2fd0b17ce3a";

        private string TakerBidSignature { get; } = "0x95fb6205e23ff6bda16a2d1dba56b9ad7c783f67c96fa149785052f47696f2be";

        private string TakerAskSignature { get; } = "0x68cd251d4d267c6e2034ff0088b990352b97b2002c0476587d0c4da889c11330";

        public bool IsOrderEventLog(TransactionReceiptLog log)
        {
            var signature = (string)log.Topics[0];

            return log.Address == ContractAddress && (signature == TakerAskSignature || signature == TakerBidSignature);
        }

        public MarketTransaction? GetTransactionInfo(TransactionReceiptLog log)
        {
            if (log.Data == null || log.Data.Length < 7 * 32)
            {
                return null;
            }

            if (log.Topics.Length < 3)
            {
                return null;
            }

            MarketTransaction? info = new();

            try
            {
                var bytes = log.Data.HexToByteArray();
                var amountBytes = bytes.Skip(5 * 32).Take(32).ToArray();
                var priceBytes = bytes.Skip(6 * 32).Take(32).ToArray();

                var addressDecoder = new AddressTypeDecoder();

                var takerTopic = (string)log.Topics[1];

                info.Buyer = addressDecoder.Decode<string>(takerTopic.HexToByteArray());

                var makerTopic = (string)log.Topics[2];

                info.Seller = addressDecoder.Decode<string>(makerTopic.HexToByteArray());

                var intTypeDecoder = new IntTypeDecoder();

                var priceParameter = intTypeDecoder.Decode<BigInteger>(priceBytes);

                info.Price = UnitConversion.Convert.FromWei(priceParameter);

                info.Amount = intTypeDecoder.Decode<int>(amountBytes);

                return info;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding market transaction: {ex.Message}");

                return null;
            }
        }
    }
}
