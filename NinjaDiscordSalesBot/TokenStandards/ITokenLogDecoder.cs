namespace NinjaDiscordSalesBot
{
    public interface ITokenLogDecoder
    {
        string Name { get; }

        string Signature { get; }

        bool IsTransferEvent(string topic);

        TokenTransferMetadata? GetTokenMetadata(TransactionReceiptLog log);
    }
}
