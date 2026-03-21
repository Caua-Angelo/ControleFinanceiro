using ControleFinanceiro.Application.DTO.Categoria;

namespace ControleFinanceiro.Application.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaConsultarDTO>> ListAsync();
        Task<CategoriaConsultarDTO> GetByIdAsync(int id);

        Task<CategoriaConsultarDTO> AddAsync(CategoriaIncluirDTO dto);
        Task<CategoriaConsultarDTO> UpdateAsync(int id, CategoriaAlterarDTO dto);
        Task DeleteAsync(int id);
    }
}
