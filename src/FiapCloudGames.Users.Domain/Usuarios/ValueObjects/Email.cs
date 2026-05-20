using System.Text.RegularExpressions;

namespace FiapCloudGames.Users.Domain.Usuarios.ValueObjects
{
    /// <summary>
    /// Representa um e-mail como um Value Object, garantindo validação e normalização do endereço.
    /// </summary>
    /// <remarks>
    /// Regras aplicadas:
    /// <list type="bullet">
    /// <item><description>Não permite valor nulo ou vazio.</description></item>
    /// <item><description>Valida o formato básico do e-mail via expressão regular.</description></item>
    /// <item><description>Normaliza removendo espaços nas extremidades e convertendo para minúsculas.</description></item>
    /// </list>
    /// </remarks>
    public class Email
    {
        private static readonly Regex EmailRegex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Endereco { get; private set; }

        public Email(string endereco)
        {
            if (string.IsNullOrEmpty(endereco))
            {
                throw new ArgumentException("Endereço de Email não pode ser vazio.", nameof(endereco));
            }

            if (!EmailRegex.IsMatch(endereco))
            {
                throw new ArgumentException("Endereço de Email inválido.", nameof(endereco));
            }

            Endereco = endereco.Trim().ToLower();
        }
    }
}
