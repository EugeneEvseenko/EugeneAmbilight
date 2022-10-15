using Eugene_Ambilight.Enums;
using Eugene_Ambilight.Extentions;
using Newtonsoft.Json;

namespace Eugene_Ambilight.Classes.Models
{
    public class BaseResponse<TData>
    {
        [JsonProperty("code")]
        public ResultCode Code { get; set; }

        public bool IsError => Code != ResultCode.Ok;

        [JsonProperty("message")] 
        public string Message { get; set; }

        [JsonProperty("data")]
        public TData Data { get; set; }

        public BaseResponse()
        {
            Code = ResultCode.GenericError;
        }
        
        public BaseResponse(TData data)
        {
            Code = ResultCode.GenericError;
            Data = data;
        }

        public BaseResponse(ResultCode code)
        {
            Code = code;
        }

        public BaseResponse(ResultCode code, TData data) : this(code)
        {
            Data = data;
        }
    }
}
