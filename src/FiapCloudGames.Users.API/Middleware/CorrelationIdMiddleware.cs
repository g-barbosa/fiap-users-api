namespace FiapCloudGames.Users.API.Middleware
{
    /// <summary>
    /// Middleware para gerenciar CorrelationId.
    /// </summary>
    public class CorrelationIdMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;
        private const string CorrelationIdHeader = "X-Correlation-ID";
        private const int MaxCorrelationIdLength = 64;

        /// <summary>
        /// Valida se um caractere é permitido em um CorrelationId, ou seja, 
        /// se é alfanumérico ou um dos caracteres '-' ou '_'.
        /// </summary>
        /// <param name="c">O caractere a ser validado.</param>
        /// <returns>True se o caractere for permitido, caso contrário, false.</returns>
        private static bool IsAllowedCorrelationIdChar(char c)
        {
            return char.IsLetterOrDigit(c) || c == '-' || c == '_';
        }

        /// <summary>
        /// Processa a requisição para garantir que um CorrelationId válido esteja presente, 
        /// seja vindo do header ou gerado, e o disponibiliza no contexto para uso posterior.
        /// </summary>
        /// <param name="context">O contexto HTTP da requisição.</param>
        /// <returns>Uma task representando a operação assíncrona.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var headerValue = context.Request.Headers[CorrelationIdHeader].FirstOrDefault();

            string correlationId;
            if (string.IsNullOrEmpty(headerValue)
                || headerValue.Length > MaxCorrelationIdLength
                || !headerValue.All(IsAllowedCorrelationIdChar))
            {
                correlationId = Guid.NewGuid().ToString("N")[..12];
            }
            else
            {
                correlationId = headerValue;
            }

            context.Items["CorrelationId"] = correlationId;
            
            context.Response.Headers[CorrelationIdHeader] = correlationId;

            await _next(context);
        }
    }
}