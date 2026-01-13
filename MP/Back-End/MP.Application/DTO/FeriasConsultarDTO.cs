using ControleFerias.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ControleFerias.Application.DTO
{
    public class FeriasConsultarDTO
    {
        public int Id { get; set; }
        [SwaggerSchema(Description = "Data de início das férias (dd/MM/yyyy)", Format = "date")]
        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime dDataInicio { get; set; }

        public int sDias { get; set; }
        [SwaggerSchema(Description = "Data de início das férias (dd/MM/yyyy)", Format = "date")]

        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime dDataFinal { get; set; }

        public string? sComentario { get; set; }

        public int ColaboradorId { get; set; }

        public StatusFerias status { get; set; }

        // Conversor fixo para dd/MM/yyyy
        public class DateFormatConverter : JsonConverter<DateTime>
        {
            private readonly string _format = "dd/MM/yyyy";

            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
                DateTime.ParseExact(reader.GetString()!, _format, null);

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
                writer.WriteStringValue(value.ToString(_format));
        }
    }
}
