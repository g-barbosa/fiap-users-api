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
    /// para executar operações como cadastro de usuários, com cache distribuído de usuários.
    /// </remarks>
    public class UsuarioService(
        IUsuarioRepository usuarioRepository, 
        IUsuarioDomainService usuarioDomainService, 
        ITokenService tokenService, 
        IUsuarioEventPublisher usuarioEventPublisher,
        IUsuarioCache usuarioCache) : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IUsuarioDomainService _usuarioDomainService = usuarioDomainService;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IUsuarioEventPublisher _usuarioEventPublisher = usuarioEventPublisher;
        private readonly IUsuarioCache _usuarioCache = usuarioCache;

        /// <inheritdoc />
        public async Task<Guid> CadastrarUsuario(CadastroUsuarioInput input)
        {
            Usuario.ValidarSenha(input.Senha);

            string senhaHash = BCrypt.Net.BCrypt.HashPassword(input.Senha);

            Usuario usuario = await _usuarioDomainService.CriarAsync(input.Nome, input.Email, senhaHash);

            await _usuarioRepository.AdicionarAsync(usuario);

            // Cachear email para validação rápida de duplicatas
            await _usuarioCache.AdicionarEmailAsync(usuario.Email.Endereco, usuario.Id);

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
            // Tenta obter do cache primeiro
            var usuarioIdEmCache = await _usuarioCache.ObterIdPorEmailAsync(input.Email);
            Usuario? usuario = null;

            if (usuarioIdEmCache.HasValue)
            {
                // Se encontrou email no cache, tenta obter dados do usuário
                usuario = await _usuarioCache.ObterPorIdAsync(usuarioIdEmCache.Value);
            }

            // Se não encontrou no cache, busca no DB
            if (usuario == null)
            {
                usuario = await _usuarioRepository.ObterPorEmailAsync(input.Email);
                
                if (usuario != null)
                {
                    // Cachear para próximas requisições
                    await _usuarioCache.AdicionarAsync(usuario);
                    await _usuarioCache.AdicionarEmailAsync(usuario.Email.Endereco, usuario.Id);
                }
            }

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

            // Invalidar cache após atualização
            await _usuarioCache.RemoverAsync(usuarioId);
        }
    }
}
