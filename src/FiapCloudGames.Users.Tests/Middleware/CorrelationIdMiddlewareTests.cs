using FiapCloudGames.Users.API.Middleware;
using Microsoft.AspNetCore.Http;

namespace FiapCloudGames.Users.Tests.Middleware
{
    public class CorrelationIdMiddlewareTests
    {
        private const string CorrelationIdHeader = "X-Correlation-ID";

        private static CorrelationIdMiddleware CriarMiddleware(RequestDelegate? next = null)
        {
            next ??= _ => Task.CompletedTask;
            return new CorrelationIdMiddleware(next);
        }

        [Fact]
        public async Task Dado_HeaderCorrelationIdAusente_Quando_InvokeAsync_Entao_DeveGerarNovoCorrelationId()
        {
            var context = new DefaultHttpContext();
            var middleware = CriarMiddleware();

            await middleware.InvokeAsync(context);

            var correlationId = context.Items["CorrelationId"]?.ToString();
            correlationId.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Dado_HeaderCorrelationIdValido_Quando_InvokeAsync_Entao_DeveUsarCorrelationIdDoHeader()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers[CorrelationIdHeader] = "meu-correlation-id";
            var middleware = CriarMiddleware();

            await middleware.InvokeAsync(context);

            context.Items["CorrelationId"].Should().Be("meu-correlation-id");
        }

        [Fact]
        public async Task Dado_HeaderCorrelationIdComCaracteresInvalidos_Quando_InvokeAsync_Entao_DeveGerarNovoCorrelationId()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers[CorrelationIdHeader] = "id inválido!@#";
            var middleware = CriarMiddleware();

            await middleware.InvokeAsync(context);

            var correlationId = context.Items["CorrelationId"]?.ToString();
            correlationId.Should().NotBe("id inválido!@#");
            correlationId.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Dado_HeaderCorrelationIdMuitoLongo_Quando_InvokeAsync_Entao_DeveGerarNovoCorrelationId()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers[CorrelationIdHeader] = new string('a', 65);
            var middleware = CriarMiddleware();

            await middleware.InvokeAsync(context);

            var correlationId = context.Items["CorrelationId"]?.ToString();
            correlationId.Should().NotBe(new string('a', 65));
            correlationId.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Dado_CorrelationIdValido_Quando_InvokeAsync_Entao_DeveAdicionarNoHeaderDeResposta()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers[CorrelationIdHeader] = "correlation-123";
            var middleware = CriarMiddleware();

            await middleware.InvokeAsync(context);

            context.Response.Headers[CorrelationIdHeader].ToString().Should().Be("correlation-123");
        }

        [Fact]
        public async Task Dado_QualquerRequisicao_Quando_InvokeAsync_Entao_DevePassarParaProximoMiddleware()
        {
            var context = new DefaultHttpContext();
            var proximoFoiChamado = false;
            var middleware = CriarMiddleware(_ => { proximoFoiChamado = true; return Task.CompletedTask; });

            await middleware.InvokeAsync(context);

            proximoFoiChamado.Should().BeTrue();
        }
    }
}
