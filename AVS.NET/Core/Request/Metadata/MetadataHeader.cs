using Newtonsoft.Json;

namespace AVS.Core.Request.Metadata
{
    public class MetadataHeader
    {
        [JsonProperty("namespace")]
        public string Namespace { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
