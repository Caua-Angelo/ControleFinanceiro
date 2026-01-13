using ControleFerias.Domain.Models;

namespace ControleFerias.Domain.Interfaces
{
    public interface IColaboradorRepository
    {
        Task<IEnumerable<Colaborador>> ConsultarColaboradores();
        Task<Colaborador> ConsultarColaboradorPorId(int id);
        Task<Colaborador> IncluirColaborador(Colaborador colaborador);
        Task SalvarMudancas();
        Task<Colaborador> ExcluirColaborador(int id);




    }
}
