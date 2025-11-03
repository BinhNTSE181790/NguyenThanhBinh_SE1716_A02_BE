using Repository.Entities;

namespace Repository.Interfaces
{
    public interface INewsArticleRepo : IGenericRepo<NewsArticle>
    {
        Task<NewsArticle?> GetNewsArticleWithDetailsAsync(int newsArticleId);
        Task<List<NewsArticle>> GetAllNewsArticlesWithDetailsAsync();
    }
}
