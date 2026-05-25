using ControleFinanceiro.Domain.Validation;
using System.Text.RegularExpressions;

namespace ControleFinanceiro.Domain.Models
{
    public class Usuario
    {
        public int Id { get; private set; }

        public string Nome { get; private set; } = null!;

        public DateOnly DataNascimento { get; private set; }

        public ICollection<Transacao> Transacao { get; private set; } = new List<Transacao>();

        public string Email { get; private set; } = null!;

        public string HashSenha { get; private set; } = null!;

        protected Usuario() { }

        public Usuario(string nome, DateOnly dataNascimento, string email, string senhaHash)
        {
            ValidateDomain(nome, dataNascimento, email);
            HashSenha = senhaHash;
        }

        public void Update(string nome, DateOnly dataNascimento)
        {
            ValidateDomain(nome, dataNascimento, Email);
        }
        public void AlterarSenha(string novaSenhaHash)
        {
            DomainExceptionValidation.When(string.IsNullOrEmpty(novaSenhaHash),
                "A senha não pode ser vazia.");

            HashSenha = novaSenhaHash;
        }


        private void ValidateDomain(string nome, DateOnly dataNascimento, string email)
        {
            nome = nome?.Trim() ?? string.Empty;

            DomainExceptionValidation.When(string.IsNullOrEmpty(nome),
                "O nome do usuário precisa ser preenchido.");

            DomainExceptionValidation.When(nome.Length < 3 || nome.Length > 60,
                "O nome do usuário deve ter entre 3 e 60 caracteres.");

            DomainExceptionValidation.When(!Regex.IsMatch(nome, @"^[\p{L}\s'-]+$"),
                "O nome do usuário contém caracteres inválidos.");

            DomainExceptionValidation.When(dataNascimento >= DateOnly.FromDateTime(DateTime.Today),
                "A data de nascimento não pode ser uma data futura.");

            email = email?.Trim() ?? string.Empty;

            DomainExceptionValidation.When(string.IsNullOrEmpty(email),
                "O e-mail precisa ser preenchido.");

            DomainExceptionValidation.When(!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"),
                "O e-mail informado não é válido.");

            Nome = nome;
            DataNascimento = dataNascimento;
            Email = email.ToLower();
        }
    }
}
