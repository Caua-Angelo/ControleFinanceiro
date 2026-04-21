using ControleFinanceiro.Application.DTO.Transacao;

namespace ControleFinanceiro.Application.Interfaces;
public interface ITransacaoService
{
    Task<IEnumerable<TransacaoConsultarDTO>> ListAsync();
    Task<TransacaoConsultarDTO> GetByIdAsync(int id);

    Task<IEnumerable<TransacaoConsultarDTO>> ListByUserAsync(int usuarioId);
    Task<IEnumerable<TransacaoConsultarDTO>> ListByCategoryAsync(int categoriaId);

    Task<TransacaoConsultarDTO> AddAsync(TransacaoCriarDTO dto);
    Task<TransacaoConsultarDTO> UpdateAsync(int id, TransacaoAtualizarDTO dto);
    Task DeleteAsync(int id);
}
