using Eugene_Ambilight.Extentions;
using Microsoft.Win32;
using NLog;
using System;
using System.Windows;

namespace Eugene_Ambilight.Classes.Models
{
    /// <summary>
    /// Класс для представления объекта экрана.
    /// </summary>
    [Serializable]
    public class ScreenEntity
    {
        /// <summary>
        /// Ширина в пикселях.
        /// </summary>
        private int Width { get; set; }

        /// <summary>
        /// Высота в пикселях.
        /// </summary>
        private int Height { get; set; }

        /// <summary>
        /// Количество пикселей на экране.
        /// </summary>
        private int Resolution { get; set; }

        /// <summary>
        /// Соотношение сторон по ширине
        /// </summary>
        private int WidthAspectRatio { get; set; }

        /// <summary>
        /// Соотношение сторон по высоте
        /// </summary>
        private int HeightAspectRatio { get; set; }

        [NonSerialized]
        private readonly Logger debugLogger = LogManager.GetLogger("debugLogger");
        public ScreenEntity() {
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            Reload(); 
        }

        private void SystemEvents_DisplaySettingsChanged(object? sender, EventArgs e)
        {
            var temp = this.DeepClone();
            Reload();
            debugLogger.Info($"Resolution has been changed from " +
                $"{temp.Width}x{temp.Height} ({temp.WidthAspectRatio}:{temp.HeightAspectRatio}) to " +
                $"{Width}x{Height} ({WidthAspectRatio}:{HeightAspectRatio}).");
        }
        
        /// <summary>
        /// Обновляет данные ширины, высоты и разрешения экрана.
        /// </summary>
        public void Reload()
        {
            Width = (int)SystemParameters.PrimaryScreenWidth;
            Height = (int)SystemParameters.PrimaryScreenHeight;
            Resolution = Width * Height;
            int nGCD = GetGreatestCommonDivisor(Width, Height);
            WidthAspectRatio = Width / nGCD;
            HeightAspectRatio = Height / nGCD;
        }

        private int GetGreatestCommonDivisor(int width, int height)
            => width == 0 ? height : GetGreatestCommonDivisor(height % width, width);

        /// <summary>
        /// Возвращает текущее значение ширины экрана в пикселях.
        /// </summary>
        /// <returns>Ширина экрана в пикселях.</returns>
        public int GetWidth() => Width;

        /// <summary>
        /// Возвращает текущее значение высоты экрана в пикселях.
        /// </summary>
        /// <returns>Высота экрана в пикселях.</returns>
        public int GetHeight() => Height;

        /// <summary>
        /// Возвращает текущее значение разрешения экрана в пикселях.
        /// </summary>
        /// <returns>Разрешение экрана в пикселях.</returns>
        public int GetResolution() => Resolution;

        /// <summary>
        /// Возвращает текущее значение соотношения экрана по ширине.
        /// </summary>
        /// <returns>Соотношение экрана по ширине.</returns>
        public int GetWidthAspectRatio() => WidthAspectRatio;

        /// <summary>
        /// Возвращает текущее значение соотношения экрана по высоте.
        /// </summary>
        /// <returns>Соотношение экрана по высоте.</returns>
        public int GetHeightAspectRatio() => HeightAspectRatio;
    }
}
