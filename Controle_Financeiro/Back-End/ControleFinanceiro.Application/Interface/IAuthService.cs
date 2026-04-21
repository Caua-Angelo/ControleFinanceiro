using ControleFinanceiro.Application.DTO.Auth;
using ControleFinanceiro.Application.DTO.Usuario;

namespace ControleFinanceiro.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
        Task RegisterAsync(UsuarioIncluirDTO dto);
    }
}
