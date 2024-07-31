using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopUp.Application.DTOs;
using TopUp.Application.Services.IServices;

namespace TopUp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LookupController : ControllerBase
    {
        private readonly ILookupService _lookupService;

        public LookupController(ILookupService lookupService)
        {
            _lookupService = lookupService;
        }

        [HttpGet("get-topup-options")]
        public async Task<IActionResult> GetTopUpOptions()
        {
            var options = await _lookupService.GetTopUpOptionsAsync();
            return Ok(options);
        }
    }
}
