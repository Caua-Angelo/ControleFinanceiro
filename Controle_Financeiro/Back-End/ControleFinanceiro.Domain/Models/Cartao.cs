
using ControleFinanceiro.Domain.Validation;

namespace ControleFinanceiro.Domain.Models
{
    public class Cartao
    {
        public int Id { get; private set; }

        public string Nome { get; private set; } = null!;

        public string Imagem { get; private set; } = null!;

        public int DiaFechamento { get; private set; }

        public int DiaVencimento { get; private set; }

        public int UsuarioId { get; private set; }

        public Usuario Usuario { get; private set; } = null!;

        protected Cartao() { }

        public Cartao(
            string nome,
            string imagem,
            int diaFechamento,
            int diaVencimento,
            Usuario usuario)
        {
            ValidateDomain(nome, imagem, diaFechamento, diaVencimento, usuario);
        }

        public void Update(string nome ,string imagem,int diaFechamento ,int diaVencimento)
        {
            ValidateDomain(nome,imagem,diaFechamento,diaVencimento,Usuario);
        }

        private void ValidateDomain(
            string nome,
            string imagem,
            int diaFechamento,
            int diaVencimento,
            Usuario usuario)
        {
            nome = nome?.Trim() ?? string.Empty;
            imagem = imagem?.Trim() ?? string.Empty;

            DomainExceptionValidation.When(
                string.IsNullOrWhiteSpace(nome),
                "O nome do cartão é obrigatório.");

            DomainExceptionValidation.When(
                nome.Length > 30,
                "O nome do cartão deve ter no máximo 30 caracteres.");

            DomainExceptionValidation.When(
                string.IsNullOrWhiteSpace(imagem),
                "A imagem do cartão é obrigatória.");

            DomainExceptionValidation.When(
                diaFechamento < 1 || diaFechamento > 31,
                "O dia de fechamento deve estar entre 1 e 31.");

            DomainExceptionValidation.When(
                diaVencimento < 1 || diaVencimento > 31,
                "O dia de vencimento deve estar entre 1 e 31.");

            DomainExceptionValidation.When(
                usuario is null,
                "Usuário é obrigatório.");

            Nome = nome;
            Imagem = imagem;
            DiaFechamento = diaFechamento;
            DiaVencimento = diaVencimento;

            Usuario = usuario;
            UsuarioId = usuario.Id;
        }
    }
}
