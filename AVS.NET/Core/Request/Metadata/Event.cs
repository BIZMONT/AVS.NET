using Newtonsoft.Json;
using System.Collections.Generic;

namespace AVS.Core.Request.Metadata
{
    public class Event
    {
        public Event()
        {
            Header = new EventHeader();
            Payload = new Dictionary<string, object>();
        }

        [JsonProperty("header")]
        public EventHeader Header { get; set; }

        [JsonProperty("payload")]
        public IDictionary<string, object> Payload { get; set; }
    }
}
