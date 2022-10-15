using System.Threading.Tasks;
using Eugene_Ambilight.Classes.Models;

namespace Eugene_Ambilight.Integrations.Interfaces
{
    public interface IMainClient
    { 
        Task<UpdateResponse> CheckUpdate();
        bool isBusy { get; }
    }
}