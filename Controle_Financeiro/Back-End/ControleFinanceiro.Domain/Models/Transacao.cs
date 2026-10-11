
using ControleFinanceiro.Domain.Enums;
using ControleFinanceiro.Domain.Validation;
using System.Text.RegularExpressions;

namespace ControleFinanceiro.Domain.Models
{
    public class Transacao
    {
        public int Id { get; private set; }

        public string Descricao { get; private set; } = null!;

        public decimal Valor { get; private set; }

        public TipoTransacao Tipo { get; private set; }

        public DateOnly Data { get; private set; }

        public int CategoriaId { get; private set; }
        public Categoria Categoria { get; private set; } = null!;

        public int UsuarioId { get; private set; }
        public Usuario Usuario { get; private set; } = null!;

        public int? CartaoId { get; private set; }
        public Cartao? Cartao { get; private set; }

        protected Transacao() { }

        public Transacao(string descricao,decimal valor,TipoTransacao tipo,Categoria categoria,Usuario usuario,DateOnly data,Cartao? cartao = null)
        {
            ValidateDomain(descricao,valor,tipo,categoria,usuario,data,cartao);
        }

        public void Update(string descricao,decimal valor,TipoTransacao tipo,Categoria categoria,Usuario usuario,DateOnly data,Cartao? cartao = null)
        {
            ValidateDomain(descricao,valor,tipo,categoria,usuario,data,cartao);
        }

        private void ValidateDomain(string descricao,decimal valor,TipoTransacao tipo,Categoria categoria,Usuario usuario,DateOnly data,Cartao? cartao)
        {
            descricao = descricao?.Trim() ?? string.Empty;

            DomainExceptionValidation.When(
                string.IsNullOrWhiteSpace(descricao),
                "A descrição da transação é obrigatória.");

            DomainExceptionValidation.When(
                descricao.Length < 3 || descricao.Length > 100,
                "A descrição deve ter entre 3 e 100 caracteres.");

            DomainExceptionValidation.When(
                !Regex.IsMatch(descricao, @"^[\p{L}\p{N}\s.,()**\\-**]+$"),
                "A descrição contém caracteres inválidos.");

            DomainExceptionValidation.When(
                valor <= 0,
                "O valor da transação deve ser maior que zero.");

            DomainExceptionValidation.When(
                !Enum.IsDefined(typeof(TipoTransacao), tipo),
                "Tipo de transação inválido.");

            DomainExceptionValidation.When(
                categoria is null,
                "Categoria é obrigatória.");

            DomainExceptionValidation.When(
                usuario is null,
                "Usuário é obrigatório.");

            DomainExceptionValidation.When(
                data == default,
                "A data da transação é obrigatória.");

            // Menor de idade só pode registrar despesas
            var hoje = DateOnly.FromDateTime(DateTime.Today);

            var idade = hoje.Year - usuario.DataNascimento.Year;

            if (usuario.DataNascimento > hoje.AddYears(-idade))
            {
                idade--;
            }

            DomainExceptionValidation.When(
                idade < 18 && tipo != TipoTransacao.Despesa,
                "Usuários menores de 18 anos podem registrar apenas despesas.");

            // Categoria deve ser compatível com o tipo da transação
            DomainExceptionValidation.When(
                tipo == TipoTransacao.Despesa &&
                categoria.Finalidade == FinalidadeCategoria.Receita,
                "Categoria de receita não pode ser usada em uma despesa.");

            DomainExceptionValidation.When(
                tipo == TipoTransacao.Receita &&
                categoria.Finalidade == FinalidadeCategoria.Despesa,
                "Categoria de despesa não pode ser usada em uma receita.");

            // Cartão opcional
            DomainExceptionValidation.When(
                cartao != null && cartao.UsuarioId != usuario.Id,
                "O cartão deve pertencer ao usuário da transação.");

            // Atribuições
            Descricao = descricao;
            Valor = valor;
            Tipo = tipo;

            Categoria = categoria;
            CategoriaId = categoria.Id;

            Usuario = usuario;
            UsuarioId = usuario.Id;

            Data = data;

            Cartao = cartao;
            CartaoId = cartao?.Id;
        }
    }
}
