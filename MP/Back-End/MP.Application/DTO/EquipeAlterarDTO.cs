using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ControleFerias.Application.DTO
{
    public class EquipeAlterarDTO
    {
        [SwaggerSchema(Description = "Nome da equipe (mínimo 2 e máximo 32 caracteres)")]
        [Required(ErrorMessage = "O nome da equipe é obrigatório.")]
        [StringLength(32, MinimumLength = 2, ErrorMessage = "O nome da equipe deve ter entre 2 e 32 caracteres.")]
        public string sNome { get; set; } = null!;
    }
}
