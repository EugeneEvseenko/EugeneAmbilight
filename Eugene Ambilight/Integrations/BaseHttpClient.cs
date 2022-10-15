using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Eugene_Ambilight.Classes;
using Eugene_Ambilight.Classes.Models;
using Eugene_Ambilight.Integrations.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace Eugene_Ambilight.Integrations
{
    public class BaseHttpClient
    {
        protected HttpClient _httpClient;
        private Logger ErrLogger { get; set; } = LogManager.GetLogger("errLogger");
        private Logger DebugLogger { get; set; } = LogManager.GetLogger("debugLogger");
        public virtual bool isBusy { get; set; } = false;
        public BaseHttpClient(string url)
        {
            _httpClient = new()
            {
                BaseAddress = new Uri(url)
            };
            _httpClient.DefaultRequestHeaders.Add("X-Version-Code", $"{Helper.GetVersionCode()}");
        }

        protected async Task<BaseResponse<TResponse>> SendPostRequest<TResponse, TRequest>(TRequest data, string requestLink)
        {
            isBusy = true;
            try
            {
                var requestJsonData = JsonConvert.SerializeObject(data);
                DebugLogger.Info($"REQUEST {_httpClient.BaseAddress.AbsoluteUri}{requestLink}\n{JValue.Parse(requestJsonData).ToString(Formatting.Indented)}");
                HttpRequestMessage httpRequest = new()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(requestLink, UriKind.Relative),
                    Content = new StringContent(requestJsonData, Encoding.UTF8, "application/json")
                };
                var response = await _httpClient.SendAsync(httpRequest);
                var jsonString = await response.Content.ReadAsStringAsync();
                DebugLogger.Info($"RESPONSE {_httpClient.BaseAddress.AbsoluteUri}{requestLink}\n{JValue.Parse(jsonString).ToString(Formatting.Indented)}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var objectResponse = JsonConvert.DeserializeObject<BaseResponse<TResponse>>(jsonString);
                    isBusy = false;
                    return objectResponse;
                }
            }
            catch (Exception ex)
            {
                ErrLogger.Error(ex);
            }
            isBusy = false;
            return new BaseResponse<TResponse>();
        }
    }
}