using Repository.DTOs;
using Repository.Entities;
using Repository.Interfaces;
using Service.Interfaces;
using static Repository.DTOs.NewsArticleDTO;

namespace Service.Services
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly IUnitOfWork _uow;

        public NewsArticleService(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        public async Task<APIResponse<List<NewsArticleResponse>>> GetAllNewsArticlesAsync()
        {
            try
            {
                var newsArticles = await _uow.NewsArticleRepo.GetAllNewsArticlesWithDetailsAsync();
                var newsArticleResponses = newsArticles.Select(n => new NewsArticleResponse
                {
                    NewsArticleId = n.NewsArticleId,
                    NewsTitle = n.NewsTitle,
                    Headline = n.Headline,
                    CreatedDate = n.CreatedDate,
                    NewsContent = n.NewsContent,
                    NewsSource = n.NewsSource,
                    CategoryId = n.CategoryId,
                    CategoryName = n.Category?.CategoryName ?? string.Empty,
                    NewsStatus = n.NewsStatus,
                    CreatedById = n.CreatedById,
                    CreatedByName = n.CreatedBy?.AccountName ?? string.Empty,
                    UpdatedById = n.UpdatedById,
                    UpdatedByName = n.UpdatedBy?.AccountName,
                    ModifiedDate = n.ModifiedDate,
                    Tags = n.Tags?.Select(t => new TagInfo
                    {
                        TagId = t.TagId,
                        TagName = t.TagName
                    }).ToList()
                }).ToList();

                return APIResponse<List<NewsArticleResponse>>.Ok(newsArticleResponses, "News articles retrieved successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<List<NewsArticleResponse>>.Fail($"Error retrieving news articles: {ex.Message}", "500");
            }
        }

        public async Task<APIResponse<NewsArticleResponse>> GetNewsArticleDetailAsync(int newsArticleId)
        {
            try
            {
                var newsArticle = await _uow.NewsArticleRepo.GetNewsArticleWithDetailsAsync(newsArticleId);
                if (newsArticle == null)
                {
                    return APIResponse<NewsArticleResponse>.Fail("News article not found", "404");
                }

                var newsArticleResponse = new NewsArticleResponse
                {
                    NewsArticleId = newsArticle.NewsArticleId,
                    NewsTitle = newsArticle.NewsTitle,
                    Headline = newsArticle.Headline,
                    CreatedDate = newsArticle.CreatedDate,
                    NewsContent = newsArticle.NewsContent,
                    NewsSource = newsArticle.NewsSource,
                    CategoryId = newsArticle.CategoryId,
                    CategoryName = newsArticle.Category?.CategoryName ?? string.Empty,
                    NewsStatus = newsArticle.NewsStatus,
                    CreatedById = newsArticle.CreatedById,
                    CreatedByName = newsArticle.CreatedBy?.AccountName ?? string.Empty,
                    UpdatedById = newsArticle.UpdatedById,
                    UpdatedByName = newsArticle.UpdatedBy?.AccountName,
                    ModifiedDate = newsArticle.ModifiedDate,
                    Tags = newsArticle.Tags?.Select(t => new TagInfo
                    {
                        TagId = t.TagId,
                        TagName = t.TagName
                    }).ToList()
                };

                return APIResponse<NewsArticleResponse>.Ok(newsArticleResponse, "News article retrieved successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<NewsArticleResponse>.Fail($"Error retrieving news article: {ex.Message}", "500");
            }
        }

        public async Task<APIResponse<NewsArticleResponse>> CreateNewsArticleAsync(int createdById, CreateNewsArticleRequest request)
        {
            try
            {
                // Kiểm tra category exists
                var category = await _uow.CategoryRepo.GetByIdAsync(request.CategoryId);
                if (category == null)
                {
                    return APIResponse<NewsArticleResponse>.Fail("Category not found", "404");
                }

                // Kiểm tra creator account exists
                var creator = await _uow.AccountRepo.GetByIdAsync(createdById);
                if (creator == null)
                {
                    return APIResponse<NewsArticleResponse>.Fail("Creator account not found", "404");
                }

                var newNewsArticle = new NewsArticle
                {
                    NewsTitle = request.NewsTitle,
                    Headline = request.Headline,
                    NewsContent = request.NewsContent,
                    NewsSource = request.NewsSource,
                    CategoryId = request.CategoryId,
                    NewsStatus = request.NewsStatus,
                    CreatedById = createdById,
                    CreatedDate = DateTime.Now
                };

                // Thêm tags nếu có
                if (request.TagIds != null && request.TagIds.Any())
                {
                    var tags = await _uow.TagRepo.GetTagsByIdsAsync(request.TagIds);
                    newNewsArticle.Tags = tags;
                }

                await _uow.NewsArticleRepo.CreateAsync(newNewsArticle);

                // Lấy lại với đầy đủ thông tin
                var createdArticle = await _uow.NewsArticleRepo.GetNewsArticleWithDetailsAsync(newNewsArticle.NewsArticleId);

                var newsArticleResponse = new NewsArticleResponse
                {
                    NewsArticleId = createdArticle!.NewsArticleId,
                    NewsTitle = createdArticle.NewsTitle,
                    Headline = createdArticle.Headline,
                    CreatedDate = createdArticle.CreatedDate,
                    NewsContent = createdArticle.NewsContent,
                    NewsSource = createdArticle.NewsSource,
                    CategoryId = createdArticle.CategoryId,
                    CategoryName = createdArticle.Category?.CategoryName ?? string.Empty,
                    NewsStatus = createdArticle.NewsStatus,
                    CreatedById = createdArticle.CreatedById,
                    CreatedByName = createdArticle.CreatedBy?.AccountName ?? string.Empty,
                    Tags = createdArticle.Tags?.Select(t => new TagInfo
                    {
                        TagId = t.TagId,
                        TagName = t.TagName
                    }).ToList()
                };

                return APIResponse<NewsArticleResponse>.Ok(newsArticleResponse, "News article created successfully", "201");
            }
            catch (Exception ex)
            {
                return APIResponse<NewsArticleResponse>.Fail($"Error creating news article: {ex.Message}", "500");
            }
        }

        public async Task<APIResponse<NewsArticleResponse>> UpdateNewsArticleAsync(int newsArticleId, int updatedById, UpdateNewsArticleRequest request)
        {
            try
            {
                var newsArticle = await _uow.NewsArticleRepo.GetNewsArticleWithDetailsAsync(newsArticleId);
                if (newsArticle == null)
                {
                    return APIResponse<NewsArticleResponse>.Fail("News article not found", "404");
                }

                // Kiểm tra category exists
                var category = await _uow.CategoryRepo.GetByIdAsync(request.CategoryId);
                if (category == null)
                {
                    return APIResponse<NewsArticleResponse>.Fail("Category not found", "404");
                }

                // Kiểm tra updater account exists
                var updater = await _uow.AccountRepo.GetByIdAsync(updatedById);
                if (updater == null)
                {
                    return APIResponse<NewsArticleResponse>.Fail("Updater account not found", "404");
                }

                newsArticle.NewsTitle = request.NewsTitle;
                newsArticle.Headline = request.Headline;
                newsArticle.NewsContent = request.NewsContent;
                newsArticle.NewsSource = request.NewsSource;
                newsArticle.CategoryId = request.CategoryId;
                newsArticle.NewsStatus = request.NewsStatus;
                newsArticle.UpdatedById = updatedById;
                newsArticle.ModifiedDate = DateTime.Now;

                // Cập nhật tags
                if (request.TagIds != null)
                {
                    var tags = await _uow.TagRepo.GetTagsByIdsAsync(request.TagIds);
                    newsArticle.Tags = tags;
                }
                else
                {
                    newsArticle.Tags?.Clear();
                }

                await _uow.NewsArticleRepo.UpdateAsync(newsArticle);

                // Lấy lại với đầy đủ thông tin
                var updatedArticle = await _uow.NewsArticleRepo.GetNewsArticleWithDetailsAsync(newsArticleId);

                var newsArticleResponse = new NewsArticleResponse
                {
                    NewsArticleId = updatedArticle!.NewsArticleId,
                    NewsTitle = updatedArticle.NewsTitle,
                    Headline = updatedArticle.Headline,
                    CreatedDate = updatedArticle.CreatedDate,
                    NewsContent = updatedArticle.NewsContent,
                    NewsSource = updatedArticle.NewsSource,
                    CategoryId = updatedArticle.CategoryId,
                    CategoryName = updatedArticle.Category?.CategoryName ?? string.Empty,
                    NewsStatus = updatedArticle.NewsStatus,
                    CreatedById = updatedArticle.CreatedById,
                    CreatedByName = updatedArticle.CreatedBy?.AccountName ?? string.Empty,
                    UpdatedById = updatedArticle.UpdatedById,
                    UpdatedByName = updatedArticle.UpdatedBy?.AccountName,
                    ModifiedDate = updatedArticle.ModifiedDate,
                    Tags = updatedArticle.Tags?.Select(t => new TagInfo
                    {
                        TagId = t.TagId,
                        TagName = t.TagName
                    }).ToList()
                };

                return APIResponse<NewsArticleResponse>.Ok(newsArticleResponse, "News article updated successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<NewsArticleResponse>.Fail($"Error updating news article: {ex.Message}", "500");
            }
        }

        public async Task<APIResponse<string>> DeleteNewsArticleAsync(int newsArticleId)
        {
            try
            {
                var newsArticle = await _uow.NewsArticleRepo.GetByIdAsync(newsArticleId);
                if (newsArticle == null)
                {
                    return APIResponse<string>.Fail("News article not found", "404");
                }

                var result = await _uow.NewsArticleRepo.RemoveAsync(newsArticle);
                if (!result)
                {
                    return APIResponse<string>.Fail("Failed to delete news article", "500");
                }

                return APIResponse<string>.Ok("News article deleted successfully", "News article deleted successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<string>.Fail($"Error deleting news article: {ex.Message}", "500");
            }
        }
    }
}
