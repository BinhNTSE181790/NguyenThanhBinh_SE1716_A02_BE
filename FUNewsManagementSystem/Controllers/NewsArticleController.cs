using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.DTOs;
using Service.Interfaces;
using static Repository.DTOs.NewsArticleDTO;

namespace FUNewsManagementSystem.Controllers
{
    [Route("api/news-articles")]
    [ApiController]
    [Authorize]
    public class NewsArticleController : ControllerBase
    {
        private readonly INewsArticleService _newsArticleService;

        public NewsArticleController(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        [HttpGet]
        public async Task<ActionResult<APIResponse<List<NewsArticleResponse>>>> GetAllNewsArticles()
        {
            try
            {
                var result = await _newsArticleService.GetAllNewsArticlesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<List<NewsArticleResponse>>.Fail($"System error: {ex.Message}", "500"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse<NewsArticleResponse>>> GetNewsArticleDetail(int id)
        {
            try
            {
                var result = await _newsArticleService.GetNewsArticleDetailAsync(id);
                if (result.StatusCode == "404")
                {
                    return NotFound(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<NewsArticleResponse>.Fail($"System error: {ex.Message}", "500"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<APIResponse<NewsArticleResponse>>> CreateNewsArticle([FromBody] CreateNewsArticleRequest request)
        {
            try
            {
                // Lấy AccountId từ token
                var accountIdClaim = User.FindFirst("AccountId")?.Value;
                if (string.IsNullOrEmpty(accountIdClaim))
                {
                    return Unauthorized(APIResponse<NewsArticleResponse>.Fail("Invalid token", "401"));
                }

                int createdById = int.Parse(accountIdClaim);
                var result = await _newsArticleService.CreateNewsArticleAsync(createdById, request);
                
                if (result.StatusCode == "404")
                {
                    return NotFound(result);
                }
                
                return CreatedAtAction(nameof(GetNewsArticleDetail), new { id = result.Data?.NewsArticleId }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<NewsArticleResponse>.Fail($"System error: {ex.Message}", "500"));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<APIResponse<NewsArticleResponse>>> UpdateNewsArticle(int id, [FromBody] UpdateNewsArticleRequest request)
        {
            try
            {
                // Lấy AccountId từ token
                var accountIdClaim = User.FindFirst("AccountId")?.Value;
                if (string.IsNullOrEmpty(accountIdClaim))
                {
                    return Unauthorized(APIResponse<NewsArticleResponse>.Fail("Invalid token", "401"));
                }

                int updatedById = int.Parse(accountIdClaim);
                var result = await _newsArticleService.UpdateNewsArticleAsync(id, updatedById, request);
                
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
                return StatusCode(500, APIResponse<NewsArticleResponse>.Fail($"System error: {ex.Message}", "500"));
            }
        }

        [Authorize(Roles = "3")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<APIResponse<string>>> DeleteNewsArticle(int id)
        {
            try
            {
                var result = await _newsArticleService.DeleteNewsArticleAsync(id);
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
