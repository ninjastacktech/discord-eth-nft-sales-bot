namespace NinjaDiscordSalesBot
{
    public interface IMarketplaceContract
    {
        string ContractAddress { get; }

        MarketTransaction? GetTransactionInfo(TransactionReceiptLog log);
    }
}
