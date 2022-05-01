using Eugene_Ambilight.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Eugene_Ambilight.Classes
{
    /// <summary>
    /// Вспомогательный статический класс.
    /// </summary>
    public static class Helper
    {
        private static Dictionary<string, double> HeightDict = new();
        private static DoubleAnimation opacityShow = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(300))
        {
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseIn }
        };
        private static DoubleAnimation opacityHide = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(300))
        {
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Анимирование объектов по высоте.
        /// </summary>
        /// <param name="animType">Тип анимации.</param>
        /// <param name="element">Объект который наследуется от <see cref="FrameworkElement"/>.</param>
        /// <param name="speed">Скорость анимации.</param>
        /// <param name="color">
        ///     <para>Цвет который необходимо установить объекту.</para>
        ///     <para>Применяется только к объектам наследующихся от класса <see cref="Control"/> к свойству Foreground.</para>
        /// </param>
        /// <param name="withoutDelay">
        ///     <para>Если <see cref="bool">False</see> - метод будет ожидать конца анимации объекта.</para>
        ///     <para>По умолчанию - <see cref="bool">True</see>.</para>
        /// </param>
        /// 
        public static async Task AnimateHeight(AnimType animType, FrameworkElement element, Speed speed = Speed.Normal, ColorText? color = null, bool withoutDelay = true)
        {
            if (!HeightDict.ContainsKey(element.Name)) HeightDict.Add(element.Name, element.ActualHeight);
            if (color.HasValue)
                if (element is Control control) 
                    control.Foreground = new SolidColorBrush(
                        color.Value == ColorText.Success 
                            ? Colors.LightGreen 
                            : color.Value == ColorText.Error 
                                ? Colors.IndianRed 
                                : Colors.LightGray);
            if (animType == AnimType.Show)
            {
                element.Visibility = Visibility.Visible;
                var animation = new DoubleAnimation(0.0, HeightDict[element.Name], TimeSpan.FromMilliseconds((double)speed))
                {
                    EasingFunction = new SineEase { EasingMode = EasingMode.EaseOut }
                };
                element.BeginAnimation(UIElement.OpacityProperty, opacityShow);
                element.BeginAnimation(FrameworkElement.HeightProperty, animation);
            }
            else
            {
                var animation = new DoubleAnimation(HeightDict[element.Name], 0.0, TimeSpan.FromMilliseconds((double)speed))
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                if(!withoutDelay)
                    animation.Completed += delegate (object? sender, EventArgs e)
                    {
                        element.Visibility = Visibility.Collapsed;
                    };
                element.BeginAnimation(UIElement.OpacityProperty, opacityHide);
                element.BeginAnimation(FrameworkElement.HeightProperty, animation);
            }
            if(!withoutDelay)
                await Task.Delay((int)speed);
        }

        public static async Task AnimateTooltip(AnimType animType, Label label, Speed speed = Speed.Normal)
        {
            await AnimateHeight(animType, label, speed);
        }

        /// <summary>
        /// <para>Цветовая анимация объекта.</para>
        /// <para>Применяется к объектам-наследникам классов <see cref="Control"/> или <see cref="Shape"/>.</para>
        /// </summary>
        /// <param name="toColor">Цвет, который должен быть у объекта к концу анимации.</param>
        /// <param name="element">Объект который наследуется от <see cref="FrameworkElement"/>.</param>
        /// <param name="speed">Скорость анимации.</param>
        public static async Task AnimateColor(System.Windows.Media.Color toColor, FrameworkElement element, Speed speed = Speed.MegaSlow)
        {
            var animation = new ColorAnimation(
                toColor, TimeSpan.FromMilliseconds((double)speed)) 
                { 
                    EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut } 
                };
            if(element is Shape shape)
            {
                shape.Fill.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                await Task.Delay((int)speed);
            }
            if (element is Control control)
            {
                control.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                await Task.Delay((int)speed);
            }
        }
        /// <summary>
        /// Искуственная задержка.
        /// </summary>
        /// <param name="millis">Время в миллисекундах.</param>
        public static Task CreateDelay(int millis)
        {
            return Task.Run(() =>
            {
                Thread.Sleep(millis);
            });
        }
        /// <summary>
        /// Сбор информации о цвете определенного пикселя на экране.
        /// </summary>
        /// <param name="x">Координата Х.</param>
        /// <param name="y">Координата У.</param>
        /// <returns>Считанный цвет <see cref="System.Drawing.Color"/>.</returns>
        public static System.Drawing.Color GetPixel(int x, int y)
        {
            using (var bitmap = new Bitmap(1, 1))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(new System.Drawing.Point(x, y), new System.Drawing.Point(0, 0), new System.Drawing.Size(1, 1));
                }
                return bitmap.GetPixel(0, 0);
            }
        }
    }
}
