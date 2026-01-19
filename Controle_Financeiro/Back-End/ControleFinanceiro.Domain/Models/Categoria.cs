using ControleFinanceiro.Domain.Enums;
using ControleFinanceiro.Domain.Validation;
using System.Text.RegularExpressions;

namespace ControleFinanceiro.Domain.Models
{
    public class Categoria
    {
        public int Id { get; private set; }

        public string Descricao { get; private set; } = null!;

        //  Define se a categoria aceita Receita, Despesa ou Ambas
        public FinalidadeCategoria Finalidade { get; private set; }

        //  Uma categoria pode estar em várias transações
        public ICollection<Transacao> Transacoes { get; private set; } = new List<Transacao>();

        protected Categoria() { }

        public Categoria(string descricao, FinalidadeCategoria finalidade)
        {
            ValidateDomain(descricao, finalidade);
        }

        public void Update(string descricao, FinalidadeCategoria finalidade)
        {
            ValidateDomain(descricao, finalidade);
        }

        private void ValidateDomain(string descricao, FinalidadeCategoria finalidade)
        {
            descricao = descricao?.Trim() ?? string.Empty;

            DomainExceptionValidation.When(string.IsNullOrEmpty(descricao),
                "A descrição da categoria precisa ser preenchida.");

            DomainExceptionValidation.When(descricao.Length < 3 || descricao.Length > 60,
                "A descrição da categoria deve ter entre 3 e 60 caracteres.");

            DomainExceptionValidation.When(!Regex.IsMatch(descricao, @"^[\p{L}\p{N}\s]+$"),
                "A descrição da categoria contém caracteres inválidos.");

            DomainExceptionValidation.When(!Enum.IsDefined(typeof(FinalidadeCategoria), finalidade),
                "Finalidade da categoria inválida.");

            Descricao = descricao;
            Finalidade = finalidade;
        }
    }
}
