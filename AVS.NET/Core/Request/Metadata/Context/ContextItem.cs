using Newtonsoft.Json;
using System.Collections.Generic;

namespace AVS.Core.Request.Metadata.Context
{
    public class ContextItem
    {
        public ContextItem()
        {
            Header = new MetadataHeader();
            Payload = new Dictionary<string, object>();
        }

        [JsonProperty("header")]
        public MetadataHeader Header { get; private set; }

        [JsonProperty("payload")]
        public IDictionary<string, object> Payload { get; private set; }
    }
}
