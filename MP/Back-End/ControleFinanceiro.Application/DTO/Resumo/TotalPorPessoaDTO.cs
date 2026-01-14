namespace ControleFinanceiro.Application.DTO.Resumo
{
    public class UsuarioResumoFinanceiroDTO
    {
        public int UsuarioId { get; set; }
        public string Nome { get; set; } = string.Empty;

        public decimal TotalReceitas { get; set; }
        public decimal TotalDespesas { get; set; }
        public decimal Saldo => TotalReceitas - TotalDespesas;
    }
}
