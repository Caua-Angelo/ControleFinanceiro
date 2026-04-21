using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ControleFinanceiro.Application.DTO.Categoria;
using ControleFinanceiro.Domain.Enums;
using ControleFinanceiro.Domain.Models;
using ControleFinanceiro.Infraestructure.Data;
using ControleFinanceiro.Test.Integration.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace ControleFinanceiro.Test.Integration.Controllers;

public class CategoriaControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public CategoriaControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task AutorizarClienteAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

        db.Usuario.Add(new Usuario("Cauã", 22, "categoria_user@teste.com", BCrypt.Net.BCrypt.HashPassword("senha123")));
        await db.SaveChangesAsync();

        var token = await AuthHelper.ObterTokenAsync(_client, "categoria_user@teste.com", "senha123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task Get_SemToken_DeveRetornar401()
    {
        var response = await _client.GetAsync("/api/categorias");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_CriarCategoria_DeveRetornar201()
    {
        // Arrange
        await AutorizarClienteAsync();

        var dto = new CategoriaIncluirDTO
        {
            Descricao = "Alimentacao",
            Finalidade = FinalidadeCategoria.Despesa
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/categorias", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<CategoriaConsultarDTO>();
        body.Should().NotBeNull();
        body!.Descricao.Should().Be("Alimentacao");
        body.Finalidade.Should().Be(FinalidadeCategoria.Despesa);
    }

    [Fact]
    public async Task Get_ListarCategorias_DeveRetornar200()
    {
        // Arrange
        await AutorizarClienteAsync();

        // Act
        var response = await _client.GetAsync("/api/categorias");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<IEnumerable<CategoriaConsultarDTO>>();
        body.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_CategoriaExistente_DeveRetornar204()
    {
        // Arrange
        await AutorizarClienteAsync();

        var dto = new CategoriaIncluirDTO
        {
            Descricao = "Transporte",
            Finalidade = FinalidadeCategoria.Despesa
        };

        var criar = await _client.PostAsJsonAsync("/api/categorias", dto);
        var categoriaCriada = await criar.Content.ReadFromJsonAsync<CategoriaConsultarDTO>();

        // Act
        var response = await _client.DeleteAsync($"/api/categorias/{categoriaCriada!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Put_AtualizarCategoria_DeveRetornar200()
    {
        // Arrange
        await AutorizarClienteAsync();

        var criar = await _client.PostAsJsonAsync("/api/categorias", new CategoriaIncluirDTO
        {
            Descricao = "Lazer",
            Finalidade = FinalidadeCategoria.Despesa
        });
        var categoriaCriada = await criar.Content.ReadFromJsonAsync<CategoriaConsultarDTO>();

        var atualizar = new CategoriaAlterarDTO
        {
            Descricao = "Lazer e Entretenimento",
            Finalidade = FinalidadeCategoria.Despesa
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/categorias/{categoriaCriada!.Id}", atualizar);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<CategoriaConsultarDTO>();
        body!.Descricao.Should().Be("Lazer e Entretenimento");
    }
}