using ControleFinanceiro.Application.DTO.Transacao;
using ControleFinanceiro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace ControleFinanceiro.API.Controllers
{
    [Authorize]
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
            Summary = "Lista todas as transações do usuário autenticado",
            Description = "Retorna todas as transações associadas ao usuário logado, identificado pelo token JWT.",
            OperationId = "ListarTransacoesUsuarioLogado")]
        public async Task<ActionResult<IEnumerable<TransacaoConsultarDTO>>> Get()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var transacoes = await _transacaoService.ListByUserAsync(userId);

            return Ok(transacoes);
        }

        // GET /api/transacoes/{id}
        [HttpGet("{id:int}")]
        [SwaggerOperation(
            Summary = "Consulta uma transação por ID",
            Description = "Retorna uma transação específica pertencente ao usuário autenticado.",
            OperationId = "ConsultarTransacaoPorId")]
        public async Task<ActionResult<TransacaoConsultarDTO>> GetById(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var transacao = await _transacaoService.GetByIdAsync(id, userId);

            return Ok(transacao);
        }

        // POST /api/transacoes
        [HttpPost]
        [SwaggerOperation(
            Summary = "Cria uma nova transação",
            Description = "Cria uma nova transação associada automaticamente ao usuário autenticado via JWT.",
            OperationId = "CriarTransacao")]
        public async Task<ActionResult<TransacaoConsultarDTO>> Post(
            [FromBody] TransacaoCriarDTO dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var transacao = await _transacaoService.AddAsync(dto, userId);

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
            Description = "Atualiza uma transação pertencente ao usuário autenticado.",
            OperationId = "AtualizarTransacao")]
        public async Task<ActionResult<TransacaoConsultarDTO>> Put(
            int id,
            [FromBody] TransacaoAtualizarDTO dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var transacao = await _transacaoService.UpdateAsync(id, dto, userId);

            return Ok(transacao);
        }

        // DELETE /api/transacoes/{id}
        [HttpDelete("{id:int}")]
        [SwaggerOperation(
            Summary = "Exclui uma transação",
            Description = "Remove uma transação pertencente ao usuário autenticado.",
            OperationId = "ExcluirTransacao")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _transacaoService.DeleteAsync(id, userId);

            return NoContent();
        }
    }
}