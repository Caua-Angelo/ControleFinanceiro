using ControleFinanceiro.Application.DTO.Categoria;
using ControleFinanceiro.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ControleFinanceiro.API.Controllers
{
    [ApiController]
    [Route("api/categorias")]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // CONSULTAR TODAS
        [HttpGet("consultar")]
        [SwaggerOperation(
            Summary = "Lista todas as categorias cadastradas",
            OperationId = "ConsultarCategorias")]
        public async Task<ActionResult<IEnumerable<CategoriaConsultarDTO>>> Consultar()
        {
            var categorias = await _categoriaService.ConsultarAsync();
            return Ok(categorias);
        }

        // CONSULTAR POR ID
        [HttpGet("consultar-por-id")]
        [SwaggerOperation(
            Summary = "Consulta uma categoria por ID",
            OperationId = "ConsultarCategoriaPorId")]
        public async Task<ActionResult<CategoriaConsultarDTO>> ConsultarPorId(int id)
        {
            var categoria = await _categoriaService.ConsultarPorIdAsync(id);
            return Ok(categoria);
        }

        // CRIAR
        [HttpPost("criar")]
        [SwaggerOperation(
            Summary = "Cria uma nova categoria",
            OperationId = "CriarCategoria")]
        public async Task<ActionResult<CategoriaConsultarDTO>> Criar(
            [FromBody] CategoriaIncluirDTO dto)
        {
            var categoria = await _categoriaService.CriarAsync(dto);
            return Ok(categoria);
        }

        // ALTERAR (POST)
        [HttpPost("alterar")]
        [SwaggerOperation(
            Summary = "Altera uma categoria existente",
            OperationId = "AlterarCategoria")]
        public async Task<ActionResult<CategoriaConsultarDTO>> Alterar(
            int id,
            [FromBody] CategoriaAlterarDTO dto)
        {
            var categoria = await _categoriaService.AlterarAsync(id, dto);
            return Ok(categoria);
        }

        // EXCLUIR (POST)
        [HttpPost("excluir")]
        [SwaggerOperation(
            Summary = "Exclui uma categoria",
            OperationId = "ExcluirCategoria")]
        public async Task<IActionResult> Excluir(int id)
        {
            await _categoriaService.ExcluirAsync(id);
            return Ok();
        }
    }
}
