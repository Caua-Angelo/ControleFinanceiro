using ControleFinanceiro.Domain.Models;

namespace ControleFinanceiro.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> ListAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task AddAsync(Usuario usuario);
        Task<Usuario?> GetByIdTrackedAsync(int id);
        Task UpdateAsync(Usuario usuario);
        Task DeleteAsync(Usuario usuario);

        Task SaveAsync();

        Task<Usuario?> GetByEmailAsync(string email);
        
    }
}
