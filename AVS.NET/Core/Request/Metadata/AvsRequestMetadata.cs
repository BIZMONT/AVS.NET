using AVS.Core.Request.Metadata.Context;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AVS.Core.Request.Metadata
{
    public class AvsRequestMetadata
    {
        public AvsRequestMetadata()
        {
            Event = new Event();
            Context = new List<ContextItem>();
        }

        [JsonProperty("context")]
        public ICollection<ContextItem> Context { get; set; }

        [JsonProperty("event")]
        public Event Event { get; set; }
    }
}
