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
    public class TornarAdminStepDefinitions
    {
        private readonly Mock<IUsuarioRepository> _repositoryMock;
        private readonly Mock<IUsuarioDomainService> _domainServiceMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly UsuarioService _usuarioService;

        private Usuario? _usuarioAtual;
        private List<Usuario> _usuariosMultiplos;
        private Exception? _excecao;
        private Guid _usuarioIdAtual;
        private string? _nomeOriginal;
        private string? _emailOriginal;
        private TipoUsuario _tipoOriginal;
        private bool _repositorioDevefalhar;

        public TornarAdminStepDefinitions()
        {
            _repositoryMock = new Mock<IUsuarioRepository>();
            _domainServiceMock = new Mock<IUsuarioDomainService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _usuariosMultiplos = new List<Usuario>();

            _usuarioService = new UsuarioService(
                _repositoryMock.Object,
                _domainServiceMock.Object,
                _tokenServiceMock.Object);
        }

        [Given(@"que estou autenticado como administrador")]
        public void DadoQueEstouAutenticadoComoAdministrador()
        {
            // Simula autenticação de admin
            // Em um cenário real, isso seria feito via token JWT
        }

        [Given(@"que existe um usuário comum com os seguintes dados:")]
        public void DadoQueExisteUmUsuarioComumComOsSeguintesDados(Table table)
        {
            var row = table.Rows[0];
            var nome = row["Nome"];
            var email = row["Email"];
            var tipo = row["Tipo"];

            var senhaHash = BCrypt.Net.BCrypt.HashPassword("Senha@123");
            _usuarioAtual = new Usuario(nome, new Email(email), senhaHash);
            _usuarioIdAtual = _usuarioAtual.Id;

            // Armazena valores originais para verificação posterior
            _nomeOriginal = _usuarioAtual.Nome;
            _emailOriginal = _usuarioAtual.Email.Endereco;
            _tipoOriginal = _usuarioAtual.Tipo;

            ConfigurarRepositorioParaUsuario(_usuarioAtual);
        }

        [Given(@"que existe um usuário administrador com os seguintes dados:")]
        public void DadoQueExisteUmUsuarioAdministradorComOsSeguintesDados(Table table)
        {
            var row = table.Rows[0];
            var nome = row["Nome"];
            var email = row["Email"];

            var senhaHash = BCrypt.Net.BCrypt.HashPassword("Senha@123");
            _usuarioAtual = new Usuario(nome, new Email(email), senhaHash);
            _usuarioAtual.TornarAdmin(); // Já promove a admin
            _usuarioIdAtual = _usuarioAtual.Id;

            _nomeOriginal = _usuarioAtual.Nome;
            _emailOriginal = _usuarioAtual.Email.Endereco;
            _tipoOriginal = _usuarioAtual.Tipo;

            ConfigurarRepositorioParaUsuario(_usuarioAtual);
        }

        [Given(@"que não existe nenhum usuário com o ID ""([^""]*)""")]
        public void DadoQueNaoExisteNenhumUsuarioComOID(string guidString)
        {
            var guid = Guid.Parse(guidString);
            _usuarioIdAtual = guid;

            _repositoryMock
                .Setup(r => r.ObterPorIdAsync(guid))
                .ReturnsAsync((Usuario?)null);
        }

        [Given(@"que existem os seguintes usuários:")]
        public void DadoQueExistemOsSeguintesUsuarios(Table table)
        {
            _usuariosMultiplos.Clear();

            foreach (var row in table.Rows)
            {
                var nome = row["Nome"];
                var email = row["Email"];
                var senhaHash = BCrypt.Net.BCrypt.HashPassword("Senha@123");
                var usuario = new Usuario(nome, new Email(email), senhaHash);
                _usuariosMultiplos.Add(usuario);

                _repositoryMock
                    .Setup(r => r.ObterPorIdAsync(usuario.Id))
                    .ReturnsAsync(usuario);
            }
        }

        [Given(@"que o repositório irá falhar ao atualizar")]
        public void DadoQueORepositorioIraFalharAoAtualizar()
        {
            _repositorioDevefalhar = true;
            _repositoryMock
                .Setup(r => r.AtualizarAsync(It.IsAny<Usuario>()))
                .ThrowsAsync(new InvalidOperationException("Erro ao atualizar no banco de dados"));
        }

        [When(@"eu tento promover o usuário a administrador")]
        public async Task QuandoEuTentoPromoverOUsuarioAAdministrador()
        {
            await TentarPromoverUsuario(_usuarioIdAtual);
        }

        [When(@"eu tento promover o usuário com ID ""([^""]*)"" a administrador")]
        public async Task QuandoEuTentoPromoverOUsuarioComIDAAdministrador(string guidString)
        {
            var guid = Guid.Parse(guidString);
            await TentarPromoverUsuario(guid);
        }

        [When(@"eu promovo todos os usuários a administradores")]
        public async Task QuandoEuPromovoTodosOsUsuariosAAdministradores()
        {
            foreach (var usuario in _usuariosMultiplos)
            {
                try
                {
                    await _usuarioService.TornarUsuarioAdmin(usuario.Id);
                }
                catch (Exception ex)
                {
                    _excecao = ex;
                    return;
                }
            }
        }

        [Then(@"a promoção deve ser bem-sucedida")]
        public void EntaoAPromocaoDeveSerBemSucedida()
        {
            _excecao.Should().BeNull("não deveria ter lançado exceção");
        }

        [Then(@"o usuário deve ter o perfil ""([^""]*)""")]
        public void EntaoOUsuarioDeveTerOPerfil(string perfilEsperado)
        {
            _usuarioAtual.Should().NotBeNull();

            var tipoEsperado = perfilEsperado == "Admin" ? TipoUsuario.Admin : TipoUsuario.Comum;
            _usuarioAtual!.Tipo.Should().Be(tipoEsperado, $"o usuário deveria ter o perfil {perfilEsperado}");
        }

        [Then(@"o usuário deve continuar com o perfil ""([^""]*)""")]
        public void EntaoOUsuarioDeveContinuarComOPerfil(string perfilEsperado)
        {
            EntaoOUsuarioDeveTerOPerfil(perfilEsperado);
        }

        [Then(@"o usuário deve ser atualizado no repositório")]
        public void EntaoOUsuarioDeveSerAtualizadoNoRepositorio()
        {
            _repositoryMock.Verify(
                r => r.AtualizarAsync(It.IsAny<Usuario>()),
                Times.Once,
                "o repositório deveria ter sido chamado uma vez para atualizar o usuário");
        }

        [Then(@"a promoção deve falhar com erro ""([^""]*)""")]
        public void EntaoAPromocaoDeveFalharComErro(string mensagemEsperada)
        {
            _excecao.Should().NotBeNull("deveria ter lançado uma exceção");
            _excecao.Should().BeOfType<KeyNotFoundException>("deveria ser KeyNotFoundException");
            _excecao!.Message.Should().Contain(mensagemEsperada);
        }

        [Then(@"a promoção deve falhar")]
        public void EntaoAPromocaoDeveFalhar()
        {
            _excecao.Should().NotBeNull("deveria ter lançado uma exceção");
        }

        [Then(@"nenhum usuário deve ser atualizado no repositório")]
        public void EntaoNenhumUsuarioDeveSerAtualizadoNoRepositorio()
        {
            _repositoryMock.Verify(
                r => r.AtualizarAsync(It.IsAny<Usuario>()),
                Times.Never,
                "o repositório não deveria ter sido chamado para atualizar usuário");
        }

        [Then(@"o método de atualização do repositório deve ser chamado exatamente uma vez")]
        public void EntaoOMetodoDeAtualizacaoDoRepositorioDeveSerChamadoExatamenteUmaVez()
        {
            _repositoryMock.Verify(
                r => r.AtualizarAsync(It.Is<Usuario>(u => u.Id == _usuarioIdAtual)),
                Times.Once,
                "o método AtualizarAsync deveria ter sido chamado exatamente uma vez");
        }

        [Then(@"todas as promoções devem ser bem-sucedidas")]
        public void EntaoTodasAsPromocoesDemSerBemSucedidas()
        {
            _excecao.Should().BeNull("nenhuma exceção deveria ter sido lançada");
        }

        [Then(@"todos os usuários devem ter o perfil ""([^""]*)""")]
        public void EntaoTodosOsUsuariosDevemTerOPerfil(string perfilEsperado)
        {
            var tipoEsperado = perfilEsperado == "Admin" ? TipoUsuario.Admin : TipoUsuario.Comum;

            foreach (var usuario in _usuariosMultiplos)
            {
                usuario.Tipo.Should().Be(tipoEsperado, $"o usuário {usuario.Nome} deveria ter o perfil {perfilEsperado}");
            }
        }

        [Then(@"o nome do usuário não deve ser alterado")]
        public void EntaoONomeDoUsuarioNaoDeveSerAlterado()
        {
            _usuarioAtual.Should().NotBeNull();
            _usuarioAtual!.Nome.Should().Be(_nomeOriginal, "o nome não deveria ter sido alterado");
        }

        [Then(@"o email do usuário não deve ser alterado")]
        public void EntaoOEmailDoUsuarioNaoDeveSerAlterado()
        {
            _usuarioAtual.Should().NotBeNull();
            _usuarioAtual!.Email.Endereco.Should().Be(_emailOriginal, "o email não deveria ter sido alterado");
        }

        [Then(@"apenas o tipo deve ser alterado para ""([^""]*)""")]
        public void EntaoApenasOTipoDeveSerAlteradoPara(string perfilEsperado)
        {
            _usuarioAtual.Should().NotBeNull();

            var tipoEsperado = perfilEsperado == "Admin" ? TipoUsuario.Admin : TipoUsuario.Comum;

            _usuarioAtual!.Tipo.Should().Be(tipoEsperado, "o tipo deveria ter sido alterado");
            _usuarioAtual.Nome.Should().Be(_nomeOriginal, "o nome não deveria ter sido alterado");
            _usuarioAtual.Email.Endereco.Should().Be(_emailOriginal, "o email não deveria ter sido alterado");
        }

        [Then(@"uma exceção deve ser lançada")]
        public void EntaoUmaExcecaoDeveSerLancada()
        {
            _excecao.Should().NotBeNull("deveria ter lançado uma exceção");
        }

        [Then(@"as alterações devem ser persistidas no repositório")]
        public void EntaoAsAlteracoesDevemSerPersitidasNoRepositorio()
        {
            _repositoryMock.Verify(
                r => r.AtualizarAsync(It.Is<Usuario>(u => u.Id == _usuarioIdAtual && u.Tipo == TipoUsuario.Admin)),
                Times.Once,
                "o usuário com perfil Admin deveria ter sido persistido");
        }

        [Then(@"o usuário atualizado deve ter o perfil ""([^""]*)""")]
        public void EntaoOUsuarioAtualizadoDeveTerOPerfil(string perfilEsperado)
        {
            var tipoEsperado = perfilEsperado == "Admin" ? TipoUsuario.Admin : TipoUsuario.Comum;

            _repositoryMock.Verify(
                r => r.AtualizarAsync(It.Is<Usuario>(u => u.Tipo == tipoEsperado)),
                Times.Once,
                $"o usuário persistido deveria ter o perfil {perfilEsperado}");
        }

        // Métodos auxiliares privados

        private void ConfigurarRepositorioParaUsuario(Usuario usuario)
        {
            _repositoryMock
                .Setup(r => r.ObterPorIdAsync(usuario.Id))
                .ReturnsAsync(usuario);

            if (!_repositorioDevefalhar)
            {
                _repositoryMock
                    .Setup(r => r.AtualizarAsync(usuario))
                    .Returns(Task.CompletedTask);
            }
        }

        private async Task TentarPromoverUsuario(Guid usuarioId)
        {
            try
            {
                await _usuarioService.TornarUsuarioAdmin(usuarioId);
                _excecao = null;
            }
            catch (Exception ex)
            {
                _excecao = ex;
            }
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            // Limpa o estado entre cenários
            _usuarioAtual = null;
            _usuariosMultiplos.Clear();
            _excecao = null;
            _usuarioIdAtual = Guid.Empty;
            _nomeOriginal = null;
            _emailOriginal = null;
            _tipoOriginal = TipoUsuario.Comum;
            _repositorioDevefalhar = false;

            // Configura comportamento padrão do repositório
            _repositoryMock.Reset();
        }
    }
}