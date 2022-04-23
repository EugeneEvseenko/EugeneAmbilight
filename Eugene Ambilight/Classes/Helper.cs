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
using System.Windows.Media.Animation;

namespace Eugene_Ambilight.Classes
{
    public static class Helper
    {
        public static Dictionary<string, double> HeightDict = new();
        public static async Task AnimateHeight(AnimType animType, FrameworkElement element, Speed speed = Speed.Slow)
        {
            if (!HeightDict.ContainsKey(element.Name)) HeightDict.Add(element.Name, element.ActualHeight);
            if (animType == AnimType.Show)
            {
                element.Visibility = Visibility.Visible;
                var animation = new DoubleAnimation(0.0, HeightDict[element.Name], TimeSpan.FromMilliseconds((double)speed))
                {
                    EasingFunction = new SineEase { EasingMode = EasingMode.EaseOut }
                };
                element.BeginAnimation(FrameworkElement.HeightProperty, animation);
            }
            else
            {
                var animation = new DoubleAnimation(HeightDict[element.Name], 0.0, TimeSpan.FromMilliseconds((double)speed))
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                animation.Completed += delegate (object? sender, EventArgs e)
                {
                    element.Visibility = Visibility.Hidden;
                };
                element.BeginAnimation(FrameworkElement.HeightProperty, animation);
            }
            await Task.Delay((int)speed);
        }

        public static async Task AnimateTooltip(AnimType animType, Label label, Speed speed = Speed.Fast)
        {
            await AnimateHeight(animType, label, speed);
        }
        public static void Animate(AnimType animType, FrameworkElement element, DependencyProperty property)
        {
            if (animType == AnimType.Show)
            {
                element.Visibility = Visibility.Visible;

            }
        }
        public static Task CreateDelay(int millis)
        {
            return Task.Run(() =>
            {
                Thread.Sleep(millis);
            });
        }
        public static Color GetPixel(int x, int y)
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
