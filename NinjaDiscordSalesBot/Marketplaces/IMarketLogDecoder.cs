namespace NinjaDiscordSalesBot
{
    public interface IMarketLogDecoder
    {
        string Name { get; }

        string ContractAddress { get; }

        bool IsOrderEventLog(TransactionReceiptLog log);

        MarketTransaction? GetTransactionInfo(TransactionReceiptLog log);
    }
}
