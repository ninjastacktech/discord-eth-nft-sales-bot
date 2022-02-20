using Newtonsoft.Json;

namespace NinjaDiscordSalesBot
{
    public class DiscordMessage
    {
        [JsonProperty("embeds")]
        public List<DiscordMessageEmbed>? Embeds { get; set; }
    }

    public class DiscordMessageEmbed
    {
        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("timestamp")]
        public string? Timestamp { get; set; }

        [JsonProperty("image")]
        public DiscordMessageEmbedImage? Image { get; set; }

        [JsonProperty("author")]
        public DiscordMessageEmbedAuthor? Author { get; set; }

        [JsonProperty("footer")]
        public DiscordMessageEmbedAuthor? Footer { get; set; }

        [JsonProperty("fields")]
        public List<DiscordMessageEmbedField>? Fields { get; set; }
    }

    public class DiscordMessageEmbedImage
    {
        [JsonProperty("url")]
        public string? Url { get; set; }
    }

    public class DiscordMessageEmbedAuthor
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("iconUrl")]
        public string? IconUrl { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }
    }

    public class DiscordMessageEmbedFooter
    {
        [JsonProperty("text")]
        public string? Text { get; set; }

        [JsonProperty("iconUrl")]
        public string? IconUrl { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }
    }

    public class DiscordMessageEmbedField
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("value")]
        public string? Value { get; set; }

        [JsonProperty("inline")]
        public string? Inline { get; set; }
    }
}
