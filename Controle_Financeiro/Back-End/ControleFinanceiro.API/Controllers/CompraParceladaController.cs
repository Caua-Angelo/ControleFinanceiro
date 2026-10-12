
using ControleFinanceiro.Application.DTO.CompraParcelada;
using ControleFinanceiro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ControleFinanceiro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompraParceladaController : ControllerBase
    {
        private readonly ICompraParceladaService _compraParceladaService;

        public CompraParceladaController(
            ICompraParceladaService compraParceladaService)
        {
            _compraParceladaService = compraParceladaService;
        }

        private int UsuarioId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var compras = await _compraParceladaService
                .ListByUserAsync(UsuarioId);

            return Ok(compras);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var compra = await _compraParceladaService
                .GetByIdAsync(id, UsuarioId);

            return Ok(compra);
        }

        [HttpPost]
        public async Task<IActionResult> Criar(
            CompraParceladaCriarDTO dto)
        {
            var compra = await _compraParceladaService
                .AddAsync(dto, UsuarioId);

            return CreatedAtAction(
                nameof(BuscarPorId),
                new { id = compra.Id },
                compra);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Atualizar(int id,CompraParceladaAtualizarDTO dto)
        {
            var compra = await _compraParceladaService.UpdateAsync(
                id, dto, UsuarioId);

            return Ok(compra);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Excluir(int id)
        {
            await _compraParceladaService
                .DeleteAsync(id, UsuarioId);

            return NoContent();
        }

        [HttpPatch("parcelas/{id:int}/pagar")]
        public async Task<IActionResult> MarcarParcelaComoPaga(int id)
        {
            await _compraParceladaService
                .MarcarParcelaComoPagaAsync(id, UsuarioId);

            return NoContent();
        }

        [HttpPatch("parcelas/{id:int}/reabrir")]
        public async Task<IActionResult> ReabrirParcela(int id)
        {
            await _compraParceladaService.
                ReabrirParcelaAsync(id, UsuarioId);

            return NoContent();
        }
    }
}
