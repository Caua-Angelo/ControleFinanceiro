using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ControleFinanceiro.Application.DTO.CompraParcelada;
using ControleFinanceiro.Domain.Models;
using ControleFinanceiro.Infraestructure.Data;
using ControleFinanceiro.Test.Integration.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ControleFinanceiro.Test.Integration.Controllers;

public class CompraParceladaControllerTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public CompraParceladaControllerTests(
        CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> CriarUsuarioEObterTokenAsync(
        string email)
    {
        using var scope = _factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<ApplicationDBContext>();

        db.Usuario.Add(new Usuario(
            "Usuario Teste",
            new DateOnly(2000, 1, 1),
            email,
            BCrypt.Net.BCrypt.HashPassword("senha123")
        ));

        await db.SaveChangesAsync();

        return await AuthHelper.ObterTokenAsync(
            _client,
            email,
            "senha123"
        );
    }

    private async Task<int> CriarCartaoAsync(string email)
    {
        using var scope = _factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<ApplicationDBContext>();

        var usuario = await db.Usuario
            .SingleAsync(u => u.Email == email);

        var cartao = new Cartao(
            "Cartao Teste",
            "visa",
            15,
            10,
            usuario
        );

        db.Cartao.Add(cartao);

        await db.SaveChangesAsync();

        return cartao.Id;
    }

    private async Task<(string Token, int CartaoId)> PrepararUsuarioECartaoAsync(
        string email)
    {
        var token = await CriarUsuarioEObterTokenAsync(email);
        var cartaoId = await CriarCartaoAsync(email);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return (token, cartaoId);
    }

    private static CompraParceladaCriarDTO CriarDTO(
        int cartaoId,
        decimal valorTotal = 1200m,
        int quantidadeParcelas = 6)
    {
        return new CompraParceladaCriarDTO
        {
            Descricao = "Monitor novo",
            ValorTotal = valorTotal,
            QuantidadeParcelas = quantidadeParcelas,
            DataCompra = new DateOnly(2026, 10, 11),
            CartaoId = cartaoId
        };
    }

    [Fact]
    public async Task Get_SemToken_DeveRetornar401()
    {
        // Act
        var response = await _client.GetAsync(
            "/api/CompraParcelada");

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_CriarCompraParcelada_DeveRetornar201()
    {
        // Arrange
        var (_, cartaoId) =
            await PrepararUsuarioECartaoAsync(
                "compra_criar@teste.com");

        var dto = CriarDTO(cartaoId);

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/CompraParcelada",
            dto);

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.Created);

        var body = await response.Content
            .ReadFromJsonAsync<CompraParceladaResponseDTO>();

        body.Should().NotBeNull();
        body!.Id.Should().BeGreaterThan(0);
        body.Descricao.Should().Be("Monitor novo");
        body.ValorTotal.Should().Be(1200m);
        body.QuantidadeParcelas.Should().Be(6);
        body.CartaoId.Should().Be(cartaoId);
        body.Parcelas.Should().HaveCount(6);
    }

    [Fact]
    public async Task Post_CriarCompraParcelada_DeveGerarParcelasComValoresCorretos()
    {
        // Arrange
        var (_, cartaoId) =
            await PrepararUsuarioECartaoAsync(
                "compra_valores@teste.com");

        var dto = CriarDTO(
            cartaoId,
            valorTotal: 100m,
            quantidadeParcelas: 3);

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/CompraParcelada",
            dto);

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.Created);

        var body = await response.Content
            .ReadFromJsonAsync<CompraParceladaResponseDTO>();

        body.Should().NotBeNull();
        body!.Parcelas.Should().HaveCount(3);

        body.Parcelas.Select(p => p.Numero)
            .Should().Equal(1, 2, 3);

        body.Parcelas.Select(p => p.Valor)
            .Should().Equal(33.34m, 33.33m, 33.33m);

        body.Parcelas.Sum(p => p.Valor)
            .Should().Be(100m);

        body.Parcelas.Should()
            .OnlyContain(p => !p.Paga);
    }

    [Fact]
    public async Task Post_CriarCompraParcelada_DeveGerarVencimentosMensais()
    {
        // Arrange
        var (_, cartaoId) =
            await PrepararUsuarioECartaoAsync(
                "compra_vencimentos@teste.com");

        var dto = CriarDTO(cartaoId);

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/CompraParcelada",
            dto);

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.Created);

        var body = await response.Content
            .ReadFromJsonAsync<CompraParceladaResponseDTO>();

        body.Should().NotBeNull();

        body!.Parcelas
            .OrderBy(p => p.Numero)
            .Select(p => p.DataVencimento)
            .Should()
            .Equal(
                new DateOnly(2026, 11, 10),
                new DateOnly(2026, 12, 10),
                new DateOnly(2027, 1, 10),
                new DateOnly(2027, 2, 10),
                new DateOnly(2027, 3, 10),
                new DateOnly(2027, 4, 10)
            );
    }

    [Fact]
    public async Task Post_QuantidadeParcelasInvalida_DeveRetornar400()
    {
        // Arrange
        var (_, cartaoId) =
            await PrepararUsuarioECartaoAsync(
                "compra_parcelas_invalidas@teste.com");

        var dto = CriarDTO(
            cartaoId,
            quantidadeParcelas: 1);

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/CompraParcelada",
            dto);

        // Assert
        response.StatusCode.Should()
     .Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Post_ValorTotalInvalido_DeveRetornar400()
    {
        // Arrange
        var (_, cartaoId) =
            await PrepararUsuarioECartaoAsync(
                "compra_valor_invalido@teste.com");

        var dto = CriarDTO(
            cartaoId,
            valorTotal: 0);

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/CompraParcelada",
            dto);

        // Assert
        response.StatusCode.Should()
    .Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Post_CartaoDeOutroUsuario_DeveRetornarErro()
    {
        // Arrange
        var (_, cartaoIdUsuario1) =
            await PrepararUsuarioECartaoAsync(
                "compra_cartao_usuario1@teste.com");

        await CriarUsuarioEObterTokenAsync(
            "compra_cartao_usuario2@teste.com");

        var tokenUsuario2 = await AuthHelper.ObterTokenAsync(
            _client,
            "compra_cartao_usuario2@teste.com",
            "senha123");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                tokenUsuario2);

        var dto = CriarDTO(cartaoIdUsuario1);

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/CompraParcelada",
            dto);

        // Assert
        response.StatusCode.Should()
      .Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Get_ListarComprasParceladas_DeveRetornar200()
    {
        // Arrange
        await PrepararUsuarioECartaoAsync(
            "compra_listar@teste.com");

        // Act
        var response = await _client.GetAsync(
            "/api/CompraParcelada");

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        var body = await response.Content
            .ReadFromJsonAsync<
                IEnumerable<CompraParceladaResponseDTO>>();

        body.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_BuscarCompraExistente_DeveRetornar200ComParcelas()
    {
        // Arrange
        var (_, cartaoId) =
            await PrepararUsuarioECartaoAsync(
                "compra_buscar@teste.com");

        var criar = await _client.PostAsJsonAsync(
            "/api/CompraParcelada",
            CriarDTO(cartaoId));

        criar.StatusCode.Should().Be(HttpStatusCode.Created);

        var compraCriada = await criar.Content
            .ReadFromJsonAsync<CompraParceladaResponseDTO>();

        compraCriada.Should().NotBeNull();

        // Act
        var response = await _client.GetAsync(
            $"/api/CompraParcelada/{compraCriada!.Id}");

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        var body = await response.Content
            .ReadFromJsonAsync<CompraParceladaResponseDTO>();

        body.Should().NotBeNull();
        body!.Id.Should().Be(compraCriada.Id);
        body.Parcelas.Should().HaveCount(6);
    }

    [Fact]
    public async Task Get_BuscarCompraInexistente_DeveRetornar404()
    {
        // Arrange
        await PrepararUsuarioECartaoAsync(
            "compra_buscar_404@teste.com");

        // Act
        var response = await _client.GetAsync(
            "/api/CompraParcelada/999999");

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_BuscarCompraDeOutroUsuario_DeveRetornar403()
    {
        // Arrange
        var (_, cartaoId) =
            await PrepararUsuarioECartaoAsync(
                "compra_proprietario@teste.com");

        var criar = await _client.PostAsJsonAsync(
            "/api/CompraParcelada",
            CriarDTO(cartaoId));

        criar.StatusCode.Should().Be(HttpStatusCode.Created);

        var compraCriada = await criar.Content
            .ReadFromJsonAsync<CompraParceladaResponseDTO>();

        compraCriada.Should().NotBeNull();

        var tokenOutroUsuario =
            await CriarUsuarioEObterTokenAsync(
                "compra_outro_usuario@teste.com");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                tokenOutroUsuario);

        // Act
        var response = await _client.GetAsync(
            $"/api/CompraParcelada/{compraCriada!.Id}");

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_CompraExistente_DeveRetornar204()
    {
        // Arrange
        var (_, cartaoId) =
            await PrepararUsuarioECartaoAsync(
                "compra_deletar@teste.com");

        var criar = await _client.PostAsJsonAsync(
            "/api/CompraParcelada",
            CriarDTO(cartaoId));

        criar.StatusCode.Should().Be(HttpStatusCode.Created);

        var compraCriada = await criar.Content
            .ReadFromJsonAsync<CompraParceladaResponseDTO>();

        compraCriada.Should().NotBeNull();

        // Act
        var response = await _client.DeleteAsync(
            $"/api/CompraParcelada/{compraCriada!.Id}");

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_CompraInexistente_DeveRetornar404()
    {
        // Arrange
        await PrepararUsuarioECartaoAsync(
            "compra_deletar_404@teste.com");

        // Act
        var response = await _client.DeleteAsync(
            "/api/CompraParcelada/999999");

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);
    }
}
