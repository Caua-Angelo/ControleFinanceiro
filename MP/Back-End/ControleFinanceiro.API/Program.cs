
using ControleFinanceiro.API.Filters;
using ControleFinanceiro.Application.Common;
using ControleFinanceiro.Application.DTO;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Application.Services;
using ControleFinanceiro.Domain.Interfaces;
using ControleFinanceiro.Example;
using ControleFinanceiro.Infra.Data.Repositories;
using ControleFinanceiro.Infraestructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json.Serialization;
 


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

builder.Services.AddControllers(options =>
{
    options.Filters.Add<JsonAndModelExceptionFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Registrar os serviços da aplicação
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<ITransacaoService, TransacaoService>();

// Registrar repositórios
builder.Services.AddScoped<IColaboradorRepository, ColaboradorRepository>();
builder.Services.AddScoped<IEquipeRepository, CategoriaRepository>();
builder.Services.AddScoped<IFeriasRepository, TransacaoRepository>();


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.ExampleFilters();
    c.EnableAnnotations();
});


builder.Services.AddSwaggerExamplesFromAssemblyOf<FeriasIncluirDTO>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<ColaboradorAlterarDTOExample>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5174") // frontend
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Configuração de resposta customizada para erros de ModelState
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var request = context.HttpContext.Request;
        bool corpoVazio = request.ContentLength == null || request.ContentLength == 0;

        // Mapeia erros de validação
        var erros = context.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .Select(e =>
            {
                var mensagens = e.Value.Errors
                    .Select(err =>
                    {
                        var msg = err.ErrorMessage;

                        if (msg.Contains("could not be converted to System.Int32"))
                            return $"O campo '{e.Key}' deve conter apenas números.";

                        if (msg.Contains("is an invalid start of a value", StringComparison.OrdinalIgnoreCase) ||
                            msg.Contains("invalid character", StringComparison.OrdinalIgnoreCase))
                        {
                            if (e.Key.Contains("Dias", StringComparison.OrdinalIgnoreCase))
                                return $"O campo '{e.Key}' deve conter apenas números.";
                            if (e.Key.Contains("Data", StringComparison.OrdinalIgnoreCase))
                                return $"O campo '{e.Key}' deve conter uma data válida (ex: 05/11/2025).";
                            return $"O valor informado para o campo '{e.Key}' é inválido.";
                        }

                        if (msg.Contains("could not be converted to System.DateTime"))
                            return $"O campo '{e.Key}' deve conter uma data válida (formato: dd/MM/yyyy).";

                        if (msg.Contains("could not be converted to System.Boolean"))
                            return $"O campo '{e.Key}' deve ser verdadeiro ou falso (true/false).";

                        if (msg.Contains("The JSON value could not be converted"))
                            return $"Valor inválido para o campo '{e.Key}'.";

                        if (msg.Contains("A non-empty request body is required", StringComparison.OrdinalIgnoreCase))
                            return "O corpo da requisição não pode estar vazio.";

                        if (msg.Contains("is required", StringComparison.OrdinalIgnoreCase))
                            return "Este campo é obrigatório e não foi informado.";

                        return msg;
                    })
                    .ToList();

                return new CampoErro
                {
                    Campo = string.IsNullOrWhiteSpace(e.Key) ? "Corpo da requisição" : e.Key,
                    Mensagens = mensagens
                };
            })
            .ToList();

        string mensagemPrincipal;

        if (corpoVazio)
            mensagemPrincipal = "Nenhum corpo JSON foi enviado na requisição.";
        else if (!erros.Any())
            mensagemPrincipal = "O corpo da requisição está vazio ou contém campos inválidos.";
        else
            mensagemPrincipal = "Um ou mais campos estão ausentes ou inválidos.";

        var resposta = ApiResponse<List<CampoErro>>.Erro(
            mensagemPrincipal,
            erros,
            StatusCodes.Status400BadRequest
        );

        return new BadRequestObjectResult(resposta);
    };
});


builder.Host.UseSerilog(); // substitui o logger padrão

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console() // opcional: log no console
    .WriteTo.File(
        @"C:\MeusLogs\ferias-log-.txt",      // caminho no computador
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyyMMdd HH:mm:ss.fff} || {Message:lj} {NewLine}{Exception}" // <-- formato semelhante ao ServicoLog
    )
    .CreateLogger();

app.UseMiddleware<RequestCaptureMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();


app.Run();

public partial class Program { }
