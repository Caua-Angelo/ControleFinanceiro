using ControleFerias.Application.DTO;
using ControleFerias.Domain.Models;

namespace ControleFerias.Application.Interfaces
{
    public interface IColaboradorService
    {
        Task<IEnumerable<ColaboradorConsultarDTO>> ConsultarColaboradores();
        Task<ColaboradorConsultarDTO> ConsultarColaboradorPorId(int id);
        Task<ColaboradorConsultarDTO> IncluirColaborador(ColaboradorIncluirDTO dto);
        Task<ColaboradorConsultarDTO> AlterarColaborador(int id, ColaboradorAlterarDTO dto);
        Task<ColaboradorConsultarDTO> ExcluirColaborador(int id);
    }
}