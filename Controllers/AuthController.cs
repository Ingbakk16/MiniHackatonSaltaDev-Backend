using AnimalesPerdidos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AnimalesPerdidos.Dtos.Usuario;

namespace AnimalesPerdidos.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto model)
        {
            var result = await _authService.RegisterAsync(model);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var result = await _authService.LoginAsync(model);
            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }
    }

}
