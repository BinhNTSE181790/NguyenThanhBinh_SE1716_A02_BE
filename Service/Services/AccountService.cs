using Microsoft.Extensions.Configuration;
using Repository.DTOs;
using Repository.Entities;
using Repository.Interfaces;
using Service.Interfaces;
using static Repository.DTOs.AccountDTO;

namespace Service.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork uow;
        private readonly IConfiguration _configuration;
        public AccountService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            uow = unitOfWork;
            _configuration = configuration;
        }
        #region authentication
        public async Task<APIResponse<SystemAccount>> GetAccountByEmailAsync(string email, string password)
        {
            // Kiểm tra admin account từ appsettings.json
            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];
            var adminName = _configuration["AdminAccount:Name"];
            var adminRole = int.Parse(_configuration["AdminAccount:Role"] ?? "0");

            if (email == adminEmail && password == adminPassword)
            {
                // Trả về admin account
                var adminAccount = new SystemAccount
                {
                    AccountId = 0,
                    AccountName = adminName,
                    AccountEmail = adminEmail,
                    AccountRole = adminRole,
                };
                return APIResponse<SystemAccount>.Ok(adminAccount, "Admin account found", "200");
            }

            var account = await uow.AccountRepo.GetAccountByEmailAsync(email, password);
            if (account == null)
            {
                return APIResponse<SystemAccount>.Fail("Account not found", "404");
                
            }
            return APIResponse<SystemAccount>.Ok(account, "Account found", "200");
        }

        public async Task<APIResponse<ProfileResponse>> GetAccountByIdAsync(int accountId)
        {
            var account = await uow.AccountRepo.GetByIdAsync(accountId);
            if (account == null || !account.IsActive)
            {
                return APIResponse<ProfileResponse>.Fail("Account not found", "404");
            }
            ProfileResponse profile = new ProfileResponse
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole
            };
            return APIResponse<ProfileResponse>.Ok(profile, "Account found", "200");
        }
        #endregion

        #region Account Management
        // CRUD Operations
        public async Task<APIResponse<List<AccountResponse>>> GetAllAccountsAsync()
        {
            try
            {
                var allAccounts = await uow.AccountRepo.GetAllAsync();
                // Chỉ lấy các account còn active
                var accounts = allAccounts.Where(a => a.IsActive).ToList();
                var accountResponses = new List<AccountResponse>();

                foreach (var account in accounts)
                {
                    var newsCount = await uow.NewsArticleRepo.CountNewsByCreatorIdAsync(account.AccountId);
                    accountResponses.Add(new AccountResponse
                    {
                        AccountId = account.AccountId,
                        AccountName = account.AccountName,
                        AccountEmail = account.AccountEmail,
                        AccountRole = account.AccountRole,
                        NewsCount = newsCount
                    });
                }

                return APIResponse<List<AccountResponse>>.Ok(accountResponses, "Accounts retrieved successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<List<AccountResponse>>.Fail($"Error retrieving accounts: {ex.Message}", "500");
            }
        }

        public async Task<APIResponse<AccountResponse>> GetAccountDetailAsync(int accountId)
        {
            try
            {
                var account = await uow.AccountRepo.GetByIdAsync(accountId);
                if (account == null || !account.IsActive)
                {
                    return APIResponse<AccountResponse>.Fail("Account not found", "404");
                }

                var accountResponse = new AccountResponse
                {
                    AccountId = account.AccountId,
                    AccountName = account.AccountName,
                    AccountEmail = account.AccountEmail,
                    AccountRole = account.AccountRole
                };

                return APIResponse<AccountResponse>.Ok(accountResponse, "Account retrieved successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<AccountResponse>.Fail($"Error retrieving account: {ex.Message}", "500");
            }
        }

        public async Task<APIResponse<AccountResponse>> CreateAccountAsync(CreateAccountRequest request)
        {
            try
            {
                // Kiểm tra email đã tồn tại
                var existingAccounts = await uow.AccountRepo.GetAllAsync();
                if (existingAccounts.Any(a => a.AccountEmail == request.AccountEmail))
                {
                    return APIResponse<AccountResponse>.Fail("Email already exists", "400");
                }

                var newAccount = new SystemAccount
                {
                    AccountName = request.AccountName,
                    AccountEmail = request.AccountEmail,
                    AccountPassword = request.AccountPassword,
                    AccountRole = request.AccountRole
                };

                await uow.AccountRepo.CreateAsync(newAccount);

                var accountResponse = new AccountResponse
                {
                    AccountId = newAccount.AccountId,
                    AccountName = newAccount.AccountName,
                    AccountEmail = newAccount.AccountEmail,
                    AccountRole = newAccount.AccountRole
                };

                return APIResponse<AccountResponse>.Ok(accountResponse, "Account created successfully", "201");
            }
            catch (Exception ex)
            {
                return APIResponse<AccountResponse>.Fail($"Error creating account: {ex.Message}", "500");
            }
        }

        public async Task<APIResponse<AccountResponse>> UpdateAccountAsync(int accountId, UpdateAccountRequest request)
        {
            try
            {
                var account = await uow.AccountRepo.GetByIdAsync(accountId);
                if (account == null)
                {
                    return APIResponse<AccountResponse>.Fail("Account not found", "404");
                }

                // Kiểm tra email mới đã tồn tại (trừ account hiện tại)
                var existingAccounts = await uow.AccountRepo.GetAllAsync();
                if (existingAccounts.Any(a => a.AccountEmail == request.AccountEmail && a.AccountId != accountId))
                {
                    return APIResponse<AccountResponse>.Fail("Email already exists", "400");
                }

                account.AccountName = request.AccountName;
                account.AccountEmail = request.AccountEmail;
                account.AccountRole = request.AccountRole;

                await uow.AccountRepo.UpdateAsync(account);

                var accountResponse = new AccountResponse
                {
                    AccountId = account.AccountId,
                    AccountName = account.AccountName,
                    AccountEmail = account.AccountEmail,
                    AccountRole = account.AccountRole
                };

                return APIResponse<AccountResponse>.Ok(accountResponse, "Account updated successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<AccountResponse>.Fail($"Error updating account: {ex.Message}", "500");
            }
        }

        public async Task<APIResponse<string>> DeleteAccountAsync(int accountId)
        {
            try
            {
                // Lấy account để soft delete
                var account = await uow.AccountRepo.GetByIdAsync(accountId);
                if (account == null)
                {
                    return APIResponse<string>.Fail("Account not found", "404");
                }

                // Kiểm tra nếu account đã bị xóa (soft deleted)
                if (!account.IsActive)
                {
                    return APIResponse<string>.Fail("Account is already deleted", "400");
                }

                // Soft delete: chuyển IsActive thành false
                account.IsActive = false;
                var result = await uow.AccountRepo.UpdateAsync(account);
                
                if (result <= 0)
                {
                    return APIResponse<string>.Fail("Failed to delete account", "500");
                }

                return APIResponse<string>.Ok("Account deleted successfully", "Account deleted successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<string>.Fail($"Error deleting account: {ex.Message}", "500");
            }
        }

        public async Task<APIResponse<string>> ChangePasswordAsync(int accountId, ChangePasswordRequest request)
        {
            try
            {
                var account = await uow.AccountRepo.GetByIdAsync(accountId);
                if (account == null)
                {
                    return APIResponse<string>.Fail("Account not found", "404");
                }

                // Kiểm tra old password
                if (account.AccountPassword != request.OldPassword)
                {
                    return APIResponse<string>.Fail("Old password is incorrect", "400");
                }

                // Cập nhật password mới
                account.AccountPassword = request.NewPassword;
                await uow.AccountRepo.UpdateAsync(account);

                return APIResponse<string>.Ok("Password changed successfully", "Password changed successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<string>.Fail($"Error changing password: {ex.Message}", "500");
            }
        }
        #endregion
    }
}
