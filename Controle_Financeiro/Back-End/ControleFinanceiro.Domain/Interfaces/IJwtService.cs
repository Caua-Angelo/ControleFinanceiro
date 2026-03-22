using ControleFinanceiro.Domain.Models;

namespace ControleFinanceiro.Domain.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(Usuario usuario);
    }
}
