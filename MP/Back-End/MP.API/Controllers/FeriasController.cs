using ControleFerias.Application.Common;
using ControleFerias.Application.DTO;
using ControleFerias.Application.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ControleFerias.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FeriasController : ControllerBase
    {
        private readonly IFeriasService _feriasService;

        public FeriasController(IFeriasService feriasService)
        {
            _feriasService = feriasService;
        }
        [HttpGet, Route("ConsultarFerias")]
        #region Documentacao
        [SwaggerOperation(
        Summary = "Consulta todas as férias disponíveis",
         Description = @"
          Entrada: 
        

         *Saida*
        - Objeto das Férias",
            OperationId = "ConsultarFeriasSwagger")]
        #endregion
        public async Task<ActionResult> ConsultarFerias()
        {
            var ferias = await _feriasService.ConsultarFerias();

            return Ok(new ApiResponse<IEnumerable<FeriasConsultarDTO>>
            {
                StatusCode = 200,
                Mensagem = "Consulta de férias realizada com sucesso.",
                Data = ferias
            });
        }

        [HttpGet, Route("ConsultarFeriasPorId")]
        #region Documentacao
        [SwaggerOperation(
        Summary = "Consulta uma férias pelo Id",
         Description = @"
          Entrada: Id da férias
        

         *Saida*
        - Objeto da Férias(Do Id enviado)",
            OperationId = "ConsultarFeriasPorIdSwagger")]
        #endregion
        public async Task<ActionResult<ColaboradorConsultarDTO>> ConsultarFeriasPorId(int id)
        {
            var feriasColaborador = await _feriasService.ConsultarFeriasPorId(id);
            return Ok(new ApiResponse<FeriasConsultarDTO>
            {
                StatusCode = 200,
                Mensagem = "Férias consultada com sucesso.",
                Data = feriasColaborador
            });
        }
        [HttpGet, Route("ConsultarFeriasPorColaborador")]
        #region Documentacao
        [SwaggerOperation(
        Summary = "Consulta todos as férias de um colaborador",
         Description = @"
          Entrada: Id do Colaborador
        

         *Saida*
        - Objeto do Colaborador(Do Id enviado)",
            OperationId = "ConsultarFeriasPorColaboradorSwagger")]
        #endregion
        public async Task<ActionResult<ColaboradorConsultarDTO>> ConsultarFeriasPorColaborador(int id)
        {
            var feriasColaborador = await _feriasService.ConsultarFeriasPorColaborador(id);
            return Ok(new ApiResponse<IEnumerable<FeriasConsultarDTO>>
            {
                StatusCode = 200,
                Mensagem = "Férias do colaborador consultadas com sucesso.",
                Data = feriasColaborador
            });
        }

        [EnableCors]
        [HttpPost, Route("IncluirFerias")]
        #region Documentacao
        [SwaggerOperation(
        Summary = "Adiciona uma Férias se os valores forem válidos",
         Description = @"
          Entrada:
        - dDataInicio: data inicial das férias (Maior que hoje)
        - sDias: dias das férias (maior que 0)
        - sDataFinal: Data Final das férias(maior que a data inicial)
        - sComentario: Comentário opcional
        - colaboradorId: Id do colaborador a ser vinculado

         *Saida*
        - Objeto Ferias 


        - Observação: Padrão de datas dia/mês/ano ",


        OperationId = "IncluirFeriasSwagger")]
        #endregion
        public async Task<ActionResult> IncluirFerias(int ColaboradorId, [FromBody] FeriasIncluirDTO dto)
        {
            

            dto.ColaboradorId = ColaboradorId;
            var feriasCriadas = await _feriasService.IncluirFerias(dto);

            return CreatedAtAction(nameof(ConsultarFeriasPorId), new { id = feriasCriadas.Id },
                new ApiResponse<FeriasConsultarDTO>
                {
                    StatusCode = 201,
                    Mensagem = "Férias criadas com sucesso.",
                    Data = feriasCriadas
                });
        }

        [HttpPost, Route("AlterarFerias")]
        #region Documentacao
        [SwaggerOperation(
        Summary = "Altera uma férias se os valores forem válidos",
         Description = @"
              Entrada:
             - Id: Id das férias (existente no banco de dados)
             - dDataInicio, dDataFinal, sDias, sComentario: novos valores

             *Saída*
             - Férias alteradas com sucesso",
        OperationId = "AlterarFeriasSwagger")]
        #endregion
        public async Task<IActionResult> AlterarFerias(int id, [FromBody] FeriasAlterarDTO feriasAlterarDTO)
        {
            

            var feriasAntiga = await _feriasService.ConsultarFeriasPorId(id);
            var feriasAlterada = await _feriasService.AlterarFerias(id, feriasAlterarDTO);

            return Ok(new ApiResponse<FeriasConsultarDTO>
            {
                StatusCode = 200,
                Mensagem = "Férias alteradas com sucesso.",
                Data = feriasAlterada
            });
        }

        [HttpPost, Route("ExcluirFerias")]
        #region Documentacao
        [SwaggerOperation(
        Summary = "Exclui uma Ferias se o Id for compatível com alguma Ferias no banco",
        Description = @"
                **Entrada:**
                - `Id`: Id das Ferias (maior que 0)

                 *Saida*
                - Ferias{Id da Ferias} excluida com sucesso`",
        OperationId = "ExcluirFeriasSwagger")]
        #endregion
        public async Task<ActionResult> ExcluirFerias(int id)
        {
            var feriasExcluida = await _feriasService.ExcluirFerias(id);

            return Ok(new ApiResponse<FeriasConsultarDTO>
            {
                StatusCode = 200,
                Mensagem = "Férias excluída com sucesso.",
                Data = feriasExcluida
            });
        }
    }
}
