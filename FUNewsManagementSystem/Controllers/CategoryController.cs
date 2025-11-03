using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.DTOs;
using Service.Interfaces;
using static Repository.DTOs.CategoryDTO;

namespace FUNewsManagementSystem.Controllers
{
    [Route("api/categories")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<APIResponse<List<CategoryResponse>>>> GetAllCategories()
        {
            try
            {
                var result = await _categoryService.GetAllCategoriesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<List<CategoryResponse>>.Fail($"System error: {ex.Message}", "500"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse<CategoryResponse>>> GetCategoryDetail(int id)
        {
            try
            {
                var result = await _categoryService.GetCategoryDetailAsync(id);
                if (result.StatusCode == "404")
                {
                    return NotFound(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<CategoryResponse>.Fail($"System error: {ex.Message}", "500"));
            }
        }

        [Authorize(Roles = "3")]
        [HttpPost]
        public async Task<ActionResult<APIResponse<CategoryResponse>>> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            try
            {
                var result = await _categoryService.CreateCategoryAsync(request);
                if (result.StatusCode == "404")
                {
                    return NotFound(result);
                }
                return CreatedAtAction(nameof(GetCategoryDetail), new { id = result.Data?.CategoryId }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<CategoryResponse>.Fail($"System error: {ex.Message}", "500"));
            }
        }

        [Authorize(Roles = "3")]
        [HttpPut("{id}")]
        public async Task<ActionResult<APIResponse<CategoryResponse>>> UpdateCategory(int id, [FromBody] UpdateCategoryRequest request)
        {
            try
            {
                var result = await _categoryService.UpdateCategoryAsync(id, request);
                if (result.StatusCode == "404")
                {
                    return NotFound(result);
                }
                if (result.StatusCode == "400")
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<CategoryResponse>.Fail($"System error: {ex.Message}", "500"));
            }
        }

        [Authorize(Roles = "3")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<APIResponse<string>>> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (result.StatusCode == "404")
                {
                    return NotFound(result);
                }
                if (result.StatusCode == "400")
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<string>.Fail($"System error: {ex.Message}", "500"));
            }
        }
    }
}
