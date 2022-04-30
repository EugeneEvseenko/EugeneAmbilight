using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eugene_Ambilight.Classes
{
    public class AmbilightRequest
    {
        [JsonProperty("Items")]
        public List<RgbLed>? Items { get; set; }
        public int ItemsCount { get; set; }
        public AmbilightRequest(List<RgbLed> rgbItems)
        {
            Items = rgbItems;
            ItemsCount = rgbItems.Count;
        }
    }
}
