//us

using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ControleFerias.Application.DTO
{
    public class ColaboradorConsultarDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome precisa ser preenchido")]
        [SwaggerSchema(Description = "Nome do colaborador")]
        public string sNome { get; set; } = null!;

        [Required(ErrorMessage = "Id de equipe precisa ser preenchido")]
        [SwaggerSchema(Description = "Identificador da equipe")]
        public int EquipeId { get; set; }

        [SwaggerSchema(Description = "Nome da equipe")]
        public string? EquipeNome { get; set; } // nova propriedade
    }
}

