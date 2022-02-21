using Nethereum.ABI.Decoders;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using System.Numerics;

namespace NinjaDiscordSalesBot
{
    public class OpenSeaLogDecoder : IMarketLogDecoder
    {
        // Event Logs (Name: "OrdersMatched")
        // topics[0] = signature
        // topics[1] = inputs[2] = maker[:address]
        // topics[2] = inputs[3] = taker[:address]
        //
        // Data:
        // 1. buyHash[:bytes32]
        // 2. sellHash[:bytes32]
        // 3. price[:uint256]

        public string Name { get; } = "OpenSea";

        public string ContractAddress { get; } = "0x7f268357a8c2552623316e2562d90e642bb538e5"; //V2
        //public string ContractAddress { get; } = "0x7be8076f4ea4a4ad08075c2508e481d6c946d12b"; //V1

        private string OrdersMatchedSignature { get; } = "0xc4109843e0b7d514e4c093114b863f8e7d8d9a458c372cd51bfe526b588006c9";

        public bool IsOrderEventLog(TransactionReceiptLog log)
        {
            return log.Address == ContractAddress && (string)log.Topics[0] == OrdersMatchedSignature;
        }

        public MarketTransaction? GetTransactionInfo(TransactionReceiptLog log)
        {
            if (log.Data == null || log.Data.Length < 3 * 32)
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
                var priceBytes = bytes.Skip(2 * 32).Take(32).ToArray();

                var addressDecoder = new AddressTypeDecoder();

                var makerTopic = (string)log.Topics[1];

                info.Buyer = addressDecoder.Decode<string>(makerTopic.HexToByteArray());

                var takerTopic = (string)log.Topics[2];

                info.Seller = addressDecoder.Decode<string>(takerTopic.HexToByteArray());

                var priceParameter = new IntTypeDecoder().Decode<BigInteger>(priceBytes);

                info.Price = UnitConversion.Convert.FromWei(priceParameter);

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
