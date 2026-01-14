using ControleFinanceiro.Application.Common;
using ControleFinanceiro.Application.DTO;
using ControleFinanceiro.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ControleFinanceiro.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ColaboradorController : ControllerBase
    {
        private readonly IUsuarioService _colaboradorService;
        private readonly ICategoriaService _equipeService;


        public ColaboradorController(IUsuarioService colaboradorService, ICategoriaService equipeService)
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
        public async Task<ActionResult<ApiResponse<IEnumerable<UsuarioConsultarDTO>>>> ConsultarColaboradores()
        {
            var colaboradores = await _colaboradorService.ConsultarColaboradores();

            if (colaboradores == null || !colaboradores.Any())
            {
                return NotFound(ApiResponse<IEnumerable<UsuarioConsultarDTO>>.Erro(
                    "Nenhum Usuario encontrado.",
                    Enumerable.Empty<UsuarioConsultarDTO>(), 
                    404
                ));
            }

            return Ok(ApiResponse<IEnumerable<UsuarioConsultarDTO>>.Sucesso(
                "Colaboradores consultados com sucesso.",
                colaboradores.ToList()
            ));
        }


        [HttpGet, Route("ConsultarColaboradorPorId")]
        #region Documentacao
        [SwaggerOperation(
        Summary = "Consulta um Usuario válidos",
         Description = @"
          Entrada: Id do Usuario
        

         *Saida*
        - Objeto do Usuario(Do Id enviado)",
            OperationId = "ConsultarColaboradorPorIdSwagger")]
        #endregion
        public async Task<ActionResult<ApiResponse<UsuarioConsultarDTO>>> ConsultarColaboradorPorId(int id)
        {
            var Usuario = await _colaboradorService.ConsultarColaboradorPorId(id);

            if (Usuario == null)
            {
                return NotFound(new ApiResponse<UsuarioConsultarDTO>
                {
                    StatusCode = 404,
                    Mensagem = $"Nenhum Usuario encontrado com o ID {id}.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<UsuarioConsultarDTO>
            {
                StatusCode = 200,
                Mensagem = "Consulta realizada com sucesso.",
                Data = Usuario
            });
        }

        [HttpPost, Route("IncluirColaborador")]
        #region Documentacao
        [SwaggerOperation(
    Summary = "Adiciona um Usuario se os valores forem válidos",
     Description = @"
          Entrada:
        - sNome: Nome do Usuario (mínimo 3 caracteres)
        - equipeId: ID da equipe existente

         *Saida*
        - Objeto Usuario
        
        -Obs:Id da equipe(Verificar no endpoint ConsultarEquipe)",
    OperationId = "IncluirColaboradorSwagger")]
        #endregion
        public async Task<ActionResult<ApiResponse<UsuarioConsultarDTO>>> IncluirColaborador([FromBody] UsuarioIncluirDTO dto)
        {
            var colaboradorCriado = await _colaboradorService.IncluirColaborador(dto);

            if (colaboradorCriado == null)
            {
                return BadRequest(new ApiResponse<UsuarioConsultarDTO>
                {
                    StatusCode = 400,
                    Mensagem = "Falha ao criar Usuario. Tente novamente.",
                    Data = null
                });
            }

             return CreatedAtAction(nameof(ConsultarColaboradorPorId), new { id = colaboradorCriado.Id },
                new ApiResponse<UsuarioConsultarDTO>
                {
                    StatusCode = 201,
                    Mensagem = $"Usuario {colaboradorCriado.sNome} adicionado com sucesso.",
                    Data = colaboradorCriado
                });
        }

        [HttpPost, Route("AlterarColaborador")]
        #region Documentacao
        [SwaggerOperation(
        Summary = "Altera um Usuario se os valores forem válidos",
         Description = @"
              Entrada:
            - Id: Id do Usuario (maior que 0 e existente no banco de dados)

            - sNome: Nome do Usuario (mínimo 3 caracteres)
            - equipeId: ID da equipe existente

             *Saida*
            -  Usuario {sNome} Alterado com sucesso
            -Obs:Id da equipe(Verificar no endpoint ConsultarColaboradores)",
        OperationId = "AlterarColaboradorSwagger")]
        #endregion
        public async Task<ActionResult<ApiResponse<UsuarioConsultarDTO>>> AlterarColaborador(int id, [FromBody] UsuarioAlterarDTO ColaboradorDTO)
        {
            var antigo = await _colaboradorService.ConsultarColaboradorPorId(id);

            if (antigo == null)
            {
                return NotFound(new ApiResponse<UsuarioConsultarDTO>
                {
                    StatusCode = 404,
                    Mensagem = $"Usuario com ID {id} não encontrado.",
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

            return Ok(new ApiResponse<UsuarioConsultarDTO>
            {
                StatusCode = 200,
                Mensagem = mensagem,
                Data = alterado
            });
        }
        [HttpPost, Route("ExcluirColaborador")]
        #region Documentacao
        [SwaggerOperation(
        Summary = "Exclui um Usuario se o Id for compatível com algum Usuario no banco",
        Description = @"
        **Entrada:**
        - `Id`: Id do Usuario (maior que 0)

         *Saida*
        - Usuario{Nome do Usuario} excluido com sucesso`",
        OperationId = "ExcluirolaboradorSwagger")]
        #endregion
        public async Task<ActionResult<ApiResponse<UsuarioConsultarDTO>>> ExcluirColaborador(int id)
        {
            var Usuario = await _colaboradorService.ExcluirColaborador(id);

            if (Usuario == null)
            {
                return NotFound(new ApiResponse<UsuarioConsultarDTO>
                {
                    StatusCode = 404,
                    Mensagem = $"Nenhum Usuario encontrado com o ID {id}.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<UsuarioConsultarDTO>
            {
                StatusCode = 200,
                Mensagem = $"Usuario {Usuario.sNome} excluído com sucesso.",
                Data = Usuario
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
                return $"Nome e equipe do Usuario foram alterados com sucesso.  Nome: {nomeAntigo} -> {nomeNovo} Equipe: {equipeAntigaNome} -> {equipeNovaNome}";

            if (nomeAlterado)
                return $"Nome do Usuario foi alterado de {nomeAntigo} para {nomeNovo} com sucesso.";

            if (equipeAlterada)
                return $"Equipe do Usuario {nomeNovo} foi alterada de {equipeAntigaNome} para {equipeNovaNome} com sucesso.";

            return "Nenhuma alteração foi realizada.";
        }

    }


}





