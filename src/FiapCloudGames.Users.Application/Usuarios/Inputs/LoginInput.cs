namespace FiapCloudGames.Users.Application.Usuarios.Inputs
{
    public class LoginInput
    {
        /// <summary>
        /// Email do usuário que está tentando se autenticar.
        /// </summary>
        public required string Email { get; set; }
        /// <summary>
        /// Senha em texto puro do usuário para autenticação.
        /// </summary>
        public required string Senha { get; set; }
    }
}
