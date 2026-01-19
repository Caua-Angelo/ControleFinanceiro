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

        // GET /api/transacoes
        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista todas as transações (opcionalmente por usuário ou categoria)",
            OperationId = "ListarTransacoes")]
        public async Task<ActionResult<IEnumerable<TransacaoConsultarDTO>>> Get(
            [FromQuery] int? usuarioId,
            [FromQuery] int? categoriaId)
        {
            if (usuarioId.HasValue)
                return Ok(await _transacaoService.ListarPorUsuarioAsync(usuarioId.Value));

            if (categoriaId.HasValue)
                return Ok(await _transacaoService.ListarPorCategoriaAsync(categoriaId.Value));

            return Ok(await _transacaoService.ListarAsync());
        }

        // GET /api/transacoes/{id}
        [HttpGet("{id:int}")]
        [SwaggerOperation(
            Summary = "Consulta uma transação por ID",
            OperationId = "ConsultarTransacaoPorId")]
        public async Task<ActionResult<TransacaoConsultarDTO>> GetById(int id)
        {
            var transacao = await _transacaoService.ObterPorIdAsync(id);
            return Ok(transacao);
        }

        // POST /api/transacoes
        [HttpPost]
        [SwaggerOperation(
            Summary = "Cria uma nova transação",
            OperationId = "CriarTransacao")]
        public async Task<ActionResult<TransacaoConsultarDTO>> Post(
            [FromBody] TransacaoCriarDTO dto)
        {
            var transacao = await _transacaoService.CriarAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = transacao.Id },
                transacao
            );
        }

        // PUT /api/transacoes/{id}
        [HttpPut("{id:int}")]
        [SwaggerOperation(
            Summary = "Atualiza uma transação existente",
            OperationId = "AtualizarTransacao")]
        public async Task<ActionResult<TransacaoConsultarDTO>> Put(
            int id,
            [FromBody] TransacaoAtualizarDTO dto)
        {
            var transacao = await _transacaoService.AtualizarAsync(id, dto);
            return Ok(transacao);
        }

        // DELETE /api/transacoes/{id}
        [HttpDelete("{id:int}")]
        [SwaggerOperation(
            Summary = "Exclui uma transação",
            OperationId = "ExcluirTransacao")]
        public async Task<IActionResult> Delete(int id)
        {
            await _transacaoService.ExcluirAsync(id);
            return NoContent();
        }
    }
}
