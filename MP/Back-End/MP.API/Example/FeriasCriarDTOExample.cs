
using ControleFerias.Application.DTO;
using Swashbuckle.AspNetCore.Filters;

public class FeriasCriarDTOExample : IExamplesProvider<FeriasIncluirDTO>
{
    public FeriasIncluirDTO GetExamples()
    {
        return new FeriasIncluirDTO
        {
            dDataInicio = new DateTime(2025, 9, 5),
            sDias = 15,
            dDataFinal = new DateTime(2025, 9, 20),
            sComentario = "Reprovado por :",
            ColaboradorId = 158
        };
    }
}
