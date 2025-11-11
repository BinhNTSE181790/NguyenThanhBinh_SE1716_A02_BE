using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.DTOs;
using Service.Interfaces;
using static Repository.DTOs.NewsArticleDTO;
using static Repository.DTOs.TagDTO;

namespace FUNewsManagementSystem.Controllers
{
    [Route("api/tags")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        // Public endpoint - Get active tags only
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<APIResponse<List<TagInfo>>>> GetAllTags()
        {
            try
            {
                var result = await _tagService.GetAllTagsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<List<TagInfo>>.Fail($"System error: {ex.Message}", "500"));
            }
        }

        // Admin only - Get all tags including inactive
        [Authorize(Roles = "0")]
        [HttpGet("management")]
        public async Task<ActionResult<APIResponse<List<TagResponse>>>> GetAllTagsForManagement()
        {
            try
            {
                var result = await _tagService.GetAllTagsForManagementAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<List<TagResponse>>.Fail($"System error: {ex.Message}", "500"));
            }
        }

        // Admin only - Get tag by ID
        [Authorize(Roles = "0")]
        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse<TagResponse>>> GetTagById(int id)
        {
            try
            {
                var result = await _tagService.GetTagByIdAsync(id);
                if (result.StatusCode == "404")
                {
                    return NotFound(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<TagResponse>.Fail($"System error: {ex.Message}", "500"));
            }
        }

        // Admin only - Create tag
        [Authorize(Roles = "0")]
        [HttpPost]
        public async Task<ActionResult<APIResponse<TagResponse>>> CreateTag([FromBody] CreateTagRequest request)
        {
            try
            {
                var result = await _tagService.CreateTagAsync(request);
                return CreatedAtAction(nameof(GetTagById), new { id = result.Data?.TagId }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<TagResponse>.Fail($"System error: {ex.Message}", "500"));
            }
        }

        // Admin only - Update tag
        [Authorize(Roles = "0")]
        [HttpPut("{id}")]
        public async Task<ActionResult<APIResponse<TagResponse>>> UpdateTag(int id, [FromBody] UpdateTagRequest request)
        {
            try
            {
                var result = await _tagService.UpdateTagAsync(id, request);
                if (result.StatusCode == "404")
                {
                    return NotFound(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<TagResponse>.Fail($"System error: {ex.Message}", "500"));
            }
        }

        // Admin only - Delete tag (soft delete)
        [Authorize(Roles = "0")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<APIResponse<string>>> DeleteTag(int id)
        {
            try
            {
                var result = await _tagService.DeleteTagAsync(id);
                if (result.StatusCode == "404")
                {
                    return NotFound(result);
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
