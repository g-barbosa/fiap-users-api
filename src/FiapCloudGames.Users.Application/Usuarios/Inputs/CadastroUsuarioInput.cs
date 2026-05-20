namespace FiapCloudGames.Users.Application.Usuarios.Inputs
{
    /// <summary>
    /// Modelo de entrada (DTO) para cadastro de usuário na aplicação.
    /// </summary>
    /// <remarks>
    /// Contém os dados necessários para criar um novo usuário, incluindo credenciais em texto puro
    /// (que devem ser tratadas de forma segura pela camada de aplicação/infraestrutura).
    /// </remarks>
    public class CadastroUsuarioInput
    {
        /// <summary>
        /// Nome do usuário.
        /// </summary>
        public required string Nome { get; set; }

        /// <summary>
        /// Endereço de e-mail do usuário.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// Senha em texto puro informada no momento do cadastro.
        /// </summary>
        public required string Senha { get; set; }
    }
}
