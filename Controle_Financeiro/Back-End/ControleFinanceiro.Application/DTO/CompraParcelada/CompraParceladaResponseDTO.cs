namespace ControleFinanceiro.Application.DTO.CompraParcelada
{
    public class CompraParceladaResponseDTO
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal ValorTotal { get; set; }
        public int QuantidadeParcelas { get; set; }
        public DateOnly DataCompra { get; set; }
        public int CartaoId { get; set; }
        public int CategoriaId { get; set; }

        public List<ParcelaResponseDTO> Parcelas { get; set; } = new();
    }
}