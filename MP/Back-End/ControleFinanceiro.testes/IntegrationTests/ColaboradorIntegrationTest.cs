using ControleFerias.API.Controllers;
using ControleFerias.Application.Common;
using ControleFerias.Application.DTO;
using ControleFerias.Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ControleFerias.testes.UnitTests
{
    public class ColaboradorControllerUnitTest
    {
        private readonly Mock<IColaboradorService> _colaboradorServiceMock;


        public ColaboradorControllerUnitTest()
        {
            _colaboradorServiceMock = new Mock<IColaboradorService>();
        }

        private ColaboradorController CriarController()
        {
            return new ColaboradorController(_colaboradorServiceMock.Object, null); // passe null para serviços não usados
        }

        [Fact(DisplayName = "POST/IncluirColaborador_Valido_201")]
        public async Task IncluirColaborador_Valido_DeveRetornar201()
        {
            // cria o dto que vai ser enviado
            var dto = new ColaboradorIncluirDTO { sNome = "lucass", EquipeId = 2 };
            var colaboradorCriado = new ColaboradorConsultarDTO { Id = 1, sNome = dto.sNome, EquipeId = dto.EquipeId };

            //cria o mock do serviço
            _colaboradorServiceMock
                .Setup(s => s.IncluirColaborador(dto))
                .ReturnsAsync(colaboradorCriado);

            var controller = CriarController();

            // Act
            var actionResult = await controller.IncluirColaborador(dto);
            var result = actionResult.Result as ObjectResult;

            var apiResponse = result?.Value as ApiResponse<ColaboradorConsultarDTO>;

            // verificação do que foi recebido
            apiResponse.Should().NotBeNull();
            apiResponse!.StatusCode.Should().Be(201);
            apiResponse.Data!.sNome.Should().Be(dto.sNome);
            apiResponse.Data.EquipeId.Should().Be(dto.EquipeId);
            apiResponse.Mensagem.Should().Be($"Colaborador {dto.sNome} adicionado com sucesso.");
        }

        [Fact(DisplayName = "POST/IncluirColaborador_NomeNuloOuVazio_400")]
        public async Task IncluirColaborador_NomeNuloOuVazio_DeveRetornarBadRequest()
        {
            // Arrange
            var dto = new ColaboradorIncluirDTO { sNome = "", EquipeId = 2 };
            var controller = CriarController();
            controller.ModelState.AddModelError("sNome", "O nome do colaborador precisa ser preenchido.");

            // Act
            var actionResult = await controller.IncluirColaborador(dto);
            var result = actionResult.Result as ObjectResult;

            var apiResponse = result?.Value as ApiResponse<ColaboradorConsultarDTO>;

            // Assert
            apiResponse.Should().NotBeNull();
            apiResponse!.StatusCode.Should().Be(400);
            apiResponse.Erros.Should().ContainSingle(e => e.Campo == "sNome" &&
                                                          e.Mensagens.Contains("O nome do colaborador precisa ser preenchido."));
        }

        [Fact(DisplayName = "POST/IncluirColaborador_EquipeIdInvalido_400")]
        public async Task IncluirColaborador_EquipeIdInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var dto = new ColaboradorIncluirDTO { sNome = "lucas", EquipeId = 0 };
            var controller = CriarController();
            controller.ModelState.AddModelError("EquipeId", "O identificador da equipe deve ser maior que zero.");

            // Act
            var actionResult = await controller.IncluirColaborador(dto);
            var result = actionResult.Result as ObjectResult;

            var apiResponse = result?.Value as ApiResponse<ColaboradorConsultarDTO>;

            // Assert
            apiResponse.Should().NotBeNull();
            apiResponse!.StatusCode.Should().Be(400);
            apiResponse.Erros.Should().ContainSingle(e => e.Campo == "EquipeId" &&
                                                          e.Mensagens.Contains("O identificador da equipe deve ser maior que zero."));
        }

        [Fact(DisplayName = "POST/IncluirColaborador_NomeCurtoOuLongo_400")]
        public async Task IncluirColaborador_NomeCurtoOuLongo_DeveRetornarBadRequest()
        {
            // Arrange
            var dto = new ColaboradorIncluirDTO { sNome = "ab", EquipeId = 2 };
            var controller = CriarController();
            controller.ModelState.AddModelError("sNome", "O nome do colaborador deve ter entre 3 e 32 caracteres.");

            // Act
            var actionResult = await controller.IncluirColaborador(dto);
            var result = actionResult.Result as ObjectResult;

            var apiResponse = result?.Value as ApiResponse<ColaboradorConsultarDTO>;

            // Assert
            apiResponse.Should().NotBeNull();
            apiResponse!.StatusCode.Should().Be(400);
            apiResponse.Erros.Should().ContainSingle(e => e.Campo == "sNome" &&
                                                          e.Mensagens.Contains("O nome do colaborador deve ter entre 3 e 32 caracteres."));
        }

        [Fact(DisplayName = "POST/IncluirColaborador_NomeApenasEspacos_400")]
        public async Task IncluirColaborador_NomeApenasEspacos_DeveRetornarBadRequest()
        {
            // Arrange
            var dto = new ColaboradorIncluirDTO { sNome = "   ", EquipeId = 2 };
            var controller = CriarController();
            controller.ModelState.AddModelError("sNome", "O nome do colaborador precisa ser preenchido.");

            // Act
            var actionResult = await controller.IncluirColaborador(dto);
            var result = actionResult.Result as ObjectResult;

            var apiResponse = result?.Value as ApiResponse<ColaboradorConsultarDTO>;

            // Assert
            apiResponse.Should().NotBeNull();
            apiResponse!.StatusCode.Should().Be(400);
            apiResponse.Erros.Should().ContainSingle(e => e.Campo == "sNome" &&
                                                          e.Mensagens.Contains("O nome do colaborador precisa ser preenchido."));
        }
    }
}
