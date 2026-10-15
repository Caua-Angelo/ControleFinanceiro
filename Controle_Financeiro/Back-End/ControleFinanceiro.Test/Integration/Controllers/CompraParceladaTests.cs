
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ControleFinanceiro.Application.DTO.CompraParcelada;
using ControleFinanceiro.Domain.Models;
using ControleFinanceiro.Domain.Enums;
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

    private async Task<string> CriarUsuarioEObterTokenAsync(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

        var usuarioExistente = await db.Usuario
            .FirstOrDefaultAsync(u => u.Email == email);

        if (usuarioExistente is null)
        {
            db.Usuario.Add(new Usuario(
                "Usuario Teste",
                new DateOnly(2000, 1, 1),
                email,
                BCrypt.Net.BCrypt.HashPassword("senha123")));

            await db.SaveChangesAsync();
        }

        return await AuthHelper.ObterTokenAsync(
            _client,
            email,
            "senha123");
    }

    private async Task<int> CriarCartaoAsync(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

        var usuario = await db.Usuario.SingleAsync(u => u.Email == email);

        var cartao = await db.Cartao.FirstOrDefaultAsync(
            c => c.UsuarioId == usuario.Id &&
                 c.Nome == "Cartao Teste");

        if (cartao is not null)
            return cartao.Id;

        cartao = new Cartao(
            "Cartao Teste",
            "visa",
            15,
            10,
            usuario);

        db.Cartao.Add(cartao);
        await db.SaveChangesAsync();

        return cartao.Id;
    }

    private async Task<(string Token, int CartaoId, int CategoriaId)>
        PrepararUsuarioCartaoECategoriaAsync(string email)
    {
        var token = await CriarUsuarioEObterTokenAsync(email);
        var cartaoId = await CriarCartaoAsync(email);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
        var usuario = await db.Usuario.SingleAsync(u => u.Email == email);

        // Reutiliza uma categoria do usuário, caso já exista.
        var categoria = await db.Categoria.FirstOrDefaultAsync(c =>
            c.UsuarioId == usuario.Id);

        if (categoria is null)
        {
            categoria = new Categoria(
                "Categoria Teste",
                FinalidadeCategoria.Despesa)
            {
                UsuarioId = usuario.Id
            };

            db.Categoria.Add(categoria);
            await db.SaveChangesAsync();
        }

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return (token, cartaoId, categoria.Id);
    }

    private static CompraParceladaCriarDTO CriarDTO(
        int cartaoId,
        int categoriaId,
        decimal valorTotal = 1200m,
        int quantidadeParcelas = 6)
    {
        return new CompraParceladaCriarDTO
        {
            Descricao = "Monitor novo",
            ValorTotal = valorTotal,
            QuantidadeParcelas = quantidadeParcelas,
            DataCompra = new DateOnly(2026, 10, 11),
            CartaoId = cartaoId,
            CategoriaId = categoriaId
        };
    }

    [Fact]
    public async Task Get_SemToken_DeveRetornar401()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync("/api/CompraParcelada");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_CriarCompraParcelada_DeveRetornar201()
    {
        var (_, cartaoId, categoriaId) =
            await PrepararUsuarioCartaoECategoriaAsync("compra_criar@teste.com");

        var response = await _client.PostAsJsonAsync(
            "/api/CompraParcelada",
            CriarDTO(cartaoId, categoriaId));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<CompraParceladaResponseDTO>();

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
        var (_, cartaoId, categoriaId) =
            await PrepararUsuarioCartaoECategoriaAsync("compra_valores@teste.com");

        var response = await _client.PostAsJsonAsync(
            "/api/CompraParcelada",
            CriarDTO(cartaoId, categoriaId, 100m, 3));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<CompraParceladaResponseDTO>();

        body.Should().NotBeNull();
        body!.Parcelas.Should().HaveCount(3);
        body.Parcelas.Select(p => p.Numero).Should().Equal(1, 2, 3);
        body.Parcelas.Select(p => p.Valor).Should().Equal(33.34m, 33.33m, 33.33m);
        body.Parcelas.Sum(p => p.Valor).Should().Be(100m);
        body.Parcelas.Should().OnlyContain(p => !p.Paga);
    }

    [Fact]
    public async Task Post_CriarCompraParcelada_DeveGerarVencimentosMensais()
    {
        var (_, cartaoId, categoriaId) =
            await PrepararUsuarioCartaoECategoriaAsync("compra_vencimentos@teste.com");

        var response = await _client.PostAsJsonAsync(
            "/api/CompraParcelada",
            CriarDTO(cartaoId, categoriaId));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<CompraParceladaResponseDTO>();

        body.Should().NotBeNull();
        body!.Parcelas.OrderBy(p => p.Numero)
            .Select(p => p.DataVencimento)
            .Should().Equal(
                new DateOnly(2026, 11, 10),
                new DateOnly(2026, 12, 10),
                new DateOnly(2027, 1, 10),
                new DateOnly(2027, 2, 10),
                new DateOnly(2027, 3, 10),
                new DateOnly(2027, 4, 10));
    }

    [Fact]
    public async Task Post_QuantidadeParcelasInvalida_DeveRetornar400()
    {
        var (_, cartaoId, categoriaId) =
            await PrepararUsuarioCartaoECategoriaAsync("compra_parcelas_invalidas@teste.com");

        var response = await _client.PostAsJsonAsync(
            "/api/CompraParcelada",
            CriarDTO(cartaoId, categoriaId, quantidadeParcelas: 1));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_ValorTotalInvalido_DeveRetornar400()
    {
        var (_, cartaoId, categoriaId) =
            await PrepararUsuarioCartaoECategoriaAsync("compra_valor_invalido@teste.com");

        var response = await _client.PostAsJsonAsync(
            "/api/CompraParcelada",
            CriarDTO(cartaoId, categoriaId, valorTotal: 0));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_CartaoDeOutroUsuario_DeveRetornarErro()
    {
        var (_, cartaoIdUsuario1, categoriaIdUsuario1) =
            await PrepararUsuarioCartaoECategoriaAsync("compra_cartao_usuario1@teste.com");

        var tokenUsuario2 = await CriarUsuarioEObterTokenAsync(
            "compra_cartao_usuario2@teste.com");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenUsuario2);

        var response = await _client.PostAsJsonAsync(
            "/api/CompraParcelada",
            CriarDTO(cartaoIdUsuario1, categoriaIdUsuario1));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Get_ListarComprasParceladas_DeveRetornar200()
    {
        await PrepararUsuarioCartaoECategoriaAsync("compra_listar@teste.com");

        var response = await _client.GetAsync("/api/CompraParcelada");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content
            .ReadFromJsonAsync<IEnumerable<CompraParceladaResponseDTO>>();

        body.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_BuscarCompraExistente_DeveRetornar200ComParcelas()
    {
        var (_, cartaoId, categoriaId) =
            await PrepararUsuarioCartaoECategoriaAsync("compra_buscar@teste.com");

        var criar = await _client.PostAsJsonAsync(
            "/api/CompraParcelada",
            CriarDTO(cartaoId, categoriaId));

        criar.StatusCode.Should().Be(HttpStatusCode.Created);

        var compraCriada = await criar.Content
            .ReadFromJsonAsync<CompraParceladaResponseDTO>();

        compraCriada.Should().NotBeNull();

        var response = await _client.GetAsync(
            $"/api/CompraParcelada/{compraCriada!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content
            .ReadFromJsonAsync<CompraParceladaResponseDTO>();

        body.Should().NotBeNull();
        body!.Id.Should().Be(compraCriada.Id);
        body.Parcelas.Should().HaveCount(6);
    }

    [Fact]
    public async Task Get_BuscarCompraInexistente_DeveRetornar404()
    {
        await PrepararUsuarioCartaoECategoriaAsync("compra_buscar_404@teste.com");

        var response = await _client.GetAsync("/api/CompraParcelada/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_BuscarCompraDeOutroUsuario_DeveRetornar403()
    {
        var (_, cartaoId, categoriaId) =
            await PrepararUsuarioCartaoECategoriaAsync("compra_proprietario@teste.com");

        var criar = await _client.PostAsJsonAsync(
            "/api/CompraParcelada",
            CriarDTO(cartaoId, categoriaId));

        criar.StatusCode.Should().Be(HttpStatusCode.Created);

        var compraCriada = await criar.Content
            .ReadFromJsonAsync<CompraParceladaResponseDTO>();

        compraCriada.Should().NotBeNull();

        var tokenOutroUsuario =
            await CriarUsuarioEObterTokenAsync("compra_outro_usuario@teste.com");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenOutroUsuario);

        var response = await _client.GetAsync(
            $"/api/CompraParcelada/{compraCriada!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_CompraExistente_DeveRetornar204()
    {
        var (_, cartaoId, categoriaId) =
            await PrepararUsuarioCartaoECategoriaAsync("compra_deletar@teste.com");

        var criar = await _client.PostAsJsonAsync(
            "/api/CompraParcelada",
            CriarDTO(cartaoId, categoriaId));

        criar.StatusCode.Should().Be(HttpStatusCode.Created);

        var compraCriada = await criar.Content
            .ReadFromJsonAsync<CompraParceladaResponseDTO>();

        compraCriada.Should().NotBeNull();

        var response = await _client.DeleteAsync(
            $"/api/CompraParcelada/{compraCriada!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_CompraInexistente_DeveRetornar404()
    {
        await PrepararUsuarioCartaoECategoriaAsync("compra_deletar_404@teste.com");

        var response = await _client.DeleteAsync("/api/CompraParcelada/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
