using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Unitic_BE.Abstracts;
using Unitic_BE.Enums;

namespace Unitic_BE.Controllers
{
    [Route("Unitic/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService service)
        {
            // Constructor logic if needed
            _accountService = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            var users = await _accountService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetAccountById(string accountId)
        {
            var user = await _accountService.GetUserById(accountId);
            return Ok(user);
        }

        [HttpPut("/{accountId}/{role}")]
        public async Task<IActionResult> UpdateAccountRoleAsync(string accountId, Role role)
        {
            await _accountService.UpdateAccountRoleAsync(accountId, role);
            return Ok($"Account updated successfully to {role}.");
        }
    }
}