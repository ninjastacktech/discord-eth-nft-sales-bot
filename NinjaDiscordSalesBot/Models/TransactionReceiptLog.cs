using Newtonsoft.Json;
#nullable disable
namespace NinjaDiscordSalesBot
{
    public class TransactionReceiptLog
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("topics")]
        public object[] Topics { get; set; }

        [JsonProperty("logIndex")]
        public string LogIndex { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
