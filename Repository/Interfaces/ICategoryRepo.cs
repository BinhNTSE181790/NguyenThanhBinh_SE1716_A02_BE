using Repository.Entities;

namespace Repository.Interfaces
{
    public interface ICategoryRepo : IGenericRepo<Category>
    {
        Task<Category?> GetCategoryWithNewsArticlesAsync(int categoryId);
        Task<Category?> GetCategoryWithParentAsync(int categoryId);
    }
}
