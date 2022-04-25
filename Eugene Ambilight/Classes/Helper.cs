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
    public static class Helper
    {
        public static Dictionary<string, double> HeightDict = new();
        public static DoubleAnimation opacityShow = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(300))
        {
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseIn }
        };
        public static DoubleAnimation opacityHide = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(300))
        {
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
        };
        
        public static async Task AnimateHeight(AnimType animType, FrameworkElement element, Speed speed = Speed.Normal, ColorText? color = null)
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
                animation.Completed += delegate (object? sender, EventArgs e)
                {
                    element.Visibility = Visibility.Hidden;
                };
                element.BeginAnimation(UIElement.OpacityProperty, opacityHide);
                element.BeginAnimation(FrameworkElement.HeightProperty, animation);
            }
            await Task.Delay((int)speed);
        }

        public static async Task AnimateTooltip(AnimType animType, Label label, Speed speed = Speed.Normal)
        {
            await AnimateHeight(animType, label, speed);
        }
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
        public static Task CreateDelay(int millis)
        {
            return Task.Run(() =>
            {
                Thread.Sleep(millis);
            });
        }
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
