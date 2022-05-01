using Eugene_Ambilight.Classes.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Eugene_Ambilight.Classes.Requests
{
    /// <summary>
    /// Модель POST запроса с информацией об измененных цветах пикселях.
    /// </summary>
    public class AmbilightRequest
    {
        /// <summary>
        /// Список с пикселями.
        /// </summary>
        [JsonProperty("Items")]
        public List<RgbLed>? Items { get; set; }

        /// <summary>
        /// Количество пикселей в списке.
        /// </summary>
        public int ItemsCount { get; set; }

        /// <param name="rgbItems">Список с пикселями.</param>
        public AmbilightRequest(List<RgbLed> rgbItems)
        {
            Items = rgbItems;
            ItemsCount = rgbItems.Count;
        }
    }
}
