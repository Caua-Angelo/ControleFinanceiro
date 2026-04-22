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
    public async Task Get_SemToken_DeveRetornar401()
    {
        var response = await _client.GetAsync("/api/usuarios");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_CriarUsuario_DeveRetornar201()
    {
        // Arrange
        await AutorizarClienteAsync();

        var dto = new UsuarioIncluirDTO
        {
            Nome = "João",
            Idade = 25,
            Email = "joao@teste.com",
            Senha = "senha123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/usuarios", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<UsuarioConsultarDTO>();
        body.Should().NotBeNull();
        body!.Nome.Should().Be("João");
        body.Email.Should().Be("joao@teste.com");
    }

    [Fact]
    public async Task Get_ListarUsuarios_DeveRetornar200()
    {
        // Arrange
        await AutorizarClienteAsync();

        // Act
        var response = await _client.GetAsync("/api/usuarios");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<IEnumerable<UsuarioConsultarDTO>>();
        body.Should().NotBeNull();
    }

    [Fact]
    public async Task Put_AtualizarUsuario_DeveRetornar200()
    {
        // Arrange
        await AutorizarClienteAsync();

        var criar = await _client.PostAsJsonAsync("/api/usuarios", new UsuarioIncluirDTO
        {
            Nome = "Maria",
            Idade = 30,
            Email = "maria@teste.com",
            Senha = "senha123"
        });
        var usuarioCriado = await criar.Content.ReadFromJsonAsync<UsuarioConsultarDTO>();

        var atualizar = new UsuarioAlterarDTO
        {
            Nome = "Maria Silva",
            Idade = 31,
            Email = "maria.silva@teste.com"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/usuarios/{usuarioCriado!.Id}", atualizar);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<UsuarioConsultarDTO>();
        body!.Nome.Should().Be("Maria Silva");
        body.Email.Should().Be("maria.silva@teste.com");
    }

    [Fact]
    public async Task Delete_UsuarioExistente_DeveRetornar204()
    {
        // Arrange
        await AutorizarClienteAsync();

        var criar = await _client.PostAsJsonAsync("/api/usuarios", new UsuarioIncluirDTO
        {
            Nome = "Pedro",
            Idade = 28,
            Email = "pedro@teste.com",
            Senha = "senha123"
        });
        var usuarioCriado = await criar.Content.ReadFromJsonAsync<UsuarioConsultarDTO>();

        // Act
        var response = await _client.DeleteAsync($"/api/usuarios/{usuarioCriado!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    [Fact]
    public async Task GetById_UsuarioInexistente_DeveRetornar404()
    {
        // Arrange
        await AutorizarClienteAsync();

        // Act
        var response = await _client.GetAsync("/api/usuarios/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_UsuarioInexistente_DeveRetornar404()
    {
        // Arrange
        await AutorizarClienteAsync();

        var dto = new UsuarioAlterarDTO
        {
            Nome = "Inexistente",
            Idade = 25,
            Email = "inexistente@teste.com"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/usuarios/99999", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_UsuarioInexistente_DeveRetornar404()
    {
        // Arrange
        await AutorizarClienteAsync();

        // Act
        var response = await _client.DeleteAsync("/api/usuarios/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_CriarUsuario_ComNomeInvalido_DeveRetornar400()
    {
        // Arrange
        await AutorizarClienteAsync();

        var dto = new UsuarioIncluirDTO
        {
            Nome = "A",
            Idade = 25,
            Email = "invalido@teste.com",
            Senha = "senha123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/usuarios", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_CriarUsuario_ComEmailJaExistente_DeveRetornar409()
    {
        // Arrange
        await AutorizarClienteAsync();

        var dto = new UsuarioIncluirDTO
        {
            Nome = "Duplicado",
            Idade = 25,
            Email = "duplicado@teste.com",
            Senha = "senha123"
        };

        await _client.PostAsJsonAsync("/api/usuarios", dto);

        // Act
        var response = await _client.PostAsJsonAsync("/api/usuarios", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}