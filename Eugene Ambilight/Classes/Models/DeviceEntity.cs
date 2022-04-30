using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eugene_Ambilight.Classes
{
    public class DeviceEntity
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("leds")]
        public int Leds { get; set; }
    }
}
