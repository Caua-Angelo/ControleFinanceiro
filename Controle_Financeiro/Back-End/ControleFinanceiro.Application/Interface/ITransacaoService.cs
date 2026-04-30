using ControleFinanceiro.Application.DTO.Transacao;

namespace ControleFinanceiro.Application.Interfaces;

public interface ITransacaoService
{
    Task<IEnumerable<TransacaoConsultarDTO>> ListByUserAsync(int userId);

    Task<TransacaoConsultarDTO> GetByIdAsync(int id, int userId);

    Task<TransacaoConsultarDTO> AddAsync(TransacaoCriarDTO dto, int userId);

    Task<TransacaoConsultarDTO> UpdateAsync(int id, TransacaoAtualizarDTO dto, int userId);

    Task DeleteAsync(int id, int userId);
}