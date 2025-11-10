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
        public async Task<ActionResult<APIResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var account = await _accountService.GetAccountByEmailAsync(request.Email, request.Password);
                if (account.Data == null)
                {
                    return Unauthorized(APIResponse<string>.Fail("Email or password is incorrect", "401"));
                }
                string token = _jwtService.GenerateToken(
                    account.Data.AccountId,
                    account.Data.AccountName,
                    account.Data.AccountEmail,
                    account.Data.AccountRole
                );
                LoginResponse response = new LoginResponse
                {
                    UserId = account.Data.AccountId,
                    UserName = account.Data.AccountName,
                    UserEmail = account.Data.AccountEmail,
                    Role = account.Data.AccountRole,
                    Token = token,
                };
                return Ok(APIResponse<LoginResponse>.Ok(response, "Login successfully"));
            }
            catch (Exception)
            {
                return StatusCode(500, APIResponse<string>.Fail("System error", "500"));
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<APIResponse<AccountDTO.ProfileResponse>>> GetProfile()
        {
            try
            {
                // Lấy thông tin từ token một cách dễ dàng
                var accountIdClaim = User.FindFirst("AccountId")?.Value;
                if (string.IsNullOrEmpty(accountIdClaim))
                {
                    return Unauthorized(APIResponse<AccountDTO.ProfileResponse>.Fail("Invalid token", "401"));
                }

                int accountId = int.Parse(accountIdClaim);
                var account = await _accountService.GetAccountByIdAsync(accountId);
                
                if (account == null || account.Data == null)
                {
                    return NotFound(APIResponse<AccountDTO.ProfileResponse>.Fail("Account not found", "404"));
                }

                // Trả về AccountResponse trực tiếp, nó đã đúng structure
                return Ok(APIResponse<AccountDTO.ProfileResponse>.Ok(account.Data, "Profile retrieved successfully"));
            }
            catch (Exception)
            {
                return StatusCode(500, APIResponse<AccountDTO.ProfileResponse>.Fail("System fail", "500"));
            }
        }
    }
}
