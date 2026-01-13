using ControleFerias.Domain.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace ControleFerias.Domain.Models
{
    public class Equipe
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Nome da equipe é obrigatório")]
        [Column(TypeName = "text")]
        [MinLength(2)]
        public string sNome { get; set; } = null!;
        public ICollection<Colaborador>? Colaboradores { get; set; }

        public Equipe() { }
        public Equipe(string sNome)
        {
            ValidateDomain(sNome);
        }
        public void Update(string sNome)
        {
            ValidateDomain(sNome);
        }

        private void ValidateDomain(string snome)
        {
            snome = snome?.Trim() ?? string.Empty;

            DomainExceptionValidation.When(string.IsNullOrEmpty(snome), "O nome da equipe precisa ser  preenchid");
            DomainExceptionValidation.When(snome.Length < 2 || snome.Length > 30, "O nome da equipe deve ter entre 2 e 30 caracteres.");
            DomainExceptionValidation.When(!Regex.IsMatch(snome, @"^[\p{L}\s/]+$"), "O nome da equipe deve conter apenas letras.");

            this.sNome = snome;
        }

    }
}