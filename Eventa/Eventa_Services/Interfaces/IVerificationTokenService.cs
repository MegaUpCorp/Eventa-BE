using System.Threading.Tasks;

namespace Eventa_Services.Interfaces
{
    public interface IVerificationTokenService
    {
        string GenerateToken(string email);
        bool ValidateToken(string token, out string email);
    }
}
