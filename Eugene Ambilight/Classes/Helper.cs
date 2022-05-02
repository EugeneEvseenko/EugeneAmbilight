﻿using Eugene_Ambilight.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        private static Dictionary<string, double> WidthDict = new();
        private static DoubleAnimation opacityShow = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(300))
        {
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseIn }
        };
        private static DoubleAnimation opacityHide = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(300))
        {
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Анимирование объектов.
        /// </summary>
        /// <param name="animAction">Событие анимации.</param>
        /// <param name="animType">Тип анимации.</param>
        /// <param name="element">Анимируемый объект который наследуется от <see cref="FrameworkElement"/>.</param>
        /// <param name="speed">Скорость анимации.</param>
        /// <param name="color">
        ///     <para>Цвет который необходимо установить объекту.</para>
        ///     <para>Применяется только к объектам наследующихся от класса <see cref="Control"/> к свойству Foreground.</para>
        /// </param>
        /// <param name="withoutDelay">
        ///     <para>Если <see cref="bool">False</see> - метод будет ожидать конца анимации объекта.</para>
        ///     <para>По умолчанию - <see cref="bool">True</see>.</para>
        /// </param>
        /// <param name="enableFade">Параллельная анимация появления/исчезновения. По умолчанию включена.</param>
        /// <param name="text">Используются совместно с анимацией текста.</param>
        public static async Task AnimateDouble(AnimAction animAction, AnimType animType, FrameworkElement element, Speed speed = Speed.Normal, ColorText? color = null, bool withoutDelay = true, bool enableFade = true, string text = "")
        {
            if (!HeightDict.ContainsKey(element.Name)) HeightDict.Add(element.Name, element.ActualHeight);
            if (!WidthDict.ContainsKey(element.Name)) WidthDict.Add(element.Name, element.ActualWidth);
            if (color.HasValue)
                if (element is Control control) 
                    control.Foreground = new SolidColorBrush(
                        color.Value == ColorText.Success 
                            ? Colors.LightGreen 
                            : color.Value == ColorText.Error 
                                ? Colors.IndianRed 
                                : Colors.LightGray);
            DoubleAnimation animation = new DoubleAnimation();
            double toValue = 0.0;
            switch (animType)
            {
                case AnimType.Height: toValue = HeightDict[element.Name]; break;
                case AnimType.Width: toValue = WidthDict[element.Name]; break;
            }
            if (animAction == AnimAction.Show)
            {
                element.Visibility = Visibility.Visible;
                animation = new DoubleAnimation(0.0, toValue, TimeSpan.FromMilliseconds((double)speed))
                {
                    EasingFunction = new SineEase { EasingMode = EasingMode.EaseOut }
                };
                if (enableFade)
                    element.BeginAnimation(UIElement.OpacityProperty, opacityShow);
            }
            else if(animAction == AnimAction.Hide || animAction == AnimAction.HideAndShow)
            {
                animation = new DoubleAnimation(toValue, 0.0, TimeSpan.FromMilliseconds((double)speed))
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                if(!withoutDelay)
                    animation.Completed += delegate (object? sender, EventArgs e)
                    {
                        if (animAction == AnimAction.Hide)
                            element.Visibility = Visibility.Hidden;
                        else
                        {
                            if (element is Label label)
                                label.Content = text;
                            if (element is TextBlock textBlock)
                                textBlock.Text = text;
                            _ = AnimateDouble(AnimAction.Show, animType, element, speed, color, withoutDelay, enableFade);
                        }  
                    };
                if (enableFade)
                    element.BeginAnimation(UIElement.OpacityProperty, opacityHide);
            }
            switch (animType)
            {
                case AnimType.Height: element.BeginAnimation(FrameworkElement.HeightProperty, animation); break;
                case AnimType.Width: element.BeginAnimation(FrameworkElement.WidthProperty, animation); break;
            }
            if (!withoutDelay)
                await Task.Delay((int)speed);
        }

        private static DoubleAnimation animationWidthToLine = new DoubleAnimation(200, TimeSpan.FromMilliseconds((double)Speed.Normal))
        {
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut }
        };
        private static DoubleAnimation animationToZero = new DoubleAnimation(0, TimeSpan.FromMilliseconds((double)Speed.Normal))
        {
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut }
        };
        private static DoubleAnimation animationToBasicHeightLine = new DoubleAnimation(2, TimeSpan.FromMilliseconds((double)Speed.Normal))
        {
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut }
        };
        private static DoubleAnimation animationHeightToRect = new DoubleAnimation(150, TimeSpan.FromMilliseconds((double)Speed.Normal))
        {
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut }
        };
        private static DoubleAnimation animationRadiusToRect = new DoubleAnimation(10, TimeSpan.FromMilliseconds((double)Speed.Normal))
        {
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut }
        };
        public static async Task AnimateRect(System.Windows.Shapes.Rectangle rect, byte index, bool withoutDelay = true)
        {
            switch (index)
            {
                case 0:
                    rect.BeginAnimation(FrameworkElement.WidthProperty, animationWidthToLine);
                    rect.BeginAnimation(FrameworkElement.HeightProperty, animationToBasicHeightLine);
                    rect.BeginAnimation(System.Windows.Shapes.Rectangle.RadiusXProperty, animationToZero);
                    rect.BeginAnimation(System.Windows.Shapes.Rectangle.RadiusYProperty, animationToZero);break;
                case 1:
                    rect.BeginAnimation(FrameworkElement.WidthProperty, animationToBasicHeightLine);
                    rect.BeginAnimation(FrameworkElement.HeightProperty, animationToBasicHeightLine);
                    rect.BeginAnimation(System.Windows.Shapes.Rectangle.RadiusXProperty, animationToZero);
                    rect.BeginAnimation(System.Windows.Shapes.Rectangle.RadiusYProperty, animationToZero); break;
                case 2:
                    rect.BeginAnimation(FrameworkElement.WidthProperty, animationWidthToLine);
                    rect.BeginAnimation(FrameworkElement.HeightProperty, animationHeightToRect);
                    rect.BeginAnimation(System.Windows.Shapes.Rectangle.RadiusXProperty, animationRadiusToRect);
                    rect.BeginAnimation(System.Windows.Shapes.Rectangle.RadiusYProperty, animationRadiusToRect); break;
            }
            if (!withoutDelay)
                await CreateDelay((int)Speed.Normal);
        }

        /// <summary>
        /// <para>Цветовая анимация объекта.</para>
        /// <para>Применяется к объектам-наследникам классов <see cref="Control"/> или <see cref="Shape"/>.</para>
        /// </summary>
        /// <param name="toColor">Цвет, который должен быть у объекта к концу анимации.</param>
        /// <param name="element">Объект который наследуется от <see cref="FrameworkElement"/>.</param>
        /// <param name="speed">Скорость анимации.</param>
        /// <param name="withoutDelay">
        ///     <para>Если <see cref="bool">False</see> - метод будет ожидать конца анимации объекта.</para>
        ///     <para>По умолчанию - <see cref="bool">True</see>.</para>
        /// </param>
        public static async Task AnimateColor(System.Windows.Media.Color toColor, FrameworkElement element, Speed speed = Speed.MegaSlow, bool withoutDelay = true)
        {
            var animation = new ColorAnimation(
                toColor, TimeSpan.FromMilliseconds((double)speed)) 
                { 
                    EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut } 
                };
            if(element is Shape shape)
            {
                shape.Fill.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                if (!withoutDelay)
                    await Task.Delay((int)speed);
            }
            if (element is Control control)
            {
                control.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                if (!withoutDelay)
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
