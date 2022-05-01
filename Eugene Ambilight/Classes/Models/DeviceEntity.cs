using Newtonsoft.Json;

namespace Eugene_Ambilight.Classes.Models
{
    /// <summary>
    /// Модель устройства
    /// </summary>
    public class DeviceEntity
    {
        /// <summary>
        /// Имя устройства указанное в прошивке.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; }

        /// <summary>
        /// Токен устройства указанный в прошивке.<br></br><br></br>
        /// <i>Токен берем из Blynk.</i>
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; }

        /// <summary>
        /// Количество светодиодов в ленте.
        /// </summary>
        [JsonProperty("leds")]
        public int Leds { get; }

        /// <param name="name">Имя устройства указанное в прошивке.</param>
        /// <param name="token">Токен устройства указанный в прошивке.</param>
        /// <param name="leds">Количество светодиодов в ленте.</param>
        public DeviceEntity(string name, string token, int leds) { Name = name; Token = token; Leds = leds; }
    }
}
