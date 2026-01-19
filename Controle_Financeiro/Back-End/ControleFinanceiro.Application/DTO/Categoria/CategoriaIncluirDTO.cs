using ControleFinanceiro.Domain.Enums;

namespace ControleFinanceiro.Application.DTO.Categoria
{
    public class CategoriaIncluirDTO
    {
        public string Descricao { get; set; } = string.Empty;
        public FinalidadeCategoria Finalidade { get; set; }
    }
}
