using ControleFinanceiro.Application.DTO.Auth;
using ControleFinanceiro.Application.DTO.Usuario;
using ControleFinanceiro.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.API.Controllers
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {

            try
            {
                var result = await _authService.LoginAsync(dto);
                return Ok(result);
            }   
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { mensagem = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UsuarioIncluirDTO dto)
        {
            try
            {
                await _authService.RegisterAsync(dto);
                return Created("", null);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensagem = ex.Message });
            }
        }
    }
}

