using System.Net.Http.Json;

namespace ControleFinanceiro.Test.Integration.Helpers;

public static class AuthHelper
{
    public static async Task<string> ObterTokenAsync(HttpClient client, string email, string senha)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Senha = senha
        });

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        return body!["token"];
    }
}