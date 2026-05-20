using System.Security;
using System.Text.Json;

namespace FiapCloudGames.Users.API.Middleware
{
    /// <summary>
    /// Middleware para tratamento de erros.
    /// </summary>
    public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(context, ex);
            }
        }

        /// <summary>
        /// Centraliza o tratamento de erros, mapeando exceções comuns para códigos HTTP apropriados e registrando detalhes do erro,
        /// incluindo o CorrelationId para rastreamento. Responde com um JSON contendo a mensagem de erro e o CorrelationId.
        /// </summary>
        /// <param name="context">O contexto HTTP da requisição.</param>
        /// <param name="ex">A exceção lançada durante o processamento da requisição.</param>
        /// <returns>Uma task representando a operação assíncrona.</returns>
        private async Task HandleErrorAsync(HttpContext context, Exception ex)
        {
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? "unknown";

            _logger.LogError(ex, "Erro não tratado - CorrelationId: {CorrelationId}", correlationId);

            var (statusCode, message) = ex switch
            {
                ArgumentException => (400, "Dados inválidos"),
                KeyNotFoundException => (404, "Recurso não encontrado"),
                UnauthorizedAccessException => (401, "Acesso não autorizado"),
                SecurityException => (403, "Acesso negado"),
                _ => (500, "Erro interno")
            };

            var response = new
            {
                error = message,
                correlationId
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}