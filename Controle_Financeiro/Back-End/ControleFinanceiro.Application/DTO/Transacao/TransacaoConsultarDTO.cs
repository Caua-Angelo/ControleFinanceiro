using ControleFinanceiro.Domain.Enums;

namespace ControleFinanceiro.Application.DTO.Transacao
{
    public class TransacaoConsultarDTO
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public TipoTransacao Tipo { get; set; }

        public int UsuarioId { get; set; }
        public string UsuarioNome { get; set; } = string.Empty;

        public int CategoriaId { get; set; }
        public string CategoriaDescricao { get; set; } = string.Empty;
        public FinalidadeCategoria FinalidadeCategoria { get; set; }

        public DateOnly Data { get; set; }

        public int? CartaoId { get; set; }
        public string? CartaoNome { get; set; }
    }
}