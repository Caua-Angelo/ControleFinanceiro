using ControleFinanceiro.Domain.Enums;
using ControleFinanceiro.Domain.Models;
using ControleFinanceiro.Domain.Validation;
using FluentAssertions;

namespace ControleFinanceiro.Test.Unit.Domain
{
    public class TransacaoTests
    {
        private readonly Usuario _usuarioValido;
        private readonly Categoria _categoriaDeValida;
        private readonly Categoria _categoriaReValida;

        public TransacaoTests()
        {
            _usuarioValido = new Usuario("Caua", 18, "caua@gmail.com", "hash");
            _categoriaDeValida = new Categoria("Alimentacao", FinalidadeCategoria.Despesa);
            _categoriaReValida = new Categoria("Salario", FinalidadeCategoria.Receita);
        }

        [Fact]
        public void InstanciarTransacao_DadosValidos_NaoDeveLancarExcecao()
        {
            Action act = () => new Transacao("Mercado", 100, TipoTransacao.Despesa, _categoriaDeValida, _usuarioValido, DateTime.Now);
            act.Should().NotThrow();
        }

        [Fact]
        public void InstanciarTransacao_MenorDeIdadeDespesa_NaoDeveLancarExcecao()
        {
            var menorDeIdade = new Usuario("Caua", 17, "caua@gmail.com", "hash");
            Action act = () => new Transacao("Mercado", 100, TipoTransacao.Despesa, _categoriaDeValida, menorDeIdade, DateTime.Now);
            act.Should().NotThrow();
        }

        [Theory]
        [InlineData("", 100, "A descrição da transação é obrigatória.")]
        [InlineData("Me", 100, "A descrição deve ter entre 3 e 100 caracteres.")]
        [InlineData("Mercadooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo", 100, "A descrição deve ter entre 3 e 100 caracteres.")]
        [InlineData("Mercado!", 100, "A descrição contém caracteres inválidos.")]
        [InlineData("Mercado", 0, "O valor da transação deve ser maior que zero.")]
        [InlineData("Mercado", -1, "O valor da transação deve ser maior que zero.")]
        public void InstanciarTransacao_DadosInvalidos_DeveLancarExcecao(string descricao, decimal valor, string mensagemEsperada)
        {
            Action act = () => new Transacao(descricao, valor, TipoTransacao.Despesa, _categoriaDeValida, _usuarioValido, DateTime.Now);
            act.Should().Throw<DomainExceptionValidation>()
               .WithMessage(mensagemEsperada);
        }

        [Fact]
        public void InstanciarTransacao_CategoriaNula_DeveLancarExcecao()
        {
            Action act = () => new Transacao("Mercado", 100, TipoTransacao.Despesa, null, _usuarioValido, DateTime.Now);
            act.Should().Throw<DomainExceptionValidation>()
               .WithMessage("Categoria é obrigatória.");
        }

        [Fact]
        public void InstanciarTransacao_UsuarioNulo_DeveLancarExcecao()
        {
            Action act = () => new Transacao("Mercado", 100, TipoTransacao.Despesa, _categoriaDeValida, null, DateTime.Now);
            act.Should().Throw<DomainExceptionValidation>()
               .WithMessage("Usuário é obrigatório.");
        }

        [Fact]
        public void InstanciarTransacao_DataDefault_DeveLancarExcecao()
        {
            Action act = () => new Transacao("Mercado", 100, TipoTransacao.Despesa, _categoriaDeValida, _usuarioValido, default);
            act.Should().Throw<DomainExceptionValidation>()
               .WithMessage("A data da transação é obrigatória.");
        }

        [Fact]
        public void InstanciarTransacao_TipoInvalido_DeveLancarExcecao()
        {
            Action act = () => new Transacao("Mercado", 100, (TipoTransacao)99, _categoriaDeValida, _usuarioValido, DateTime.Now);
            act.Should().Throw<DomainExceptionValidation>()
               .WithMessage("Tipo de transação inválido.");
        }

        [Fact]
        public void InstanciarTransacao_MenorDeIdadeReceita_DeveLancarExcecao()
        {
            var menorDeIdade = new Usuario("Caua", 17, "caua@gmail.com", "hash");
            Action act = () => new Transacao("Salario", 100, TipoTransacao.Receita, _categoriaReValida, menorDeIdade, DateTime.Now);
            act.Should().Throw<DomainExceptionValidation>()
               .WithMessage("Usuários menores de 18 anos podem registrar apenas despesas.");
        }

        [Fact]
        public void InstanciarTransacao_CategoriaReceitaEmDespesa_DeveLancarExcecao()
        {
            Action act = () => new Transacao("Mercado", 100, TipoTransacao.Despesa, _categoriaReValida, _usuarioValido, DateTime.Now);
            act.Should().Throw<DomainExceptionValidation>()
               .WithMessage("Categoria de receita não pode ser usada em uma despesa.");
        }

        [Fact]
        public void InstanciarTransacao_CategoriaDespesaEmReceita_DeveLancarExcecao()
        {
            Action act = () => new Transacao("Salario", 100, TipoTransacao.Receita, _categoriaDeValida, _usuarioValido, DateTime.Now);
            act.Should().Throw<DomainExceptionValidation>()
               .WithMessage("Categoria de despesa não pode ser usada em uma receita.");
        }

        [Fact]
        public void Update_DadosValidos_NaoDeveLancarExcecao()
        {
            var transacao = new Transacao("Mercado", 100, TipoTransacao.Despesa, _categoriaDeValida, _usuarioValido, DateTime.Now);
            Action act = () => transacao.Update("Mercado atualizado", 200, TipoTransacao.Despesa, _categoriaDeValida, _usuarioValido, DateTime.Now);
            act.Should().NotThrow();
        }

        [Fact]
        public void Update_DadosInvalidos_DeveLancarExcecao()
        {
            var transacao = new Transacao("Mercado", 100, TipoTransacao.Despesa, _categoriaDeValida, _usuarioValido, DateTime.Now);
            Action act = () => transacao.Update("", 200, TipoTransacao.Despesa, _categoriaDeValida, _usuarioValido, DateTime.Now);
            act.Should().Throw<DomainExceptionValidation>()
               .WithMessage("A descrição da transação é obrigatória.");
        }
    }
}