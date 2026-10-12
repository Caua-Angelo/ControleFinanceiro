
using ControleFinanceiro.Application.DTO.CompraParcelada;

namespace ControleFinanceiro.Application.Interfaces
{
    public interface ICompraParceladaService
    {
        Task<CompraParceladaResponseDTO> AddAsync(
            CompraParceladaCriarDTO dto,
            int userId);

        Task<IEnumerable<CompraParceladaResponseDTO>> ListByUserAsync(
            int userId);

        Task<CompraParceladaResponseDTO> GetByIdAsync(
            int id,
            int userId);
        Task<CompraParceladaResponseDTO> UpdateAsync(
            int id,
            CompraParceladaAtualizarDTO dto,
            int userId);
        Task DeleteAsync(int id, int userId);

        Task MarcarParcelaComoPagaAsync(
            int parcelaId,
            int usuarioId);

        Task ReabrirParcelaAsync(
            int parcelaId,
            int usuarioId);
    }
}
