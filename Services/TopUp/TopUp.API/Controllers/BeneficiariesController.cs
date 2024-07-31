using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TopUp.Application.DTOs;
using TopUp.Application.Services.IServices;

namespace TopUp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BeneficiariesController : ControllerBase
    {
        private readonly IBeneficiaryService _beneficiaryService;

        public BeneficiariesController(IBeneficiaryService beneficiaryService)
        {
            _beneficiaryService = beneficiaryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBeneficiaries()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var beneficiaries = await _beneficiaryService.GetBeneficiariesAsync(username);
            return Ok(beneficiaries);
        }

        [HttpPost]
        public async Task<IActionResult> AddBeneficiary([FromBody] BeneficiaryDTO model)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            await _beneficiaryService.AddBeneficiaryAsync(username, model.Nickname, model.PhoneNumber);
            return Ok("Add Beneficiary complete, Success");
        }

        [HttpPost("topup")]
        public async Task<IActionResult> TopUpBeneficiary([FromBody] TopUpDTO model)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            await _beneficiaryService.TopUpBeneficiaryAsync(username, model.BeneficiaryId, model.LookupAmountValueId, model.idempotencyKey);
            return Ok("Transaction complete, Success");

        }
    }
}
