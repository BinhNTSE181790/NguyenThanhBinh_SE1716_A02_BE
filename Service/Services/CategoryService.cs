using Repository.DTOs;
using Repository.Entities;
using Repository.Interfaces;
using Service.Interfaces;
using static Repository.DTOs.CategoryDTO;

namespace Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        public async Task<APIResponse<List<CategoryResponse>>> GetAllCategoriesAsync()
        {
            try
            {
                var allCategories = await _uow.CategoryRepo.GetAllAsync();
                // Chỉ lấy các category còn active (Status = 1)
                var categories = allCategories.Where(c => c.Status == 1).ToList();
                var categoryResponses = categories.Select(c => new CategoryResponse
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryDesciption = c.CategoryDesciption,
                    ParentCategoryId = c.ParentCategoryId,
                    ParentCategoryName = c.ParentCategory?.CategoryName,
                    Status = c.Status
                }).ToList();

                return APIResponse<List<CategoryResponse>>.Ok(categoryResponses, "Categories retrieved successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<List<CategoryResponse>>.Fail($"Error retrieving categories: {ex.Message}", "500");
            }
        }

        public async Task<APIResponse<CategoryResponse>> GetCategoryDetailAsync(int categoryId)
        {
            try
            {
                var category = await _uow.CategoryRepo.GetCategoryWithParentAsync(categoryId);
                if (category == null || category.Status != 1)
                {
                    return APIResponse<CategoryResponse>.Fail("Category not found", "404");
                }

                var categoryResponse = new CategoryResponse
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    CategoryDesciption = category.CategoryDesciption,
                    ParentCategoryId = category.ParentCategoryId,
                    ParentCategoryName = category.ParentCategory?.CategoryName,
                    Status = category.Status
                };

                return APIResponse<CategoryResponse>.Ok(categoryResponse, "Category retrieved successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<CategoryResponse>.Fail($"Error retrieving category: {ex.Message}", "500");
            }
        }

        public async Task<APIResponse<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request)
        {
            try
            {
                // Kiểm tra parent category nếu có
                if (request.ParentCategoryId.HasValue)
                {
                    var parentCategory = await _uow.CategoryRepo.GetByIdAsync(request.ParentCategoryId.Value);
                    if (parentCategory == null)
                    {
                        return APIResponse<CategoryResponse>.Fail("Parent category not found", "404");
                    }
                }

                var newCategory = new Category
                {
                    CategoryName = request.CategoryName,
                    CategoryDesciption = request.CategoryDesciption,
                    ParentCategoryId = request.ParentCategoryId,
                    Status = request.Status
                };

                await _uow.CategoryRepo.CreateAsync(newCategory);

                var categoryResponse = new CategoryResponse
                {
                    CategoryId = newCategory.CategoryId,
                    CategoryName = newCategory.CategoryName,
                    CategoryDesciption = newCategory.CategoryDesciption,
                    ParentCategoryId = newCategory.ParentCategoryId,
                    Status = newCategory.Status
                };

                return APIResponse<CategoryResponse>.Ok(categoryResponse, "Category created successfully", "201");
            }
            catch (Exception ex)
            {
                return APIResponse<CategoryResponse>.Fail($"Error creating category: {ex.Message}", "500");
            }
        }

        public async Task<APIResponse<CategoryResponse>> UpdateCategoryAsync(int categoryId, UpdateCategoryRequest request)
        {
            try
            {
                var category = await _uow.CategoryRepo.GetByIdAsync(categoryId);
                if (category == null)
                {
                    return APIResponse<CategoryResponse>.Fail("Category not found", "404");
                }

                // Kiểm tra parent category nếu có
                if (request.ParentCategoryId.HasValue)
                {
                    // Không cho phép category là parent của chính nó
                    if (request.ParentCategoryId.Value == categoryId)
                    {
                        return APIResponse<CategoryResponse>.Fail("Category cannot be parent of itself", "400");
                    }

                    var parentCategory = await _uow.CategoryRepo.GetByIdAsync(request.ParentCategoryId.Value);
                    if (parentCategory == null)
                    {
                        return APIResponse<CategoryResponse>.Fail("Parent category not found", "404");
                    }
                }

                category.CategoryName = request.CategoryName;
                category.CategoryDesciption = request.CategoryDesciption;
                category.ParentCategoryId = request.ParentCategoryId;
                // Status không được update qua API Update, chỉ update qua Delete (soft delete)

                await _uow.CategoryRepo.UpdateAsync(category);

                // Reload category from database to get updated values
                var updatedCategory = await _uow.CategoryRepo.GetByIdAsync(categoryId);

                var categoryResponse = new CategoryResponse
                {
                    CategoryId = updatedCategory!.CategoryId,
                    CategoryName = updatedCategory.CategoryName,
                    CategoryDesciption = updatedCategory.CategoryDesciption,
                    ParentCategoryId = updatedCategory.ParentCategoryId,
                    Status = updatedCategory.Status
                };

                return APIResponse<CategoryResponse>.Ok(categoryResponse, "Category updated successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<CategoryResponse>.Fail($"Error updating category: {ex.Message}", "500");
            }
        }

        public async Task<APIResponse<string>> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                var category = await _uow.CategoryRepo.GetByIdAsync(categoryId);
                if (category == null || category.Status != 1)
                {
                    return APIResponse<string>.Fail("Category not found", "404");
                }

                // Soft delete: chuyển Status thành 0 (Inactive)
                category.Status = 0;
                var result = await _uow.CategoryRepo.UpdateAsync(category);
                
                if (result <= 0)
                {
                    return APIResponse<string>.Fail("Failed to delete category", "500");
                }

                return APIResponse<string>.Ok("Category deleted successfully", "Category deleted successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<string>.Fail($"Error deleting category: {ex.Message}", "500");
            }
        }
    }
}
