using Repository.DTOs;
using static Repository.DTOs.NewsArticleDTO;

namespace Service.Interfaces
{
    public interface INewsArticleService
    {
        Task<APIResponse<List<NewsArticleResponse>>> GetAllNewsArticlesAsync();
        Task<APIResponse<List<NewsArticleResponse>>> SearchNewsArticlesAsync(string searchTerm);
        Task<APIResponse<List<NewsArticleResponse>>> GetNewsByAccountIdAsync(int accountId);
        Task<APIResponse<NewsArticleResponse>> GetNewsArticleDetailAsync(int newsArticleId);
        Task<APIResponse<NewsArticleResponse>> CreateNewsArticleAsync(int createdById, CreateNewsArticleRequest request);
        Task<APIResponse<NewsArticleResponse>> UpdateNewsArticleAsync(int newsArticleId, int updatedById, UpdateNewsArticleRequest request);
        Task<APIResponse<string>> DeleteNewsArticleAsync(int newsArticleId, int accountId);
        Task<APIResponse<NewsStatisticsResponse>> GetNewsStatisticsAsync(DateTime startDate, DateTime endDate);
    }
}
