using ControleFinanceiro.Domain.Models;

namespace ControleFinanceiro.Domain.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> ConsultarAsync();
        Task<Categoria?> ConsultarPorIdAsync(int id);

        Task AdicionarAsync(Categoria categoria);
        Task RemoverAsync(Categoria categoria);

        Task SalvarAsync();
    }
}
