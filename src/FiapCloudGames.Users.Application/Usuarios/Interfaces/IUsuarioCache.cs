using FiapCloudGames.Users.Domain.Usuarios.Entities;

namespace FiapCloudGames.Users.Application.Usuarios.Interfaces
{
    /// <summary>
    /// Interface para cache de usuários (sessões e dados frequentes).
    /// Deve ser implementada pela camada de Infrastructure.
    /// </summary>
    public interface IUsuarioCache
    {
        /// <summary>
        /// Obter usuário do cache por ID.
        /// </summary>
        /// <param name="usuarioId">ID do usuário</param>
        /// <returns>Usuário em cache ou null se não encontrado</returns>
        Task<Usuario?> ObterPorIdAsync(Guid usuarioId);

        /// <summary>
        /// Armazenar usuário no cache.
        /// </summary>
        /// <param name="usuario">Usuário para cache</param>
        /// <param name="ttlSeconds">Tempo de expiração em segundos (padrão: 300s)</param>
        Task AdicionarAsync(Usuario usuario, int ttlSeconds = 300);

        /// <summary>
        /// Remover usuário do cache.
        /// </summary>
        /// <param name="usuarioId">ID do usuário</param>
        Task RemoverAsync(Guid usuarioId);

        /// <summary>
        /// Obter email do cache (para validação de duplicatas).
        /// </summary>
        /// <param name="email">Email do usuário</param>
        /// <returns>ID do usuário se em cache, null caso contrário</returns>
        Task<Guid?> ObterIdPorEmailAsync(string email);

        /// <summary>
        /// Armazenar email no cache.
        /// </summary>
        /// <param name="email">Email do usuário</param>
        /// <param name="usuarioId">ID do usuário</param>
        Task AdicionarEmailAsync(string email, Guid usuarioId, int ttlSeconds = 300);

        /// <summary>
        /// Remover email do cache.
        /// </summary>
        /// <param name="email">Email do usuário</param>
        Task RemoverEmailAsync(string email);
    }
}
