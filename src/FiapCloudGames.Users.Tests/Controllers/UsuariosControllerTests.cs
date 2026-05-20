using FiapCloudGames.Users.API.Controllers;
using FiapCloudGames.Users.Application.Usuarios.Inputs;
using FiapCloudGames.Users.Application.Usuarios.Interfaces;
using FiapCloudGames.Users.Application.Usuarios.Outputs;
using FiapCloudGames.Users.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Users.Tests.Controllers
{
    public class UsuariosControllerTests
    {
        private readonly Mock<IUsuarioService> _usuarioServiceMock = new();
        private readonly Mock<ICorrelationIdService> _correlationIdServiceMock = new();
        private readonly Mock<ILogger<UsuariosController>> _loggerMock = new();
        private readonly UsuariosController _controller;

        private const string CorrelationIdFixo = "correlation-id-teste";

        public UsuariosControllerTests()
        {
            _correlationIdServiceMock
                .Setup(s => s.GetCorrelationId())
                .Returns(CorrelationIdFixo);

            _controller = new UsuariosController(
                _usuarioServiceMock.Object,
                _correlationIdServiceMock.Object,
                _loggerMock.Object);
        }

        // CadastrarUsuario 

        [Fact]
        public async Task Dado_InputValido_Quando_CadastrarUsuario_Entao_DeveRetornar200OkComIdECorrelationId()
        {
            var usuarioId = Guid.NewGuid();
            var input = new CadastroUsuarioInput { Nome = "João", Email = "joao@email.com", Senha = "Senha@123" };

            _usuarioServiceMock
                .Setup(s => s.CadastrarUsuario(input))
                .ReturnsAsync(usuarioId);

            var resultado = await _controller.CadastrarUsuario(input);

            var okResult = resultado.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(new { id = usuarioId, correlationId = CorrelationIdFixo });
        }

        [Fact]
        public async Task Dado_ServicoLancaExcecao_Quando_CadastrarUsuario_Entao_DeveRetornar400ComMensagemDeErro()
        {
            var mensagemErro = "Email já cadastrado";
            var input = new CadastroUsuarioInput { Nome = "João", Email = "joao@email.com", Senha = "Senha@123" };

            _usuarioServiceMock
                .Setup(s => s.CadastrarUsuario(input))
                .ThrowsAsync(new Exception(mensagemErro));

            var resultado = await _controller.CadastrarUsuario(input);

            var badRequest = resultado.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().BeEquivalentTo(new { error = mensagemErro, correlationId = CorrelationIdFixo });
        }

        // Login

        [Fact]
        public async Task Dado_CredenciaisValidas_Quando_Login_Entao_DeveRetornar200OkComToken()
        {
            var loginOutput = new LoginOutput { Token = "jwt.token.valido" };
            var input = new LoginInput { Email = "joao@email.com", Senha = "Senha@123" };

            _usuarioServiceMock
                .Setup(s => s.Login(input))
                .ReturnsAsync(loginOutput);

            var resultado = await _controller.Login(input);

            var okResult = resultado.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(loginOutput);
        }

        [Fact]
        public async Task Dado_CredenciaisInvalidas_Quando_Login_Entao_DeveRetornar401Unauthorized()
        {
            var input = new LoginInput { Email = "joao@email.com", Senha = "SenhaErrada@1" };

            _usuarioServiceMock
                .Setup(s => s.Login(input))
                .ThrowsAsync(new UnauthorizedAccessException("Credenciais inválidas."));

            var resultado = await _controller.Login(input);

            resultado.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Dado_ErroInesperado_Quando_Login_Entao_DeveRetornar400BadRequest()
        {
            var input = new LoginInput { Email = "joao@email.com", Senha = "Senha@123" };

            _usuarioServiceMock
                .Setup(s => s.Login(input))
                .ThrowsAsync(new Exception("Erro inesperado."));

            var resultado = await _controller.Login(input);

            resultado.Should().BeOfType<BadRequestObjectResult>();
        }

        // TornarAdmin

        [Fact]
        public async Task Dado_UsuarioExistente_Quando_TornarAdmin_Entao_DeveRetornar204NoContent()
        {
            var usuarioId = Guid.NewGuid();

            _usuarioServiceMock
                .Setup(s => s.TornarUsuarioAdmin(usuarioId))
                .Returns(Task.CompletedTask);

            var resultado = await _controller.TornarAdmin(usuarioId);

            resultado.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Dado_ServicoLancaExcecao_Quando_TornarAdmin_Entao_DeveRetornar400BadRequest()
        {
            var usuarioId = Guid.NewGuid();

            _usuarioServiceMock
                .Setup(s => s.TornarUsuarioAdmin(usuarioId))
                .ThrowsAsync(new KeyNotFoundException("Usuário não encontrado."));

            var resultado = await _controller.TornarAdmin(usuarioId);

            resultado.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}