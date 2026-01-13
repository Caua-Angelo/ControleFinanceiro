using ControleFerias.Application.DTO;

namespace ControleFerias.Application.Interfaces
{
    public interface IFeriasService
    {
        Task<IEnumerable<FeriasConsultarDTO>> ConsultarFerias();
        Task<FeriasConsultarDTO> ConsultarFeriasPorId(int id);
        Task<IEnumerable<FeriasConsultarDTO>> ConsultarFeriasPorColaborador(int id);
        Task<IEnumerable<FeriasConsultarDTO>> ConsultarFeriasPorEquipe(int id);
        Task<FeriasConsultarDTO> IncluirFerias(FeriasIncluirDTO dto);
        Task<FeriasConsultarDTO> AlterarFerias(int id, FeriasAlterarDTO dto);
        Task<FeriasConsultarDTO> ExcluirFerias(int id);
    }
}
