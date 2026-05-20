using FiapCloudGames.Users.Domain.Usuarios.Entities;

namespace FiapCloudGames.Users.Tests.Domain
{
    public class ValidacaoSenhaTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Dado_SenhaNulaOuVazia_Quando_Validar_Entao_DeveLancarArgumentException(string? senha)
        {
            var acao = () => Usuario.ValidarSenha(senha!);

            acao.Should().Throw<ArgumentException>()
                .WithMessage("*obrigatória*");
        }

        [Theory]
        [InlineData("Ab1@")]
        [InlineData("Ab1@567")]
        public void Dado_SenhaComMenosDeOitoCaracteres_Quando_Validar_Entao_DeveLancarArgumentException(string senha)
        {
            var acao = () => Usuario.ValidarSenha(senha);

            acao.Should().Throw<ArgumentException>()
                .WithMessage("*mínimo 8*");
        }

        [Theory]
        [InlineData("abcdefg1!")]
        [InlineData("minuscula1!")]
        public void Dado_SenhaSemLetraMaiuscula_Quando_Validar_Entao_DeveLancarArgumentException(string senha)
        {
            var acao = () => Usuario.ValidarSenha(senha);

            acao.Should().Throw<ArgumentException>()
                .WithMessage("*maiúscula*");
        }

        [Theory]
        [InlineData("ABCDEFG1!")]
        [InlineData("MAIUSCULA1!")]
        public void Dado_SenhaSemLetraMinuscula_Quando_Validar_Entao_DeveLancarArgumentException(string senha)
        {
            var acao = () => Usuario.ValidarSenha(senha);

            acao.Should().Throw<ArgumentException>()
                .WithMessage("*minúscula*");
        }

        [Theory]
        [InlineData("Abcdefg@!")]
        [InlineData("SemNumero@!")]
        public void Dado_SenhaSemNumero_Quando_Validar_Entao_DeveLancarArgumentException(string senha)
        {
            var acao = () => Usuario.ValidarSenha(senha);

            acao.Should().Throw<ArgumentException>()
                .WithMessage("*número*");
        }

        [Theory]
        [InlineData("Abcdefg12")]
        [InlineData("SemEspecial12")]
        public void Dado_SenhaSemCaractereEspecial_Quando_Validar_Entao_DeveLancarArgumentException(string senha)
        {
            var acao = () => Usuario.ValidarSenha(senha);

            acao.Should().Throw<ArgumentException>()
                .WithMessage("*especial*");
        }

        [Theory]
        [InlineData("Senha@123")]
        [InlineData("Fiap#2024")]
        [InlineData("MinhaSenha1!")]
        [InlineData("P@ssw0rd")]
        public void Dado_SenhaValida_Quando_Validar_Entao_NaoDeveLancarExcecao(string senha)
        {
            var acao = () => Usuario.ValidarSenha(senha);

            acao.Should().NotThrow();
        }
    }
}
