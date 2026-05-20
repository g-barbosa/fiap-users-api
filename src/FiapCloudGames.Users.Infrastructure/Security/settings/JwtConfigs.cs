using System.Diagnostics.CodeAnalysis;

namespace FiapCloudGames.Users.Infrastructure.Security.settings
{
    /// <summary>
    /// Representa as configurações de JWT utilizadas para emissão e validação de tokens.
    /// </summary>
    /// <remarks>
    /// Geralmente é carregada a partir de configuração (por exemplo, `appsettings.json`) via Options.
    /// </remarks>
    
    [ExcludeFromCodeCoverage]
    public class JwtConfigs
    {
        /// <summary>
        /// Chave secreta usada para assinar o token.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Emissor (Issuer) do token.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Público-alvo (Audience) aceito para o token.
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Tempo de expiração do token, em minutos.
        /// </summary>
        public int ExpiracaoMinutos { get; set; }
    }
}
