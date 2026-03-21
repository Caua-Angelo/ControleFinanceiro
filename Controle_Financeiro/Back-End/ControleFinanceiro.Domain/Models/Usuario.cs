using ControleFinanceiro.Domain.Validation;
using System.Text.RegularExpressions;

namespace ControleFinanceiro.Domain.Models
{
    public class Usuario
    {
        public int Id { get; private set; }

        public string Nome { get; private set; } = null!;

        public int Idade { get; private set; }

        //  1 usuário possui várias transações
        public ICollection<Transacao> Transacao { get; private set; } = new List<Transacao>();

        public string Email { get; private set; } = null!;

        public string HashSenha { get; private set; } = null!;

        protected Usuario() { }

        public Usuario(string nome, int idade, string email)
        {
            ValidateDomain(nome, idade,email);
            HashSenha = HashSenha;
        }

        public void Update(string nome, int idade,string email)
        {
            ValidateDomain(nome, idade,email);
        }
        public void AlterarSenha(string novaSenhaHash)
        {
            DomainExceptionValidation.When(string.IsNullOrEmpty(novaSenhaHash),
                "A senha não pode ser vazia.");

            HashSenha = novaSenhaHash;
        }


        private void ValidateDomain(string nome, int idade,string email)
        {
            nome = nome?.Trim() ?? string.Empty;

            DomainExceptionValidation.When(string.IsNullOrEmpty(nome),
                "O nome do usuário precisa ser preenchido.");

            DomainExceptionValidation.When(nome.Length < 3 || nome.Length > 60,
                "O nome do usuário deve ter entre 3 e 60 caracteres.");

            DomainExceptionValidation.When(!Regex.IsMatch(nome, @"^[\p{L}\s]+$"),
                "O nome do usuário deve conter apenas letras.");

            DomainExceptionValidation.When(idade <= 0,
                "A idade deve ser um número inteiro positivo.");

            email = email?.Trim() ?? string.Empty;

            DomainExceptionValidation.When(string.IsNullOrEmpty(email),
                "O e-mail precisa ser preenchido.");

            DomainExceptionValidation.When(!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"),
                "O e-mail informado não é válido.");

            Nome = nome;
            Idade = idade;
            Email = email;
        }
    }
}
