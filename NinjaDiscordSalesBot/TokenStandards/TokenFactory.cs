namespace NinjaDiscordSalesBot
{
    public static class TokenFactory
    {
        private static readonly List<IToken> TokenStandards = new()
        {
            new ERC1155Token(),
            new ERC721Token(),
        };

        public static IToken? GetToken(string topic)
        {
            foreach (var tokenStandard in TokenStandards)
            {
                if (tokenStandard.IsTransferEvent(topic))
                {
                    return tokenStandard;
                }
            }

            return null;
        }
    }
}