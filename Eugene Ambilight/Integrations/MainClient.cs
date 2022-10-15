using Eugene_Ambilight.Classes.Models;
using Eugene_Ambilight.Classes.Requests;
using System.Threading.Tasks;
using Eugene_Ambilight.Integrations.Interfaces;

namespace Eugene_Ambilight.Integrations
{
    public class MainClient : BaseHttpClient, IMainClient
    {
        public MainClient() : base("https://ambilight.evseenko.kz/") { }

        public override bool isBusy { get; set; } = false;

        public async Task<UpdateResponse> CheckUpdate()
        {
            var updateResponse = await SendPostRequest<UpdateResponse, UpdateRequest>(new UpdateRequest(), "update");
            return updateResponse.Data;
        }
    }
}
