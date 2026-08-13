using ControleFinanceiro.Application.DTO.Categoria;

namespace ControleFinanceiro.Application.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaConsultarDTO>> ListAsync(int usuarioId);
        Task<CategoriaConsultarDTO> GetByIdAsync(int id, int usuarioId);

        Task<CategoriaConsultarDTO> AddAsync(CategoriaIncluirDTO dto, int usuarioId);
        Task<CategoriaConsultarDTO> UpdateAsync(int id, CategoriaAlterarDTO dto, int usuarioId);
        Task DeleteAsync(int id, int usuarioId);
    }
}
