using ControleFerias.Domain.Enums;
using ControleFerias.Domain.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;



namespace ControleFerias.Domain.Models
{
    public class Ferias
    {
        [Key]
        public int Id { get; set; }

        [Required, JsonConverter(typeof(DateTimeJsonConverter))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dDataInicio { get; set; }
       
        public int sDias { get; set; }

        [Required, JsonConverter(typeof(DateTimeJsonConverter))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dDataFinal { get; set; }

        public StatusFerias Status { get; set; } = StatusFerias.Pendente;
        public string? sComentario { get; set; }


        public ICollection<ColaboradorFerias>? ColaboradorFerias { get; set; }
        public ICollection<Colaborador>? Colaborador { get; set; }

        public Ferias() { }
        public Ferias(DateTime ddatainicio, int dias, DateTime ddatafinal, string comentario, int colaboradorId)
        {
            ValidateDomainCriar(ddatainicio, dias, ddatafinal, StatusFerias.Pendente, comentario, colaboradorId);
        }
        //public Ferias(DateTime ddataInicio, DateTime ddatafinal, StatusFerias status, string scomentario)
        //{
        //    ValidateDomain(ddataInicio, ddatafinal, status, scomentario);
        //}

        public void UpdateCriar(DateTime ddataInicio, DateTime ddatafinal,int dias = 0, StatusFerias status = StatusFerias.Pendente, string comentario = "", int colaboradorId = 0)
        {
            ValidateDomainCriar(ddataInicio, dias, ddatafinal, status, comentario, colaboradorId);
        }
        public void UpdateAlterar(DateTime ddataInicio, DateTime ddatafinal,int dias, StatusFerias status, string scomentario)
        {
            ValidateDomainAlterar(ddataInicio, ddatafinal,dias, status, scomentario);
        }
        private void ValidateDomainCriar(DateTime ddatainicio, int dias, DateTime ddatafinal, StatusFerias status, string scomentario, int colaboradorId)
        {
            scomentario = scomentario?.Trim() ?? string.Empty;


            DomainExceptionValidation.When((ddatafinal <= ddatainicio), "A data final deve ser maior que a data inicial ");

            DomainExceptionValidation.When((ddatainicio <= DateTime.Today), "A data inicial deve ser maior que a data atual ");

            DomainExceptionValidation.When((dias <= 0), "A quantidade de dias deve ser maior que zero.");
            

            DomainExceptionValidation.When(dias != (ddatafinal - ddatainicio).Days, "A quantidade de dias não corresponde ao período entre data de início e final.");

            DomainExceptionValidation.When(status != StatusFerias.Pendente, "O status inicial de uma nova férias deve ser Pendente.");

            DomainExceptionValidation.When(scomentario.Length > 200, "O comentário não pode ter mais de 200 caracteres.");

            DomainExceptionValidation.When(ddatainicio.DayOfWeek == DayOfWeek.Saturday || ddatainicio.DayOfWeek == DayOfWeek.Sunday, "A data inicial não pode ser sábado ou domingo.");

            this.dDataInicio = ddatainicio;
            this.sDias = dias;
            this.dDataFinal = ddatafinal;
            this.sComentario = scomentario?.Trim() ?? string.Empty;
        }
        private void ValidateDomainAlterar(DateTime ddatainicio, DateTime ddatafinal, int dias, StatusFerias status, string scomentario)
        {
            scomentario = scomentario?.Trim() ?? string.Empty;

            DomainExceptionValidation.When((ddatafinal <= ddatainicio), "A data final deve ser maior que a data inicial ");

            DomainExceptionValidation.When((ddatainicio <= DateTime.Today), "A data inicial deve ser maior que a data atual ");

            DomainExceptionValidation.When((dias <= 0), "A quantidade de dias deve ser maior que zero.");

            DomainExceptionValidation.When(dias != (ddatafinal - ddatainicio).Days, "A quantidade de dias não corresponde ao período entre data de início e final.");

            DomainExceptionValidation.When(scomentario.Length > 200, "O comentário não pode ter mais de 200 caracteres.");

            DomainExceptionValidation.When(ddatainicio.DayOfWeek == DayOfWeek.Saturday || ddatainicio.DayOfWeek == DayOfWeek.Sunday, "A data inicial não pode ser sábado ou domingo.");
            // Aqui você pode atribuir apenas as propriedades que estão sendo alteradas:
            this.dDataInicio = ddatainicio;
            this.dDataFinal = ddatafinal;
            this.Status = status;
            this.sComentario = scomentario;
        }

        //Alterar o formato de data para dd/MM/yyyy
        public class DateTimeJsonConverter : JsonConverter<DateTime>
        {
            private readonly string _format = "dd/MM/yyyy";

            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return DateTime.ParseExact(reader.GetString()!, _format, null);
            }
            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString(_format));
            }
        }
    }
}
