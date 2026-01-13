using System.Text.Json.Serialization;

namespace ControleFerias.Domain.Models
{
	public class ColaboradorFerias
	{
		public int ColaboradorId { get; set; }
		public Colaborador? Colaborador { get; set; }
		public int FeriasId { get; set; }
		public Ferias? Ferias { get; set; }
	}
}
