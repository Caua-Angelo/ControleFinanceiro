namespace ControleFinanceiro.Application.DTO.CompraParcelada
{
    public class CompraParceladaCriarDTO
    {
        public string Descricao { get; set; } = string.Empty;
        public decimal ValorTotal { get; set; }
        public int QuantidadeParcelas { get; set; }
        public DateOnly DataCompra { get; set; }
        public int CartaoId { get; set; }
        public int CategoriaId { get; set; }
    }
}