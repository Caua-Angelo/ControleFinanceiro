using ControleFerias.Application.Common;
using ControleFerias.Application.DTO;
using ControleFerias.Application.Interfaces;
using ControleFerias.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ControleFerias.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EquipeController : Controller
    {
        private readonly IEquipeService _equipeService;

        public EquipeController(IEquipeService equipeService)
        {
            _equipeService = equipeService;
        }

        [HttpGet, Route("ConsultarEquipes")]
        #region Documentacao
        [SwaggerOperation(
        Summary = "Consulta todas as equipes disponíveis",
         Description = @"
          Entrada:
        

         *Saida*
        - Todas as equipes disponíveis
        
        ",
        OperationId = "ConsultarEquipesSwagger")]
        #endregion
        public async Task<ActionResult> ConsultarEquipes()
        {
            var equipes = await _equipeService.ConsultarEquipes();
            return Ok(ApiResponse<IEnumerable<EquipeConsultarDTO>>.Sucesso(
        "Consulta realizada com sucesso.", equipes));
        }

        [HttpGet, Route("ConsultarEquipePorId")]
        #region Documentacao
        [SwaggerOperation(
        Summary = "Consulta uma Equipe por Id",
         Description = @"
          Entrada: Id da Equipe
        

         *Saida*
        - Objeto da equipe(Do Id enviado)",
            OperationId = "ConsultarEquipePorIdSwagger")]
        #endregion
        public async Task<ActionResult<EquipeConsultarDTO>> ConsultarEquipePorId(int id)
        {
            var equipe = await _equipeService.ConsultarEquipePorId(id);
            return Ok(ApiResponse<EquipeConsultarDTO>.Sucesso(
       "Consulta realizada com sucesso.", equipe));

        }
        [HttpGet, Route("ConsultarColaboradoresPorEquipe")]
        #region Documentacao
        [SwaggerOperation(
        Summary = "Consulta todos os colaboradores de uma equipe",
         Description = @"
          Entrada: Id da equipe
        

         *Saida*
        - Todos os colaboradores da equipe(Do Id enviado)
        
        ",
        OperationId = "ConsultarColaboradoresPorEquipeSwagger")]
        #endregion
        public async Task<ActionResult> ConsultarColaboradoresPorEquipe(int id)
        {
            var colaboradores = await _equipeService.ConsultarColaboradoresPorEquipe(id);
            return Ok(ApiResponse<IEnumerable<ColaboradorConsultarDTO>>.Sucesso(
                "Colaboradores consultados com sucesso.", colaboradores));
        }
        [HttpPost, Route("IncluirEquipe")]
        #region Documentacao
        [SwaggerOperation(
        Summary = "Adiciona uma equipe se os valores forem válidos",
         Description = @"
          Entrada:
        - sNome: Nome da equipe (mínimo 2 caracteres)

         *Saida*
        - Equipe (nomeequipe) criada com sucesso",

        OperationId = "IncluirEquipeSwagger")]
        #endregion
        public async Task<ActionResult> IncluirEquipe(EquipeIncluirDTO EquipeIncluirDTO)
        {
            var equipeCriada = await _equipeService.IncluirEquipe(EquipeIncluirDTO);

            return CreatedAtAction(nameof(ConsultarEquipePorId), new { id = equipeCriada.Id },
        ApiResponse<EquipeConsultarDTO>.Sucesso(
            $"Equipe {equipeCriada.sNome} criada com sucesso.", equipeCriada, 201));
        }

        [HttpPost, Route("AlterarEquipe")]
        #region Documentacao
        [SwaggerOperation(
        Summary = "Altera uma equipe se os valores forem válidos",
         Description = @"
              Entrada:
            - Id: Id da Equipe (existente no banco de dados)

            - sNome: Nome da equipe (mínimo 2 caracteres)

             *Saida*
            -  Nome da equipe foi alterada de {Nome antigo} para {Nome Novo} com sucesso.
            -Obs:Id da equipe(Verificar no endpoint ConsultarEquipe)",
        OperationId = "AlterarEquipeSwagger")]
        #endregion
        public async Task<IActionResult> AlterarEquipe(int id, [FromBody] EquipeAlterarDTO EquipeAlterarDTO)
        {
            var equipeAtual = await _equipeService.ConsultarEquipePorId(id);
            var equipeAlterada = await _equipeService.AlterarEquipe(id, EquipeAlterarDTO);

            bool nomeAlterado = equipeAtual.sNome != EquipeAlterarDTO.sNome;
            var mensagem = nomeAlterado
                ? $"Nome da equipe foi alterado de {equipeAtual.sNome} para {EquipeAlterarDTO.sNome} com sucesso."
                : "Nenhuma alteração foi realizada.";

            return Ok(ApiResponse<EquipeConsultarDTO>.Sucesso(mensagem, equipeAlterada));
        }

        [HttpPost]
        [Route("ExcluirEquipe")]
        #region Documentacao
        [SwaggerOperation(
        Summary = "Exclui uma Equipe se o Id for compatível com alguma Equipe no banco",
        Description = @"
            **Entrada:**
            - `Id`: Id da Equipe (maior que 0)

            *Saida*
            - Equipe(Nome da Equipe) excluida com sucesso.",
        OperationId = "ExcluirEquipeSwagger")]
        #endregion
        public async Task<ActionResult> ExcluirEquipe(int id)
        {
            var equipe = await _equipeService.ExcluirEquipe(id);

            return Ok(ApiResponse<EquipeConsultarDTO>.Sucesso(
                $"Equipe {equipe.sNome} excluída com sucesso.", equipe));
        }
    }
}
