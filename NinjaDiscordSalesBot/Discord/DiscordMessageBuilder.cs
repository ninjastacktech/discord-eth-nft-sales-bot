namespace NinjaDiscordSalesBot
{ 
    public class DiscordMessageBuilder
    {
        private readonly DiscordMessage _discordMessage;
        private readonly DiscordMessageEmbed _discordMessageEmbed;

        public DiscordMessageBuilder()
        {
            _discordMessageEmbed = new DiscordMessageEmbed();
            _discordMessage = new DiscordMessage
            {
                Embeds = new List<DiscordMessageEmbed> { _discordMessageEmbed },
            };
        }

        public DiscordMessageBuilder SetTitle(string title)
        {
            _discordMessageEmbed.Title = title;

            return this;
        }

        public DiscordMessageBuilder SetDescription(string description)
        {
            _discordMessageEmbed.Description = description;

            return this;
        }

        public DiscordMessageBuilder SetUrl(string url)
        {
            _discordMessageEmbed.Url = url;

            return this;
        }

        public DiscordMessageBuilder SetTimestamp(DateTime? timestamp)
        {
            if (timestamp == null)
            {
                return this;
            }

            _discordMessageEmbed.Timestamp = timestamp.Value.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");

            return this;
        }

        public DiscordMessageBuilder SetImageUrl(string? imageUrl)
        {
            if (imageUrl == null)
            {
                return this;
            }

            _discordMessageEmbed.Image = new DiscordMessageEmbedImage
            {
                Url = imageUrl,
            };

            return this;
        }

        public DiscordMessageBuilder AddField(string name, string value, bool inline = false)
        {
            if (_discordMessageEmbed.Fields == null)
            {
                _discordMessageEmbed.Fields = new List<DiscordMessageEmbedField>();
            }

            _discordMessageEmbed.Fields.Add(new DiscordMessageEmbedField
            {
                Name = name,
                Value = value,
                Inline = inline.ToString().ToLower(),
            });

            return this;
        }

        public DiscordMessage Build() => _discordMessage;
    }
}
