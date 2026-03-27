using ControleFinanceiro.Domain.Enums;
using ControleFinanceiro.Domain.Models;
using ControleFinanceiro.Domain.Validation;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleFinanceiro.Test.Unit.Domain
{
    public class CategoriaTests
    {
        [Theory]
        [InlineData(FinalidadeCategoria.Despesa)]
        [InlineData(FinalidadeCategoria.Receita)]
        [InlineData(FinalidadeCategoria.Ambas)]
        public void IntanciarCategoria_Dadosvalidos_NãoDeveRetornarDomainExceptionValidation(FinalidadeCategoria finalidade)
        {
            Action action = () => new Categoria("Descrição", finalidade);

            action.Should().NotThrow();
        }
        [Theory]
        [InlineData(null, FinalidadeCategoria.Despesa, "A descrição da categoria precisa ser preenchida.")]
        [InlineData("", FinalidadeCategoria.Despesa, "A descrição da categoria precisa ser preenchida.")]
        [InlineData("de", FinalidadeCategoria.Despesa, "A descrição da categoria deve ter entre 3 e 60 caracteres.")]
        [InlineData("Desccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc",FinalidadeCategoria.Despesa, "A descrição da categoria deve ter entre 3 e 60 caracteres.")]
        [InlineData("Categoria!", FinalidadeCategoria.Despesa, "A descrição da categoria contém caracteres inválidos.")]
        [InlineData("Categoria@", FinalidadeCategoria.Despesa, "A descrição da categoria contém caracteres inválidos.")]
        [InlineData("Descricao", (FinalidadeCategoria)99, "Finalidade da categoria inválida.")]
        public void IntanciarCategoria_DadosInvalidos_DeveRetornarDomainExceptionValidation(string descricao, FinalidadeCategoria finalidade, string mensagemEsperada)
        {
            Action action = () => new Categoria(descricao, finalidade);
            action.Should().Throw<DomainExceptionValidation>()
                .WithMessage(mensagemEsperada);
        }
        [Fact]
        public void Update_DadosValidos_NaoDeveLancarExcecao()
        {
            var categoria = new Categoria("Descricao", FinalidadeCategoria.Despesa);
            Action act = () => categoria.Update("Nova Descricao", FinalidadeCategoria.Receita);
            act.Should().NotThrow();
        }

        [Fact]
        public void Update_DadosInvalidos_DeveLancarExcecao()
        {
            var categoria = new Categoria("Descricao", FinalidadeCategoria.Despesa);
            Action act = () => categoria.Update("", FinalidadeCategoria.Despesa);
            act.Should().Throw<DomainExceptionValidation>()
               .WithMessage("A descrição da categoria precisa ser preenchida.");
        }










    }
}
