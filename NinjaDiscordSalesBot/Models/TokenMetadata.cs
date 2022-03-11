namespace NinjaDiscordSalesBot
{
    public class TokenTransferMetadata
    {
        public int? TokenId { get; set; }

        public string? Seller { get; set; }

        public string? Buyer { get; set; }
    }

    public class TokenMetadata
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? ExternalUrl { get; set; }

        public List<int>? TokenIds { get; set; }

        public string? TokenStandard { get; set; }

        public string? Marketplace { get; set; }

        public string? CollectionName { get; set; }

        public string? ImageUrl { get; set; }

        public decimal? TotalPriceEth { get; set; }

        public int? Amount { get; set; }

        public string? Seller { get; set; }

        public string? Buyer { get; set; }
    }
}
