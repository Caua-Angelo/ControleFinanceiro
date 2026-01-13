using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ControleFerias.Application.DTO
{
    public class ColaboradorIncluirDTO
    {
        [SwaggerSchema(Description = "Nome do colaborador (mínimo 3 e máximo 32 caracteres)")]
        [Required(ErrorMessage = "O nome do colaborador precisa ser preenchido.")]
        [StringLength(32, MinimumLength = 3, ErrorMessage = "O nome do colaborador deve ter entre 3 e 32 caracteres.")]
        public string sNome { get; set; } = null!;

        [SwaggerSchema(Description = "Id da equipe à qual o colaborador pertence. Deve ser maior que 0.")]
        [Range(1, int.MaxValue, ErrorMessage = "O identificador da equipe deve ser maior que zero.")]
        public int EquipeId { get; set; }
    }
}
