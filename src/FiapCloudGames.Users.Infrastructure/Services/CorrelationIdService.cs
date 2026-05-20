using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace FiapCloudGames.Users.Infrastructure.Services
{
    public interface ICorrelationIdService
    {
        string GetCorrelationId();
    }

    [ExcludeFromCodeCoverage]
    public class CorrelationIdService(IHttpContextAccessor httpContextAccessor) : ICorrelationIdService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public string GetCorrelationId()
        {
            return _httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString() 
                   ?? Guid.NewGuid().ToString("N")[..12];
        }
    }
}