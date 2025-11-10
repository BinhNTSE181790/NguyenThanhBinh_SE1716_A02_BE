using Repository.Entities;

namespace Repository.Interfaces
{
    public interface INewsArticleRepo : IGenericRepo<NewsArticle>
    {
        Task<NewsArticle?> GetNewsArticleWithDetailsAsync(int newsArticleId);
        Task<List<NewsArticle>> GetAllNewsArticlesWithDetailsAsync();
        Task<List<NewsArticle>> SearchNewsArticlesByTitleAsync(string searchTerm);
        Task<int> CountNewsByCreatorIdAsync(int creatorId);
    }
}
