
using ControleFerias.Domain.Models;
using ControleFerias.Domain.Validation;
using FluentAssertions;
using Xunit;

namespace ControleFerias.testes.UnitTests
{
    public class ColaboradorUnitTest
    {
        [Fact(DisplayName = "Criar Colaborador com Nome e equipeid nulos/vazios")]
        public void IncluirColaborador_NomeEquipeIdNuloVazio_DisparaDomainExceptionValidation()
        {
            Action action = () => new Colaborador(null!,0);
            action.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("sNome e equipeId precisam ser preenchidos");
        }
        [Fact(DisplayName = "Criar Colaborador com dados válidos")]
        public void IncluirColaborador_Parametrosvalidos_Objetovalido()
        {
            Action action = () => new Colaborador("Astolfo Borbato", 1);
            action.Should()
                .NotThrow<DomainExceptionValidation>();
        }
        [Fact(DisplayName = "Incluir Colaborador com nome menor que 3 caracteres")]
        public void IncluirColaborador_NomeCurto_DisparaDomainExceptionValidation()
        {
            Action action = () => new Colaborador("As",1);
            action.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("O nome do colaborador deve ter entre 3 e 44 caracteres.");
        }
        [Fact(DisplayName = "Incluir Colaborador com nome nulo ou vazio")]
        public void CreateCategory_NomeVazio_DomainExceptionRequiredName()
        {
            Action action = () => new Colaborador("", 1);
            action.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("O nome do colaborador precisa ser preenchido.");
        }
        [Fact(DisplayName = "Incluir Colaborador com simbolo no nome")]
        public void IncluirColaborador_NomeComSimbolos_DisparaDomainExceptionValidation()
        {
            Action action = () => new Colaborador("####", 1);
            action.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("O nome do colaborador deve conter apenas letras.");
        }
        [Fact(DisplayName = "Incluir Colaborador com número no nome")]
        public void IncluirColaborador_NomeComNumero_DisparaDomainExceptionValidation()
        {
            Action action = () => new Colaborador("11111aas", 1);
            action.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("O nome do colaborador deve conter apenas letras.");
        }
        [Fact(DisplayName = "Incluir Colaborador com id menor que 0")]
        public void IncluirColaborador_Id0_DisparaDomainExceptionValidation()
        {
            Action action = () => new Colaborador("john cena",0);
            action.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("Id de equipe inválido,Deve ser maior que 0 e menor que 100");
        }
        [Fact(DisplayName = "Incluir Colaborador com id menor que 0 ou maior que 100 caracteres")]
        public void IncluirColaborador_Idmaiorque100_DisparaDomainExceptionValidation()
        {
            Action action = () => new Colaborador("john cena",101);
            action.Should()
                .Throw<DomainExceptionValidation>()
                .WithMessage("Id de equipe inválido,Deve ser maior que 0 e menor que 100");
        }
    }
}

