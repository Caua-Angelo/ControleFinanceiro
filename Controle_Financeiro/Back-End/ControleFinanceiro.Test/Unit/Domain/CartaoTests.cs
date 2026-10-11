
using ControleFinanceiro.Domain.Models;
using ControleFinanceiro.Domain.Validation;
using FluentAssertions;

namespace ControleFinanceiro.Test.Unit.Domain
{
    public class CartaoTests
    {
        private static Usuario CriarUsuario()
        {
            return new Usuario(
                "Usuário Teste",
                new DateOnly(2000, 1, 1),
                "teste@teste.com",
                "Senha123!");
        }

        [Fact]
        public void InstanciarCartao_DadosValidos_NaoDeveLancarExcecao()
        {
            Action action = () => new Cartao(
                "Nubank",
                "cartao-roxo",
                10,
                17,
                CriarUsuario());

            action.Should().NotThrow();
        }

        [Theory]
        [InlineData(null, "cartao-roxo", 10, 17,
            "O nome do cartão é obrigatório.")]
        [InlineData("", "cartao-roxo", 10, 17,
            "O nome do cartão é obrigatório.")]
        [InlineData("   ", "cartao-roxo", 10, 17,
            "O nome do cartão é obrigatório.")]
        [InlineData("Nubank", null, 10, 17,
            "A imagem do cartão é obrigatória.")]
        [InlineData("Nubank", "", 10, 17,
            "A imagem do cartão é obrigatória.")]
        [InlineData("Nubank", "cartao-roxo", 0, 17,
            "O dia de fechamento deve estar entre 1 e 31.")]
        [InlineData("Nubank", "cartao-roxo", 32, 17,
            "O dia de fechamento deve estar entre 1 e 31.")]
        [InlineData("Nubank", "cartao-roxo", 10, 0,
            "O dia de vencimento deve estar entre 1 e 31.")]
        [InlineData("Nubank", "cartao-roxo", 10, 32,
            "O dia de vencimento deve estar entre 1 e 31.")]
        public void InstanciarCartao_DadosInvalidos_DeveLancarExcecao(
            string nome,
            string imagem,
            int diaFechamento,
            int diaVencimento,
            string mensagemEsperada)
        {
            Action action = () => new Cartao(
                nome,
                imagem,
                diaFechamento,
                diaVencimento,
                CriarUsuario());

            action.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage(mensagemEsperada);
        }

        [Fact]
        public void InstanciarCartao_NomeMaiorQue50Caracteres_DeveLancarExcecao()
        {
            var nome = new string('A', 51);

            Action action = () => new Cartao(
                nome,
                "cartao-roxo",
                10,
                17,
                CriarUsuario());

            action.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage(
                    "O nome do cartão deve ter no máximo 30 caracteres.");
        }

        [Fact]
        public void Update_DadosValidos_NaoDeveLancarExcecao()
        {
            var cartao = new Cartao(
                "Nubank",
                "cartao-roxo",
                10,
                17,
                CriarUsuario());

            Action action = () => cartao.Update(
                "Inter",
                "cartao-laranja",
                12,
                19);

            action.Should().NotThrow();
        }

        [Fact]
        public void Update_DadosInvalidos_DeveLancarExcecao()
        {
            var cartao = new Cartao(
                "Nubank",
                "cartao-roxo",
                10,
                17,
                CriarUsuario());

            Action action = () => cartao.Update(
                "",
                "cartao-roxo",
                10,
                17);

            action.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("O nome do cartão é obrigatório.");
        }
    }
}
