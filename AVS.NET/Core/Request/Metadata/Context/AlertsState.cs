using AVS.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace AVS.Core.Request.Metadata.Context
{
    public class AlertsState : ContextItem
    {
        public AlertsState(IEnumerable<Alert> allAlerts, IEnumerable<Alert> activeAlerts)
        {
            Header.Namespace = "Alerts";
            Header.Name = "AlertsState";

            Payload.Add("allAlerts", allAlerts);
            Payload.Add("activeAlerts", activeAlerts);
        }
    }

    public class Alert
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AlertType Type { get; set; }

        [JsonProperty("scheduledTime")]
        public string ScheduledTime { get; set; }
    }
}
