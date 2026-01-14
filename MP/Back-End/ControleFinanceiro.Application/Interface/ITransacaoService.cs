using ControleFinanceiro.Application.DTO;

namespace ControleFinanceiro.Application.Interfaces
{
    public interface ITransacaoService
    {
        Task<IEnumerable<TransacaoConsultarDTO>> ConsultarAsync();
        Task<TransacaoConsultarDTO> ConsultarPorIdAsync(int id);

        Task<IEnumerable<TransacaoConsultarDTO>> ConsultarPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<TransacaoConsultarDTO>> ConsultarPorCategoriaAsync(int categoriaId);

        Task<TransacaoConsultarDTO> CriarAsync(TransacaoCriarDTO dto);
        Task<TransacaoConsultarDTO> AlterarAsync(int id, TransacaoAtualizarDTO dto);
        Task ExcluirAsync(int id);
    }
}
