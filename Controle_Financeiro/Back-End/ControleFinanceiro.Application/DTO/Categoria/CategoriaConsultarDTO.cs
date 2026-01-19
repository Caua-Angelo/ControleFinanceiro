using ControleFinanceiro.Domain.Enums;

namespace ControleFinanceiro.Application.DTO.Categoria
{
    public class CategoriaConsultarDTO
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public FinalidadeCategoria Finalidade { get; set; }
    }
}
