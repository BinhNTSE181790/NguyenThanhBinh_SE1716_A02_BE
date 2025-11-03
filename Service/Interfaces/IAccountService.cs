using Repository.DTOs;
using Repository.Entities;
using static Repository.DTOs.AccountDTO;

namespace Service.Interfaces
{
    public interface IAccountService
    {
        // Authentication
        Task<APIResponse<SystemAccount>> GetAccountByEmailAsync(string email, string password);
        Task<APIResponse<ProfileResponse>> GetAccountByIdAsync(int accountId);

        // CRUD Operations
        Task<APIResponse<List<AccountResponse>>> GetAllAccountsAsync();
        Task<APIResponse<AccountResponse>> GetAccountDetailAsync(int accountId);
        Task<APIResponse<AccountResponse>> CreateAccountAsync(CreateAccountRequest request);
        Task<APIResponse<AccountResponse>> UpdateAccountAsync(int accountId, UpdateAccountRequest request);
        Task<APIResponse<string>> DeleteAccountAsync(int accountId);
        Task<APIResponse<string>> ChangePasswordAsync(int accountId, ChangePasswordRequest request);
    }
}
