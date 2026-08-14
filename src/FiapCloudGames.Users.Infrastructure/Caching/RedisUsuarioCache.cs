using FiapCloudGames.Users.Application.Usuarios.Interfaces;
using FiapCloudGames.Users.Domain.Usuarios.Entities;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FiapCloudGames.Users.Infrastructure.Caching
{
    /// <summary>
    /// Implementação de cache de usuários usando Redis.
    /// Armazena usuários e emails com TTL configurável.
    /// </summary>
    public class RedisUsuarioCache : IUsuarioCache
    {
        private readonly IDistributedCache _cache;
        private const string USUARIO_KEY_PREFIX = "usuario:";
        private const string EMAIL_KEY_PREFIX = "usuario:email:";

        public RedisUsuarioCache(IDistributedCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<Usuario?> ObterPorIdAsync(Guid usuarioId)
        {
            try
            {
                var key = $"{USUARIO_KEY_PREFIX}{usuarioId}";
                var json = await _cache.GetStringAsync(key);
                
                if (string.IsNullOrEmpty(json))
                    return null;

                return JsonSerializer.Deserialize<Usuario>(json);
            }
            catch
            {
                // Em caso de erro no cache, retorna null para fallback ao DB
                return null;
            }
        }

        public async Task AdicionarAsync(Usuario usuario, int ttlSeconds = 300)
        {
            try
            {
                var key = $"{USUARIO_KEY_PREFIX}{usuario.Id}";
                var json = JsonSerializer.Serialize(usuario);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttlSeconds)
                };

                await _cache.SetStringAsync(key, json, options);
            }
            catch
            {
                // Falhas no cache não devem impedir operação da aplicação
            }
        }

        public async Task RemoverAsync(Guid usuarioId)
        {
            try
            {
                var key = $"{USUARIO_KEY_PREFIX}{usuarioId}";
                await _cache.RemoveAsync(key);
            }
            catch
            {
                // Falhas no cache não devem impedir operação da aplicação
            }
        }

        public async Task<Guid?> ObterIdPorEmailAsync(string email)
        {
            try
            {
                var key = $"{EMAIL_KEY_PREFIX}{email.ToLower()}";
                var json = await _cache.GetStringAsync(key);
                
                if (string.IsNullOrEmpty(json))
                    return null;

                if (Guid.TryParse(json, out var usuarioId))
                    return usuarioId;

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task AdicionarEmailAsync(string email, Guid usuarioId, int ttlSeconds = 300)
        {
            try
            {
                var key = $"{EMAIL_KEY_PREFIX}{email.ToLower()}";
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttlSeconds)
                };

                await _cache.SetStringAsync(key, usuarioId.ToString(), options);
            }
            catch
            {
                // Falhas no cache não devem impedir operação da aplicação
            }
        }

        public async Task RemoverEmailAsync(string email)
        {
            try
            {
                var key = $"{EMAIL_KEY_PREFIX}{email.ToLower()}";
                await _cache.RemoveAsync(key);
            }
            catch
            {
                // Falhas no cache não devem impedir operação da aplicação
            }
        }
    }
}
