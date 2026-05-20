namespace FiapCloudGames.Users.Application.Usuarios.Outputs
{
    /// <summary>
    /// Modelo de saída (DTO) retornado após autenticação bem-sucedida.
    /// </summary>
    public class LoginOutput
    {
        /// <summary>
        /// Token de autenticação (ex.: JWT) a ser utilizado nas chamadas subsequentes a endpoints protegidos.
        /// </summary>
        public required string Token { get; set; }
    }
}
