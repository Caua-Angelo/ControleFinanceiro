using ControleFinanceiro.Application.DTO;
using ControleFinanceiro.Domain.Models;

namespace ControleFinanceiro.Application.Interfaces
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