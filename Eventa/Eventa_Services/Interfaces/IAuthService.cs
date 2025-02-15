using System.Threading.Tasks;

namespace Eventa_Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> GoogleLogin();
        Task<object> GoogleCallback(string token);
    }
}