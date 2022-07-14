using Eugene_Ambilight.Extentions;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;

namespace Eugene_Ambilight.Classes.Requests
{
    /// <summary>
    /// Модель запроса обновления
    /// </summary>
    public class UpdateRequest
    {
        /// <summary>
        /// Текущая версия приложения (VersionCode)
        /// </summary>
        [JsonProperty("version")]
        public int Version { get; set; }

        public UpdateRequest()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            Version = fvi.FileVersion.ToInt32(0);
        }
    }
}
