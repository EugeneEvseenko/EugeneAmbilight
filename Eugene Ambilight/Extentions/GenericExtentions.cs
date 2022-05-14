using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Eugene_Ambilight.Extentions
{
    public static class GenericExtentions
    {
        /// <summary>
        /// Клонирование объекта без привязки по ссылке.
        /// </summary>
        /// <returns>Клонированный объект не ссылающийся на исходный.</returns>
        public static T DeepClone<T>(this T obj) where T : class
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
