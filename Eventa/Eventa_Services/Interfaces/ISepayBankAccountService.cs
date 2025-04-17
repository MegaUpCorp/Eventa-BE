using Eventa_BusinessObject.DTOs;

namespace Eventa_Services.Interfaces;

public interface ISepayBankAccountService
{
    Task<BankAccountListResponseDto> GetBankAccountsAsync(string accessToken);
}