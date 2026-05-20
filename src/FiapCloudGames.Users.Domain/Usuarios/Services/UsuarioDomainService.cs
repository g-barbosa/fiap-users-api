using FiapCloudGames.Users.Domain.Usuarios.Entities;
using FiapCloudGames.Users.Domain.Usuarios.Interfaces;
using FiapCloudGames.Users.Domain.Usuarios.ValueObjects;

namespace FiapCloudGames.Users.Domain.Usuarios.Services
{
    /// <summary>
    /// Implementa o serviço de domínio responsável por encapsular regras de negócio na criação de <see cref="Usuario"/>.
    /// </summary>
    /// <remarks>
    /// Serviços de domínio são utilizados quando uma regra não se encaixa naturalmente em uma única entidade
    /// ou value object, mantendo a lógica centralizada no domínio.
    /// </remarks>
    public class UsuarioDomainService(IUsuarioRepository usuarioRepository) : IUsuarioDomainService
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

        /// <inheritdoc />
        public async Task<Usuario> CriarAsync(string nome, string email, string senhaHash)
        {
            var emailValueObject = new Email(email);

            if (await _usuarioRepository.EmailExisteAsync(email))
                throw new Exception("Email já cadastrado");

            return new Usuario(nome, emailValueObject, senhaHash);
        }
    }
}
