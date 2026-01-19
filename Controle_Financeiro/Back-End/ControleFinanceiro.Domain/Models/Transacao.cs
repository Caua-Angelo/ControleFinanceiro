using ControleFinanceiro.Domain.Enums;
using ControleFinanceiro.Domain.Validation;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ControleFinanceiro.Domain.Models
{
    public class Transacao
    {
        public int Id { get; private set; }

        public string Descricao { get; private set; } = null!;

        public decimal Valor { get; private set; }

        public TipoTransacao Tipo { get; private set; }

        public DateTime Data { get; private set; }

        // 🔹 Relacionamento com Categoria
        public int CategoriaId { get; private set; }
        public Categoria Categoria { get; private set; } = null!;

        // 🔹 Relacionamento com Usuário
        public int UsuarioId { get; private set; }
        public Usuario Usuario { get; private set; } = null!;

        protected Transacao() { }

        public Transacao(
            string descricao,
            decimal valor,
            TipoTransacao tipo,
            Categoria categoria,
            Usuario usuario,
             DateTime data)
        {
            ValidateDomain(descricao, valor, tipo, categoria, usuario, data);
        }

        public void Update(
            string descricao,
            decimal valor,
            TipoTransacao tipo,
            Categoria categoria,
            Usuario usuario,
            DateTime data)
        {
            ValidateDomain(descricao, valor, tipo, categoria, usuario, data);
        }

        private void ValidateDomain(
            string descricao,
            decimal valor,
            TipoTransacao tipo,
            Categoria categoria,
            Usuario usuario,
               DateTime data)
        {
            descricao = descricao?.Trim() ?? string.Empty;

            // 🔹 Validações básicas
            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(descricao),
                "A descrição da transação é obrigatória.");

            DomainExceptionValidation.When(descricao.Length < 3 || descricao.Length > 100,
                "A descrição deve ter entre 3 e 100 caracteres.");

            DomainExceptionValidation.When(!Regex.IsMatch(descricao, @"^[\p{L}\p{N}\s]+$"),
                "A descrição contém caracteres inválidos.");

            DomainExceptionValidation.When(valor <= 0,
                "O valor da transação deve ser maior que zero.");

            DomainExceptionValidation.When(!Enum.IsDefined(typeof(TipoTransacao), tipo),
                "Tipo de transação inválido.");

            DomainExceptionValidation.When(categoria is null,
                "Categoria é obrigatória.");

            DomainExceptionValidation.When(usuario is null,
                "Usuário é obrigatório.");

            // ← ADICIONAR validação de data (opcional)
            DomainExceptionValidation.When(data == default,
                "A data da transação é obrigatória.");

            // 🔴 REGRA 1: Menor de idade só pode registrar despesa
            DomainExceptionValidation.When(
                usuario.Idade < 18 && tipo != TipoTransacao.Despesa,
                "Usuários menores de 18 anos podem registrar apenas despesas."
            );

            // Categoria deve ser compatível com o tipo da transação
            DomainExceptionValidation.When(
                tipo == TipoTransacao.Despesa &&
                categoria.Finalidade == FinalidadeCategoria.Receita,
                "Categoria de receita não pode ser usada em uma despesa."
            );

            DomainExceptionValidation.When(
                tipo == TipoTransacao.Receita &&
                categoria.Finalidade == FinalidadeCategoria.Despesa,
                "Categoria de despesa não pode ser usada em uma receita."
            );

            // 🔹 Atribuições
            Descricao = descricao;
            Valor = valor;
            Tipo = tipo;

            Categoria = categoria;
            CategoriaId = categoria.Id;

            Usuario = usuario;
            UsuarioId = usuario.Id;
            Data = data; 
        }
    }
}
