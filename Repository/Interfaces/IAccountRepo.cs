using Repository.Entities;

namespace Repository.Interfaces
{
    public interface IAccountRepo : IGenericRepo<SystemAccount>
    {
        Task<SystemAccount?> GetAccountByEmailAsync(string email, string password);
        Task<SystemAccount?> GetAccountWithNewsArticlesAsync(int accountId);
    }
}
