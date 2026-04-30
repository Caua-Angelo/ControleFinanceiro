using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ControleFinanceiro.Application.DTO.Transacao;
using ControleFinanceiro.Domain.Enums;
using ControleFinanceiro.Domain.Models;
using ControleFinanceiro.Infraestructure.Data;
using ControleFinanceiro.Test.Integration.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace ControleFinanceiro.Test.Integration.Controllers;

public class TransacaoControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public TransacaoControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
       
    }

    private async Task<string> CriarUsuarioEObterTokenAsync(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

        db.Usuario.Add(new Usuario("Cauã", 22, email, BCrypt.Net.BCrypt.HashPassword("senha123")));
        await db.SaveChangesAsync();

        return await AuthHelper.ObterTokenAsync(_client, email, "senha123");
    }

    private async Task<Categoria> CriarCategoriaAsync(string descricao, FinalidadeCategoria finalidade)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

        var categoria = new Categoria(descricao, finalidade);
        db.Categoria.Add(categoria);
        await db.SaveChangesAsync();

        return categoria;
    }

    [Fact]
    public async Task Get_SemToken_DeveRetornar401()
    {
        var response = await _client.GetAsync("/api/transacoes");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_CriarTransacao_DeveRetornar201()
    {
        // Arrange
        var token = await CriarUsuarioEObterTokenAsync("transacao_criar@teste.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var categoria = await CriarCategoriaAsync("Salario", FinalidadeCategoria.Receita);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

        var dto = new TransacaoCriarDTO
        {
            Descricao = "Salario",
            Valor = 3000,
            Tipo = TipoTransacao.Receita,
            CategoriaId = categoria.Id,
            Data = DateTime.Now
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/transacoes", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<TransacaoConsultarDTO>();
        body.Should().NotBeNull();
        body!.Descricao.Should().Be("Salario");
        body.Valor.Should().Be(3000);
    }

    [Fact]
    public async Task Get_ListarTransacoes_DeveRetornar200()
    {
        // Arrange
        var token = await CriarUsuarioEObterTokenAsync("transacao_listar@teste.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/transacoes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<IEnumerable<TransacaoConsultarDTO>>();
        body.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_TransacaoExistente_DeveRetornar204()
    {
        // Arrange
        var token = await CriarUsuarioEObterTokenAsync("transacao_deletar@teste.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var categoria = await CriarCategoriaAsync("Alimentacao", FinalidadeCategoria.Despesa);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

        var dto = new TransacaoCriarDTO
        {
            Descricao = "Almoco",
            Valor = 50,
            Tipo = TipoTransacao.Despesa,
            CategoriaId = categoria.Id,
            Data = DateTime.Now
        };

        var criar = await _client.PostAsJsonAsync("/api/transacoes", dto);
        var transacaoCriada = await criar.Content.ReadFromJsonAsync<TransacaoConsultarDTO>();

        // Act
        var response = await _client.DeleteAsync($"/api/transacoes/{transacaoCriada!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    [Fact]
    public async Task GetById_TransacaoInexistente_DeveRetornar404()
    {
        // Arrange
        var token = await CriarUsuarioEObterTokenAsync("transacao_getbyid404@teste.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/transacoes/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_TransacaoInexistente_DeveRetornar404()
    {
        // Arrange
        var token = await CriarUsuarioEObterTokenAsync("transacao_delete404@teste.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/transacoes/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_CriarTransacao_ComDescricaoInvalida_DeveRetornar400()
    {
        // Arrange
        var token = await CriarUsuarioEObterTokenAsync("transacao_400@teste.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var categoria = await CriarCategoriaAsync("Salario", FinalidadeCategoria.Receita);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

        var dto = new TransacaoCriarDTO
        {
            Descricao = "A", 
            Valor = 100,
            Tipo = TipoTransacao.Receita,
            CategoriaId = categoria.Id,
            Data = DateTime.Now
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/transacoes", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_CriarTransacao_ComCategoriaIncompativel_DeveRetornar409()
    {
        // Arrange
        var token = await CriarUsuarioEObterTokenAsync("transacao_incompativel@teste.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var categoria = await CriarCategoriaAsync("Alimentacao", FinalidadeCategoria.Despesa);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

        var dto = new TransacaoCriarDTO
        {
            Descricao = "Salario",
            Valor = 3000,
            Tipo = TipoTransacao.Receita, 
            CategoriaId = categoria.Id,
            Data = DateTime.Now
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/transacoes", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    [Fact]
    public async Task GetById_TransacaoDeOutroUsuario_DeveRetornar403()
    {
        // usuário 1
        var token1 = await CriarUsuarioEObterTokenAsync("user1@teste.com");

        // usuário 2
        var token2 = await CriarUsuarioEObterTokenAsync("user2@teste.com");

        var categoria = await CriarCategoriaAsync("Alimentacao", FinalidadeCategoria.Despesa);

        // cria transação com user1
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token1);

        var dto = new TransacaoCriarDTO
        {
            Descricao = "Almoco",
            Valor = 50,
            Tipo = TipoTransacao.Despesa,
            CategoriaId = categoria.Id,
            Data = DateTime.Now
        };

        var criar = await _client.PostAsJsonAsync("/api/transacoes", dto);
        var transacao = await criar.Content.ReadFromJsonAsync<TransacaoConsultarDTO>();

        // tenta acessar com user2
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token2);

        var response = await _client.GetAsync($"/api/transacoes/{transacao!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}