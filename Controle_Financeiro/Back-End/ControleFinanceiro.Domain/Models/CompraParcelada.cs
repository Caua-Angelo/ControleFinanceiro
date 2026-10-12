
using ControleFinanceiro.Domain.Validation;

namespace ControleFinanceiro.Domain.Models
{
    public class CompraParcelada
    {
        public int Id { get; private set; }

        public string Descricao { get; private set; } = null!;

        public decimal ValorTotal { get; private set; }

        public int QuantidadeParcelas { get; private set; }

        public DateOnly DataCompra { get; private set; }

        public int CartaoId { get; private set; }

        public Cartao Cartao { get; private set; } = null!;

        public int UsuarioId { get; private set; }

        public Usuario Usuario { get; private set; } = null!;

        public ICollection<Parcela> Parcelas { get; private set; }
            = new List<Parcela>();

        protected CompraParcelada() { }

        public CompraParcelada(
            string descricao,
            decimal valorTotal,
            int quantidadeParcelas,
            DateOnly dataCompra,
            Cartao cartao,
            Usuario usuario)
        {
            ValidateDomain(
                descricao,
                valorTotal,
                quantidadeParcelas,
                dataCompra,
                cartao,
                usuario);
        }

        public void AdicionarParcela(Parcela parcela)
        {
            DomainExceptionValidation.When(
                parcela is null,
                "A parcela é obrigatória."
            );

            Parcelas.Add(parcela);
        }
        public void Atualizar(string descricao,decimal valorTotal,int quantidadeParcelas,DateOnly dataCompra,Cartao cartao)
        {
            if (string.IsNullOrWhiteSpace(descricao) || descricao.Length > 100)
                throw new ArgumentException("A descrição deve ter entre 1 e 100 caracteres.");

            if (valorTotal <= 0)
                throw new ArgumentException("O valor total deve ser maior que zero.");

            if (quantidadeParcelas < 2 || quantidadeParcelas > 60)
                throw new ArgumentException("A quantidade de parcelas deve estar entre 2 e 60.");

            ArgumentNullException.ThrowIfNull(cartao);

            if (cartao.UsuarioId != UsuarioId)
                throw new InvalidOperationException("O cartão informado não pertence ao usuário.");

            Descricao = descricao;
            ValorTotal = valorTotal;
            QuantidadeParcelas = quantidadeParcelas;
            DataCompra = dataCompra;
            CartaoId = cartao.Id;
            Cartao = cartao;
        }

        private void ValidateDomain(
            string descricao,
            decimal valorTotal,
            int quantidadeParcelas,
            DateOnly dataCompra,
            Cartao cartao,
            Usuario usuario)
        {
            descricao = descricao?.Trim() ?? string.Empty;

            DomainExceptionValidation.When(
                string.IsNullOrWhiteSpace(descricao),
                "A descrição da compra é obrigatória.");

            DomainExceptionValidation.When(
                descricao.Length > 100,
                "A descrição deve ter no máximo 100 caracteres.");

            DomainExceptionValidation.When(
                valorTotal <= 0,
                "O valor total deve ser maior que zero.");

            DomainExceptionValidation.When(
                quantidadeParcelas < 2 || quantidadeParcelas > 60,
                "A quantidade de parcelas deve estar entre 2 e 60.");

            DomainExceptionValidation.When(
                dataCompra == default,
                "A data da compra é obrigatória.");

            DomainExceptionValidation.When(
                cartao is null,
                "O cartão é obrigatório.");

            DomainExceptionValidation.When(
                usuario is null,
                "O usuário é obrigatório.");

            DomainExceptionValidation.When(
                cartao is not null &&
                usuario is not null &&
                cartao.UsuarioId != usuario.Id,
                "O cartão informado não pertence ao usuário.");

            Descricao = descricao;
            ValorTotal = valorTotal;
            QuantidadeParcelas = quantidadeParcelas;
            DataCompra = dataCompra;

            Cartao = cartao;
            CartaoId = cartao.Id;

            Usuario = usuario;
            UsuarioId = usuario.Id;
        }
    }
}
