using ControleFinanceiro.Application.DTO.Usuario;
using ControleFinanceiro.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ControleFinanceiro.API.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

      
        [HttpGet("consultar")]
        [SwaggerOperation(
            Summary = "Lista todos os usuários cadastrados",
            OperationId = "ListarUsuarios")]
        public async Task<ActionResult<IEnumerable<UsuarioConsultarDTO>>> Consultar()
        {
            var usuarios = await _usuarioService.ConsultarAsync();
            return Ok(usuarios);
        }

      
        [HttpGet("consultar-por-id")]
        [SwaggerOperation(
            Summary = "Consulta um usuário por ID",
            OperationId = "ConsultarUsuarioPorId")]
        public async Task<ActionResult<UsuarioConsultarDTO>> ConsultarPorId(int id)
        {
            var usuario = await _usuarioService.ConsultarPorIdAsync(id);
            return Ok(usuario);
        }

        [HttpPost("criar")]
        [SwaggerOperation(
            Summary = "Cria um novo usuário",
            OperationId = "CriarUsuario")]
        public async Task<ActionResult<UsuarioConsultarDTO>> Criar(
            [FromBody] UsuarioIncluirDTO dto)
        {
            var usuario = await _usuarioService.CriarAsync(dto);

            return Ok(usuario);
        }

        
        [HttpPost("alterar")]
        [SwaggerOperation(
            Summary = "Altera um usuário existente",
            OperationId = "AlterarUsuario")]
        public async Task<ActionResult<UsuarioConsultarDTO>> Alterar(
            int id,
            [FromBody] UsuarioAlterarDTO dto)
        {
            var usuario = await _usuarioService.AlterarAsync(id, dto);
            return Ok(usuario);
        }

        // EXCLUIR (POST)
        [HttpPost("excluir")]
        [SwaggerOperation(
            Summary = "Exclui um usuário",
            OperationId = "ExcluirUsuario")]
        public async Task<IActionResult> Excluir(int id)
        {
            await _usuarioService.ExcluirAsync(id);
            return Ok();
        }
    }
}
