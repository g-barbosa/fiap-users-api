using FiapCloudGames.Users.Application.Usuarios.Inputs;
using FiapCloudGames.Users.Application.Usuarios.Interfaces;
using FiapCloudGames.Users.Application.Usuarios.Outputs;
using FiapCloudGames.Users.Application.Usuarios.Services;
using FiapCloudGames.Users.Domain.Usuarios.Entities;
using FiapCloudGames.Users.Domain.Usuarios.Enums;
using FiapCloudGames.Users.Domain.Usuarios.Interfaces;
using FiapCloudGames.Users.Domain.Usuarios.ValueObjects;
using Reqnroll;

namespace FiapCloudGames.Users.BDD.Tests.StepDefinitions
{
    [Binding]
    public class LoginStepDefinitions
    {
        private readonly Mock<IUsuarioRepository> _repositoryMock;
        private readonly Mock<IUsuarioDomainService> _domainServiceMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly UsuarioService _usuarioService;

        private Usuario? _usuarioCadastrado;
        private LoginOutput? _resultado;
        private Exception? _excecao;
        private string _emailTentativa = string.Empty;
        private string _senhaTentativa = string.Empty;

        public LoginStepDefinitions()
        {
            _repositoryMock = new Mock<IUsuarioRepository>();
            _domainServiceMock = new Mock<IUsuarioDomainService>();
            _tokenServiceMock = new Mock<ITokenService>();

            _usuarioService = new UsuarioService(
                _repositoryMock.Object,
                _domainServiceMock.Object,
                _tokenServiceMock.Object);
        }

        [Given(@"que existe um usuário cadastrado com os seguintes dados:")]
        public void DadoQueExisteUmUsuarioCadastradoComOsSeguintesDados(Table table)
        {
            var row = table.Rows[0];
            var nome = row["Nome"];
            var email = row["Email"];
            var senha = row["Senha"];

            var senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);
            _usuarioCadastrado = new Usuario(nome, new Email(email), senhaHash);

            _repositoryMock
                .Setup(r => r.ObterPorEmailAsync(email))
                .ReturnsAsync(_usuarioCadastrado);
        }

        [Given(@"que o usuário ""([^""]*)"" foi promovido a Admin")]
        public void DadoQueOUsuarioFoiPromovidoAAdmin(string email)
        {
            if (_usuarioCadastrado != null && _usuarioCadastrado.Email.Endereco == email)
            {
                _usuarioCadastrado.TornarAdmin();
            }
        }

        [When(@"eu tento fazer login com o email ""([^""]*)"" e senha ""([^""]*)""")]
        public async Task QuandoEuTentoFazerLoginComOEmailESenha(string email, string senha)
        {
            _emailTentativa = email;
            _senhaTentativa = senha;

            var input = new LoginInput
            {
                Email = email,
                Senha = senha
            };

            try
            {
                _resultado = await _usuarioService.Login(input);
                _excecao = null;
            }
            catch (Exception ex)
            {
                _excecao = ex;
                _resultado = null;
            }
        }

        [Then(@"o login deve ser bem-sucedido")]
        public void EntaoOLoginDeveSerBemSucedido()
        {
            _excecao.Should().BeNull("não deveria ter lançado exceção");
            _resultado.Should().NotBeNull("deveria ter retornado resultado de login");
        }

        [Then(@"um token JWT deve ser retornado")]
        public void EntaoUmTokenJWTDeveSerRetornado()
        {
            _resultado.Should().NotBeNull();
            _resultado!.Token.Should().NotBeNullOrEmpty("o token não pode ser vazio");
        }

        [Then(@"o login deve falhar com erro ""([^""]*)""")]
        public void EntaoOLoginDeveFalharComErro(string mensagemEsperada)
        {
            _excecao.Should().NotBeNull("deveria ter lançado uma exceção");
            _excecao.Should().BeOfType<UnauthorizedAccessException>("deveria ser UnauthorizedAccessException");
            _excecao!.Message.Should().Contain(mensagemEsperada);
        }

        [Then(@"nenhum token deve ser retornado")]
        public void EntaoNenhumTokenDeveSerRetornado()
        {
            _resultado.Should().BeNull("não deveria ter retornado resultado de login");
        }

        [Then(@"o token gerado deve incluir o perfil ""([^""]*)""")]
        public void EntaoOTokenGeradoDeveIncluirOPerfil(string perfilEsperado)
        {
            _resultado.Should().NotBeNull();

            var tipoEsperado = perfilEsperado == "Admin" ? TipoUsuario.Admin : TipoUsuario.Comum;

            _tokenServiceMock.Verify(
                t => t.GerarToken(It.Is<Usuario>(u => u.Tipo == tipoEsperado)),
                Times.Once,
                $"o token deveria ter sido gerado para um usuário do tipo {perfilEsperado}");
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            // Configura comportamento padrão: usuários não existem
            _repositoryMock
                .Setup(r => r.ObterPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((Usuario?)null);

            // Configura geração de token padrão
            _tokenServiceMock
                .Setup(t => t.GerarToken(It.IsAny<Usuario>()))
                .Returns("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIn0.dozjgNryP4J3jVmNHl0w5N_XgL0n3I9PlFUP0THsR8U");
        }
    }
}