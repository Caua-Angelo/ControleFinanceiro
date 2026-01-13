
using ControleFerias.Application.DTO;
using Swashbuckle.AspNetCore.Filters;

namespace ControleFerias.Example
{
    public class ColaboradorAlterarDTOExample : IExamplesProvider<ColaboradorAlterarDTO>
    {
        public ColaboradorAlterarDTO GetExamples()
        {
          return new ColaboradorAlterarDTO
          {
             sNome = "Nome alterado",
             EquipeId = 1,
        };
        }
    }
}

