using ControleFinanceiro.Application.DTO.Usuario;

namespace ControleFinanceiro.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioConsultarDTO> GetByIdAsync(int id);

        Task<UsuarioConsultarDTO> AddAsync(UsuarioIncluirDTO dto);
        Task<UsuarioConsultarDTO> UpdateAsync(int id, UsuarioAlterarDTO dto);
        Task DeleteAsync(int id);
    }
}
