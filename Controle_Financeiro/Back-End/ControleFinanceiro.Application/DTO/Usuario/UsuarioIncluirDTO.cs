namespace ControleFinanceiro.Application.DTO.Usuario
{
    public class UsuarioIncluirDTO
    {
        public string Nome { get; set; } = string.Empty;
        public int Idade { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; } = string.Empty;
    }
}
