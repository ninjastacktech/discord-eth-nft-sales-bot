namespace NinjaDiscordSalesBot
{
    public static class MarketplaceContractFactory
    {
        private static readonly List<IMarketplaceContract> MarketplaceContracts = new() { new OpenSeaContract() };

        public static IMarketplaceContract? GetMarketplaceContract(string contractAddress)
        {
            foreach (var marketplaceContract in MarketplaceContracts)
            {
                if (marketplaceContract.ContractAddress == contractAddress)
                {
                    return marketplaceContract;
                }
            }

            return null;
        }
    }
}
