using ControleFinanceiro.Application.DTO.Usuario;
using ControleFinanceiro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ControleFinanceiro.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/usuarios")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet("me")]
        [SwaggerOperation(
        Summary = "Obtém os dados do usuário autenticado",
        Description = "Retorna as informações do usuário atualmente logado com base no token JWT.",
        OperationId = "ObterUsuarioLogado")]
        public async Task<ActionResult<UsuarioConsultarDTO>> GetMe()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            var usuario = await _usuarioService.GetByIdAsync(userId);
            return Ok(usuario);
        }


        [HttpPut("me")]
        [SwaggerOperation(
         Summary = "Atualiza os dados do usuário autenticado",
         Description = "Permite atualizar nome e idade do usuário logado. O e-mail não é alterado por este endpoint.",
         OperationId = "AtualizarUsuarioLogado")]
        public async Task<ActionResult<UsuarioConsultarDTO>> UpdateMe(
        [FromBody] UsuarioAlterarDTO dto)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            var usuario = await _usuarioService.UpdateAsync(userId, dto);
            return Ok(usuario);
        }

        [HttpDelete("me")]
        [SwaggerOperation(
         Summary = "Exclui a conta do usuário autenticado",
         Description = "Remove permanentemente o usuário logado e todos os dados associados.",
         OperationId = "ExcluirUsuarioLogado")]
        public async Task<IActionResult> DeleteMe()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            await _usuarioService.DeleteAsync(userId);
            return NoContent();
        }
    }
}
