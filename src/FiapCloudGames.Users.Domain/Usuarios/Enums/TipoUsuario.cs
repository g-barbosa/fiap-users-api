namespace FiapCloudGames.Users.Domain.Usuarios.Enums
{
    /// <summary>
    /// Define os tipos (perfis) de usuário suportados pelo domínio.
    /// </summary>
    public enum TipoUsuario
    {
        /// <summary>
        /// Usuário padrão, sem privilégios administrativos.
        /// </summary>
        Comum = 0,

        /// <summary>
        /// Usuário com privilégios administrativos.
        /// </summary>
        Admin = 1
    }
}
