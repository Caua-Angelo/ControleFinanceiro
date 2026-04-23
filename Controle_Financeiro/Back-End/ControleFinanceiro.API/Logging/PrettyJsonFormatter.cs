using Serilog.Events;
using Serilog.Formatting;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ControleFinanceiro.API.Logging
{
    public class PrettyJsonFormatter : ITextFormatter
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public void Format(LogEvent logEvent, TextWriter output)
        {
            var temPropriedadesCustom = logEvent.Properties.ContainsKey("Log");

            var obj = new Dictionary<string, object?>
            {
                ["timestamp"] = logEvent.Timestamp.LocalDateTime.ToString("yyyy-MM-ddTHH:mm:sszzz"), 
                ["nivel"] = logEvent.Level.ToString()
            };

            if (!temPropriedadesCustom)
                obj["mensagem"] = logEvent.RenderMessage();

            foreach (var prop in logEvent.Properties)
            {
                if (prop.Key == "Log" && prop.Value is StructureValue sv)
                {
                    foreach(var p in sv.Properties)
{
                        if (p.Name == "timestamp") continue;

                        var valor = p.Value.ToString().Trim('"').Replace("\\\"", "\"");
                        if (p.Name == "body")
                        {
                            try
                            {
                                obj[p.Name] = JsonSerializer.Deserialize<JsonElement>(valor);
                            }
                            catch
                            {
                                obj[p.Name] = valor;
                            }
                        }
                        else
                        {
                            obj[p.Name] = valor;
                        }
                    }
                }
            }

            if (logEvent.Exception != null)
                obj["excecao"] = logEvent.Exception.Message;

            output.WriteLine(JsonSerializer.Serialize(obj, _options));
            output.WriteLine();
        }
    }
}