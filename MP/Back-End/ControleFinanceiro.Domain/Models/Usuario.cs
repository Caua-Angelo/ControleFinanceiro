using ControleFinanceiro.Domain.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace ControleFinanceiro.Domain.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Nome é obrigatório")]
        [Column(TypeName = "text")]
        public string sNome { get; set; } = null!;
        [Column()]
        public int EquipeId { get; set; }

        public Equipe? Equipe { get; set; }
        
        public ICollection<Ferias>? Ferias { get; set; }

        public Usuario() { }
        public Usuario(string sNome, int EquipeId)
        {
            ValidateDomain(sNome, EquipeId);
        }
        public void Update(string snome, int equipeid)
        {
            ValidateDomain(snome, equipeid);
        }
        private void ValidateDomain(string snome, int equipeid)
        {
            snome = snome?.Trim() ?? string.Empty;

            DomainExceptionValidation.When((string.IsNullOrEmpty(snome) && equipeid <= 0), "sNome e equipeId precisam ser preenchidos");

            DomainExceptionValidation.When(string.IsNullOrEmpty(snome), "O nome do Usuario precisa ser preenchido.");

            DomainExceptionValidation.When(snome.Length < 3 || snome.Length > 44, "O nome do Usuario deve ter entre 3 e 44 caracteres.");

            DomainExceptionValidation.When(!Regex.IsMatch(snome, @"^[\p{L}\s]+$"), "O nome do Usuario deve conter apenas letras.");

            DomainExceptionValidation.When(equipeid <= 0 || equipeid > 100, "Id de equipe inválido,Deve ser maior que 0 e menor que 100");


            this.sNome = snome;
            this.EquipeId = equipeid;
        }
    }
}