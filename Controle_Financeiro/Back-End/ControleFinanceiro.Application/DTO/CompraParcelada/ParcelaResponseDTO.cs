
namespace ControleFinanceiro.Application.DTO.CompraParcelada
{
    public class ParcelaResponseDTO
    {
        public int Id { get; set; }

        public int Numero { get; set; }

        public decimal Valor { get; set; }

        public DateOnly DataVencimento { get; set; }

        public bool Paga { get; set; }
    }
}
