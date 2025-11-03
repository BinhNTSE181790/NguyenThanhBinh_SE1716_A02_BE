using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Interfaces;

namespace Repository.Repositories
{
    public class CategoryRepo : GenericRepo<Category>, ICategoryRepo
    {
        public CategoryRepo(DbContext context) : base(context)
        {
        }

        public async Task<Category?> GetCategoryWithNewsArticlesAsync(int categoryId)
        {
            return await _dbSet
                .Include(c => c.NewsArticles)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }

        public async Task<Category?> GetCategoryWithParentAsync(int categoryId)
        {
            return await _dbSet
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }
    }
}
