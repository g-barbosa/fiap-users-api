using FiapCloudGames.Users.Domain.Usuarios.Entities;

namespace FiapCloudGames.Users.Domain.Usuarios.Interfaces
{
    /// <summary>
    /// Define regras e operações de domínio para criação e manipulação de <see cref="Usuario"/>.
    /// </summary>
    public interface IUsuarioDomainService
    {
        /// <summary>
        /// Cria um novo <see cref="Usuario"/> aplicando regras de domínio necessárias.
        /// </summary>
        /// <param name="nome">Nome do usuário.</param>
        /// <param name="email">E-mail do usuário.</param>
        /// <param name="senhaHash">Hash da senha já processada (não deve ser a senha em texto puro).</param>
        /// <returns>O <see cref="Usuario"/> criado.</returns>
        Task<Usuario> CriarAsync(string nome, string email, string senhaHash);
    }
}
