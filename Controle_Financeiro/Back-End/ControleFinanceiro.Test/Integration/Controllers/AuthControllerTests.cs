using System.Net;
using System.Net.Http.Json;
using ControleFinanceiro.Application.DTO.Auth;
using ControleFinanceiro.Domain.Models;
using ControleFinanceiro.Infraestructure.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

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
             22,
             "caua@teste.com",
             BCrypt.Net.BCrypt.HashPassword("senha123")
            ));
        await db.SaveChangesAsync();

        var dto = new LoginRequestDto("caua@teste.com", "senha123");

        var response = await _client.PostAsJsonAsync("/api/auth/login", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
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
         22,
         "caua2@teste.com",
         BCrypt.Net.BCrypt.HashPassword("senha123")
 ));
        await db.SaveChangesAsync();

        var dto = new LoginRequestDto("caua2@teste.com", "senhaerrada");

        var response = await _client.PostAsJsonAsync("/api/auth/login", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}