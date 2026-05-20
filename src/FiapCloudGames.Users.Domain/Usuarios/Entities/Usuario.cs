using FiapCloudGames.Users.Domain.Core;
using FiapCloudGames.Users.Domain.Usuarios.Enums;
using FiapCloudGames.Users.Domain.Usuarios.ValueObjects;
using System.Text.RegularExpressions;

namespace FiapCloudGames.Users.Domain.Usuarios.Entities
{
    public class Usuario : EntityBase
    {
        public string Nome { get; private set; }
        public Email Email { get; private set; }
        public string SenhaHash { get; private set; }
        public TipoUsuario Tipo { get; private set; } = TipoUsuario.Comum;

        protected Usuario() { }
        public Usuario(string nome, Email email, string passwordHash)
        {
            Nome = nome;
            Email = email;
            SenhaHash = passwordHash;
        }

        /// <summary>
        /// Valida se a senha atende aos requisitos mínimos de complexidade.
        /// </summary>
        /// <param name="senha">Senha em texto puro a ser validada.</param>
        /// <remarks>
        /// Regras aplicadas:
        /// <list type="bullet">
        /// <item><description>Obrigatória (não nula, vazia ou composta apenas por espaços).</description></item>
        /// <item><description>Mínimo de 8 caracteres.</description></item>
        /// <item><description>Deve conter ao menos uma letra maiúscula (A-Z).</description></item>
        /// <item><description>Deve conter ao menos uma letra minúscula (a-z).</description></item>
        /// <item><description>Deve conter ao menos um número (0-9).</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// Lançada quando a senha não atende a qualquer uma das regras de validação.
        /// </exception>
        public static void ValidarSenha(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
                throw new ArgumentException("Senha é obrigatória.");

            if (senha.Length < 8)
                throw new ArgumentException("A senha deve ter no mínimo 8 caracteres.");

            if (!Regex.IsMatch(senha, "[A-Z]"))
                throw new ArgumentException("A senha deve conter ao menos uma letra maiúscula.");

            if (!Regex.IsMatch(senha, "[a-z]"))
                throw new ArgumentException("A senha deve conter ao menos uma letra minúscula.");

            if (!Regex.IsMatch(senha, "[0-9]"))
                throw new ArgumentException("A senha deve conter ao menos um número.");

            if (!Regex.IsMatch(senha, "[!@#$%^&*(),.?\":{}|<>]"))
                throw new ArgumentException("A senha deve conter ao menos um caractere especial.");

        }

        /// <summary>
        /// Promove o usuario para o tipo Admin, caso ele ainda seja do tipo Comum. 
        /// Se o usuário já for Admin, nenhuma ação é realizada.
        /// </summary>
        public void TornarAdmin()
        {
            if (Tipo == TipoUsuario.Admin)
                return;

            Tipo = TipoUsuario.Admin;
        }
    }
}
