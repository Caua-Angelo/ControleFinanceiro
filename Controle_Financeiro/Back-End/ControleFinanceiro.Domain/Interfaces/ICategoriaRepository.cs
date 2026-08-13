using ControleFinanceiro.Domain.Models;

namespace ControleFinanceiro.Domain.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> ListAsync(int usuarioId);
        Task<Categoria?> GetByIdAsync(int id, int usuarioId);

        Task AddAsync(Categoria categoria);
        Task DeleteAsync(Categoria categoria);

        Task SaveAsync();
    }
}
