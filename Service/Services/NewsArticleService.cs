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
                // Chỉ lấy news có NewsStatus = 1 (Published)
                var activeNewsArticles = newsArticles.Where(n => n.NewsStatus == 1).ToList();
                var newsArticleResponses = activeNewsArticles.Select(n => new NewsArticleResponse
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

        public async Task<APIResponse<List<NewsArticleResponse>>> SearchNewsArticlesAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllNewsArticlesAsync();
                }

                var newsArticles = await _uow.NewsArticleRepo.SearchNewsArticlesByTitleAsync(searchTerm);
                // Chỉ lấy news có NewsStatus = 1 (Published)
                var activeNewsArticles = newsArticles.Where(n => n.NewsStatus == 1).ToList();
                var newsArticleResponses = activeNewsArticles.Select(n => new NewsArticleResponse
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
                return APIResponse<List<NewsArticleResponse>>.Fail($"Error searching news articles: {ex.Message}", "500");
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
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                // Thêm tags nếu có
                if (request.TagIds != null && request.TagIds.Any())
                {
                    var tags = await _uow.TagRepo.GetTagsByIdsAsync(request.TagIds);
                    newNewsArticle.Tags = tags.ToList();
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

                // Log tags trước khi update
                Console.WriteLine($"[Service] Current tags count before update: {newsArticle.Tags?.Count ?? 0}");
                Console.WriteLine($"[Service] Request tagIds: {string.Join(", ", request.TagIds ?? new List<int>())}");

                // KHÔNG gọi UpdateAsync qua repo mà tự xử lý tags và save
                // Lấy danh sách tag IDs hiện tại và mới
                var currentTagIds = newsArticle.Tags?.Select(t => t.TagId).ToList() ?? new List<int>();
                var newTagIds = request.TagIds ?? new List<int>();

                // Tags cần xóa (có trong current nhưng không có trong new)
                var tagsToRemove = newsArticle.Tags?.Where(t => !newTagIds.Contains(t.TagId)).ToList() ?? new List<Tag>();
                
                // Tags cần thêm (có trong new nhưng không có trong current)
                var tagIdsToAdd = newTagIds.Where(id => !currentTagIds.Contains(id)).ToList();

                Console.WriteLine($"[Service] Tags to remove: {tagsToRemove.Count}");
                Console.WriteLine($"[Service] Tag IDs to add: {tagIdsToAdd.Count}");

                // Xóa tags không còn cần
                foreach (var tag in tagsToRemove)
                {
                    newsArticle.Tags?.Remove(tag);
                    Console.WriteLine($"[Service] Removed tag: {tag.TagId} - {tag.TagName}");
                }

                // Thêm tags mới
                if (tagIdsToAdd.Any())
                {
                    var tagsToAdd = await _uow.TagRepo.GetTagsByIdsAsync(tagIdsToAdd);
                    foreach (var tag in tagsToAdd)
                    {
                        newsArticle.Tags?.Add(tag);
                        Console.WriteLine($"[Service] Added tag: {tag.TagId} - {tag.TagName}");
                    }
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

        public async Task<APIResponse<List<NewsArticleResponse>>> GetNewsByAccountIdAsync(int accountId)
        {
            try
            {
                var newsArticles = await _uow.NewsArticleRepo.GetAllNewsArticlesWithDetailsAsync();
                // Lấy tất cả news của user (không lọc IsActive nữa)
                var userNews = newsArticles.Where(n => n.CreatedById == accountId).ToList();
                
                var newsArticleResponses = userNews.Select(n => new NewsArticleResponse
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

                return APIResponse<List<NewsArticleResponse>>.Ok(newsArticleResponses, "User news articles retrieved successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<List<NewsArticleResponse>>.Fail($"Error retrieving user news articles: {ex.Message}", "500");
            }
        }

        public async Task<APIResponse<string>> DeleteNewsArticleAsync(int newsArticleId, int accountId)
        {
            try
            {
                var newsArticle = await _uow.NewsArticleRepo.GetByIdAsync(newsArticleId);
                if (newsArticle == null)
                {
                    return APIResponse<string>.Fail("News article not found", "404");
                }

                // Check if the user is the creator of the news article
                if (newsArticle.CreatedById != accountId)
                {
                    return APIResponse<string>.Fail("You don't have permission to delete this news article", "403");
                }

                // Hard delete - NewsArticle không có soft delete nữa
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

        public async Task<APIResponse<NewsStatisticsResponse>> GetNewsStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var newsArticles = await _uow.NewsArticleRepo.GetAllNewsArticlesWithDetailsAsync();
                
                // Filter by CreatedDate range only (không có IsActive nữa)
                var filteredNews = newsArticles
                    .Where(n => n.CreatedDate.Date >= startDate.Date 
                        && n.CreatedDate.Date <= endDate.Date)
                    .ToList();

                // Overall statistics
                var totalNews = filteredNews.Count;
                var totalPublished = filteredNews.Count(n => n.NewsStatus == 1);
                var totalDraft = filteredNews.Count(n => n.NewsStatus == 0);
                var totalAuthors = filteredNews.Select(n => n.CreatedById).Distinct().Count();

                // Top category
                var topCategory = filteredNews
                    .GroupBy(n => new { n.CategoryId, n.Category!.CategoryName })
                    .Select(g => new CategoryStatistics
                    {
                        CategoryId = g.Key.CategoryId,
                        CategoryName = g.Key.CategoryName,
                        Count = g.Count()
                    })
                    .OrderByDescending(cs => cs.Count)
                    .FirstOrDefault();

                // Daily breakdown - group by CreatedDate with category breakdown
                var dailyBreakdown = filteredNews
                    .GroupBy(n => n.CreatedDate.Date)
                    .Select(g => new DailyStatistics
                    {
                        Date = g.Key,
                        TotalNews = g.Count(),
                        CategoryBreakdown = g
                            .GroupBy(n => new { n.CategoryId, n.Category!.CategoryName })
                            .Select(cg => new CategoryStatistics
                            {
                                CategoryId = cg.Key.CategoryId,
                                CategoryName = cg.Key.CategoryName,
                                Count = cg.Count()
                            })
                            .OrderByDescending(cs => cs.Count)
                            .ToList()
                    })
                    .OrderByDescending(d => d.Date)
                    .ToList();

                var statistics = new NewsStatisticsResponse
                {
                    TotalNews = totalNews,
                    TotalPublished = totalPublished,
                    TotalDraft = totalDraft,
                    TotalAuthors = totalAuthors,
                    TopCategory = topCategory,
                    DailyBreakdown = dailyBreakdown
                };

                return APIResponse<NewsStatisticsResponse>.Ok(statistics, "Statistics retrieved successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<NewsStatisticsResponse>.Fail($"Error retrieving statistics: {ex.Message}", "500");
            }
        }

        // Optimized API: Get only summary statistics (lighter)
        public async Task<APIResponse<StatisticsSummary>> GetStatisticsSummaryAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var newsArticles = await _uow.NewsArticleRepo.GetAllNewsArticlesWithDetailsAsync();
                
                // Filter by CreatedDate range only (không có IsActive nữa)
                var filteredNews = newsArticles
                    .Where(n => n.CreatedDate.Date >= startDate.Date 
                        && n.CreatedDate.Date <= endDate.Date)
                    .ToList();

                // Overall statistics only
                var totalNews = filteredNews.Count;
                var totalPublished = filteredNews.Count(n => n.NewsStatus == 1);
                var totalDraft = filteredNews.Count(n => n.NewsStatus == 0);
                var totalAuthors = filteredNews.Select(n => n.CreatedById).Distinct().Count();

                // Top category
                var topCategory = filteredNews
                    .GroupBy(n => new { n.CategoryId, n.Category!.CategoryName })
                    .Select(g => new CategoryStatistics
                    {
                        CategoryId = g.Key.CategoryId,
                        CategoryName = g.Key.CategoryName,
                        Count = g.Count()
                    })
                    .OrderByDescending(cs => cs.Count)
                    .FirstOrDefault();

                var summary = new StatisticsSummary
                {
                    TotalNews = totalNews,
                    TotalPublished = totalPublished,
                    TotalDraft = totalDraft,
                    TotalAuthors = totalAuthors,
                    TopCategory = topCategory
                };

                return APIResponse<StatisticsSummary>.Ok(summary, "Summary statistics retrieved successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<StatisticsSummary>.Fail($"Error retrieving summary: {ex.Message}", "500");
            }
        }

        // Optimized API: Get only daily breakdown (can be loaded separately)
        public async Task<APIResponse<List<DailyStatistics>>> GetDailyBreakdownAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var newsArticles = await _uow.NewsArticleRepo.GetAllNewsArticlesWithDetailsAsync();
                
                // Filter by CreatedDate range only (không có IsActive nữa)
                var filteredNews = newsArticles
                    .Where(n => n.CreatedDate.Date >= startDate.Date 
                        && n.CreatedDate.Date <= endDate.Date)
                    .ToList();

                // Daily breakdown only
                var dailyBreakdown = filteredNews
                    .GroupBy(n => n.CreatedDate.Date)
                    .Select(g => new DailyStatistics
                    {
                        Date = g.Key,
                        TotalNews = g.Count(),
                        CategoryBreakdown = g
                            .GroupBy(n => new { n.CategoryId, n.Category!.CategoryName })
                            .Select(cg => new CategoryStatistics
                            {
                                CategoryId = cg.Key.CategoryId,
                                CategoryName = cg.Key.CategoryName,
                                Count = cg.Count()
                            })
                            .OrderByDescending(cs => cs.Count)
                            .ToList()
                    })
                    .OrderByDescending(d => d.Date)
                    .ToList();

                return APIResponse<List<DailyStatistics>>.Ok(dailyBreakdown, "Daily breakdown retrieved successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<List<DailyStatistics>>.Fail($"Error retrieving daily breakdown: {ex.Message}", "500");
            }
        }
    }
}
