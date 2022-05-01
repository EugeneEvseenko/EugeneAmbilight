using Newtonsoft.Json;
using System;

namespace Eugene_Ambilight.Classes.Models
{
    /// <summary>
    /// Модель одного пикселя ленты.
    /// </summary>
    [Serializable]
    public class RgbLed
    {
        /// <summary>
        /// Индекс пикселя в ленте.
        /// </summary>
        [JsonProperty("i")]
        public int Index { get; set; }

        /// <summary>
        /// Красный цвет. (0 - 255)
        /// </summary>
        [JsonProperty("r")]
        public byte Red { get; set; }

        /// <summary>
        /// Зеленный цвет. (0 - 255)
        /// </summary>
        [JsonProperty("g")]
        public byte Green { get; set; }

        /// <summary>
        /// Синий цвет. (0 - 255)
        /// </summary>
        [JsonProperty("b")]
        public byte Blue { get; set; }
    }
}
