// API/Middlewares/ExceptionMiddleware.cs
using ControleFinanceiro.Domain.Validation;
using System.Net;
using System.Text.Json;

namespace ControleFinanceiro.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string body = string.Empty;

            try
            {
                context.Request.Body.Position = 0;
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                await _next(context);
            }
            catch (DomainExceptionValidation ex)
            {
                _logger.LogWarning("Erro de validação {@Log}", new
                {
                    tipo = "ErroDominio",
                    rota = $"{context.Request.Method} {context.Request.Path}",
                    body = DeserializarBody(body),
                    erro = ex.Message,
                    timestamp = DateTime.UtcNow
                });
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Recurso não encontrado {@Log}", new
                {
                    tipo = "NaoEncontrado",
                    rota = $"{context.Request.Method} {context.Request.Path}",
                    body = DeserializarBody(body),
                    erro = ex.Message,
                    timestamp = DateTime.UtcNow
                });
                await HandleExceptionAsync(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (ForbiddenException ex)
            {
                _logger.LogWarning("Acesso proibido {@Log}", new
                {
                    tipo = "Forbidden",
                    rota = $"{context.Request.Method} {context.Request.Path}",
                    body = DeserializarBody(body),
                    erro = ex.Message,
                    timestamp = DateTime.UtcNow
                });

                await HandleExceptionAsync(context, HttpStatusCode.Forbidden, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Acesso não autorizado {@Log}", new
                {
                    tipo = "AcessoNaoAutorizado",
                    rota = $"{context.Request.Method} {context.Request.Path}",
                    body = DeserializarBody(body),
                    erro = ex.Message,
                    timestamp = DateTime.UtcNow
                });
                await HandleExceptionAsync(context, HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Operação inválida {@Log}", new
                {
                    tipo = "OperacaoInvalida",
                    rota = $"{context.Request.Method} {context.Request.Path}",
                    body = DeserializarBody(body),
                    erro = ex.Message,
                    timestamp = DateTime.UtcNow
                });
                await HandleExceptionAsync(context, HttpStatusCode.Conflict, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro interno {@Log}", new
                {
                    tipo = "ErroInterno",
                    rota = $"{context.Request.Method} {context.Request.Path}",
                    body = DeserializarBody(body),
                    erro = ex.Message,
                    timestamp = DateTime.UtcNow
                });
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "Erro interno no servidor.");
            }
        }

        private static string DeserializarBody(string body) => body;

        private static async Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string mensagem)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = JsonSerializer.Serialize(new { mensagem });
            await context.Response.WriteAsync(response);
        }
    }
}