using ControleFinanceiro.Domain.Interfaces;
using ControleFinanceiro.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ControleFinanceiro.Infra.Data.Services
{
    public class JwtService : IJwtService
    {
        //Nota: estou deixando esses comentários porquê ainda estou aprendendo sobre JWT e achei importante entender cada passo do processo de geração do token
        private readonly IConfiguration _config;

        // IConfiguration dá acesso ao appsettings.json
        // para pegar a chave secreta, issuer e audience
        public JwtService(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateToken(Usuario usuario)
        {
            // transforma a chave secreta do appsettings em um objeto
            // que o JWT entende — sem ela ninguém consegue assinar um token válido
            var keyValue = _config["Jwt:Key"]
             ?? throw new Exception("JWT Key não configurada");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(keyValue));

            // combina a chave com o algoritmo HmacSha256
            // garante que o token não pode ser falsificado sem conhecer a chave
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            // claims são as informações que ficam dentro do token
            // o frontend pode extrair Id, Email e Nome sem consultar o banco
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nome),
            };

            // monta o token com todas as informações:
            // issuer  — quem emitiu (a API)
            // audience — pra quem foi emitido (o frontend)
            // expires  — expira em 8 horas, depois disso o usuário precisa logar de novo
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            // serializa o objeto token na string final que o frontend recebe
            // aquele texto longo separado por pontos que começa com eyJ...
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
