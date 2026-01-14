using ControleFinanceiro.Domain.Models;

namespace ControleFinanceiro.Domain.Interfaces
{
    public interface ITransacaoRepository
    {
        Task<IEnumerable<Transacao>> ListarAsync();
        Task<Transacao?> ObterPorIdAsync(int id);

        Task<IEnumerable<Transacao>> ListarPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<Transacao>> ListarPorCategoriaAsync(int categoriaId);

        Task AdicionarAsync(Transacao transacao);
        Task AtualizarAsync(Transacao transacao);
        Task RemoverAsync(Transacao transacao);

        Task SalvarAsync();
    }
}
