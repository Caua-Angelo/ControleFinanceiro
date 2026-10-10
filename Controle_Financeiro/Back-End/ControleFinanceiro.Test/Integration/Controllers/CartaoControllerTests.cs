
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ControleFinanceiro.Application.DTO.Cartao;
using ControleFinanceiro.Domain.Models;
using ControleFinanceiro.Infraestructure.Data;
using ControleFinanceiro.Test.Integration.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace ControleFinanceiro.Test.Integration.Controllers;

public class CartaoControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public CartaoControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task AutorizarClienteAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

        var email = $"cartao_{Guid.NewGuid():N}@teste.com";
        const string senha = "senha123";

        db.Usuario.Add(new Usuario(
            "Usuario Teste",
            new DateOnly(2000, 1, 1),
            email,
            BCrypt.Net.BCrypt.HashPassword(senha)
        ));

        await db.SaveChangesAsync();

        var token = await AuthHelper.ObterTokenAsync(_client, email, senha);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    private static CartaoIncluirDTO CriarCartaoDTO() => new()
    {
        Nome = "Nubank",
        Imagem = "cartao-roxo",
        DiaFechamento = 10,
        DiaVencimento = 17
    };

    [Fact]
    public async Task Get_SemToken_DeveRetornar401()
    {
        var response = await _client.GetAsync("/api/Cartao");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_CriarCartao_DeveRetornar201()
    {
        await AutorizarClienteAsync();

        var response = await _client.PostAsJsonAsync(
            "/api/Cartao",
            CriarCartaoDTO());

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content
            .ReadFromJsonAsync<CartaoConsultarDTO>();

        body.Should().NotBeNull();
        body!.Nome.Should().Be("Nubank");
        body.Imagem.Should().Be("cartao-roxo");
        body.DiaFechamento.Should().Be(10);
        body.DiaVencimento.Should().Be(17);
    }

    [Fact]
    public async Task Get_ListarCartoes_DeveRetornar200()
    {
        await AutorizarClienteAsync();

        var response = await _client.GetAsync("/api/Cartao");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content
            .ReadFromJsonAsync<IEnumerable<CartaoConsultarDTO>>();

        body.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_CartaoExistente_DeveRetornar200()
    {
        await AutorizarClienteAsync();

        var criar = await _client.PostAsJsonAsync(
            "/api/Cartao",
            CriarCartaoDTO());

        var criado = await criar.Content
            .ReadFromJsonAsync<CartaoConsultarDTO>();

        var response = await _client.GetAsync(
            $"/api/Cartao/{criado!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content
            .ReadFromJsonAsync<CartaoConsultarDTO>();

        body.Should().NotBeNull();
        body!.Id.Should().Be(criado.Id);
    }

    [Fact]
    public async Task Put_AtualizarCartao_DeveRetornar200()
    {
        await AutorizarClienteAsync();

        var criar = await _client.PostAsJsonAsync(
            "/api/Cartao",
            CriarCartaoDTO());

        var criado = await criar.Content
            .ReadFromJsonAsync<CartaoConsultarDTO>();

        var dto = new CartaoAlterarDTO
        {
            Nome = "Inter",
            Imagem = "cartao-laranja",
            DiaFechamento = 12,
            DiaVencimento = 19
        };

        var response = await _client.PutAsJsonAsync(
            $"/api/Cartao/{criado!.Id}",
            dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content
            .ReadFromJsonAsync<CartaoConsultarDTO>();

        body.Should().NotBeNull();
        body!.Nome.Should().Be("Inter");
        body.Imagem.Should().Be("cartao-laranja");
        body.DiaFechamento.Should().Be(12);
        body.DiaVencimento.Should().Be(19);
    }

    [Fact]
    public async Task Delete_CartaoExistente_DeveRetornar204()
    {
        await AutorizarClienteAsync();

        var criar = await _client.PostAsJsonAsync(
            "/api/Cartao",
            CriarCartaoDTO());

        var criado = await criar.Content
            .ReadFromJsonAsync<CartaoConsultarDTO>();

        var response = await _client.DeleteAsync(
            $"/api/Cartao/{criado!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetById_CartaoInexistente_DeveRetornar404()
    {
        await AutorizarClienteAsync();

        var response = await _client.GetAsync("/api/Cartao/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_CartaoInexistente_DeveRetornar404()
    {
        await AutorizarClienteAsync();

        var dto = new CartaoAlterarDTO
        {
            Nome = "Inter",
            Imagem = "cartao-laranja",
            DiaFechamento = 12,
            DiaVencimento = 19
        };

        var response = await _client.PutAsJsonAsync(
            "/api/Cartao/99999",
            dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_CartaoInexistente_DeveRetornar404()
    {
        await AutorizarClienteAsync();

        var response = await _client.DeleteAsync("/api/Cartao/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_CriarCartao_ComNomeInvalido_DeveRetornar400()
    {
        await AutorizarClienteAsync();

        var dto = new CartaoIncluirDTO
        {
            Nome = "",
            Imagem = "cartao-roxo",
            DiaFechamento = 10,
            DiaVencimento = 17
        };

        var response = await _client.PostAsJsonAsync(
            "/api/Cartao",
            dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
