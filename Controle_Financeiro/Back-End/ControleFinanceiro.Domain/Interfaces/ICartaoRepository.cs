
using ControleFinanceiro.Domain.Models;

namespace ControleFinanceiro.Domain.Interfaces
{
    public interface ICartaoRepository
    {
        Task<IEnumerable<Cartao>> ListAsync(int usuarioId);
        Task<Cartao?> GetByIdAsync(int id, int usuarioId);

        Task AddAsync(Cartao cartao);
        Task DeleteAsync(Cartao cartao);

        Task SaveAsync();
    }
}
