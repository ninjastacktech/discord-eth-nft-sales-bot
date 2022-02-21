using Nethereum.ABI.Decoders;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using System.Numerics;

namespace NinjaDiscordSalesBot
{
    public class X2Y2LogDecoder : IMarketLogDecoder
    {
        // Event Logs (Name: "EvProfit")
        // topics[0] = signature
        //
        // Data:
        // 1. itemHash[:bytes32]
        // 2. currency[:address]
        // 3. to[:address]
        // 4. amount[:uint256]

        public string Name { get; } = "x2y2";

        public string ContractAddress { get; } = "0x74312363e45dcaba76c59ec49a7aa8a65a67eed3";

        private string EvProfitSignature { get; } = "0xe2c49856b032c255ae7e325d18109bc4e22a2804e2e49a017ec0f59f19cd447b";

        public bool IsOrderEventLog(TransactionReceiptLog log)
        {
            return log.Address.ToLower() == ContractAddress && ((string)log.Topics[0]).ToLower() == EvProfitSignature;
        }

        public MarketTransaction? GetTransactionInfo(TransactionReceiptLog log)
        {
            if (log.Data == null || log.Data.Length < 4 * 32)
            {
                return null;
            }

            if (log.Topics.Length < 1)
            {
                return null;
            }

            MarketTransaction? info = new();

            try
            {
                var bytes = log.Data.HexToByteArray();
                var priceBytes = bytes.Skip(3 * 32).Take(32).ToArray();

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
