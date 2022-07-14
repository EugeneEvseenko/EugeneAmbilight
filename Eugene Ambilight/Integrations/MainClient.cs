using Eugene_Ambilight.Classes.Models;
using Eugene_Ambilight.Classes.Requests;
using Eugene_Ambilight.Enums;
using Newtonsoft.Json;
using NLog;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Eugene_Ambilight.Integrations
{
    public class MainClient
    {
        private HttpClient _httpClient;
        private Logger errLogger { get; set; } = LogManager.GetLogger("errLogger");

        public MainClient()
        {
            _httpClient = new()
            {
                BaseAddress = new Uri("https://ambilight.evseenko.kz/")
            };
        }

        private async Task<BaseResponse<TResponse>?> SendPostRequest<TResponse, TRequest>(TRequest data, string requestLink)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data);
                HttpRequestMessage AmbiChangeStateRequest = new()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(requestLink, UriKind.Relative),
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                var response = await _httpClient.SendAsync(AmbiChangeStateRequest);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var objectResponse = JsonConvert.DeserializeObject<BaseResponse<TResponse>>(jsonString);
                    return objectResponse;
                }
            }
            catch (Exception ex)
            {
                errLogger.Error(ex);
            }
            return new BaseResponse<TResponse>();
        }

        public async Task<UpdateResponse?> CheckUpdate()
        {
            var updateResponse = await SendPostRequest<UpdateResponse, UpdateRequest>(new UpdateRequest(), "update");
            if (updateResponse.Code == ResultCode.Ok)
                return updateResponse.Data;
            return null;
        }
    }
}
