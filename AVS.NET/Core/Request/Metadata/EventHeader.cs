using Newtonsoft.Json;

namespace AVS.Core.Request.Metadata
{
    public class EventHeader : MetadataHeader
    {
        [JsonProperty("messageId")]
        public string MessageId { get; set; }
    }
}
