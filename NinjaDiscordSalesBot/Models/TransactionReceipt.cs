using Newtonsoft.Json;
#nullable disable
namespace NinjaDiscordSalesBot
{
    public class TransactionReceipt
    {
        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("logs")]
        public List<TransactionReceiptLog> Logs { get; set; }
    }
}
