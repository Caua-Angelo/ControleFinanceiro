using ControleFinanceiro.Application.DTO.Categoria;
using ControleFinanceiro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace ControleFinanceiro.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/categorias")]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // GET /api/categorias
        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista todas as categorias cadastradas",
            OperationId = "ListarCategorias")]
        public async Task<ActionResult<IEnumerable<CategoriaConsultarDTO>>> Get()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var categorias = await _categoriaService.ListAsync(usuarioId);
            return Ok(categorias);
        }

        // GET /api/categorias/{id}
        [HttpGet("{id:int}")]
        [SwaggerOperation(
            Summary = "Consulta uma categoria por ID",
            OperationId = "ConsultarCategoriaPorId")]
        public async Task<ActionResult<CategoriaConsultarDTO>> GetById(int id)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var categoria = await _categoriaService.GetByIdAsync(id, usuarioId);
            return Ok(categoria);
        }

        // POST /api/categorias
        [HttpPost]
        [SwaggerOperation(
            Summary = "Cria uma nova categoria",
            OperationId = "CriarCategoria")]
        public async Task<ActionResult<CategoriaConsultarDTO>> Post(
            [FromBody] CategoriaIncluirDTO dto)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var categoria = await _categoriaService.AddAsync(dto, usuarioId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = categoria.Id },
                categoria
            );
        }

        // PUT /api/categorias/{id}
        [HttpPut("{id:int}")]
        [SwaggerOperation(
            Summary = "Altera uma categoria existente",
            OperationId = "AlterarCategoria")]
        public async Task<ActionResult<CategoriaConsultarDTO>> Put(
            int id,
            [FromBody] CategoriaAlterarDTO dto)
        {
            var categoria = await _categoriaService.UpdateAsync(id, dto);
            return Ok(categoria);
        }

        // DELETE /api/categorias/{id}
        [HttpDelete("{id:int}")]
        [SwaggerOperation(
            Summary = "Exclui uma categoria",
            OperationId = "ExcluirCategoria")]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoriaService.DeleteAsync(id);
            return NoContent();
        }
    }
}
