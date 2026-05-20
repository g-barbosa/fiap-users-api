using FiapCloudGames.Users.Domain.Usuarios.Entities;
using FiapCloudGames.Users.Domain.Usuarios.Interfaces;
using FiapCloudGames.Users.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace FiapCloudGames.Users.Infrastructure.Data.Persistence.Repositories
{
    /// <summary>
    /// Implementação de <see cref="IUsuarioRepository"/> baseada em <see cref="FiapCloudGamesDbContext"/>.
    /// </summary>
    /// <remarks>
    /// Inicializa uma nova instância de <see cref="UsuarioRepository"/>.
    /// </remarks>
    /// <param name="context">Contexto de persistência usado para operações com usuários.</param>
    /// <param name="logger">Logger para registrar eventos do repositório.</param>
    /// <param name="correlationIdService">Serviço para obter o CorrelationId da requisição.</param>
    
    [ExcludeFromCodeCoverage]
    public class UsuarioRepository(
        FiapCloudGamesDbContext context,
        ILogger<UsuarioRepository> logger,
        ICorrelationIdService correlationIdService) : IUsuarioRepository
    {
        private readonly FiapCloudGamesDbContext _context = context;
        private readonly ILogger<UsuarioRepository> _logger = logger;
        private readonly ICorrelationIdService _correlationIdService = correlationIdService;

        /// <inheritdoc />
        public async Task AdicionarAsync(Usuario usuario)
        {
            var correlationId = _correlationIdService.GetCorrelationId();
            
            _logger.LogInformation("Adicionando usuário - CorrelationId: {CorrelationId}, Email: {Email}", 
                correlationId, usuario.Email.Endereco);

            try
            {
                await _context.Usuarios.AddAsync(usuario);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuário adicionado com sucesso - CorrelationId: {CorrelationId}, UsuarioId: {UsuarioId}", 
                    correlationId, usuario.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar usuário - CorrelationId: {CorrelationId}, Email: {Email}", 
                    correlationId, usuario.Email.Endereco);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task AtualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<bool> EmailExisteAsync(string email)
        {
            var correlationId = _correlationIdService.GetCorrelationId();
            
            _logger.LogDebug("Verificando email - CorrelationId: {CorrelationId}, Email: {Email}", 
                correlationId, email);

            try
            {
                var existe = await _context.Usuarios.AnyAsync(u => u.Email.Endereco == email);
                
                _logger.LogDebug("Email verificado - CorrelationId: {CorrelationId}, Email: {Email}, Existe: {Existe}", 
                    correlationId, email, existe);
                
                return existe;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar email - CorrelationId: {CorrelationId}, Email: {Email}", 
                    correlationId, email);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.Endereco == email);
        }

        /// <inheritdoc />
        public async Task<Usuario?> ObterPorIdAsync(Guid id)
        {
            return await _context.Usuarios.FindAsync(id);
        }
    }
}
