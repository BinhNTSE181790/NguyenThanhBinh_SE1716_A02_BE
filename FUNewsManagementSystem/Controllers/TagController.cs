using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.DTOs;
using Service.Interfaces;
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

        // Public endpoint - Get all tags (read-only)
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<APIResponse<List<TagResponse>>>> GetAllTags()
        {
            try
            {
                var result = await _tagService.GetAllTagsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<List<TagResponse>>.Fail($"System error: {ex.Message}", "500"));
            }
        }
    }
}
