using ControleFinanceiro.Domain.Enums;
using ControleFinanceiro.Domain.Models;
using ControleFinanceiro.Domain.Validation;
using FluentAssertions;

namespace ControleFinanceiro.Test.Unit.Domain
{
    public class CompraParceladaTests
    {
        private readonly Usuario _usuarioValido;
        private readonly Cartao _cartaoValido;
        private readonly Categoria _categoriaValida;
        private readonly DateOnly _dataCompra = new(2026, 10, 11);

        public CompraParceladaTests()
        {
            _usuarioValido = new Usuario(
                "Caua",
                new DateOnly(2000, 1, 1),
                "caua@gmail.com",
                "hash"
            );

            _cartaoValido = new Cartao(
                "Cartao principal",
                "visa",
                15,
                10,
                _usuarioValido
            );

            _categoriaValida = new Categoria(
                "Eletronicos",
                FinalidadeCategoria.Despesa
            )
            {
                Usuario = _usuarioValido,
                UsuarioId = _usuarioValido.Id
            };
        }

        private CompraParcelada CriarCompraValida(
            decimal valorTotal = 1200m,
            int quantidadeParcelas = 6)
        {
            return new CompraParcelada(
                "Monitor novo",
                valorTotal,
                quantidadeParcelas,
                _dataCompra,
                _cartaoValido,
                _categoriaValida,
                _usuarioValido
            );
        }

        [Fact]
        public void InstanciarCompraParcelada_DadosValidos_NaoDeveLancarExcecao()
        {
            Action act = () => CriarCompraValida();

            act.Should().NotThrow();
        }

        [Fact]
        public void InstanciarCompraParcelada_DeveAtribuirPropriedadesCorretamente()
        {
            var compra = CriarCompraValida();

            compra.Descricao.Should().Be("Monitor novo");
            compra.ValorTotal.Should().Be(1200m);
            compra.QuantidadeParcelas.Should().Be(6);
            compra.DataCompra.Should().Be(_dataCompra);
            compra.CartaoId.Should().Be(_cartaoValido.Id);
            compra.CategoriaId.Should().Be(_categoriaValida.Id);
            compra.UsuarioId.Should().Be(_usuarioValido.Id);
            compra.Categoria.Should().BeSameAs(_categoriaValida);
            compra.Parcelas.Should().BeEmpty();
        }

        [Theory]
        [InlineData("", 1200, 6, "A descrição da compra é obrigatória.")]
        [InlineData("   ", 1200, 6, "A descrição da compra é obrigatória.")]
        [InlineData("Monitor novo", 0, 6, "O valor total deve ser maior que zero.")]
        [InlineData("Monitor novo", -1, 6, "O valor total deve ser maior que zero.")]
        [InlineData("Monitor novo", 1200, 1, "A quantidade de parcelas deve estar entre 2 e 60.")]
        [InlineData("Monitor novo", 1200, 61, "A quantidade de parcelas deve estar entre 2 e 60.")]
        public void InstanciarCompraParcelada_DadosInvalidos_DeveLancarExcecao(
            string descricao,
            decimal valorTotal,
            int quantidadeParcelas,
            string mensagemEsperada)
        {
            Action act = () => new CompraParcelada(
                descricao,
                valorTotal,
                quantidadeParcelas,
                _dataCompra,
                _cartaoValido,
                _categoriaValida,
                _usuarioValido
            );

            act.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage(mensagemEsperada);
        }

        [Fact]
        public void InstanciarCompraParcelada_DescricaoMaiorQue100Caracteres_DeveLancarExcecao()
        {
            var descricao = new string('A', 101);

            Action act = () => new CompraParcelada(
                descricao,
                1200,
                6,
                _dataCompra,
                _cartaoValido,
                _categoriaValida,
                _usuarioValido
            );

            act.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("A descrição deve ter no máximo 100 caracteres.");
        }

        [Fact]
        public void InstanciarCompraParcelada_DataDefault_DeveLancarExcecao()
        {
            Action act = () => new CompraParcelada(
                "Monitor novo",
                1200,
                6,
                default,
                _cartaoValido,
                _categoriaValida,
                _usuarioValido
            );

            act.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("A data da compra é obrigatória.");
        }

        [Fact]
        public void InstanciarCompraParcelada_CartaoNulo_DeveLancarExcecao()
        {
            Action act = () => new CompraParcelada(
                "Monitor novo",
                1200,
                6,
                _dataCompra,
                null!,
                _categoriaValida,
                _usuarioValido
            );

            act.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("O cartão é obrigatório.");
        }

        [Fact]
        public void InstanciarCompraParcelada_CategoriaNula_DeveLancarExcecao()
        {
            Action act = () => new CompraParcelada(
                "Monitor novo",
                1200,
                6,
                _dataCompra,
                _cartaoValido,
                null!,
                _usuarioValido
            );

            act.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("A categoria é obrigatória.");
        }

        [Fact]
        public void InstanciarCompraParcelada_UsuarioNulo_DeveLancarExcecao()
        {
            Action act = () => new CompraParcelada(
                "Monitor novo",
                1200,
                6,
                _dataCompra,
                _cartaoValido,
                _categoriaValida,
                null!
            );

            act.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("O usuário é obrigatório.");
        }

        [Fact]
        public void InstanciarCompraParcelada_CartaoDeOutroUsuario_DeveLancarExcecao()
        {
            var outroUsuario = new Usuario(
                "Outro Usuario",
                new DateOnly(1995, 5, 10),
                "outro@gmail.com",
                "hash"
            );

            var outroCartao = new Cartao(
                "Cartao de outro usuario",
                "mastercard",
                15,
                10,
                outroUsuario
            );

            typeof(Usuario)
                .GetProperty(nameof(Usuario.Id))!
                .SetValue(_usuarioValido, 1);

            typeof(Usuario)
                .GetProperty(nameof(Usuario.Id))!
                .SetValue(outroUsuario, 2);

            typeof(Cartao)
                .GetProperty(nameof(Cartao.UsuarioId))!
                .SetValue(outroCartao, 2);

            Action act = () => new CompraParcelada(
                "Monitor novo",
                1200m,
                6,
                _dataCompra,
                outroCartao,
                _categoriaValida,
                _usuarioValido
            );

            act.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("O cartão informado não pertence ao usuário.");
        }

        [Fact]
        public void AdicionarParcela_ParcelaValida_DeveAdicionarNaColecao()
        {
            var compra = CriarCompraValida();

            var parcela = new Parcela(
                1,
                200m,
                new DateOnly(2026, 11, 10),
                compra
            );

            compra.AdicionarParcela(parcela);

            compra.Parcelas.Should()
                .ContainSingle()
                .Which.Should()
                .BeSameAs(parcela);
        }

        [Fact]
        public void AdicionarParcela_ParcelaNula_DeveLancarExcecao()
        {
            var compra = CriarCompraValida();

            Action act = () => compra.AdicionarParcela(null!);

            act.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("A parcela é obrigatória.");
        }
    }
}
