using ControleFinanceiro.Application.DTO.Usuario;

namespace ControleFinanceiro.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioConsultarDTO>> ConsultarAsync();
        Task<UsuarioConsultarDTO> ConsultarPorIdAsync(int id);

        Task<UsuarioConsultarDTO> CriarAsync(UsuarioIncluirDTO dto);
        Task<UsuarioConsultarDTO> AlterarAsync(int id, UsuarioAlterarDTO dto);
        Task ExcluirAsync(int id);
    }
}
