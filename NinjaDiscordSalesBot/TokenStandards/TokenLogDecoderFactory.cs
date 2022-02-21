namespace NinjaDiscordSalesBot
{
    public static class TokenLogDecoderFactory
    {
        private static readonly List<ITokenLogDecoder> TokenDecoders = new()
        {
            new ERC1155LogDecoder(),
            new ERC721LogDecoder(),
        };

        public static ITokenLogDecoder? GetTokenDecoder(string topic)
        {
            foreach (var tokenDecoder in TokenDecoders)
            {
                if (tokenDecoder.IsTransferEvent(topic))
                {
                    return tokenDecoder;
                }
            }

            return null;
        }
    }
}