using FiapCloudGames.Users.Application.Usuarios.Inputs;
using FiapCloudGames.Users.Application.Usuarios.Interfaces;
using FiapCloudGames.Users.Application.Usuarios.Services;
using FiapCloudGames.Users.Domain.Usuarios.Entities;
using FiapCloudGames.Users.Domain.Usuarios.Enums;
using FiapCloudGames.Users.Domain.Usuarios.Interfaces;
using FiapCloudGames.Users.Domain.Usuarios.ValueObjects;
using Reqnroll;

namespace FiapCloudGames.Users.BDD.Tests.StepDefinitions
{
    [Binding]
    public class CadastroUsuarioStepDefinitions
    {
        private readonly Mock<IUsuarioRepository> _repositoryMock;
        private readonly Mock<IUsuarioDomainService> _domainServiceMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly UsuarioService _usuarioService;

        private CadastroUsuarioInput? _input;
        private Guid? _usuarioIdResultado;
        private Exception? _excecao;
        private Usuario? _usuarioCriado;
        private string? _senhaTextoPlano;

        public CadastroUsuarioStepDefinitions()
        {
            _repositoryMock = new Mock<IUsuarioRepository>();
            _domainServiceMock = new Mock<IUsuarioDomainService>();
            _tokenServiceMock = new Mock<ITokenService>();

            _usuarioService = new UsuarioService(
                _repositoryMock.Object,
                _domainServiceMock.Object,
                _tokenServiceMock.Object);
        }

        [Given(@"que não existe nenhum usuário cadastrado com o email ""([^""]*)""")]
        public void DadoQueNaoExisteNenhumUsuarioCadastradoComOEmail(string email)
        {
            _repositoryMock
                .Setup(r => r.EmailExisteAsync(email))
                .ReturnsAsync(false);

            _domainServiceMock
                .Setup(s => s.CriarAsync(It.IsAny<string>(), email, It.IsAny<string>()))
                .ReturnsAsync((string nome, string emailParam, string senhaHash) =>
                {
                    var usuario = new Usuario(nome, new Email(emailParam), senhaHash);
                    _usuarioCriado = usuario;
                    return usuario;
                });
        }

        [Given(@"que já existe um usuário cadastrado com o email ""([^""]*)""")]
        public void DadoQueJaExisteUmUsuarioCadastradoComOEmail(string email)
        {
            _domainServiceMock
                .Setup(s => s.CriarAsync(It.IsAny<string>(), email, It.IsAny<string>()))
                .ThrowsAsync(new Exception("Email já cadastrado"));
        }

        [When(@"eu tento cadastrar um usuário com os seguintes dados:")]
        public async Task QuandoEuTentoCadastrarUmUsuarioComOsSeguintesDados(Table table)
        {
            var row = table.Rows[0];
            var nome = row["Nome"];
            var email = row["Email"];
            var senha = row["Senha"];

            _senhaTextoPlano = senha;

            _input = new CadastroUsuarioInput
            {
                Nome = nome,
                Email = email,
                Senha = senha
            };

            try
            {
                _usuarioIdResultado = await _usuarioService.CadastrarUsuario(_input);
                _excecao = null;
            }
            catch (Exception ex)
            {
                _excecao = ex;
                _usuarioIdResultado = null;
            }
        }

        [Then(@"o cadastro deve ser bem-sucedido")]
        public void EntaoOCadastroDeveSerBemSucedido()
        {
            _excecao.Should().BeNull("não deveria ter lançado exceção");
            _usuarioIdResultado.Should().NotBeNull("deveria ter retornado um ID de usuário");
        }

        [Then(@"o ID do usuário deve ser retornado")]
        public void EntaoOIDDoUsuarioDeveSerRetornado()
        {
            _usuarioIdResultado.Should().NotBeNull();
            _usuarioIdResultado.Should().NotBe(Guid.Empty, "o ID não pode ser um GUID vazio");
        }

        [Then(@"o usuário deve ser adicionado ao repositório")]
        public void EntaoOUsuarioDeveSerAdicionadoAoRepositorio()
        {
            _repositoryMock.Verify(
                r => r.AdicionarAsync(It.IsAny<Usuario>()),
                Times.Once,
                "o repositório deveria ter sido chamado uma vez para adicionar o usuário");
        }

        [Then(@"o cadastro deve falhar com erro ""([^""]*)""")]
        public void EntaoOCadastroDeveFalharComErro(string mensagemEsperada)
        {
            _excecao.Should().NotBeNull("deveria ter lançado uma exceção");
            _excecao!.Message.Should().Contain(mensagemEsperada);
        }

        [Then(@"o cadastro deve falhar")]
        public void EntaoOCadastroDeveFalhar()
        {
            _excecao.Should().NotBeNull("deveria ter lançado uma exceção");
        }

        [Then(@"nenhum usuário deve ser adicionado ao repositório")]
        public void EntaoNenhumUsuarioDeveSerAdicionadoAoRepositorio()
        {
            _repositoryMock.Verify(
                r => r.AdicionarAsync(It.IsAny<Usuario>()),
                Times.Never,
                "o repositório não deveria ter sido chamado para adicionar usuário");
        }

        [Then(@"o usuário criado deve ter o perfil ""([^""]*)""")]
        public void EntaoOUsuarioCriadoDeveTerOPerfil(string perfilEsperado)
        {
            _usuarioCriado.Should().NotBeNull("um usuário deveria ter sido criado");

            var tipoEsperado = perfilEsperado == "Admin" ? TipoUsuario.Admin : TipoUsuario.Comum;

            _domainServiceMock.Verify(
                s => s.CriarAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once,
                "o serviço de domínio deveria ter sido chamado para criar o usuário");

            if (_usuarioCriado != null)
            {
                _usuarioCriado.Tipo.Should().Be(tipoEsperado, $"o usuário deveria ter o perfil {perfilEsperado}");
            }
        }

        [Then(@"a senha deve estar armazenada como hash BCrypt")]
        public void EntaoASenhaDeveEstarArmazenadaComoHashBCrypt()
        {
            _usuarioCriado.Should().NotBeNull("um usuário deveria ter sido criado");
            _usuarioCriado!.SenhaHash.Should().NotBeNullOrEmpty("o hash da senha não pode ser vazio");

            // Verifica se o hash começa com o prefixo BCrypt
            _usuarioCriado.SenhaHash.Should().StartWith("$2", "o hash BCrypt deve começar com $2");

            // Verifica se a senha original pode ser verificada com o hash
            if (_senhaTextoPlano != null)
            {
                var senhaValida = BCrypt.Net.BCrypt.Verify(_senhaTextoPlano, _usuarioCriado.SenhaHash);
                senhaValida.Should().BeTrue("o hash BCrypt deve validar a senha original");
            }
        }

        [Then(@"a senha em texto plano não deve estar armazenada")]
        public void EntaoASenhaEmTextoPlanoNaoDeveEstarArmazenada()
        {
            _usuarioCriado.Should().NotBeNull("um usuário deveria ter sido criado");

            if (_senhaTextoPlano != null)
            {
                _usuarioCriado!.SenhaHash.Should().NotBe(_senhaTextoPlano, "a senha em texto plano não deve estar armazenada");
                _usuarioCriado.SenhaHash.Length.Should().BeGreaterThan(_senhaTextoPlano.Length, "o hash deve ser maior que a senha original");
            }
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            // Limpa o estado entre cenários
            _input = null;
            _usuarioIdResultado = null;
            _excecao = null;
            _usuarioCriado = null;
            _senhaTextoPlano = null;

            // Configura comportamento padrão do repositório
            _repositoryMock
                .Setup(r => r.AdicionarAsync(It.IsAny<Usuario>()))
                .Returns(Task.CompletedTask);
        }
    }
}