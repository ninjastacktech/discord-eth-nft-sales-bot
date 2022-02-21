namespace NinjaDiscordSalesBot
{
    public interface ITokenLogDecoder
    {
        string Name { get; }

        string Signature { get; }

        bool IsTransferEvent(string topic);

        int? GetTokenId(TransactionReceiptLog log);
    }
}
