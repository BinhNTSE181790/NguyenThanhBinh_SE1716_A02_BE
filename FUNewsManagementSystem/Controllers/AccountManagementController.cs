using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.DTOs;
using Service.Interfaces;
using static Repository.DTOs.AccountDTO;

namespace FUNewsManagementSystem.Controllers
{
    [Route("api/account-managemenet")]
    [ApiController]
    public class AccountManagementController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountManagementController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Authorize(Roles = "3")]
        [HttpGet]
        public async Task<ActionResult<APIResponse<List<AccountResponse>>>> GetAllAccount()
        {
            try
            {
                var accounts = await _accountService.GetAllAccountsAsync();
                if(accounts == null)
                {
                    return NotFound(APIResponse<List<AccountResponse>>.Fail("No accounts found", "404"));
                }
                return Ok(APIResponse<List<AccountResponse>>.Ok(accounts.Data,"Get list account successful"));
            }
            catch (Exception)
            {
                return StatusCode(500, APIResponse<List<AccountResponse>>.Fail("System fail", "500"));
            }
        }

        [Authorize]
        [HttpPut("{accountId}")]
        public async Task<ActionResult<APIResponse<AccountResponse>>> UpdateAccount(int accountId, [FromBody]UpdateAccountRequest request)
        {
            try
            {
                var updated = await _accountService.UpdateAccountAsync(accountId,request);
                if(updated == null)
                {
                    return NotFound(APIResponse<AccountResponse>.Fail("Update account fail", "400"));
                }
                return Ok(APIResponse<AccountResponse>.Ok(updated.Data, "Update account successful"));
            }
            catch (Exception)
            {
                return StatusCode(500, APIResponse<List<AccountResponse>>.Fail("System fail", "500"));
            }
        }
    }
}
