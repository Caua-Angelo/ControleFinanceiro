using ControleFinanceiro.Application.DTO.Transacao;
using ControleFinanceiro.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ControleFinanceiro.API.Controllers
{
    [ApiController]
    [Route("api/transacoes")]
    public class TransacaoController : ControllerBase
    {
        private readonly ITransacaoService _transacaoService;

        public TransacaoController(ITransacaoService transacaoService)
        {
            _transacaoService = transacaoService;
        }

        // CONSULTAR TODAS
        [HttpGet("consultar")]
        [SwaggerOperation(
            Summary = "Lista todas as transações",
            OperationId = "ConsultarTransacoes")]
        public async Task<ActionResult<IEnumerable<TransacaoConsultarDTO>>> Consultar()
        {
            var transacoes = await _transacaoService.ListarAsync();
            return Ok(transacoes);
        }

        // CONSULTAR POR ID
        [HttpGet("consultar-por-id")]
        [SwaggerOperation(
            Summary = "Consulta uma transação por ID",
            OperationId = "ConsultarTransacaoPorId")]
        public async Task<ActionResult<TransacaoConsultarDTO>> ConsultarPorId(
            [FromQuery] int id)
        {
            var transacao = await _transacaoService.ObterPorIdAsync(id);
            return Ok(transacao);
        }

        // CONSULTAR POR USUÁRIO
        [HttpGet("consultar-por-usuario")]
        [SwaggerOperation(
            Summary = "Lista as transações de um usuário",
            OperationId = "ConsultarTransacoesPorUsuario")]
        public async Task<ActionResult<IEnumerable<TransacaoConsultarDTO>>> ConsultarPorUsuario(
            [FromQuery] int usuarioId)
        {
            var transacoes = await _transacaoService.ListarPorUsuarioAsync(usuarioId);
            return Ok(transacoes);
        }

        // CONSULTAR POR CATEGORIA
        [HttpGet("consultar-por-categoria")]
        [SwaggerOperation(
            Summary = "Lista as transações de uma categoria",
            OperationId = "ConsultarTransacoesPorCategoria")]
        public async Task<ActionResult<IEnumerable<TransacaoConsultarDTO>>> ConsultarPorCategoria(
            [FromQuery] int categoriaId)
        {
            var transacoes = await _transacaoService.ListarPorCategoriaAsync(categoriaId);
            return Ok(transacoes);
        }

        // CRIAR
        [HttpPost("criar")]
        [SwaggerOperation(
            Summary = "Cria uma nova transação",
            OperationId = "CriarTransacao")]
        public async Task<ActionResult<TransacaoConsultarDTO>> Criar(
            [FromBody] TransacaoCriarDTO dto)
        {
            var transacao = await _transacaoService.CriarAsync(dto);
            return Ok(transacao);
        }

        // ATUALIZAR
        [HttpPost("atualizar")]
        [SwaggerOperation(
            Summary = "Atualiza uma transação existente",
            OperationId = "AtualizarTransacao")]
        public async Task<ActionResult<TransacaoConsultarDTO>> Atualizar(
            [FromQuery] int id,
            [FromBody] TransacaoAtualizarDTO dto)
        {
            var transacao = await _transacaoService.AtualizarAsync(id, dto);
            return Ok(transacao);
        }

        // EXCLUIR
        [HttpPost("excluir")]
        [SwaggerOperation(
            Summary = "Exclui uma transação",
            OperationId = "ExcluirTransacao")]
        public async Task<IActionResult> Excluir(
            [FromQuery] int id)
        {
            await _transacaoService.ExcluirAsync(id);
            return Ok();
        }
    }
}
