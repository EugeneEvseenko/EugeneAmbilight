namespace Eugene_Ambilight.Classes.Requests
{
    /// <summary>
    /// Модель POST запроса на смену состояния Ambilight.
    /// </summary>
    public class ChangeStateRequest
    {
        /// <summary>
        /// Состояние Ambilight.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("state")]
        public bool? State { get; set; }
    }
}
