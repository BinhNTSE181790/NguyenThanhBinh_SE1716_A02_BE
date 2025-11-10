using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.DTOs;
using Service.Interfaces;
using static Repository.DTOs.AccountDTO;

namespace FUNewsManagementSystem.Controllers
{
    [Route("api/account-management")]
    [ApiController]
    public class AccountManagementController : ControllerBase
    {
        private readonly IAccountService _accountService;
        
        public AccountManagementController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Authorize(Roles ="0")]
        [HttpGet]
        public async Task<ActionResult<APIResponse<List<AccountResponse>>>> GetAllAccounts()
        {
            try
            {
                var result = await _accountService.GetAllAccountsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<List<AccountResponse>>.Fail($"System error: {ex.Message}", "500"));
            }
        }

        [Authorize]
        [HttpGet("{accountId}")]
        public async Task<ActionResult<APIResponse<AccountResponse>>> GetAccountDetail(int accountId)
        {
            try
            {
                var result = await _accountService.GetAccountDetailAsync(accountId);
                if (result.StatusCode == "404")
                {
                    return NotFound(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<AccountResponse>.Fail($"System error: {ex.Message}", "500"));
            }
        }

        [Authorize(Roles ="0")]
        [HttpPost]
        public async Task<ActionResult<APIResponse<AccountResponse>>> CreateAccount([FromBody] CreateAccountRequest request)
        {
            try
            {
                var result = await _accountService.CreateAccountAsync(request);
                if (result.StatusCode == "400")
                {
                    return BadRequest(result);
                }
                return CreatedAtAction(nameof(GetAccountDetail), new { accountId = result.Data?.AccountId }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<AccountResponse>.Fail($"System error: {ex.Message}", "500"));
            }
        }

        [Authorize]
        [HttpPut("{accountId}")]
        public async Task<ActionResult<APIResponse<AccountResponse>>> UpdateAccount(int accountId, [FromBody] UpdateAccountRequest request)
        {
            try
            {
                var result = await _accountService.UpdateAccountAsync(accountId, request);
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
                return StatusCode(500, APIResponse<AccountResponse>.Fail($"System error: {ex.Message}", "500"));
            }
        }

        [Authorize(Roles ="0")]
        [HttpDelete("{accountId}")]
        public async Task<ActionResult<APIResponse<string>>> DeleteAccount(int accountId)
        {
            try
            {
                var result = await _accountService.DeleteAccountAsync(accountId);
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

        
        [HttpPut("{accountId}/change-password")]
        public async Task<ActionResult<APIResponse<string>>> ChangePassword(int accountId, [FromBody] ChangePasswordRequest request)
        {
            try
            {
                var result = await _accountService.ChangePasswordAsync(accountId, request);
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
