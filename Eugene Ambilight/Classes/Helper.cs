using Eugene_Ambilight.Classes.Models;
using Eugene_Ambilight.Enums;
using Eugene_Ambilight.Windows;
using Newtonsoft.Json;
using NLog;
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
        private static Logger ErrLogger { get; set; } = LogManager.GetLogger("errLogger");
        private static Logger DebugLogger { get; set; } = LogManager.GetLogger("debugLogger");
        private static Dictionary<string, double> HeightDict = new();
        private static Dictionary<string, double> WidthDict = new();
        private static DoubleAnimation rotateAnimation = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromMilliseconds(1000)))
        {
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut },
            RepeatBehavior = RepeatBehavior.Forever
        };
        private static DoubleAnimation opacityShow = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(300))
        {
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseIn }
        };
        private static DoubleAnimation opacityHide = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(300))
        {
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
        };

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
            var animation = new DoubleAnimation();
            var toValue = 0.0;
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

        /// <summary>
        /// Анимирование окна.
        /// </summary>
        /// <param name="element">Окно <see cref="Window"/>.</param>
        /// <param name="position">Кортеж координат конечной точки анимации. X, Y.</param>
        /// <param name="speed">Скорость анимации.</param>
        /// <param name="withoutDelay">
        ///     <para>Если <see cref="bool">False</see> - метод будет ожидать конца анимации объекта.</para>
        ///     <para>По умолчанию - <see cref="bool">True</see>.</para>
        /// </param>
        public static async Task AnimateWindowPosition(Window element, (double, double) position, Speed speed = Speed.Normal, bool withoutDelay = true)
        {
            var leftAnim = new DoubleAnimation(position.Item1, TimeSpan.FromMilliseconds((double)speed))
            {
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseOut }
            };
            var topAnim = new DoubleAnimation(position.Item2, TimeSpan.FromMilliseconds((double)speed))
            {
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseIn }
            };
            element.Focus();
            leftAnim.Completed += delegate (object animSender, EventArgs anim)
             {
                 //Task.Delay((int)speed);
                 element.BeginAnimation(Window.LeftProperty, null);
                 element.Left = position.Item1;

             };
            topAnim.Completed += delegate(object animSender, EventArgs anim)
            {
                //Task.Delay((int)speed);
                element.BeginAnimation(Window.TopProperty, null);
                element.Top = position.Item2;
            };

            element.BeginAnimation(Window.LeftProperty, leftAnim);
            element.BeginAnimation(Window.TopProperty, topAnim);
            if (!withoutDelay)
                await Task.Delay((int)speed);
        }

        /// <summary>
        /// Метод для расположения коллекций окон <see cref="PointWindow"/> в нужном порядке.
        /// </summary>
        /// <param name="windows">Список окон.</param>
        /// <param name="place">Место расположения. </param>
        /// <param name="screen">"Экземпляр объекта <see cref="ScreenEntity"/> для вспомогательных операции.</param>
        /// <param name="ledsCount">Общее количество светодиодов.</param>
        public static async Task PlaceWindows((WindowsPlace, List<PointWindow>) windows, ScreenEntity screen, int ledsCount)
        {
            double sizeX = 0, sizeY = 0;
            Func<int, double> calcTop = (x) => 0, calcLeft = (x) => 0;
            switch (windows.Item1)
            {
                case WindowsPlace.CenterLine:
                    {
                        sizeX = Math.Round(screen.GetWidth() / ledsCount);
                        sizeY = sizeX <= 50 ? sizeX * 2 : sizeX;
                        calcTop = (i) => screen.GetHeight() / 2 - sizeY / 2;
                        calcLeft = (i) => i * sizeX;
                    }
                    break;
                case WindowsPlace.Top:
                    {
                        sizeX = Math.Round(screen.GetWidth() / windows.Item2.Count);
                        sizeY = Math.Round(screen.GetHeight() * 0.1);
                        calcLeft = (i) => i * sizeX;
                    }
                    break;
                case WindowsPlace.Bottom:
                    {
                        sizeX = Math.Round(screen.GetWidth() / windows.Item2.Count);
                        sizeY = Math.Round(screen.GetHeight() * 0.1);
                        calcTop = (i) => screen.GetHeight() - sizeY;
                        calcLeft = (i) => screen.GetWidth() - i * sizeX - sizeX;
                    }
                    break;
                case WindowsPlace.Right:
                    {
                        sizeX = Math.Round(screen.GetHeight() * 0.1);
                        sizeY = Math.Round(screen.GetHeight() / windows.Item2.Count);
                        calcTop = (i) => i * sizeY;
                        calcLeft = (i) => screen.GetWidth() - sizeX;
                    }
                    break;
                case WindowsPlace.Left:
                    {
                        sizeX = Math.Round(screen.GetHeight() * 0.1);
                        sizeY = Math.Round(screen.GetHeight() / windows.Item2.Count);
                        calcTop = (i) => screen.GetHeight() - sizeY * i - sizeY;
                    }
                    break;
            }
            
            DebugLogger.Debug("\n" + JsonConvert.SerializeObject(new
            {
                WindowsPlace = windows.Item1.ToString(),
                SizeX = sizeX,
                SizeY = sizeY,
                ScreenInfo = new
                {
                    Width = screen.GetWidth(),
                    Height = screen.GetHeight(),
                    HeightAspectRatio = screen.GetHeightAspectRatio(),
                    WidthAspectRatio = screen.GetWidthAspectRatio(),
                },
                LedsCount = ledsCount
            }, Formatting.Indented));

            List<Task> tasks = new();
            for (var i = 0; i < windows.Item2.Count; i++)
            {
                var topTo = calcTop.Invoke(i);
                var leftTo = calcLeft.Invoke(i);
                DebugLogger.Debug($"Window: {windows.Item2[i].LedNumber.Content} X:{leftTo} Y:{topTo}");
                windows.Item2[i].Width = sizeX;
                windows.Item2[i].Height = sizeY;
                if (!windows.Item2[i].IsVisible)
                    windows.Item2[i].Show();
                if (windows.Item2[i].Left != leftTo || windows.Item2[i].Top != topTo)
                    tasks.Add(AnimateWindowPosition(windows.Item2[i], (leftTo, topTo), Speed.Normal));
                //if (windows[i].Left != leftTo || windows[i].Top != topTo)
                    //await AnimateWindowPosition(windows[i], (leftTo, topTo), Speed.Normal);
            }
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Вспомогательный метод анимации этапа <see cref="WindowShowing.ChoosingLocationLEDs"/>.
        /// </summary>
        /// <param name="rect">Объект <see cref="System.Windows.Shapes.Rectangle rect"/></param>
        /// <param name="index">Индекс выбранного расположения светодиодов.</param>
        /// <param name="withoutDelay">
        ///     <para>Если <see cref="bool">False</see> - метод будет ожидать конца анимации объекта.</para>
        ///     <para>По умолчанию - <see cref="bool">True</see>.</para>
        /// </param>
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
        /// Искусственная задержка.
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

        /// <summary>
        /// Набор окончаний для светодиодов
        /// </summary>
        public static readonly string[] LedsEnding = new string[] { "светодиод", "светодиода", "светодиодов"};

        /// <summary>
        /// Функция для склонения слов относительно числа.
        /// </summary>
        /// <param name="number">Число</param>
        /// <param name="titles">Набор из трёх вариантов слов</param>
        /// <param name="withoutNumber">
        ///     <para>Если <see cref="bool">True</see> - функция вернет только выбранное слово, 
        ///           иначе подставит число к нему в начале.</para>
        ///     <para>По умолчанию - <see cref="bool">False</see>.</para>
        /// </param>
        /// <returns></returns>
        public static string GetEnding(int number, string[] titles, bool withoutNumber = false)
        {
            var cases = new byte[] {2, 0, 1, 1, 1, 2};
            return 
                withoutNumber 
                ? "" 
                : $"{number} " +
                titles[(number % 100 > 4 && number % 100 < 20) ? 2 : cases[Math.Min(number % 10, 5)]];
        }

        public static void RotateAnimation(Animatable element, bool stop = false)
        {
            element.BeginAnimation(RotateTransform.AngleProperty, stop ? null : rotateAnimation);
        }
    }
}
