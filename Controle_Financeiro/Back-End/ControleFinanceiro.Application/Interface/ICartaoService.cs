
using ControleFinanceiro.Application.DTO.Cartao;

namespace ControleFinanceiro.Application.Interfaces
{
    public interface ICartaoService
    {
        Task<IEnumerable<CartaoConsultarDTO>> ListAsync(int usuarioId);
        Task<CartaoConsultarDTO> GetByIdAsync(int id, int usuarioId);
        Task<CartaoConsultarDTO> AddAsync(CartaoIncluirDTO dto, int usuarioId);
        Task<CartaoConsultarDTO> UpdateAsync(int id, CartaoAlterarDTO dto, int usuarioId);
        Task DeleteAsync(int id, int usuarioId);
    }
}
