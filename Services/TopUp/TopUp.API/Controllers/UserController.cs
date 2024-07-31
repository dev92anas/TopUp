using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TopUp.Application.DTOs;
using TopUp.Application.Services.IServices;

namespace TopUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddTopUpUser(CreateUserDTO model)
        {
            await _userService.AddUserAsync(model.Username);
            return Ok("Topup User Created, Success");
        }
    }
}
