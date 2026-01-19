using ControleFinanceiro.Domain.Enums;

namespace ControleFinanceiro.Application.DTO.Transacao
{
    public class TransacaoCriarDTO
    {
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public TipoTransacao Tipo { get; set; }

        public int UsuarioId { get; set; }
        public int CategoriaId { get; set; }
        public DateTime Data { get; set; }
    }
}
