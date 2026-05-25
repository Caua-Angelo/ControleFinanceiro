namespace ControleFinanceiro.Application.DTO.Usuario
{
    public class UsuarioIncluirDTO
    {
        public string Nome { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
