using Newtonsoft.Json;
using System;

namespace Eugene_Ambilight.Classes.Models
{
    public class UpdateResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("changes")]
        public string Changes { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }
    }
}
