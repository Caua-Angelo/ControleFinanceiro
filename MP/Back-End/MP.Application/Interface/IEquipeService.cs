using ControleFerias.Application.DTO;
using ControleFerias.Domain.Models;

namespace ControleFerias.Application.Interfaces
{
    public interface IEquipeService
    {
        Task<IEnumerable<EquipeConsultarDTO>> ConsultarEquipes();          
        Task<EquipeConsultarDTO> ConsultarEquipePorId(int id);                  
        Task<IEnumerable<ColaboradorConsultarDTO>> ConsultarColaboradoresPorEquipe(int id);
        Task<EquipeConsultarDTO> IncluirEquipe(EquipeIncluirDTO dto);                
        Task<EquipeConsultarDTO> AlterarEquipe(int id, EquipeAlterarDTO dto);          
        Task<EquipeConsultarDTO> ExcluirEquipe(int id);                                   
    }
}
