using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eugene_Ambilight.Classes
{
    [Serializable]
    public class RgbLed
    {
        [JsonProperty("i")]
        public int Index { get; set; }
        [JsonProperty("r")]
        public int Red { get; set; }

        [JsonProperty("g")]
        public int Green { get; set; }

        [JsonProperty("b")]
        public int Blue { get; set; }
    }
}
