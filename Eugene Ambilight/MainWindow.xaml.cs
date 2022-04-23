using Eugene_Ambilight.Classes;
using Eugene_Ambilight.Enums;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                        new RgbLed(){Index = rnd.Next(0, 6), Red = rnd.Next(0, 256), Green = rnd.Next(0, 256), Blue = rnd.Next(0, 256)},
                        new RgbLed(){Index = rnd.Next(0, 6), Red = rnd.Next(0, 256), Green = rnd.Next(0, 256), Blue = rnd.Next(0, 256)},
                        new RgbLed(){Index = rnd.Next(0, 6), Red = rnd.Next(0, 256), Green = rnd.Next(0, 256), Blue = rnd.Next(0, 256)}
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
            await Helper.AnimateHeight(AnimType.Show, ManualLabel);
        }

        private async void ManualBtn_MouseLeave(object sender, MouseEventArgs e) 
            => await Helper.AnimateHeight(AnimType.Hide, ManualLabel, Speed.Fast);

        private async void AutoBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            if (AutoLabel.IsVisible)
                await Helper.AnimateHeight(AnimType.Hide, ManualLabel, Speed.Fast);
            await Helper.AnimateHeight(AnimType.Show, AutoLabel);
        }

        private async void AutoBtn_MouseLeave(object sender, MouseEventArgs e)
            => await Helper.AnimateHeight(AnimType.Hide, AutoLabel, Speed.Fast);

        private async void ManualBtn_Click(object sender, RoutedEventArgs e)
        {
            await Helper.AnimateHeight(AnimType.Hide, FirstStage);
        }

        private async void AutoBtn_Click(object sender, RoutedEventArgs e)
        {
            await Helper.AnimateHeight(AnimType.Hide, FirstStage);
        }

        
    }
}
