
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControleFerias.Application.DTO
{
    public class ColaboradorConsultarFeriasDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Nome é obrigatório")]
        [Column(TypeName = "text")]
        public string sNome { get; set; } = null!;
        public int EquipeId { get; set; }
        public string? EquipeNome { get; set; }

        public List<FeriasConsultarDTO> Ferias { get; set; } = new List<FeriasConsultarDTO>();



    }
}
