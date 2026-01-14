using ControleFinanceiro.Application.DTO.Transacao;

public interface ITransacaoService
{
    Task<IEnumerable<TransacaoConsultarDTO>> ListarAsync();
    Task<TransacaoConsultarDTO> ObterPorIdAsync(int id);

    Task<IEnumerable<TransacaoConsultarDTO>> ListarPorUsuarioAsync(int usuarioId);
    Task<IEnumerable<TransacaoConsultarDTO>> ListarPorCategoriaAsync(int categoriaId);

    Task<TransacaoConsultarDTO> CriarAsync(TransacaoCriarDTO dto);
    Task<TransacaoConsultarDTO> AtualizarAsync(int id, TransacaoAtualizarDTO dto);
    Task ExcluirAsync(int id);
}
