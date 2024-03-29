﻿using Eugene_Ambilight.Classes;
using Eugene_Ambilight.Classes.Models;
using Eugene_Ambilight.Enums;
using Eugene_Ambilight.Integrations;
using Eugene_Ambilight.Properties;
using Eugene_Ambilight.Windows;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Eugene_Ambilight.Integrations.Interfaces;

namespace Eugene_Ambilight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void TitleGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
        private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Settings.Default.DeviceInfo))
            {
                DebugLogger.Info($"App started with saved device {Settings.Default.DeviceInfo}");
                if (LoadDevice(Settings.Default.DeviceInfo))
                {
                    DebugLogger.Info($"Loading {_targetDevice.Name} device was successful");
                    await ShowWindow(WindowShowing.ChoosingAddingMethodDevice);
                    //await ShowWindow(WindowShowing.DeviceManagement);
                }
                else
                {
                    ErrLogger.Error($"Device <>{Settings.Default.DeviceInfo}<> loading failed. Reset settigns.");
                    Settings.Default.Reset();
                    Settings.Default.Save();
                    await ShowWindow(WindowShowing.ChoosingAddingMethodDeviceAfterError);
                }
            }
            else
            {
                await ShowWindow(WindowShowing.ChoosingAddingMethodDevice);
                DebugLogger.Info("App started without device");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _pointList.ForEach(x => x.Close());
        }

        #region Variables
        private bool _isLedsReversed = false;
        private Logger ErrLogger { get; set; } = LogManager.GetLogger("errLogger");
        private Logger DebugLogger { get; set; } = LogManager.GetLogger("debugLogger");
        private DeviceEntity _targetDevice;
        private byte _targetPlace = 0;
        private readonly string[] _ledPlaceVariants = new string[] { "Линия", "Свой", "Прямоугольник" };
        private List<PointWindow> _pointList = new();
        private readonly ScreenEntity _screenInfo = new();
        private bool _mockEnabled = true;
        private IMainClient _mainClient = new MainClient();
        #endregion
        //private async void btnSend_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        IEnumerable<int> ids = Enumerable.Range(0, 60);
        //        Stopwatch allSW = Stopwatch.StartNew();
        //        foreach (var kek in ids)
        //        {
        //            List<RgbLed> items = new List<RgbLed>() {
        //                new RgbLed(){Index = rnd.Next(0, 6), Red = (byte)rnd.Next(0, 256), Green = (byte)rnd.Next(0, 256), Blue = (byte)rnd.Next(0, 256)},
        //                new RgbLed(){Index = rnd.Next(0, 6), Red = (byte)rnd.Next(0, 256), Green = (byte)rnd.Next(0, 256), Blue = (byte)rnd.Next(0, 256)},
        //                new RgbLed(){Index = rnd.Next(0, 6), Red = (byte)rnd.Next(0, 256), Green = (byte)rnd.Next(0, 256), Blue = (byte)rnd.Next(0, 256)}
        //            };

        //            AmbilightRequest dataItem = new(items);
        //            string json = JsonConvert.SerializeObject(dataItem);
        //            HttpRequestMessage AmbilightRgbRequest = new()
        //            {
        //                Method = HttpMethod.Post,
        //                Content = new StringContent(json, Encoding.UTF8, "application/json")
        //            };
        //            AmbilightRgbRequest.RequestUri = new Uri("/ambilight", UriKind.Relative);
        //            Stopwatch stopwatch = Stopwatch.StartNew();
        //            var response = await httpClient.SendAsync(AmbilightRgbRequest);
        //            stopwatch.Stop();
        //            PingLabel.Content = $"Ping: {stopwatch.ElapsedMilliseconds} ms";
        //        }
        //        allSW.Stop();
        //        PointLabel.Content = $"Elapsed {ids.Count()} - {allSW.ElapsedMilliseconds} ms";
        //    }catch (HttpRequestException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //private async void AmbilightState_Changed(object sender, RoutedEventArgs e)
        //{
        //    ChangeStateRequest changeState = new() { State = AmbilightState.IsChecked };
        //    string json = JsonConvert.SerializeObject(changeState);
        //    HttpRequestMessage AmbiChangeStateRequest = new()
        //    {
        //        Method = HttpMethod.Post,
        //        RequestUri = new Uri("ambilight-state", UriKind.Relative),
        //        Content = new StringContent(json, Encoding.UTF8, "application/json")
        //    };
        //    Stopwatch stopwatch = Stopwatch.StartNew();
        //    var response = await httpClient.SendAsync(AmbiChangeStateRequest);
        //    stopwatch.Stop();
        //    PingLabel.Content = $"Ping: {stopwatch.ElapsedMilliseconds} ms";
        //}

        public async Task ShowWindow(WindowShowing show)
        {
            if (FirstStage.Visibility == Visibility.Visible)
                await Helper.AnimateDouble(AnimAction.Hide, AnimType.Height, FirstStage);
            if (SecondStageManual.Visibility == Visibility.Visible)
                await Helper.AnimateDouble(AnimAction.Hide, AnimType.Height, SecondStageManual);
            if (SecondStageAuto.Visibility == Visibility.Visible)
                await Helper.AnimateDouble(AnimAction.Hide, AnimType.Height, SecondStageAuto);
            if (ThirdStage.Visibility == Visibility.Visible)
                await Helper.AnimateDouble(AnimAction.Hide, AnimType.Height, ThirdStage);

            if (ZoneManagementGrid.Visibility == Visibility.Visible)
            {
                await Helper.AnimateDouble(AnimAction.Hide, AnimType.Height, ZoneManagementGrid);
                _pointList.ForEach(x => x.Hide());
            }
                
            if (DeviceGrid.Visibility == Visibility.Visible)
                await Helper.AnimateDouble(AnimAction.Hide, AnimType.Height, DeviceGrid);

            switch (show)
            {
                case WindowShowing.ChoosingAddingMethodDevice:
                case WindowShowing.ChoosingAddingMethodDeviceAfterError:
                    ErrorLoadingDeviceTB.Visibility = show == WindowShowing.ChoosingAddingMethodDeviceAfterError 
                        ? Visibility.Visible
                        : Visibility.Hidden;
                    await Helper.AnimateDouble(AnimAction.Show, AnimType.Height, FirstStage);break;
                case WindowShowing.FindDeviceManual:
                    await Helper.AnimateDouble(AnimAction.Show, AnimType.Height, SecondStageManual); break;
                case WindowShowing.FindDeviceAuto:
                    await Helper.AnimateDouble(AnimAction.Show, AnimType.Height, SecondStageAuto); break;
                case WindowShowing.ConfirmingFindedDevice:
                    await Helper.AnimateDouble(AnimAction.Show, AnimType.Height, ThirdStage); break;

                case WindowShowing.ChoosingLocationLEDs:
                    await Helper.AnimateDouble(AnimAction.Show, AnimType.Height, ZoneManagementGrid); break;
                case WindowShowing.DeviceManagement:
                    await Helper.AnimateDouble(AnimAction.Show, AnimType.Height, DeviceGrid); break;
            }
        }

        private async void ManualBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            if(AutoLabel.IsVisible)
                await Helper.AnimateDouble(AnimAction.Hide, AnimType.Height, AutoLabel, Speed.Fast);
            await Helper.AnimateDouble(AnimAction.Show, AnimType.Height, ManualLabel, color: ColorText.Regular);
        }

        private async void ManualBtn_MouseLeave(object sender, MouseEventArgs e) 
            => await Helper.AnimateDouble(AnimAction.Hide, AnimType.Height, ManualLabel, Speed.Normal);

        private async void AutoBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            if (AutoLabel.IsVisible)
                await Helper.AnimateDouble(AnimAction.Hide, AnimType.Height, ManualLabel, Speed.Fast);
            await Helper.AnimateDouble(AnimAction.Show, AnimType.Height, AutoLabel, color: ColorText.Regular);
        }

        private async void AutoBtn_MouseLeave(object sender, MouseEventArgs e)
            => await Helper.AnimateDouble(AnimAction.Hide, AnimType.Height, AutoLabel, Speed.Normal);

        private async void ManualBtn_Click(object sender, RoutedEventArgs e)
        {
            await ShowWindow(WindowShowing.FindDeviceManual);
            IPTextBox.Focus();
            IPTextBox.SelectionStart = IPTextBox.Text.Length;
            IPTextBox.SelectionLength = 0;
        }

        private async void AutoBtn_Click(object sender, RoutedEventArgs e)
        {
            await ShowWindow(WindowShowing.FindDeviceAuto);
            StartAddress.Focus();
            StartAddress.SelectionStart = StartAddress.Text.Length;
            StartAddress.SelectionLength = 0;
        }

        private async void BackToFirstStage(object sender, RoutedEventArgs e)
        {
            await ShowWindow(WindowShowing.ChoosingAddingMethodDevice);
        }

        /// <summary>
        /// Вывод ошибки в <see cref="Label"/>. Вспомогательный метод для <see cref="CheckIP"/>.
        /// </summary>
        /// <param name="errLabel"><see cref="Label"/> в который нужно вывести ошибку.</param>
        /// <param name="content">Текст ошибки.</param>
        /// <param name="hideError">Нужно ли скрыть <see cref="Label"/> с экрана.</param>
        /// <returns></returns>
        public static async Task<bool> GoError(Label errLabel, string? content = null, bool hideError = false)
        {
            if (hideError) { 
                await Helper.AnimateDouble(AnimAction.Hide, AnimType.Height, errLabel, withoutDelay: true); 
                return false; 
            }
            errLabel.Content = content;
            await Helper.AnimateDouble(AnimAction.Show, AnimType.Height, errLabel, color: ColorText.Error);
            return false;
        }
        /// <summary>
        ///     Валидация TextBox-ов с IP адресами.
        /// </summary>
        /// <param name="controls">Список TextBox-ов и Label-ов для валидации.</param>
        /// <returns>
        ///     Логическое значение успешности валидации.<br></br>
        ///     <see cref="bool">True</see> - в случение успеха, <see cref="bool">False</see> при неудаче.
        /// </returns>
        private static async Task<bool> CheckIP(CheckTextBoxModel[] controls)
        {
            var errors = false;
            foreach (var item in controls)
            {
                if (item.errLabel.Visibility == Visibility.Visible && item.errLabel.IsVisible)
                    await GoError(item.errLabel, hideError: true);
                item.textBox.Text = item.textBox.Text.Replace(',', '.').Trim();//;
                string text = item.textBox.Text.Trim();
                if (text.Length == 0){
                    await GoError(item.errLabel, "Адрес не может быть пустым"); errors = true;
                }
                else if (text.Contains(" ")){
                    await GoError(item.errLabel, "Никаких пробелов в адресе"); errors = true;
                }
                else if (text.Count(a => a == '.') != 3/* || !text.StartsWith("192.168.")*/) {  // 🤷‍♂️
                    await GoError(item.errLabel, "Неверный формат адреса"); errors = true;
                }
                else if (text.Count(a => char.IsLetter(a)) != 0)
                {
                    await GoError(item.errLabel, "Никаких букв в адресе! IPv4!"); errors = true;
                }
            }
            if (errors) return false;
            if (controls.Length > 1)
            {
                var ipFirstSlices = controls[0].textBox.Text.Split(".").ToList();
                var ipSecondSlices = controls[1].textBox.Text.Split(".").ToList();
                for (int i = 0; i < ipFirstSlices.Count; i++)
                {
                    if (!byte.TryParse(ipFirstSlices[i], out byte byteFirstSlice))
                        return await GoError(controls[0].errLabel, "Ой ёй, исправляй давай.");
                    if (!byte.TryParse(ipSecondSlices[i], out byte byteSecondSlice))
                        return await GoError(controls[1].errLabel, $"{controls[1].textBox.Text}? Серьёзно?");
                    if (byteFirstSlice > byteSecondSlice)
                    {
                        var temp = controls[0].textBox.Text;
                        controls[0].textBox.Text = controls[1].textBox.Text;
                        controls[1].textBox.Text = temp;
                        return true;
                    }
                }
            }
            return true;
        }

        /// <summary>
        ///     Поиск устройства по IP.
        /// </summary>
        /// <param name="ip">IP адрес устройства.</param>
        /// <param name="ignoreOutputs">
        /// Флаг, если установленно <see cref="bool">True</see> - будут игнорироваться выводы об ошибках.
        /// По умолчанию <see cref="bool">False</see>.
        /// </param>
        /// <returns>В случае успеха вернет объект с информацией об устройстве <see cref="DeviceEntity"/>, либо null.</returns>
        private async Task<bool> FindDevice(string ip, bool ignoreOutputs = false)
        {
            HttpClient pingClient = new();
            pingClient.Timeout = TimeSpan.FromSeconds(5);
            var address = $"http://{ip}/";
            SSMInfoLabel.Content = $"Ждем ответа от {ip}...";
            await Helper.AnimateDouble(AnimAction.Show, AnimType.Height, SSMInfoLabel);
            CancellationTokenSource cancellationTokenSource = new();
            try
            {
                pingClient.BaseAddress = new Uri(address);
                var response = await pingClient.GetAsync("/ping", cancellationTokenSource.Token);

                if (response.StatusCode == System.Net.HttpStatusCode.OK || _mockEnabled)
                {
                    var jsonString = !_mockEnabled 
                        ? await response.Content.ReadAsStringAsync() 
                        : "{\"name\":\"ESP - 12F\",\"token\":\"ecu_9SYsb8rQNnaZ3P5wFzwyGt12W5vL\",\"leds\":30}";
                    if (!LoadDevice(jsonString)) 
                        throw new ArgumentNullException();
                    InfoProgressBar.Visibility = Visibility.Hidden;
                    SSMInfoLabel.Content = $"Успешно!";
                    await Helper.AnimateDouble(AnimAction.Show, AnimType.Height, SSMInfoLabel, color: ColorText.Success);
                    await Helper.CreateDelay(500);
                    await Helper.AnimateDouble(AnimAction.Hide, AnimType.Height, SSMInfoLabel);
                    return true;
                }
                else
                {
                    InfoProgressBar.Visibility = Visibility.Hidden;
                    if (!ignoreOutputs)
                    {
                        SSMInfoLabel.Content = $"Видимо не туда стучимся. Код ответа {(int)response.StatusCode}.";
                        await Helper.AnimateDouble(AnimAction.Show, AnimType.Height, SSMInfoLabel, color: ColorText.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                InfoProgressBar.Visibility = Visibility.Hidden;
                if (!ignoreOutputs)
                {
                    if (ex is ArgumentNullException || ex is JsonReaderException)
                        SSMInfoLabel.Content = $"Устройство ответило неверно. Проверь версию прошивки.";
                    else
                        SSMInfoLabel.Content = $"Устройство не отвечает. Проверь подключение.";
                    await Helper.AnimateDouble(AnimAction.Show, AnimType.Height, SSMInfoLabel, color: ColorText.Error);
                }
                cancellationTokenSource.Cancel(); pingClient.CancelPendingRequests();
            }
            return false;
        }

        private bool LoadDevice(string json)
        {
            try
            {
                var device = JsonConvert.DeserializeObject<DeviceEntity>(json);
                if (device == null)
                    return false;
                else
                {
                    Settings.Default.DeviceInfo = json;
                    Settings.Default.Save();
                    _targetDevice = device;
                    //targetDevice = new DeviceEntity(device.Name, device.Token, 20);
                    return true;
                }
            }
            catch(Exception ex)
            {
                ErrLogger.Error(ex);
                return false;
            }
        }

        private void IPTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                CheckIPBtn_Click(sender, e);
        }
        private async void CheckIPBtn_Click(object sender, RoutedEventArgs e)
        {
            if (await CheckIP(new[] { new CheckTextBoxModel(IPTextBox, SSMInfoLabel) }))
            {
                SSMInfoLabel.Content = "Сейчас проверим доступность устройства";
                InfoProgressBar.Visibility = Visibility.Visible;
                await Helper.AnimateDouble(AnimAction.Show, AnimType.Height, SSMInfoLabel, color: ColorText.Regular);
                await Helper.CreateDelay(200);
                if (await FindDevice(IPTextBox.Text))
                {
                    TSDeviceNameLabel.Text = _targetDevice.Name;
                    TSDeviceTokenLabel.Text = _targetDevice.Token;
                    TSDeviceLedsLabel.Text = Helper.GetEnding(_targetDevice.Leds, Helper.LedsEnding);
                    await ShowWindow(WindowShowing.ConfirmingFindedDevice);
                }
            }
        }

        private async void CancelDeviceBtn_Click(object sender, RoutedEventArgs e)
        {
            await ShowWindow(WindowShowing.FindDeviceManual);
            IPTextBox.Focus();
            IPTextBox.SelectionStart = IPTextBox.Text.Length;
            IPTextBox.SelectionLength = 0;
        }

        private async void ConfirmDeviceBtn_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.DeviceInfo = JsonConvert.SerializeObject(_targetDevice);
            Settings.Default.Save();
            if(_pointList.Count != _targetDevice?.Leds)
            {
                _pointList.Clear();
                foreach (var item in Enumerable.Range(0, _targetDevice?.Leds ?? 0))
                    _pointList.Add(new PointWindow(item + 1));
            }
            if (DeviceState.Fill.IsFrozen) DeviceState.Fill = new SolidColorBrush(Colors.IndianRed);
            await Helper.AnimateColor(Colors.LightGreen, DeviceState);
            await ShowWindow(WindowShowing.ChoosingLocationLEDs);
            PlacePoints();
        }
        #region auto adding in progress
        private async void CheckAutoIPBtn_Click(object sender, RoutedEventArgs e)
        {
            if (await CheckIP(new CheckTextBoxModel[] {
                new CheckTextBoxModel(StartAddress, SSAInfoLabelStart),
                new CheckTextBoxModel(EndAddress, SSAInfoLabelEnd)
            }))
                foreach (var third in Enumerable.Range(0, 255))
                {
                    foreach (var fourth in Enumerable.Range(0, 255))
                    {
                        // in progress
                    }
                }

        }
        
        private void Address_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                CheckAutoIPBtn_Click(sender, e);
        }
        #endregion

        private async void BackZoneBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_targetPlace != 0) _targetPlace--; else _targetPlace = 2;
            await Helper.AnimateDouble(AnimAction.HideAndShow, AnimType.Width, TargetPlaceLbl, Speed.Fast,
                text: _ledPlaceVariants[_targetPlace], withoutDelay: false, enableFade: false);
            await Helper.AnimateRect(RectDemonstration, _targetPlace); PlacePoints();
        }

        private async void ForwardZoneBtn_Click(object sender, RoutedEventArgs e)
        {
            if(_targetPlace != 2) _targetPlace++; else _targetPlace = 0;
            await Helper.AnimateDouble(AnimAction.HideAndShow, AnimType.Width, TargetPlaceLbl, Speed.Fast, 
                text: _ledPlaceVariants[_targetPlace], withoutDelay: false, enableFade: false);
            await Helper.AnimateRect(RectDemonstration, _targetPlace); PlacePoints();
        }

        private async void HideStartLedsButtons(AnimAction action)
        {
            await Helper.AnimateDouble(action, AnimType.Width, StartLeftTopRB, enableFade: true);
            await Helper.AnimateDouble(action, AnimType.Width, StartRightTopRB, enableFade: true);
            await Helper.AnimateDouble(action, AnimType.Width, StartLeftBottomRB, enableFade: true);
            await Helper.AnimateDouble(action, AnimType.Width, StartRightBottomRB, enableFade: true);
        }

        public async void PlacePoints()
        {
            if (_targetPlace != 2)
                if (StartRightBottomRB.Opacity > 0.0)
                    HideStartLedsButtons(AnimAction.Hide);
            if (_isLedsReversed != ReverseLedsToggle.IsChecked.GetValueOrDefault(false))
            {
                _pointList.Reverse();
                _isLedsReversed = !_isLedsReversed;
            }
            if (_targetPlace == 0)
            {
                await Helper.PlaceWindows((WindowsPlace.CenterLine, _pointList), _screenInfo, _targetDevice.Leds);
            }else if(_targetPlace == 2)
            {
                if (StartRightBottomRB.Opacity < 1.0)
                    HideStartLedsButtons(AnimAction.Show);
                (int LedsX, int LedsY) = _screenInfo.GetLedsCount(_targetDevice.Leds);

                (WindowsPlace, List<PointWindow>) firstSide, secondSide, thirdSide, fourthSide;

                if (StartLeftTopRB.IsChecked.GetValueOrDefault(false))
                {
                    firstSide = (WindowsPlace.Top, _pointList.Take(LedsX / 2).ToList());
                    secondSide = (WindowsPlace.Right, _pointList.Skip(firstSide.Item2.Count).Take(LedsY / 2).ToList());
                    thirdSide = (WindowsPlace.Bottom, _pointList.Skip(firstSide.Item2.Count + secondSide.Item2.Count).Take(LedsX / 2).ToList());
                    fourthSide = (WindowsPlace.Left, _pointList.Skip(firstSide.Item2.Count + secondSide.Item2.Count + thirdSide.Item2.Count).Take(LedsY / 2).ToList());
                }
                else if (StartRightTopRB.IsChecked.GetValueOrDefault(false))
                {
                    firstSide = (WindowsPlace.Right, _pointList.Take(LedsY / 2).ToList());
                    secondSide = (WindowsPlace.Bottom, _pointList.Skip(firstSide.Item2.Count).Take(LedsX / 2).ToList());
                    thirdSide = (WindowsPlace.Left, _pointList.Skip(firstSide.Item2.Count + secondSide.Item2.Count).Take(LedsY / 2).ToList());
                    fourthSide = (WindowsPlace.Top, _pointList.Skip(firstSide.Item2.Count + secondSide.Item2.Count + thirdSide.Item2.Count).Take(LedsX / 2).ToList());
                }
                else if (StartRightBottomRB.IsChecked.GetValueOrDefault(false))
                {
                    firstSide = (WindowsPlace.Bottom, _pointList.Take(LedsX / 2).ToList());
                    secondSide = (WindowsPlace.Left, _pointList.Skip(firstSide.Item2.Count).Take(LedsY / 2).ToList());
                    thirdSide = (WindowsPlace.Top, _pointList.Skip(firstSide.Item2.Count + secondSide.Item2.Count).Take(LedsX / 2).ToList());
                    fourthSide = (WindowsPlace.Right, _pointList.Skip(firstSide.Item2.Count + secondSide.Item2.Count + thirdSide.Item2.Count).Take(LedsY / 2).ToList());
                }
                else
                {
                    firstSide = (WindowsPlace.Left, _pointList.Take(LedsY / 2).ToList());
                    secondSide = (WindowsPlace.Top, _pointList.Skip(firstSide.Item2.Count).Take(LedsX / 2).ToList());
                    thirdSide = (WindowsPlace.Right, _pointList.Skip(firstSide.Item2.Count + secondSide.Item2.Count).Take(LedsY / 2).ToList());
                    fourthSide = (WindowsPlace.Bottom, _pointList.Skip(firstSide.Item2.Count + secondSide.Item2.Count + thirdSide.Item2.Count).Take(LedsX / 2).ToList());
                }
                var topTask = Helper.PlaceWindows(firstSide, _screenInfo, _targetDevice.Leds);
                var rightTask = Helper.PlaceWindows(secondSide, _screenInfo, _targetDevice.Leds);
                var bottomTask = Helper.PlaceWindows(thirdSide, _screenInfo, _targetDevice.Leds);
                var leftTask = Helper.PlaceWindows(fourthSide, _screenInfo, _targetDevice.Leds);
                await Task.WhenAll(topTask, rightTask, bottomTask, leftTask);
            }
            Topmost = false;
            Topmost = TopmostToggle.IsChecked.GetValueOrDefault(false);
            Focus();
        }

        private void PlaceLedsEvent(object sender, RoutedEventArgs e) => PlacePoints();

        private void TopmostToggle_Click(object sender, RoutedEventArgs e)
        {
            _pointList.ForEach(item => item.Topmost = TopmostToggle.IsChecked.GetValueOrDefault(false));
            Topmost = TopmostToggle.IsChecked.GetValueOrDefault(false);
        }

        private async void ZMBackBtn_Click(object sender, RoutedEventArgs e)
        {
            await ShowWindow(WindowShowing.ConfirmingFindedDevice);
        }

        private async Task CheckUpdate()
        {
            await Helper.CreateDelay(500);
            UpdateAvailableIcon.Visibility = Visibility.Collapsed;
            CheckUpdateIcon.Visibility = Visibility.Visible;
            await Helper.AnimateDouble(AnimAction.HideAndShow, AnimType.Width, UpdateTB, Speed.Normal,
                text: "Проверка обновлений", withoutDelay: false, enableFade: false);
            Helper.RotateAnimation(UpdateAnimAngle);
            var update = await _mainClient.CheckUpdate();
            await Helper.CreateDelay(3000);
            Helper.RotateAnimation(UpdateAnimAngle, true);
            if (UpdateAvailableIcon.Fill.IsFrozen) UpdateAvailableIcon.Fill = new SolidColorBrush(Colors.Gray);
            if (CheckUpdateIcon.Fill.IsFrozen) CheckUpdateIcon.Fill = new SolidColorBrush(Colors.Gray);
            if (update != null)
            {
                await Helper.AnimateDouble(AnimAction.HideAndShow, AnimType.Width, UpdateTB, Speed.Normal,
                    text: "Доступно обновление", withoutDelay: false, enableFade: false, color: ColorText.Success);
                await Helper.AnimateDouble(AnimAction.Hide, AnimType.Width, CheckUpdateIcon, Speed.Normal,
                    withoutDelay: false);
                await Helper.CreateDelay(1000);
                await Helper.AnimateDouble(AnimAction.Show, AnimType.Width, UpdateAvailableIcon, Speed.Normal,
                    withoutDelay: false);
                await Helper.AnimateColor(Colors.LightGreen, UpdateAvailableIcon);
            }
            else
            {
                await Helper.AnimateDouble(AnimAction.HideAndShow, AnimType.Width, UpdateTB, Speed.Normal,
                text: "Обновления не найдены", withoutDelay: false, enableFade: false, color: ColorText.Error);
                await Helper.AnimateColor(Colors.LightGreen, CheckUpdateIcon);
            }
        }

        private async void UpdateViewBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_mainClient.isBusy)
                await CheckUpdate();
        }
    }
}
