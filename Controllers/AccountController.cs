using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Unitic_BE.Abstracts;

namespace Unitic_BE.Controllers
{
    [Route("Unitic/[controller]")]
    [ApiController]
    public class AccountController : Controller
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

        [HttpGet("/{accountId}")]
        public async Task<IActionResult> GetAccountById(string accountId)
        {
            var user = await _accountService.GetUserById(accountId);
            return Ok(user);
        }
    }
}