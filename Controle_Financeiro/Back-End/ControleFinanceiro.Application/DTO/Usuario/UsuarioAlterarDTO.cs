namespace ControleFinanceiro.Application.DTO.Usuario
{
    public class UsuarioAlterarDTO
    {
        public string Nome { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
