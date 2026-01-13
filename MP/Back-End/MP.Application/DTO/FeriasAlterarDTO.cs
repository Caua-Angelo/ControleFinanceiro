using ControleFerias.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ControleFerias.Application.DTO
{
    public class FeriasAlterarDTO
    {
        [SwaggerSchema(Description = "Data de início das férias (dd/MM/yyyy)", Format = "date")]
        [JsonConverter(typeof(DateFormatConverter))]
        [Required(ErrorMessage = "A data de início é obrigatória.")]
        public DateTime dDataInicio { get; set; }

        [SwaggerSchema(Description = "Quantidade de dias de férias")]
        [Required(ErrorMessage = "A quantidade de dias é obrigatória.")]
        [Range(5,30, ErrorMessage = "A quantidade de dias deve estar entre 5 e 30.")]
        public int sDias { get; set; }

        [SwaggerSchema(Description = "Data final das férias (dd/MM/yyyy)", Format = "date")]
        [JsonConverter(typeof(DateFormatConverter))]
        [Required(ErrorMessage = "A data final é obrigatória.")]
        public DateTime dDataFinal { get; set; }

        [SwaggerSchema(Description = "Comentário opcional sobre a alteração da solicitação de férias")]
        [MaxLength(200, ErrorMessage = "O comentário não pode ter mais de 200 caracteres.")]
        public string? sComentario { get; set; }

        [SwaggerSchema(Description = "Status da solicitação de férias (Pendente, Aprovado, Reprovado, etc.)")]
        [EnumDataType(typeof(StatusFerias), ErrorMessage = "O status informado é inválido.")]
        public StatusFerias status { get; set; }

        // Conversor fixo para dd/MM/yyyy
        public class DateFormatConverter : JsonConverter<DateTime>
        {
            private readonly string _format = "dd/MM/yyyy";

            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var value = reader.GetString();
                if (string.IsNullOrWhiteSpace(value))
                    throw new JsonException("O formato da data está vazio ou inválido. Use o formato dd/MM/yyyy.");

                if (!DateTime.TryParseExact(value, _format, null, System.Globalization.DateTimeStyles.None, out var date))
                    throw new JsonException($"Data inválida: '{value}'. Use o formato dd/MM/yyyy.");

                return date;
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
                writer.WriteStringValue(value.ToString(_format));
        }
    }
}
