
using ControleFinanceiro.Application.DTO;
using ControleFinanceiro.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

public class FeriasAlterarDTOExample : IExamplesProvider<FeriasAlterarDTO>
{
    public FeriasAlterarDTO GetExamples()
    {
        return new FeriasAlterarDTO
        {
            dDataInicio = new DateTime(2025, 11, 5),
            sDias = 15,
            dDataFinal = new DateTime(2025, 11, 20),
            sComentario = "Reprovado por : 50% ou mais da equipe já está em férias nesse período",
            status = StatusFerias.Reprovado
        };
    }
}
