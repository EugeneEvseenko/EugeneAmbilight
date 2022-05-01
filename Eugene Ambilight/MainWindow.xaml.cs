using Eugene_Ambilight.Classes;
using Eugene_Ambilight.Classes.Models;
using Eugene_Ambilight.Classes.Requests;
using Eugene_Ambilight.Enums;
using Eugene_Ambilight.Properties;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            debugLogger.Info("App started");
            await Helper.AnimateHeight(AnimType.Show, StartGrid);
        }
        private Logger errLogger = LogManager.GetLogger("errLogger");
        private Logger debugLogger = LogManager.GetLogger("debugLogger");
        private DeviceEntity targetDevice;
        HttpClient httpClient = new HttpClient();
        Random rnd = new Random();
        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IEnumerable<int> ids = Enumerable.Range(0, 60);
                Stopwatch allSW = Stopwatch.StartNew();
                foreach (var kek in ids)
                {
                    List<RgbLed> items = new List<RgbLed>() {
                        new RgbLed(){Index = rnd.Next(0, 6), Red = (byte)rnd.Next(0, 256), Green = (byte)rnd.Next(0, 256), Blue = (byte)rnd.Next(0, 256)},
                        new RgbLed(){Index = rnd.Next(0, 6), Red = (byte)rnd.Next(0, 256), Green = (byte)rnd.Next(0, 256), Blue = (byte)rnd.Next(0, 256)},
                        new RgbLed(){Index = rnd.Next(0, 6), Red = (byte)rnd.Next(0, 256), Green = (byte)rnd.Next(0, 256), Blue = (byte)rnd.Next(0, 256)}
                    };

                    AmbilightRequest dataItem = new(items);
                    string json = JsonConvert.SerializeObject(dataItem);
                    HttpRequestMessage AmbilightRgbRequest = new()
                    {
                        Method = HttpMethod.Post,
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    };
                    AmbilightRgbRequest.RequestUri = new Uri("/ambilight", UriKind.Relative);
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    var response = await httpClient.SendAsync(AmbilightRgbRequest);
                    stopwatch.Stop();
                    PingLabel.Content = $"Ping: {stopwatch.ElapsedMilliseconds} ms";
                }
                allSW.Stop();
                PointLabel.Content = $"Elapsed {ids.Count()} - {allSW.ElapsedMilliseconds} ms";
            }catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void AmbilightState_Changed(object sender, RoutedEventArgs e)
        {
            ChangeStateRequest changeState = new() { State = AmbilightState.IsChecked };
            string json = JsonConvert.SerializeObject(changeState);
            HttpRequestMessage AmbiChangeStateRequest = new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("ambilight-state", UriKind.Relative),
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            Stopwatch stopwatch = Stopwatch.StartNew();
            var response = await httpClient.SendAsync(AmbiChangeStateRequest);
            stopwatch.Stop();
            PingLabel.Content = $"Ping: {stopwatch.ElapsedMilliseconds} ms";
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            foreach (var third in Enumerable.Range(0, 255))
            {
                foreach (var fourth in Enumerable.Range(0, 255))
                {
                    HttpClient pingClient = new();
                    pingClient.Timeout = TimeSpan.FromMilliseconds(200);
                    var address = $"http://192.168.{third}.{fourth}/";
                    PingLabel.Content = $"Address: {address}";
                    CancellationTokenSource cancellationTokenSource = new();
                    try {
                        pingClient.BaseAddress = new Uri(address);
                        var response = await pingClient.GetAsync("/ping", cancellationTokenSource.Token);
                        
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var responseJson = JsonConvert.DeserializeObject<DeviceEntity>(jsonString);
                            DeviceList.Items.Add(responseJson?.Name);
                        }
                    }
                    catch (Exception)
                    {
                        cancellationTokenSource.Cancel(); pingClient.CancelPendingRequests();
                    }
                }
            }
        }

        private async void ManualBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            if(AutoLabel.IsVisible)
                await Helper.AnimateHeight(AnimType.Hide, AutoLabel, Speed.Fast);
            await Helper.AnimateHeight(AnimType.Show, ManualLabel, color: ColorText.Regular);
        }

        private async void ManualBtn_MouseLeave(object sender, MouseEventArgs e) 
            => await Helper.AnimateHeight(AnimType.Hide, ManualLabel, Speed.Normal);

        private async void AutoBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            if (AutoLabel.IsVisible)
                await Helper.AnimateHeight(AnimType.Hide, ManualLabel, Speed.Fast);
            await Helper.AnimateHeight(AnimType.Show, AutoLabel, color: ColorText.Regular);
        }

        private async void AutoBtn_MouseLeave(object sender, MouseEventArgs e)
            => await Helper.AnimateHeight(AnimType.Hide, AutoLabel, Speed.Normal);

        private async void ManualBtn_Click(object sender, RoutedEventArgs e)
        {
            await Helper.AnimateHeight(AnimType.Hide, FirstStage);
            await Helper.AnimateHeight(AnimType.Show, SecondStageManual);
            IPTextBox.Focus();
            IPTextBox.SelectionStart = IPTextBox.Text.Length;
            IPTextBox.SelectionLength = 0;
        }

        private async void AutoBtn_Click(object sender, RoutedEventArgs e)
        {
            await Helper.AnimateHeight(AnimType.Hide, FirstStage);
            await Helper.AnimateHeight(AnimType.Show, SecondStageAuto);
            StartAddress.Focus();
            StartAddress.SelectionStart = StartAddress.Text.Length;
            StartAddress.SelectionLength = 0;
        }

        private async void BackToFirstStage(object sender, RoutedEventArgs e)
        {
            await Helper.AnimateHeight(AnimType.Hide, (e.Source as Button)?.Name == SSMBackBtn.Name ? SecondStageManual : SecondStageAuto);
            await Helper.AnimateHeight(AnimType.Show, FirstStage);
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
                await Helper.AnimateHeight(AnimType.Hide, errLabel, withoutDelay: true); 
                return false; 
            }
            errLabel.Content = content;
            await Helper.AnimateHeight(AnimType.Show, errLabel, color: ColorText.Error);
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
            bool errors = false;
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
        private async Task<DeviceEntity?> FindDevice(string ip, bool ignoreOutputs = false)
        {
            HttpClient pingClient = new();
            pingClient.Timeout = TimeSpan.FromSeconds(5);
            var address = $"http://{ip}/";
            SSMInfoLabel.Content = $"Ждем ответа от {ip}...";
            await Helper.AnimateHeight(AnimType.Show, SSMInfoLabel);
            CancellationTokenSource cancellationTokenSource = new();
            try
            {
                pingClient.BaseAddress = new Uri(address);
                var response = await pingClient.GetAsync("/ping", cancellationTokenSource.Token);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var responseJson = JsonConvert.DeserializeObject<DeviceEntity>(jsonString);
                    if (responseJson == null) 
                        throw new ArgumentNullException();
                    InfoProgressBar.Visibility = Visibility.Hidden;
                    SSMInfoLabel.Content = $"Успешно!";
                    await Helper.AnimateHeight(AnimType.Show, SSMInfoLabel, color: ColorText.Success);
                    await Helper.CreateDelay(500);
                    targetDevice = responseJson;
                    await Helper.AnimateHeight(AnimType.Hide, SSMInfoLabel);
                    return responseJson;
                }
                else
                {
                    InfoProgressBar.Visibility = Visibility.Hidden;
                    if (!ignoreOutputs)
                    {
                        SSMInfoLabel.Content = $"Видимо не туда стучимся. Код ответа {(int)response.StatusCode}.";
                        await Helper.AnimateHeight(AnimType.Show, SSMInfoLabel, color: ColorText.Error);
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
                    await Helper.AnimateHeight(AnimType.Show, SSMInfoLabel, color: ColorText.Error);
                }
                cancellationTokenSource.Cancel(); pingClient.CancelPendingRequests();
            }
            return null;
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
                await Helper.AnimateHeight(AnimType.Show, SSMInfoLabel, color: ColorText.Regular);
                await Helper.CreateDelay(500);
                if (await FindDevice(IPTextBox.Text) != null)
                {
                    await Helper.AnimateHeight(AnimType.Hide, SecondStageManual);
                    TSDeviceNameLabel.Text = targetDevice.Name;
                    TSDeviceTokenLabel.Text = targetDevice.Token;
                    TSDeviceLedsLabel.Text = $"{targetDevice.Leds} светодиодов";
                    await Helper.AnimateHeight(AnimType.Show, ThirdStage);
                }
            }
        }

        private async void CancelDeviceBtn_Click(object sender, RoutedEventArgs e)
        {
            await Helper.AnimateHeight(AnimType.Hide, ThirdStage);
            await Helper.AnimateHeight(AnimType.Show, SecondStageManual);
            IPTextBox.Focus();
            IPTextBox.SelectionStart = IPTextBox.Text.Length;
            IPTextBox.SelectionLength = 0;
        }

        private async void ConfirmDeviceBtn_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.DeviceInfo = JsonConvert.SerializeObject(targetDevice);
            Settings.Default.Save();
            if (DeviceState.Fill.IsFrozen) DeviceState.Fill = new SolidColorBrush(Colors.IndianRed);
            await Helper.AnimateColor(Colors.LightGreen, DeviceState);
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
    }
}
