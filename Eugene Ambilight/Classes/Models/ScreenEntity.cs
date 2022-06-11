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
        private double Width { get; set; }

        /// <summary>
        /// Высота в пикселях.
        /// </summary>
        private double Height { get; set; }

        /// <summary>
        /// Количество пикселей на экране.
        /// </summary>
        private double Resolution { get; set; }

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
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            Resolution = Width * Height;
            int nGCD = GetGreatestCommonDivisor((int)Width, (int)Height);
            WidthAspectRatio = (int)Width / nGCD;
            HeightAspectRatio = (int)Height / nGCD;
        }

        private int GetGreatestCommonDivisor(int width, int height)
            => width == 0 ? height : GetGreatestCommonDivisor(height % width, width);

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

        /// <summary>
        /// Возвращает кортеж с общим количеством вмещаемых светодиодов по горизонтали и вертикали.
        /// </summary>
        /// <param name="ledsCount">Общее количество светодиодов устройства.</param>
        /// <returns><see cref="Tuple"/> с общим количеством вмещаемых светодиодов по горизонтали и вертикали.</returns>
        public (int, int) GetLedsCount(int ledsCount)
        {
            double allParts = WidthAspectRatio * 2 + HeightAspectRatio * 2;
            double xPercent = WidthAspectRatio / allParts * 100;
            double yPercent = HeightAspectRatio / allParts * 100;

            var xLeds = (int)Math.Round((double)ledsCount / 100 * xPercent);
            var yLeds = (int)Math.Round((double)ledsCount / 100 * yPercent);

            return (xLeds * 2, yLeds * 2);
        }
    }
}
