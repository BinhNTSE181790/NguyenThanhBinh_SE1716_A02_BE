using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Interfaces;

namespace Repository.Repositories
{
    public class AccountRepo : GenericRepo<SystemAccount>,IAccountRepo
    {
        public AccountRepo(DbContext context) : base(context)
        {
        }
        public async Task<SystemAccount> GetAccountByEmailAsync(string email, string password)
        {
            SystemAccount account = await _dbSet
                .Where(a => a.AccountEmail == email && a.AccountPassword == password)
                .Select(a => new SystemAccount
                {
                    AccountId = a.AccountId,
                    AccountName = a.AccountName,
                    AccountEmail = a.AccountEmail,
                    AccountRole = a.AccountRole,
                    AccountPassword = a.AccountPassword
                })
                .FirstOrDefaultAsync();

            return account;
        }

        public async Task<SystemAccount> GetAccountWithNewsArticlesAsync(int accountId)
        {
            return await _dbSet
                .Include(a => a.NewsArticleCreatedBies)
                .Include(a => a.NewsArticleUpdatedBies)
                .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }
    }
}
