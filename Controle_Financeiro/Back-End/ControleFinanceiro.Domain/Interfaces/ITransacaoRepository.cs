using ControleFinanceiro.Domain.Models;

namespace ControleFinanceiro.Domain.Interfaces
{
    public interface ITransacaoRepository
    {
        Task<IEnumerable<Transacao>> ListAsync();
        Task<Transacao?> GetByIdAsync(int id);

        Task<IEnumerable<Transacao>> ListByUserAsync(int usuarioId);
        Task<IEnumerable<Transacao>> ListByCategoryAsync(int categoriaId);

        Task AddAsync(Transacao transacao);
        Task UpdateAsync(Transacao transacao);
        Task DeleteAsync(Transacao transacao);

        Task SaveAsync();
    }
}
