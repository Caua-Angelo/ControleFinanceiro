
using ControleFinanceiro.Domain.Models;

namespace ControleFinanceiro.Domain.Interfaces
{
    public interface ICompraParceladaRepository
    {
        Task<CompraParcelada?> GetByIdAsync(int id);

        Task<IEnumerable<CompraParcelada>> ListByUserAsync(int usuarioId);

        Task<Parcela?> GetParcelaByIdAsync(int parcelaId);

        Task AddAsync(CompraParcelada compra);

        Task DeleteAsync(CompraParcelada compra);

        Task RemoverParcelasAsync(IEnumerable<Parcela> parcelas);

        Task SaveAsync();
    }
}
