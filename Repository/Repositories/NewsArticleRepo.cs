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

        public async Task<int> CountNewsByCreatorIdAsync(int creatorId)
        {
            return await _dbSet
                .Where(n => n.CreatedById == creatorId)
                .CountAsync();
        }

        public override async Task<int> CreateAsync(NewsArticle entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Nếu có tags, attach các existing tags vào context
            if (entity.Tags != null && entity.Tags.Any())
            {
                var tagIds = entity.Tags.Select(t => t.TagId).ToList();
                var existingTags = await _context.Set<Tag>()
                    .Where(t => tagIds.Contains(t.TagId))
                    .ToListAsync();

                entity.Tags.Clear();
                foreach (var tag in existingTags)
                {
                    entity.Tags.Add(tag);
                }
            }

            await _dbSet.AddAsync(entity);
            return await _context.SaveChangesAsync();
        }

        public override async Task<int> UpdateAsync(NewsArticle entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Entity đã được tracked từ Service
            _context.Entry(entity).State = EntityState.Modified;

            // Xử lý Tags: Cần so sánh và sync tags đúng cách
            var requestedTagIds = entity.Tags?.Select(t => t.TagId).ToList() ?? new List<int>();
            
            Console.WriteLine($"[Repo] Requested tag IDs: {string.Join(", ", requestedTagIds)}");

            // Tags trong entity hiện tại đã được Service clear và add mới
            // Chỉ cần đảm bảo các Tag entities tồn tại trong context
            if (entity.Tags != null && entity.Tags.Any())
            {
                var tagsToAttach = new List<Tag>();
                foreach (var tag in entity.Tags.ToList())
                {
                    // Kiểm tra xem tag đã được tracked chưa
                    var trackedTag = _context.Set<Tag>().Local.FirstOrDefault(t => t.TagId == tag.TagId);
                    if (trackedTag != null)
                    {
                        tagsToAttach.Add(trackedTag);
                    }
                    else
                    {
                        // Attach tag vào context
                        _context.Attach(tag);
                        tagsToAttach.Add(tag);
                    }
                }
                
                Console.WriteLine($"[Repo] Attached {tagsToAttach.Count} tags to context");
            }

            return await _context.SaveChangesAsync();
        }
    }
}
