namespace ControleFinanceiro.Application.DTO.Usuario
{
    public class UsuarioConsultarDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateOnly DataNascimento { get; set; }
        public string Email { get; set; } = string.Empty;

    }
}
