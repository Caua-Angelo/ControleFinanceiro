using ControleFinanceiro.Domain.Models;

namespace ControleFinanceiro.Domain.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> ListAsync();
        Task<Categoria?> GetByIdAsync(int id);

        Task AddAsync(Categoria categoria);
        Task DeleteAsync(Categoria categoria);

        Task SaveAsync();
    }
}
