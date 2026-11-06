using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POSSystem.Application.Services;
using System.Security.Claims;

namespace POSSystem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;


        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("balance")]
        public IActionResult GetBalance()
        {
            //JWTからアカウントIDを取得
            var accountIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(!int.TryParse(accountIdValue, out int accountId))
            {
                return Unauthorized();
            }

            var account = _accountService.GetAccountById(accountId);

            if (account == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                cashBalance = account.CashBalance,
                electronicMoneyBalance = account.ElectronicMoneyBalance
            });
        }
    }
}
