using FiapCloudGames.Users.Application.Usuarios.Inputs;
using FiapCloudGames.Users.Application.Usuarios.Outputs;

namespace FiapCloudGames.Users.Application.Usuarios.Interfaces
{
    /// <summary>
    /// Define operações de aplicação relacionadas ao gerenciamento de usuários.
    /// </summary>
    public interface IUsuarioService
    {
        /// <summary>
        /// Cadastra um novo usuário a partir dos dados informados na entrada.
        /// </summary>
        /// <param name="input">Dados necessários para o cadastro do usuário.</param>
        /// <returns>O identificador único (GUID) do usuário cadastrado.</returns>
        Task<Guid> CadastrarUsuario(CadastroUsuarioInput input);

        /// <summary>
        /// Recebe as credenciais de login do usuário e retorna um token de autenticação se as credenciais forem válidas.
        /// </summary>
        /// <param name="input">Dados de entrada contendo o e-mail e a senha do usuário.</param>
        /// <returns>Um objeto <see cref="LoginOutput"/> contendo o token de autenticação.</returns>
        Task<LoginOutput> Login(LoginInput input);

        /// <summary>
        /// Torna um usuário existente um administrador, concedendo-lhe privilégios administrativos.
        /// </summary>
        /// <param name="usuarioId">O identificador único (GUID) do usuário a ser promovido.</param>
        /// <returns>Uma tarefa assíncrona representando a operação.</returns>
        Task TornarUsuarioAdmin(Guid usuarioId);
    }
}
