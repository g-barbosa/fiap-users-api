using FiapCloudGames.Users.Domain.Usuarios.Enums;
using FiapCloudGames.Users.Domain.Usuarios.Interfaces;
using FiapCloudGames.Users.Domain.Usuarios.Services;

namespace FiapCloudGames.Users.Tests.Services
{
    public class UsuarioDomainServiceTests
    {
        private readonly Mock<IUsuarioRepository> _repositoryMock = new();
        private readonly UsuarioDomainService _usuarioDomainService;

        public UsuarioDomainServiceTests()
        {
            _usuarioDomainService = new UsuarioDomainService(_repositoryMock.Object);
        }

        [Fact]
        public async Task Dado_EmailJaCadastrado_Quando_CriarAsync_Entao_DeveLancarException()
        {
            _repositoryMock
                .Setup(r => r.EmailExisteAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var acao = async () => await _usuarioDomainService.CriarAsync("João Silva", "joao@email.com", "hashQualquer");

            await acao.Should().ThrowAsync<Exception>()
                .WithMessage("*Email já cadastrado*");
        }

        [Fact]
        public async Task Dado_EmailDisponivel_Quando_CriarAsync_Entao_DeveCriarUsuarioComNomeEEmailCorretos()
        {
            _repositoryMock
                .Setup(r => r.EmailExisteAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            var usuario = await _usuarioDomainService.CriarAsync("João Silva", "joao@email.com", "hashQualquer");

            usuario.Nome.Should().Be("João Silva");
            usuario.Email.Endereco.Should().Be("joao@email.com");
            usuario.SenhaHash.Should().Be("hashQualquer");
        }

        [Fact]
        public async Task Dado_EmailDisponivel_Quando_CriarAsync_Entao_UsuarioDeveTerIdGerado()
        {
            _repositoryMock
                .Setup(r => r.EmailExisteAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            var usuario = await _usuarioDomainService.CriarAsync("Maria Souza", "maria@email.com", "hash");

            usuario.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Dado_EmailDisponivel_Quando_CriarAsync_Entao_UsuarioDeveSerDoTipoComumPorPadrao()
        {
            _repositoryMock
                .Setup(r => r.EmailExisteAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            var usuario = await _usuarioDomainService.CriarAsync("Carlos Lima", "carlos@email.com", "hash");

            usuario.Tipo.Should().Be(TipoUsuario.Comum);
        }
    }
}
