
using ControleFinanceiro.Application.DTO.Cartao;
using ControleFinanceiro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ControleFinanceiro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartaoController : ControllerBase
    {
        private readonly ICartaoService _cartaoService;

        public CartaoController(ICartaoService cartaoService)
        {
            _cartaoService = cartaoService;
        }

        private int UsuarioId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var cartoes = await _cartaoService.ListAsync(UsuarioId);
            return Ok(cartoes);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var cartao = await _cartaoService.GetByIdAsync(id, UsuarioId);
            return Ok(cartao);
        }

        [HttpPost]
        public async Task<IActionResult> Criar(CartaoIncluirDTO dto)
        {
            var cartao = await _cartaoService.AddAsync(dto, UsuarioId);
            return CreatedAtAction(
                nameof(BuscarPorId),
                new { id = cartao.Id },
                cartao);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Atualizar(
            int id, CartaoAlterarDTO dto)
        {
            var cartao = await _cartaoService.UpdateAsync(
                id, dto, UsuarioId);

            return Ok(cartao);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Excluir(int id)
        {
            await _cartaoService.DeleteAsync(id, UsuarioId);
            return NoContent();
        }
    }
}
