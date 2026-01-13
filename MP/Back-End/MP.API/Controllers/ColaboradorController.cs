using ControleFerias.Application.Common;
using ControleFerias.Application.DTO;
using ControleFerias.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ControleFerias.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ColaboradorController : ControllerBase
    {
        private readonly IColaboradorService _colaboradorService;
        private readonly IEquipeService _equipeService;


        public ColaboradorController(IColaboradorService colaboradorService, IEquipeService equipeService)
        {
            _colaboradorService = colaboradorService;
            _equipeService = equipeService;
        }

        [HttpGet, Route("ConsultarColaboradores")]
        #region Documentacao
        [SwaggerOperation(
        Summary = "Consulta todos os colaboradores válidos",
         Description = @"
          Entrada:
            - Parâmetro 'id' no caminho da URL
        

         *Saida*
        - Objetos de todos os Colaboradores(Por ordem de Id)",

        OperationId = "ConsultarColaboradoresSwagger")]
        #endregion
        public async Task<ActionResult<ApiResponse<IEnumerable<ColaboradorConsultarDTO>>>> ConsultarColaboradores()
        {
            var colaboradores = await _colaboradorService.ConsultarColaboradores();

            if (colaboradores == null || !colaboradores.Any())
            {
                return NotFound(ApiResponse<IEnumerable<ColaboradorConsultarDTO>>.Erro(
                    "Nenhum colaborador encontrado.",
                    Enumerable.Empty<ColaboradorConsultarDTO>(), 
                    404
                ));
            }

            return Ok(ApiResponse<IEnumerable<ColaboradorConsultarDTO>>.Sucesso(
                "Colaboradores consultados com sucesso.",
                colaboradores.ToList()
            ));
        }


        [HttpGet, Route("ConsultarColaboradorPorId")]
        #region Documentacao
        [SwaggerOperation(
        Summary = "Consulta um colaborador válidos",
         Description = @"
          Entrada: Id do Colaborador
        

         *Saida*
        - Objeto do Colaborador(Do Id enviado)",
            OperationId = "ConsultarColaboradorPorIdSwagger")]
        #endregion
        public async Task<ActionResult<ApiResponse<ColaboradorConsultarDTO>>> ConsultarColaboradorPorId(int id)
        {
            var colaborador = await _colaboradorService.ConsultarColaboradorPorId(id);

            if (colaborador == null)
            {
                return NotFound(new ApiResponse<ColaboradorConsultarDTO>
                {
                    StatusCode = 404,
                    Mensagem = $"Nenhum colaborador encontrado com o ID {id}.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<ColaboradorConsultarDTO>
            {
                StatusCode = 200,
                Mensagem = "Consulta realizada com sucesso.",
                Data = colaborador
            });
        }

        [HttpPost, Route("IncluirColaborador")]
        #region Documentacao
        [SwaggerOperation(
    Summary = "Adiciona um Colaborador se os valores forem válidos",
     Description = @"
          Entrada:
        - sNome: Nome do colaborador (mínimo 3 caracteres)
        - equipeId: ID da equipe existente

         *Saida*
        - Objeto Colaborador
        
        -Obs:Id da equipe(Verificar no endpoint ConsultarEquipe)",
    OperationId = "IncluirColaboradorSwagger")]
        #endregion
        public async Task<ActionResult<ApiResponse<ColaboradorConsultarDTO>>> IncluirColaborador([FromBody] ColaboradorIncluirDTO dto)
        {
            var colaboradorCriado = await _colaboradorService.IncluirColaborador(dto);

            if (colaboradorCriado == null)
            {
                return BadRequest(new ApiResponse<ColaboradorConsultarDTO>
                {
                    StatusCode = 400,
                    Mensagem = "Falha ao criar colaborador. Tente novamente.",
                    Data = null
                });
            }

             return CreatedAtAction(nameof(ConsultarColaboradorPorId), new { id = colaboradorCriado.Id },
                new ApiResponse<ColaboradorConsultarDTO>
                {
                    StatusCode = 201,
                    Mensagem = $"Colaborador {colaboradorCriado.sNome} adicionado com sucesso.",
                    Data = colaboradorCriado
                });
        }

        [HttpPost, Route("AlterarColaborador")]
        #region Documentacao
        [SwaggerOperation(
        Summary = "Altera um Colaborador se os valores forem válidos",
         Description = @"
              Entrada:
            - Id: Id do colaborador (maior que 0 e existente no banco de dados)

            - sNome: Nome do colaborador (mínimo 3 caracteres)
            - equipeId: ID da equipe existente

             *Saida*
            -  colaborador {sNome} Alterado com sucesso
            -Obs:Id da equipe(Verificar no endpoint ConsultarColaboradores)",
        OperationId = "AlterarColaboradorSwagger")]
        #endregion
        public async Task<ActionResult<ApiResponse<ColaboradorConsultarDTO>>> AlterarColaborador(int id, [FromBody] ColaboradorAlterarDTO ColaboradorDTO)
        {
            var antigo = await _colaboradorService.ConsultarColaboradorPorId(id);

            if (antigo == null)
            {
                return NotFound(new ApiResponse<ColaboradorConsultarDTO>
                {
                    StatusCode = 404,
                    Mensagem = $"Colaborador com ID {id} não encontrado.",
                    Data = null
                });
            }

            var nomeAntigo = antigo.sNome;
            var equipeIdAntigo = antigo.EquipeId;

            var alterado = await _colaboradorService.AlterarColaborador(id, ColaboradorDTO);

            var equipeAntiga = (await _equipeService.ConsultarEquipePorId(equipeIdAntigo))?.sNome ?? "Indefinida";
            var equipeNova = (await _equipeService.ConsultarEquipePorId(alterado.EquipeId))?.sNome ?? "Indefinida";

            // Gera mensagem precisa
            var mensagem = MontarMensagemAlteracao(
                nomeAntigo, equipeIdAntigo,
                alterado.sNome, alterado.EquipeId,
                equipeAntiga, equipeNova);

            return Ok(new ApiResponse<ColaboradorConsultarDTO>
            {
                StatusCode = 200,
                Mensagem = mensagem,
                Data = alterado
            });
        }
        [HttpPost, Route("ExcluirColaborador")]
        #region Documentacao
        [SwaggerOperation(
        Summary = "Exclui um Colaborador se o Id for compatível com algum Colaborador no banco",
        Description = @"
        **Entrada:**
        - `Id`: Id do colaborador (maior que 0)

         *Saida*
        - Colaborador{Nome do Colaborador} excluido com sucesso`",
        OperationId = "ExcluirolaboradorSwagger")]
        #endregion
        public async Task<ActionResult<ApiResponse<ColaboradorConsultarDTO>>> ExcluirColaborador(int id)
        {
            var colaborador = await _colaboradorService.ExcluirColaborador(id);

            if (colaborador == null)
            {
                return NotFound(new ApiResponse<ColaboradorConsultarDTO>
                {
                    StatusCode = 404,
                    Mensagem = $"Nenhum colaborador encontrado com o ID {id}.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<ColaboradorConsultarDTO>
            {
                StatusCode = 200,
                Mensagem = $"Colaborador {colaborador.sNome} excluído com sucesso.",
                Data = colaborador
            });
        }

        private string MontarMensagemAlteracao(
            string nomeAntigo, int equipeIdAntigo,
            string nomeNovo, int equipeIdNovo,
            string equipeAntigaNome, string equipeNovaNome)
        {
            bool nomeAlterado = nomeAntigo != nomeNovo;
            bool equipeAlterada = equipeIdAntigo != equipeIdNovo;

            if (nomeAlterado && equipeAlterada)
                return $"Nome e equipe do colaborador foram alterados com sucesso.  Nome: {nomeAntigo} -> {nomeNovo} Equipe: {equipeAntigaNome} -> {equipeNovaNome}";

            if (nomeAlterado)
                return $"Nome do colaborador foi alterado de {nomeAntigo} para {nomeNovo} com sucesso.";

            if (equipeAlterada)
                return $"Equipe do colaborador {nomeNovo} foi alterada de {equipeAntigaNome} para {equipeNovaNome} com sucesso.";

            return "Nenhuma alteração foi realizada.";
        }

    }


}





