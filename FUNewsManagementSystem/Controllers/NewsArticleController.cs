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

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<ActionResult<APIResponse<List<NewsArticleResponse>>>> SearchNewsArticles([FromQuery] string searchTerm)
        {
            try
            {
                var result = await _newsArticleService.SearchNewsArticlesAsync(searchTerm ?? "");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<List<NewsArticleResponse>>.Fail($"System error: {ex.Message}", "500"));
            }
        }

        [HttpGet("my-news")]
        public async Task<ActionResult<APIResponse<List<NewsArticleResponse>>>> GetMyNewsArticles()
        {
            try
            {
                // Lấy AccountId từ token
                var accountIdClaim = User.FindFirst("AccountId")?.Value;
                if (string.IsNullOrEmpty(accountIdClaim))
                {
                    return Unauthorized(APIResponse<List<NewsArticleResponse>>.Fail("Invalid token", "401"));
                }

                int accountId = int.Parse(accountIdClaim);
                var result = await _newsArticleService.GetNewsByAccountIdAsync(accountId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<List<NewsArticleResponse>>.Fail($"System error: {ex.Message}", "500"));
            }
        }

        [AllowAnonymous]
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
                Console.WriteLine($"Received CreateNewsArticleRequest with {request.TagIds?.Count ?? 0} tags");
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
                Console.WriteLine($"Received UpdateNewsArticleRequest with {request.TagIds?.Count ?? 0} tags");
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

        [Authorize(Roles = "1,2")] // Staff or Lecturer
        [HttpDelete("{id}")]
        public async Task<ActionResult<APIResponse<string>>> DeleteNewsArticle(int id)
        {
            try
            {
                // Lấy AccountId từ token
                var accountIdClaim = User.FindFirst("AccountId")?.Value;
                if (string.IsNullOrEmpty(accountIdClaim))
                {
                    return Unauthorized(APIResponse<string>.Fail("Invalid token", "401"));
                }

                int accountId = int.Parse(accountIdClaim);
                var result = await _newsArticleService.DeleteNewsArticleAsync(id, accountId);
                if (result.StatusCode == "404")
                {
                    return NotFound(result);
                }
                if (result.StatusCode == "403")
                {
                    return StatusCode(403, result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<string>.Fail($"System error: {ex.Message}", "500"));
            }
        }

        [Authorize(Roles = "0")] // Admin only
        [HttpPost("statistics")]
        public async Task<ActionResult<APIResponse<NewsStatisticsResponse>>> GetNewsStatistics([FromBody] NewsStatisticsRequest request)
        {
            try
            {
                var result = await _newsArticleService.GetNewsStatisticsAsync(request.StartDate, request.EndDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<NewsStatisticsResponse>.Fail($"System error: {ex.Message}", "500"));
            }
        }
    }
}
