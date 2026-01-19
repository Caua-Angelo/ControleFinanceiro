using ControleFinanceiro.Application.DTO.Categoria;

namespace ControleFinanceiro.Application.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaConsultarDTO>> ConsultarAsync();
        Task<CategoriaConsultarDTO> ConsultarPorIdAsync(int id);

        Task<CategoriaConsultarDTO> CriarAsync(CategoriaIncluirDTO dto);
        Task<CategoriaConsultarDTO> AlterarAsync(int id, CategoriaAlterarDTO dto);
        Task ExcluirAsync(int id);
    }
}
