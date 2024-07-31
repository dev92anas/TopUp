using BalanceService.Application.DTOs;
using BalanceService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BalanceService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BalanceController : ControllerBase
    {
        private readonly IUserBalanceService _balanceService;

        public BalanceController(IUserBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetBalance()
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                var balance = await _balanceService.GetBalanceAsync(username);
                return Ok(balance);
            }
            catch (Exception ex)
            {
                throw new Exception("Cant Get Balance", ex);
            }

        }

        [Authorize]
        [HttpPost("debit")]
        public async Task<IActionResult> DebitBalance([FromBody] decimal amount)
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _balanceService.DebitBalanceAsync(username, amount);
                if (result)
                {
                    return Ok("Debit Complete, Success");
                }

                throw new Exception("Insufficient balance.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("credit")]
        public async Task<IActionResult> CreditBalance([FromBody] decimal amount)
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                var result = await _balanceService.CreditBalanceAsync(username, amount);
                if (result)
                {
                    return Ok("Credit Complete, Success");
                }

                throw new Exception("error can't credit this account.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<IActionResult> CreateUserBalance([FromBody] CreateUserBalanceRequest model)
        {
            try
            {
                var result = await _balanceService.CreateUserBalanceAsync(model.Username, model.InitialBalance);
                if (result)
                {
                    return Ok("Create new balance, Success");
                }

                throw new Exception("error can't create new balance for this account.");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }


}

