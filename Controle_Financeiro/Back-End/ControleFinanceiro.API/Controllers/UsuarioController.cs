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

        // GET /api/usuarios
        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista todos os usuários cadastrados",
            OperationId = "ListarUsuarios")]
        public async Task<ActionResult<IEnumerable<UsuarioConsultarDTO>>> Get()
        {
            var usuarios = await _usuarioService.ConsultarAsync();
            return Ok(usuarios);
        }

        // GET /api/usuarios/{id}
        [HttpGet("{id:int}")]
        [SwaggerOperation(
            Summary = "Consulta um usuário por ID",
            OperationId = "ConsultarUsuarioPorId")]
        public async Task<ActionResult<UsuarioConsultarDTO>> GetById(int id)
        {
            var usuario = await _usuarioService.ConsultarPorIdAsync(id);
            return Ok(usuario);
        }

        // POST /api/usuarios
        [HttpPost]
        [SwaggerOperation(
            Summary = "Cria um novo usuário",
            OperationId = "CriarUsuario")]
        public async Task<ActionResult<UsuarioConsultarDTO>> Post(
            [FromBody] UsuarioIncluirDTO dto)
        {
            var usuario = await _usuarioService.CriarAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = usuario.Id },
                usuario
            );
        }

        // PUT /api/usuarios/{id}
        [HttpPut("{id:int}")]
        [SwaggerOperation(
            Summary = "Altera um usuário existente",
            OperationId = "AlterarUsuario")]
        public async Task<ActionResult<UsuarioConsultarDTO>> Put(
            int id,
            [FromBody] UsuarioAlterarDTO dto)
        {
            var usuario = await _usuarioService.AlterarAsync(id, dto);
            return Ok(usuario);
        }

        // DELETE /api/usuarios/{id}
        [HttpDelete("{id:int}")]
        [SwaggerOperation(
            Summary = "Exclui um usuário",
            OperationId = "ExcluirUsuario")]
        public async Task<IActionResult> Delete(int id)
        {
            await _usuarioService.ExcluirAsync(id);
            return NoContent();
        }
    }
}
