
using ControleFinanceiro.Application.DTO;
using Swashbuckle.AspNetCore.Filters;

namespace ControleFinanceiro.Example
{
    public class ColaboradorAlterarDTOExample : IExamplesProvider<UsuarioAlterarDTO>
    {
        public UsuarioAlterarDTO GetExamples()
        {
          return new UsuarioAlterarDTO
          {
             sNome = "Nome alterado",
             EquipeId = 1,
        };
        }
    }
}

