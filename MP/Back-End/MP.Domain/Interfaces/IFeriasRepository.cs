using ControleFerias.Domain.Models;

namespace ControleFerias.Domain.Interfaces
{
    public interface IFeriasRepository
    {
        Task<IEnumerable<Ferias>> ConsultarFerias();
        Task<Ferias> ConsultarFeriasPorId(int id);
        Task<IEnumerable<Ferias>> ConsultarFeriasPorColaborador(int id);
        Task<IEnumerable<Ferias>> ConsultarFeriasPorEquipe(int id);
        Task<Ferias> IncluirFerias(Ferias ferias);
        //Task<Ferias> AlterarFerias(Ferias ferias);
        Task SalvarMudancas();
        Task<Ferias> ExcluirFerias(int id);

    }
}
