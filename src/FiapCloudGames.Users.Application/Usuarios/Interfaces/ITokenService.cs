using FiapCloudGames.Users.Domain.Usuarios.Entities;

namespace FiapCloudGames.Users.Application.Usuarios.Interfaces
{
    public interface ITokenService
    {
        /// <summary>
        /// Gera um token de autenticação para o usuário especificado.
        /// </summary>
        /// <param name="usuario">O usuário para o qual o token será gerado.</param>
        /// <returns>O token de autenticação gerado.</returns>
        string GerarToken(Usuario usuario);
    }
}
