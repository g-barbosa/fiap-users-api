using FiapCloudGames.Users.Application.Usuarios.Inputs;
using FiapCloudGames.Users.Application.Usuarios.Interfaces;
using FiapCloudGames.Users.Application.Usuarios.Services;
using FiapCloudGames.Users.Domain.Usuarios.Entities;
using FiapCloudGames.Users.Domain.Usuarios.Enums;
using FiapCloudGames.Users.Domain.Usuarios.Interfaces;
using FiapCloudGames.Users.Domain.Usuarios.ValueObjects;

namespace FiapCloudGames.Users.Tests.Services
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _repositoryMock = new();
        private readonly Mock<IUsuarioDomainService> _domainServiceMock = new();
        private readonly Mock<ITokenService> _tokenServiceMock = new();
        private readonly UsuarioService _usuarioService;

        public UsuarioServiceTests()
        {
            _usuarioService = new UsuarioService(_repositoryMock.Object, _domainServiceMock.Object, _tokenServiceMock.Object);
        }

        // CadastrarUsuario

        [Theory]
        [InlineData("Ab1@")]
        [InlineData("abcdefg1!")]
        [InlineData("ABCDEFG1!")]
        [InlineData("Abcdefg@!")]
        [InlineData("Abcdefg12")]
        public async Task Dado_SenhaInvalida_Quando_CadastrarUsuario_Entao_DeveLancarArgumentException(string senhaInvalida)
        {
            var input = new CadastroUsuarioInput { Nome = "Teste", Email = "teste@email.com", Senha = senhaInvalida };

            var acao = async () => await _usuarioService.CadastrarUsuario(input);

            await acao.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task Dado_InputValido_Quando_CadastrarUsuario_Entao_DeveRetornarIdDoUsuarioCriado()
        {
            var usuarioCriado = new Usuario("Novo", new Email("novo@email.com"), "hashGerado");
            var input = new CadastroUsuarioInput { Nome = "Novo", Email = "novo@email.com", Senha = "Senha@123" };

            _domainServiceMock
                .Setup(s => s.CriarAsync(input.Nome, input.Email, It.IsAny<string>()))
                .ReturnsAsync(usuarioCriado);

            var id = await _usuarioService.CadastrarUsuario(input);

            id.Should().Be(usuarioCriado.Id);
        }

        [Fact]
        public async Task Dado_InputValido_Quando_CadastrarUsuario_Entao_DeveAdicionarUsuarioNoRepositorio()
        {
            var usuarioCriado = new Usuario("Novo", new Email("novo@email.com"), "hashGerado");
            var input = new CadastroUsuarioInput { Nome = "Novo", Email = "novo@email.com", Senha = "Senha@123" };

            _domainServiceMock
                .Setup(s => s.CriarAsync(input.Nome, input.Email, It.IsAny<string>()))
                .ReturnsAsync(usuarioCriado);

            await _usuarioService.CadastrarUsuario(input);

            _repositoryMock.Verify(r => r.AdicionarAsync(usuarioCriado), Times.Once);
        }

        // Login

        [Fact]
        public async Task Dado_UsuarioNaoEncontrado_Quando_Login_Entao_DeveLancarUnauthorizedAccessException()
        {
            _repositoryMock
                .Setup(r => r.ObterPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((Usuario?)null);

            var input = new LoginInput { Email = "inexistente@email.com", Senha = "Senha@123" };

            var acao = async () => await _usuarioService.Login(input);

            await acao.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*Credenciais inválidas*");
        }

        [Fact]
        public async Task Dado_SenhaIncorreta_Quando_Login_Entao_DeveLancarUnauthorizedAccessException()
        {
            var usuario = new Usuario("Usuário", new Email("usuario@email.com"), BCrypt.Net.BCrypt.HashPassword("Senha@123"));

            _repositoryMock
                .Setup(r => r.ObterPorEmailAsync(usuario.Email.Endereco))
                .ReturnsAsync(usuario);

            var input = new LoginInput { Email = usuario.Email.Endereco, Senha = "SenhaErrada@1" };

            var acao = async () => await _usuarioService.Login(input);

            await acao.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*Credenciais inválidas*");
        }

        [Fact]
        public async Task Dado_CredenciaisValidas_Quando_Login_Entao_DeveRetornarTokenGerado()
        {
            const string senhaPlana = "Senha@123";
            const string tokenEsperado = "jwt.token.gerado";
            var usuario = new Usuario("Usuário", new Email("usuario@email.com"), BCrypt.Net.BCrypt.HashPassword(senhaPlana));

            _repositoryMock
                .Setup(r => r.ObterPorEmailAsync(usuario.Email.Endereco))
                .ReturnsAsync(usuario);
            _tokenServiceMock
                .Setup(t => t.GerarToken(usuario))
                .Returns(tokenEsperado);

            var resultado = await _usuarioService.Login(new LoginInput { Email = usuario.Email.Endereco, Senha = senhaPlana });

            resultado.Token.Should().Be(tokenEsperado);
        }

        // TornarAdmin

        [Fact]
        public async Task Dado_UsuarioNaoEncontrado_Quando_TornarAdmin_Entao_DeveLancarKeyNotFoundException()
        {
            _repositoryMock
                .Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Usuario?)null);

            var acao = async () => await _usuarioService.TornarUsuarioAdmin(Guid.NewGuid());

            await acao.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*não encontrado*");
        }

        [Fact]
        public async Task Dado_UsuarioExistente_Quando_TornarAdmin_Entao_DevePromoverParaAdminEAtualizarRepositorio()
        {
            var usuario = new Usuario("Usuário", new Email("usuario@email.com"), "hash");

            _repositoryMock
                .Setup(r => r.ObterPorIdAsync(usuario.Id))
                .ReturnsAsync(usuario);

            await _usuarioService.TornarUsuarioAdmin(usuario.Id);

            usuario.Tipo.Should().Be(TipoUsuario.Admin);
            _repositoryMock.Verify(r => r.AtualizarAsync(usuario), Times.Once);
        }
    }
}
