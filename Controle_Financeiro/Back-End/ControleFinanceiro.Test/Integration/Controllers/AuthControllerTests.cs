using ControleFinanceiro.Application.DTO.Auth;
using ControleFinanceiro.Application.DTO.Usuario;
using ControleFinanceiro.Domain.Models;
using ControleFinanceiro.Infraestructure.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace ControleFinanceiro.Test.Integration.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_DeveRetornar200ComToken()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

        db.Usuario.Add(new Usuario(
            "Cauã",
            new DateTime(2000, 1, 1),
            "caua@teste.com",
            BCrypt.Net.BCrypt.HashPassword("senha123")
        ));

        await db.SaveChangesAsync();

        var dto = new LoginRequestDto(
            "caua@teste.com",
            "senha123"
        );

        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            dto
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content
            .ReadFromJsonAsync<Dictionary<string, string>>();

        body.Should().NotBeNull();
        body!.Should().ContainKey("token");
        body["token"].Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_ComSenhaErrada_DeveRetornar401()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

        db.Usuario.Add(new Usuario(
            "Cauã",
            new DateTime(2000, 1, 1),
            "caua2@teste.com",
            BCrypt.Net.BCrypt.HashPassword("senha123")
        ));

        await db.SaveChangesAsync();

        var dto = new LoginRequestDto(
            "caua2@teste.com",
            "senhaerrada"
        );

        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            dto
        );

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_ComDadosValidos_DeveRetornar201()
    {
        // Arrange
        var dto = new UsuarioIncluirDTO
        {
            Nome = "Carlos",
            DataNascimento = new DateTime(1996, 1, 1),
            Email = "carlos@teste.com",
            Senha = "senha123"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            dto
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Register_ComEmailJaExistente_DeveRetornar409()
    {
        // Arrange
        var dto = new UsuarioIncluirDTO
        {
            Nome = "Ana",
            DataNascimento = new DateTime(1999, 1, 1),
            Email = "ana@teste.com",
            Senha = "senha123"
        };

        await _client.PostAsJsonAsync(
            "/api/auth/register",
            dto
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            dto
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_ELogin_FluxoCompleto_DeveRetornar200ComToken()
    {
        // Arrange
        var registerDto = new UsuarioIncluirDTO
        {
            Nome = "Lucas",
            DataNascimento = new DateTime(1994, 1, 1),
            Email = "lucas@teste.com",
            Senha = "senha123"
        };

        await _client.PostAsJsonAsync(
            "/api/auth/register",
            registerDto
        );

        var loginDto = new LoginRequestDto(
            "lucas@teste.com",
            "senha123"
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            loginDto
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content
            .ReadFromJsonAsync<Dictionary<string, string>>();

        body.Should().NotBeNull();
        body!.Should().ContainKey("token");
        body["token"].Should().NotBeNullOrEmpty();
    }
}