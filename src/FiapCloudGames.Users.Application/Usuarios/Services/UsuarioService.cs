using FiapCloudGames.Users.Application.Usuarios.Inputs;
using FiapCloudGames.Users.Application.Usuarios.Interfaces;
using FiapCloudGames.Users.Application.Usuarios.Interfaces.Messaging;
using FiapCloudGames.Users.Application.Usuarios.Outputs;
using FiapCloudGames.Users.Domain.Usuarios.Entities;
using FiapCloudGames.Users.Domain.Usuarios.Events;
using FiapCloudGames.Users.Domain.Usuarios.Interfaces;

namespace FiapCloudGames.Users.Application.Usuarios.Services
{
    /// <summary>
    /// Implementa os casos de uso relacionados ao gerenciamento de usuários na camada de aplicação.
    /// </summary>
    /// <remarks>
    /// Esta classe coordena validações e orquestra dependências (por exemplo, repositórios e serviços de segurança)
    /// para executar operações como cadastro de usuários.
    /// </remarks>
    public class UsuarioService(
        IUsuarioRepository usuarioRepository, 
        IUsuarioDomainService usuarioDomainService, 
        ITokenService tokenService, 
        IUsuarioEventPublisher usuarioEventPublisher) : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IUsuarioDomainService _usuarioDomainService = usuarioDomainService;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IUsuarioEventPublisher _usuarioEventPublisher = usuarioEventPublisher;

        /// <inheritdoc />
        public async Task<Guid> CadastrarUsuario(CadastroUsuarioInput input)
        {
            Usuario.ValidarSenha(input.Senha);

            string senhaHash = BCrypt.Net.BCrypt.HashPassword(input.Senha);

            Usuario usuario = await _usuarioDomainService.CriarAsync(input.Nome, input.Email, senhaHash);

            await _usuarioRepository.AdicionarAsync(usuario);

            await _usuarioEventPublisher.PublicarUsuarioCriadoAsync(new UsuarioCriadoEvent 
            { 
                Email = usuario.Email.Endereco,
                Id = usuario.Id,
                Nome = usuario.Nome
            });

            return usuario.Id;
        }

        /// <inheritdoc />
        public async Task<LoginOutput> Login(LoginInput input)
        {
            Usuario? usuario = await _usuarioRepository.ObterPorEmailAsync(input.Email);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(input.Senha, usuario.SenhaHash))
            {
                throw new UnauthorizedAccessException("Credenciais inválidas.");
            }

            string token = _tokenService.GerarToken(usuario);

            return new LoginOutput { Token = token };
        }

        /// <inheritdoc />
        public async Task TornarUsuarioAdmin(Guid usuarioId)
        {
            Usuario? usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId) 
                ?? throw new KeyNotFoundException("Usuário não encontrado.");
            
            usuario.TornarAdmin();

            await _usuarioRepository.AtualizarAsync(usuario);
        }
    }
}
