using FiapCloudGames.Users.Domain.Usuarios.ValueObjects;

namespace FiapCloudGames.Users.Tests.Domain
{
    public class ValidacaoEmailTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Dado_EmailNuloOuVazio_Quando_Criar_Entao_DeveLancarArgumentException(string? email)
        {
            var acao = () => new Email(email!);

            acao.Should().Throw<ArgumentException>()
                .WithMessage("*vazio*");
        }

        [Theory]
        [InlineData("semArroba")]
        [InlineData("@semlocal.com")]
        [InlineData("sem.dominio@")]
        [InlineData("dois@@arrobas.com")]
        [InlineData("espacos @email.com")]
        public void Dado_EmailComFormatoInvalido_Quando_Criar_Entao_DeveLancarArgumentException(string email)
        {
            var acao = () => new Email(email);

            acao.Should().Throw<ArgumentException>()
                .WithMessage("*inválido*");
        }

        [Theory]
        [InlineData("usuario@email.com", "usuario@email.com")]
        [InlineData("USUARIO@EMAIL.COM", "usuario@email.com")]
        public void Dado_EmailValido_Quando_Criar_Entao_DeveNormalizarParaMinusculo(string email, string esperado)
        {
            var emailVO = new Email(email);

            emailVO.Endereco.Should().Be(esperado);
        }

        [Fact]
        public void Dado_EmailValido_Quando_Criar_Entao_DeveArmazenarEnderecoCorretamente()
        {
            var emailVO = new Email("usuario@fiap.com.br");

            emailVO.Endereco.Should().Be("usuario@fiap.com.br");
        }
    }
}
