
using ControleFinanceiro.Domain.Validation;

namespace ControleFinanceiro.Domain.Models
{
    public class Parcela
    {
        public int Id { get; private set; }

        public int Numero { get; private set; }

        public decimal Valor { get; private set; }

        public DateOnly DataVencimento { get; private set; }

        public bool Paga { get; private set; }

        public int CompraParceladaId { get; private set; }

        public CompraParcelada CompraParcelada { get; private set; } = null!;

        protected Parcela() { }

        public Parcela(
            int numero,
            decimal valor,
            DateOnly dataVencimento,
            CompraParcelada compraParcelada)
        {
            ValidateDomain(numero, valor, dataVencimento, compraParcelada);
        }

        public void MarcarComoPaga()
        {
            Paga = true;
        }

        public void Reabrir()
        {
            Paga = false;
        }

        private void ValidateDomain(
            int numero,
            decimal valor,
            DateOnly dataVencimento,
            CompraParcelada compraParcelada)
        {
            DomainExceptionValidation.When(
                numero < 1,
                "O número da parcela deve ser maior que zero.");

            DomainExceptionValidation.When(
                valor <= 0,
                "O valor da parcela deve ser maior que zero.");

            DomainExceptionValidation.When(
                dataVencimento == default,
                "A data de vencimento é obrigatória.");

            DomainExceptionValidation.When(
                compraParcelada is null,
                "A compra parcelada é obrigatória.");

            Numero = numero;
            Valor = valor;
            DataVencimento = dataVencimento;

            CompraParcelada = compraParcelada;
            CompraParceladaId = compraParcelada.Id;
        }
    }
}
