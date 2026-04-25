using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ControleFinanceiro.Application.DTO.Usuario;
using ControleFinanceiro.Domain.Models;
using ControleFinanceiro.Infraestructure.Data;
using ControleFinanceiro.Test.Integration.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace ControleFinanceiro.Test.Integration.Controllers;

public class UsuarioControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public UsuarioControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task AutorizarClienteAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

        db.Usuario.Add(new Usuario("Cauã", 22, "usuario_auth@teste.com", BCrypt.Net.BCrypt.HashPassword("senha123")));
        await db.SaveChangesAsync();

        var token = await AuthHelper.ObterTokenAsync(_client, "usuario_auth@teste.com", "senha123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetMe_SemToken_DeveRetornar401()
    {
        var response = await _client.GetAsync("/api/usuarios/me");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMe_ComToken_DeveRetornar200()
    {
        // Arrange
        await AutorizarClienteAsync();

        // Act
        var response = await _client.GetAsync("/api/usuarios/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<UsuarioConsultarDTO>();
        body.Should().NotBeNull();
        body!.Email.Should().Be("usuario_auth@teste.com");
    }

    [Fact]
    public async Task PutMe_AtualizarUsuario_DeveRetornar200()
    {
        // Arrange
        await AutorizarClienteAsync();

        var atualizar = new UsuarioAlterarDTO
        {
            Nome = "Novo Nome",
            Idade = 30
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/usuarios/me", atualizar);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<UsuarioConsultarDTO>();
        body!.Nome.Should().Be("Novo Nome");
    }

    [Fact]
    public async Task DeleteMe_DeveRetornar204()
    {
        // Arrange
        await AutorizarClienteAsync();

        // Act
        var response = await _client.DeleteAsync("/api/usuarios/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Register_CriarUsuario_DeveRetornar201()
    {
        var dto = new UsuarioIncluirDTO
        {
            Nome = "João",
            Idade = 25,
            Email = "joao@teste.com",
            Senha = "senha123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Register_EmailDuplicado_DeveRetornar409()
    {
        var dto = new UsuarioIncluirDTO
        {
            Nome = "Duplicado",
            Idade = 25,
            Email = "duplicado@teste.com",
            Senha = "senha123"
        };

        await _client.PostAsJsonAsync("/api/auth/register", dto);

        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_NomeInvalido_DeveRetornar400()
    {
        var dto = new UsuarioIncluirDTO
        {
            Nome = "A",
            Idade = 25,
            Email = "invalido@teste.com",
            Senha = "senha123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}