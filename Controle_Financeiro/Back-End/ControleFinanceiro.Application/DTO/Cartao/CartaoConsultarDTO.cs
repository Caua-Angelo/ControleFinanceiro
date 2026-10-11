
namespace ControleFinanceiro.Application.DTO.Cartao
{
    public class CartaoConsultarDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Imagem { get; set; } = string.Empty;
        public int DiaFechamento { get; set; }
        public int DiaVencimento { get; set; }
    }
}
