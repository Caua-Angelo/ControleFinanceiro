using ControleFinanceiro.Domain.Enums;
using ControleFinanceiro.Domain.Models;
using ControleFinanceiro.Domain.Validation;
using FluentAssertions;

namespace ControleFinanceiro.Test.Unit.Domain
{
    public class ParcelaTests
    {
        private readonly Usuario _usuario;
        private readonly Cartao _cartao;
        private readonly Categoria _categoria;
        private readonly CompraParcelada _compra;
        private readonly DateOnly _vencimento = new(2026, 11, 10);

        public ParcelaTests()
        {
            _usuario = new Usuario(
                "Caua",
                new DateOnly(2000, 1, 1),
                "caua@gmail.com",
                "hash"
            );

            _cartao = new Cartao(
                "Cartao principal",
                "visa",
                15,
                10,
                _usuario
            );

            _categoria = new Categoria(
                "Eletronicos",
                FinalidadeCategoria.Despesa
            )
            {
                UsuarioId = _usuario.Id,
                Usuario = _usuario
            };

            _compra = new CompraParcelada(
                "Monitor novo",
                1200m,
                6,
                new DateOnly(2026, 10, 11),
                _cartao,
                _categoria,
                _usuario
            );
        }

        private Parcela CriarParcelaValida()
        {
            return new Parcela(
                1,
                200m,
                _vencimento,
                _compra
            );
        }

        [Fact]
        public void InstanciarParcela_DadosValidos_DeveAtribuirPropriedades()
        {
            var parcela = CriarParcelaValida();

            parcela.Numero.Should().Be(1);
            parcela.Valor.Should().Be(200m);
            parcela.DataVencimento.Should().Be(_vencimento);
            parcela.CompraParcelada.Should().BeSameAs(_compra);
            parcela.CompraParceladaId.Should().Be(_compra.Id);
            parcela.Paga.Should().BeFalse();
        }

        [Theory]
        [InlineData(0, 200, "O número da parcela deve ser maior que zero.")]
        [InlineData(-1, 200, "O número da parcela deve ser maior que zero.")]
        [InlineData(1, 0, "O valor da parcela deve ser maior que zero.")]
        [InlineData(1, -10, "O valor da parcela deve ser maior que zero.")]
        public void InstanciarParcela_DadosInvalidos_DeveLancarExcecao(
            int numero,
            decimal valor,
            string mensagemEsperada)
        {
            Action act = () => new Parcela(
                numero,
                valor,
                _vencimento,
                _compra
            );

            act.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage(mensagemEsperada);
        }

        [Fact]
        public void InstanciarParcela_DataDefault_DeveLancarExcecao()
        {
            Action act = () => new Parcela(
                1,
                200m,
                default,
                _compra
            );

            act.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("A data de vencimento é obrigatória.");
        }

        [Fact]
        public void InstanciarParcela_CompraNula_DeveLancarExcecao()
        {
            Action act = () => new Parcela(
                1,
                200m,
                _vencimento,
                null!
            );

            act.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("A compra parcelada é obrigatória.");
        }

        [Fact]
        public void MarcarComoPaga_ParcelaNaoPaga_DeveAlterarStatus()
        {
            var parcela = CriarParcelaValida();

            parcela.Paga.Should().BeFalse();

            parcela.MarcarComoPaga();

            parcela.Paga.Should().BeTrue();
        }

        [Fact]
        public void MarcarComoPaga_ParcelaJaPaga_DeveContinuarPaga()
        {
            var parcela = CriarParcelaValida();

            parcela.MarcarComoPaga();
            parcela.MarcarComoPaga();

            parcela.Paga.Should().BeTrue();
        }

        [Fact]
        public void Reabrir_ParcelaPaga_DeveAlterarStatusParaNaoPaga()
        {
            var parcela = CriarParcelaValida();

            parcela.MarcarComoPaga();
            parcela.Reabrir();

            parcela.Paga.Should().BeFalse();
        }

        [Fact]
        public void Reabrir_ParcelaJaAberta_DeveContinuarNaoPaga()
        {
            var parcela = CriarParcelaValida();

            parcela.Reabrir();

            parcela.Paga.Should().BeFalse();
        }
    }
}
