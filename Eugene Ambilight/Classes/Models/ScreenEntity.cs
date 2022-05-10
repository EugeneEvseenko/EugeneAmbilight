using Microsoft.Win32;
using NLog;
using System;
using System.Windows;

namespace Eugene_Ambilight.Classes.Models
{
    /// <summary>
    /// Класс для представления объекта экрана.
    /// </summary>
    public class ScreenEntity
    {
        /// <summary>
        /// Ширина в пикселях.
        /// </summary>
        private double Width { get; set; }

        /// <summary>
        /// Высота в пикселях.
        /// </summary>
        private double Height { get; set; }

        /// <summary>
        /// Количество пикселей на экране.
        /// </summary>
        private double Resolution { get; set; }
        private readonly Logger debugLogger = LogManager.GetLogger("debugLogger");
        public ScreenEntity() {
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            Reload(); 
        }

        private void SystemEvents_DisplaySettingsChanged(object? sender, EventArgs e)
        {
            debugLogger.Info($"Resolution has been changed from {Width}x{Height} to " +
                $"{SystemParameters.PrimaryScreenWidth}x" +
                $"{SystemParameters.PrimaryScreenHeight}");
            Reload();
        }

        /// <summary>
        /// Обновляет данные ширины, высоты и разрешения экрана.
        /// </summary>
        public void Reload()
        {
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            Resolution = Width * Height;
        }

        /// <summary>
        /// Возвращает текущее значение ширины экрана в пикселях.
        /// </summary>
        /// <returns>Ширина экрана в пикселях.</returns>
        public double GetWidth() => Width;

        /// <summary>
        /// Возвращает текущее значение высоты экрана в пикселях.
        /// </summary>
        /// <returns>Высота экрана в пикселях.</returns>
        public double GetHeight() => Height;

        /// <summary>
        /// Возвращает текущее значение разрешения экрана в пикселях.
        /// </summary>
        /// <returns>Разрешение экрана в пикселях.</returns>
        public double GetResolution() => Resolution;
    }
}
