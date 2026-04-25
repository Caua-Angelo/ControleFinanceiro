using ControleFinanceiro.Domain.Models;
using ControleFinanceiro.Domain.Validation;
using FluentAssertions;

namespace ControleFinanceiro.Test.Unit.Domain
{
    public class UsuarioTests
    {
        [Fact]
        public void IntanciarUsuario_Dadosvalidos_NãoDeveRetornarDomainExceptionValidation()
        {
            Action action = () => new Usuario("josé do pastel", 30, "email@teste.com", "hash");

            action.Should().NotThrow();
        }
        [Theory]
        [InlineData("", 18, "Caua@gmail.com", "AASSHG2G2HASH", "O nome do usuário precisa ser preenchido.")]
        [InlineData("Caua123", 18, "Caua@gmail.com", "AASSHG2G2HASH", "O nome do usuário deve conter apenas letras.")]
        [InlineData("Caua@", 18, "Caua@gmail.com", "AASSHG2G2HASH", "O nome do usuário deve conter apenas letras.")]
        [InlineData("Ca", 18, "Caua@gmail.com", "AASSHG2G2HASH", "O nome do usuário deve ter entre 3 e 60 caracteres.")]
        [InlineData("Caaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", 18, "Caua@gmail.com", "AASSHG2G2HASH", "O nome do usuário deve ter entre 3 e 60 caracteres.")]
        [InlineData("Caua", 0, "Caua@gmail.com", "AASSHG2G2HASH", "A idade deve ser um número inteiro positivo.")]
        [InlineData("Caua", -1, "Caua@gmail.com", "AASSHG2G2HASH", "A idade deve ser um número inteiro positivo.")]
        [InlineData("Caua", 18, "Caua@gmail.", "AASSHG2G2HASH", "O e-mail informado não é válido.")]
        public void IntanciarUsuario_DadosInvalidos_DeveRetornarDomainExceptionValidation(string nome, int idade, string email, string senhaHash, string mensagemEsperada)
        {
            Action action = () => new Usuario(nome, idade, email, senhaHash);
            action.Should().Throw<DomainExceptionValidation>()
                .WithMessage(mensagemEsperada);
        }
        [Fact]
        public void Update_DadosValidos_NaoDeveLancarExcecao()
        {
            var usuario = new Usuario("Caua", 18, "caua@gmail.com", "hash");
            Action act = () => usuario.Update("Caua Silva", 19);
            act.Should().NotThrow();
        }
        [Fact]
        public void Update_DadosInvalidos_DeveLancarExcecao()
        {
            var usuario = new Usuario("Caua", 18, "caua@gmail.com", "hash");
            Action act = () => usuario.Update("", 19);
            act.Should().Throw<DomainExceptionValidation>()
               .WithMessage("O nome do usuário precisa ser preenchido.");
        }
        [Fact]
        public void AlterarSenha_SenhaValida_NaoDeveLancarExcecao()
        {
            var usuario = new Usuario("Caua", 18, "caua@gmail.com", "hash");
            Action act = () => usuario.AlterarSenha("novoHash");
            act.Should().NotThrow();
        }
        [Fact]
        public void AlterarSenha_SenhaVazia_DeveLancarExcecao()
        {
            var usuario = new Usuario("Caua", 18, "caua@gmail.com", "hash");
            Action act = () => usuario.AlterarSenha("");
            act.Should().Throw<DomainExceptionValidation>()
               .WithMessage("A senha não pode ser vazia.");
        }
    }
}