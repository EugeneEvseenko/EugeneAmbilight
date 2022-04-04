using Eugene_Ambilight.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
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
        public MainWindow()
        {
            InitializeComponent();
            
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (httpClient.BaseAddress == null) httpClient.BaseAddress = new Uri($"http://{uriServer.Text}/");
        }

        HttpClient httpClient = new HttpClient();
        Random rnd = new Random();
        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
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
                AmbilightRgbRequest.RequestUri = new Uri("/ambilight/");
                Stopwatch stopwatch = Stopwatch.StartNew();
                var response = await httpClient.SendAsync(AmbilightRgbRequest);
                stopwatch.Stop();
                ResponseText.Text = response.ToString();
                ResponseText.Text += $"\n Time: {stopwatch.ElapsedMilliseconds} ms";
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
                RequestUri = new Uri("ambilight-state"),
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            Stopwatch stopwatch = Stopwatch.StartNew();
            var response = await httpClient.SendAsync(AmbiChangeStateRequest);
            stopwatch.Stop();
            ResponseText.Text = response.ToString();
            ResponseText.Text += $"\n Time: {stopwatch.ElapsedMilliseconds} ms";
        }

        
    }
}
