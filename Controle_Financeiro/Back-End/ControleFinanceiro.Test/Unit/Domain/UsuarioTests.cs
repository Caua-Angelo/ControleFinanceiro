using ControleFinanceiro.Domain.Models;
using ControleFinanceiro.Domain.Validation;
using FluentAssertions;

namespace ControleFinanceiro.Test.Unit.Domain
{
    public class UsuarioTests
    {
        [Fact]
        public void InstanciarUsuario_DadosValidos_NaoDeveRetornarDomainExceptionValidation()
        {
            Action action = () =>
                new Usuario(
                    "José do Pastel",
                    new DateTime(1995, 1, 1),
                    "email@teste.com",
                    "hash"
                );

            action.Should().NotThrow();
        }

        [Fact]
        public void InstanciarUsuario_NomeVazio_DeveRetornarExcecao()
        {
            Action action = () =>
                new Usuario(
                    "",
                    new DateTime(1995, 1, 1),
                    "email@teste.com",
                    "hash"
                );

            action.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("O nome do usuário precisa ser preenchido.");
        }

        [Fact]
        public void InstanciarUsuario_NomeComCaracteresInvalidos_DeveRetornarExcecao()
        {
            Action action = () =>
                new Usuario(
                    "Caua123",
                    new DateTime(1995, 1, 1),
                    "email@teste.com",
                    "hash"
                );

            action.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("O nome do usuário contém caracteres inválidos.");
        }

        [Fact]
        public void InstanciarUsuario_NomeMuitoCurto_DeveRetornarExcecao()
        {
            Action action = () =>
                new Usuario(
                    "Ca",
                    new DateTime(1995, 1, 1),
                    "email@teste.com",
                    "hash"
                );

            action.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("O nome do usuário deve ter entre 3 e 60 caracteres.");
        }

        [Fact]
        public void InstanciarUsuario_NomeMuitoGrande_DeveRetornarExcecao()
        {
            Action action = () =>
                new Usuario(
                    "Caaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                    new DateTime(1995, 1, 1),
                    "email@teste.com",
                    "hash"
                );

            action.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("O nome do usuário deve ter entre 3 e 60 caracteres.");
        }

        [Fact]
        public void InstanciarUsuario_DataNascimentoFutura_DeveRetornarExcecao()
        {
            var dataFutura = DateTime.Today.AddDays(1);

            Action action = () =>
                new Usuario(
                    "Caua",
                    dataFutura,
                    "email@teste.com",
                    "hash"
                );

            action.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("A data de nascimento não pode ser uma data futura.");
        }

        [Fact]
        public void InstanciarUsuario_EmailInvalido_DeveRetornarExcecao()
        {
            Action action = () =>
                new Usuario(
                    "Caua",
                    new DateTime(1995, 1, 1),
                    "email@teste.",
                    "hash"
                );

            action.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("O e-mail informado não é válido.");
        }

        [Fact]
        public void Update_DadosValidos_NaoDeveLancarExcecao()
        {
            var usuario = new Usuario(
                "Caua",
                new DateTime(1995, 1, 1),
                "caua@gmail.com",
                "hash"
            );

            Action act = () =>
                usuario.Update(
                    "Caua Silva",
                    new DateTime(1990, 1, 1)
                );

            act.Should().NotThrow();
        }

        [Fact]
        public void Update_DadosInvalidos_DeveLancarExcecao()
        {
            var usuario = new Usuario(
                "Caua",
                new DateTime(1995, 1, 1),
                "caua@gmail.com",
                "hash"
            );

            Action act = () =>
                usuario.Update(
                    "",
                    new DateTime(1990, 1, 1)
                );

            act.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("O nome do usuário precisa ser preenchido.");
        }

        [Fact]
        public void AlterarSenha_SenhaValida_NaoDeveLancarExcecao()
        {
            var usuario = new Usuario(
                "Caua",
                new DateTime(1995, 1, 1),
                "caua@gmail.com",
                "hash"
            );

            Action act = () => usuario.AlterarSenha("novoHash");

            act.Should().NotThrow();
        }

        [Fact]
        public void AlterarSenha_SenhaVazia_DeveLancarExcecao()
        {
            var usuario = new Usuario(
                "Caua",
                new DateTime(1995, 1, 1),
                "caua@gmail.com",
                "hash"
            );

            Action act = () => usuario.AlterarSenha("");

            act.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("A senha não pode ser vazia.");
        }
    }
}