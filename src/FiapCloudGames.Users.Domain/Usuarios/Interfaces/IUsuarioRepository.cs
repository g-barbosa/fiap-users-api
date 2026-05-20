using FiapCloudGames.Users.Domain.Usuarios.Entities;

namespace FiapCloudGames.Users.Domain.Usuarios.Interfaces
{
    /// <summary>
    /// Define o contrato de persistência para a entidade <see cref="Usuario"/>.
    /// </summary>
    public interface IUsuarioRepository
    {
        /// <summary>
        /// Adiciona um novo usuário ao repositório.
        /// </summary>
        /// <param name="usuario">O usuário a ser adicionado.</param>
        /// <returns>Uma tarefa assíncrona.</returns>
        Task AdicionarAsync(Usuario usuario);

        /// <summary>
        /// Verifica se um e-mail já está associado a um usuário existente no repositório.
        /// </summary>
        /// <param name="email">O e-mail a ser verificado.</param>
        /// <returns>Retorna <c>true</c> se o e-mail já estiver em uso; caso contrário, <c>false</c>.</returns>
        Task<bool> EmailExisteAsync(string email);

        /// <summary>
        /// Obtém um usuário pelo seu e-mail. Retorna <c>null</c> se nenhum usuário for encontrado com o e-mail fornecido.
        /// </summary>
        /// <param name="email">O e-mail do usuário a ser obtido.</param>
        /// <returns>O usuário correspondente ao e-mail fornecido ou <c>null</c> se não encontrado.</returns>
        Task<Usuario?> ObterPorEmailAsync(string email);

        /// <summary>
        /// Obtém um usuário pelo seu identificador único (ID). Retorna <c>null</c> se nenhum usuário for encontrado com o ID fornecido.
        /// </summary>
        /// <param name="id">O identificador único (GUID) do usuário a ser obtido.</param>
        /// <returns>O usuário correspondente ao ID fornecido ou <c>null</c> se não encontrado.</returns>
        Task<Usuario?> ObterPorIdAsync(Guid id);

        /// <summary>
        /// Atualiza as informações de um usuário existente no repositório. O usuário a ser atualizado é identificado pelo seu ID.
        /// </summary>
        /// <param name="usuario">O usuário com as informações atualizadas.</param>
        /// <returns>Uma tarefa assíncrona.</returns>
        Task AtualizarAsync(Usuario usuario);
    }
}
