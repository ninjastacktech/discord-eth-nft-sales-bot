using Nethereum.ABI.Decoders;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using System.Numerics;

namespace NinjaDiscordSalesBot
{
    public class OpenSeaContract : IMarketplaceContract
    {
        // Event Logs (Name: "OrdersMatched")
        // topics[0] = signature
        // topics[1] = inputs[2] = maker[:address]
        // topics[2] = inputs[3] = taker[:address]
        // data (buyHash[:bytes32], sellHash[:bytes32], price[:uint256])

        public string Name { get; } = "OpenSea";

        //public string ContractAddress { get; } = "0x7f268357a8c2552623316e2562d90e642bb538e5"; //V2
        public string ContractAddress { get; } = "0x7be8076f4ea4a4ad08075c2508e481d6c946d12b"; //V1

        public MarketTransaction? GetTransactionInfo(TransactionReceiptLog log)
        {
            if (log.Data == null || log.Data.Length < 64)
            {
                return null;
            }

            if (log.Topics.Length < 3)
            {
                return null;
            }

            MarketTransaction? info = new();

            var bytes = log.Data.HexToByteArray();
            var priceBytes = bytes.Skip(64).Take(32).ToArray();

            try
            {
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
