namespace NinjaDiscordSalesBot
{
    public interface IToken
    {
        string Signature { get; }

        bool IsTransferEvent(string topic);

        int? GetTokenId(TransactionReceiptLog log);
    }
}
