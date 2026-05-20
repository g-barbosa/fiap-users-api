using FiapCloudGames.Users.Application.Usuarios.Inputs;
using FiapCloudGames.Users.Application.Usuarios.Interfaces;
using FiapCloudGames.Users.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Users.API.Controllers
{
    /// <summary>
    /// Controller responsável por expor endpoints de gerenciamento de usuários.
    /// </summary>
    /// <remarks>
    /// Inicializa uma nova instância de <see cref="UsuariosController"/>.
    /// </remarks>
    /// <param name="usuarioService">Serviço de aplicação para operações de usuário.</param>
    /// <param name="correlationIdService">Serviço para obter o CorrelationId.</param>
    /// <param name="logger">Logger para registrar eventos.</param>
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController(
        IUsuarioService usuarioService,
        ICorrelationIdService correlationIdService,
        ILogger<UsuariosController> logger) : ControllerBase
    {
        private readonly IUsuarioService _usuarioService = usuarioService;
        private readonly ICorrelationIdService _correlationIdService = correlationIdService;
        private readonly ILogger<UsuariosController> _logger = logger;

        /// <summary>
        /// Cadastra um novo usuário.
        /// </summary>
        /// <param name="input">Dados necessários para o cadastro.</param>
        /// <returns>
        /// Retorna <see cref="StatusCodes.Status200OK"/> com o identificador do usuário criado
        /// ou um código de erro apropriado (por exemplo, <see cref="StatusCodes.Status400BadRequest"/> ou
        /// <see cref="StatusCodes.Status500InternalServerError"/>) conforme tratado pelo middleware global
        /// de tratamento de exceções.
        /// </returns>
        /// <response code="200">Usuário cadastrado com sucesso.</response>
        /// <response code="400">Requisição inválida ou erro de validação/negócio ao cadastrar usuário, conforme tratado pelo middleware global.</response>
        /// <response code="500">Erro interno ao cadastrar usuário, conforme tratado pelo middleware global.</response>
        [HttpPost]
        public async Task<IActionResult> CadastrarUsuario([FromBody] CadastroUsuarioInput input)
        {
            var correlationId = _correlationIdService.GetCorrelationId();
            
            _logger.LogInformation("Iniciando cadastro de usuário - CorrelationId: {CorrelationId}, Email: {Email}", 
                correlationId, input.Email);

            try
            {
                var usuarioId = await _usuarioService.CadastrarUsuario(input);
                
                _logger.LogInformation("Usuário cadastrado com sucesso - CorrelationId: {CorrelationId}, UsuarioId: {UsuarioId}", 
                    correlationId, usuarioId);

                return Ok(new { id = usuarioId, correlationId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar usuário - CorrelationId: {CorrelationId}, Email: {Email}", 
                    correlationId, input.Email);

                return BadRequest(new { error = ex.Message, correlationId });
            }
        }

        /// <summary>
        /// Autentica um usuário e retorna um token de acesso.
        /// </summary>
        /// <param name="input">Credenciais para autenticação.</param>
        /// <returns>
        /// Retorna <see cref="StatusCodes.Status200OK"/> com o token de autenticação em caso de sucesso,
        /// <see cref="StatusCodes.Status401Unauthorized"/> em caso de credenciais inválidas,
        /// ou <see cref="StatusCodes.Status400BadRequest"/> para outros erros de requisição.
        /// </returns>
        /// <response code="200">Login realizado com sucesso.</response>
        /// <response code="401">Credenciais inválidas.</response>
        /// <response code="400">Requisição inválida ou erro ao processar o login.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginInput input)
        {
            try
            {
                var resultado = await _usuarioService.Login(input);
                return Ok(resultado);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Concede o perfil de administrador para um usuário existente.
        /// </summary>
        /// <param name="usuarioId">Identificador do usuário que terá o perfil alterado.</param>
        /// <returns>
        /// Retorna <see cref="StatusCodes.Status204NoContent"/> quando a alteração é concluída com sucesso,
        /// </returns>
        /// <remarks>
        /// Requer autenticação e autorização com a role <c>Admin</c>.
        /// </remarks>
        [HttpPatch("{usuarioId}/tornar-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TornarAdmin([FromRoute] Guid usuarioId)
        {
            try
            {
                await _usuarioService.TornarUsuarioAdmin(usuarioId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }
    }
}