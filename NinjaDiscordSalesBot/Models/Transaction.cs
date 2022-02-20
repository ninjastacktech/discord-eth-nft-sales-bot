using Newtonsoft.Json;
#nullable disable
namespace NinjaDiscordSalesBot
{
    public class Transaction
    {
        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("input")]
        public string Input { get; set; }
    }
}
