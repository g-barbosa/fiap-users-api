using FiapCloudGames.Users.API.Middleware;
using System.Diagnostics.CodeAnalysis;

namespace FiapCloudGames.Users.API.Extensions
{
    /// <summary>
    /// Extensões para facilitar o registro de middlewares personalizados.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Registra o middleware de CorrelationId na pipeline da aplicação.
        /// </summary>
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CorrelationIdMiddleware>();
        }

        /// <summary>
        /// Registra o middleware de tratamento de erros na pipeline da aplicação.
        /// </summary>
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}