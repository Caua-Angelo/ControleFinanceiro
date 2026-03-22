using ControleFinanceiro.Application.DTO.Auth;
using ControleFinanceiro.Application.DTO.Usuario;
using ControleFinanceiro.Application.Interface;
using ControleFinanceiro.Domain.Interfaces;
using ControleFinanceiro.Domain.Models;


namespace ControleFinanceiro.Application.Services
{
    public class AuthService : IAuthService
    {

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IJwtService _jwtService;

        public AuthService(IUsuarioRepository usuarioRepository, IJwtService jwtService)
        {
            _usuarioRepository = usuarioRepository;
            _jwtService = jwtService;
        }
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(dto.Email);

            if (usuario is null || !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.HashSenha))
                throw new UnauthorizedAccessException("Credenciais inválidas.");

            var token = _jwtService.GenerateToken(usuario);
            return new LoginResponseDto(token);
        }

        public async Task RegisterAsync(UsuarioIncluirDTO dto)
        {
            var existente = await _usuarioRepository.GetByEmailAsync(dto.Email);
            if (existente is not null)
                throw new InvalidOperationException("E-mail já cadastrado.");

            var senhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
            var usuario = new Usuario(dto.Nome, dto.Idade, dto.Email, senhaHash);

            await _usuarioRepository.AddAsync(usuario);
            await _usuarioRepository.SaveAsync();
        }
    }
}
