using Repository.DTOs;
using static Repository.DTOs.CategoryDTO;

namespace Service.Interfaces
{
    public interface ICategoryService
    {
        Task<APIResponse<List<CategoryResponse>>> GetAllCategoriesAsync();
        Task<APIResponse<CategoryResponse>> GetCategoryDetailAsync(int categoryId);
        Task<APIResponse<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request);
        Task<APIResponse<CategoryResponse>> UpdateCategoryAsync(int categoryId, UpdateCategoryRequest request);
        Task<APIResponse<string>> DeleteCategoryAsync(int categoryId);
    }
}
