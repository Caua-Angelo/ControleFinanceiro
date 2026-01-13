using ControleFerias.Domain.Models;

namespace ControleFerias.Domain.Interfaces
{
    public interface IEquipeRepository
    {
        Task<IEnumerable<Equipe>> ConsultarEquipes();
        //Task<Equipe> AlterarEquipe(Equipe equipe);
        Task<Equipe> ConsultarEquipePorId(int id);
        Task<IEnumerable<Colaborador>> ConsultarColaboradoresPorEquipe(int id);
        Task<Equipe> IncluirEquipe(Equipe equipe);
        Task SalvarMudancas();
        Task<Equipe> ExcluirEquipe(int id);


    }
}
