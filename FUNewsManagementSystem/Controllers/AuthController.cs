using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.DTOs;
using Repository.Entities;
using Service.Interfaces;
using static Repository.DTOs.AccountDTO;
using static Repository.DTOs.AuthDTO;

namespace FUNewsManagementSystem.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IJWTService _jwtService;
        public AuthController(IAccountService accountService, IJWTService jWTService)
        {
            _accountService = accountService;
            _jwtService = jWTService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<APIResponse<string>>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var account = await _accountService.GetAccountByEmailAsync(request.Email, request.Password);
                if (account == null)
                {
                    return NotFound(APIResponse<string>.Fail("Account not exist", "404"));
                }
                string token = _jwtService.GenerateToken(
                    account.Data.AccountId,
                    account.Data.AccountName,
                    account.Data.AccountEmail,
                    account.Data.AccountRole
                );
                return Ok(APIResponse<string>.Ok(token, "Login successfull"));
            }
            catch (Exception)
            {
                return StatusCode(500, APIResponse<string>.Fail("System fail", "500"));
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<APIResponse<ProfileResponse>>> GetProfile()
        {
            try
            {
                // Lấy thông tin từ token một cách dễ dàng
                var accountIdClaim = User.FindFirst("AccountId")?.Value;
                if (string.IsNullOrEmpty(accountIdClaim))
                {
                    return Unauthorized(APIResponse<ProfileResponse>.Fail("Invalid token", "401"));
                }

                int accountId = int.Parse(accountIdClaim);
                var account = await _accountService.GetAccountByIdAsync(accountId);
                
                if (account == null || account.Data == null)
                {
                    return NotFound(APIResponse<ProfileResponse>.Fail("Account not found", "404"));
                }

                return Ok(APIResponse<ProfileResponse>.Ok(account.Data, "Profile retrieved successfully"));
            }
            catch (Exception)
            {
                return StatusCode(500, APIResponse<ProfileResponse>.Fail("System fail", "500"));
            }
        }
    }
}
