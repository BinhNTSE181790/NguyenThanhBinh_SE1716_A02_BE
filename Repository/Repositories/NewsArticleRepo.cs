using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Interfaces;

namespace Repository.Repositories
{
    public class NewsArticleRepo : GenericRepo<NewsArticle>, INewsArticleRepo
    {
        public NewsArticleRepo(DbContext context) : base(context)
        {
        }

        public async Task<NewsArticle?> GetNewsArticleWithDetailsAsync(int newsArticleId)
        {
            return await _dbSet
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.UpdatedBy)
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == newsArticleId);
        }

        public async Task<List<NewsArticle>> GetAllNewsArticlesWithDetailsAsync()
        {
            return await _dbSet
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.UpdatedBy)
                .Include(n => n.Tags)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
