using FiapCloudGames.Users.API.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security;
using System.Text.Json;

namespace FiapCloudGames.Users.Tests.Middleware
{
    public class ErrorHandlingMiddlewareTests
    {
        private readonly Mock<ILogger<ErrorHandlingMiddleware>> _loggerMock = new();

        private DefaultHttpContext CriarContexto(string? correlationId = "correlation-id-teste")
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            if (correlationId is not null)
                context.Items["CorrelationId"] = correlationId;
            return context;
        }

        private ErrorHandlingMiddleware CriarMiddleware(RequestDelegate next)
            => new(next, _loggerMock.Object);

        private static async Task<(int statusCode, string? error)> LerResposta(HttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(body);
            var error = json.GetProperty("error").GetString();
            return (context.Response.StatusCode, error);
        }

        [Fact]
        public async Task Dado_ArgumentException_Quando_InvokeAsync_Entao_DeveRetornar400()
        {
            var context = CriarContexto();
            var middleware = CriarMiddleware(_ => throw new ArgumentException("dado inválido"));

            await middleware.InvokeAsync(context);

            var (statusCode, error) = await LerResposta(context);
            statusCode.Should().Be(400);
            error.Should().Be("Dados inválidos");
        }

        [Fact]
        public async Task Dado_KeyNotFoundException_Quando_InvokeAsync_Entao_DeveRetornar404()
        {
            var context = CriarContexto();
            var middleware = CriarMiddleware(_ => throw new KeyNotFoundException("não encontrado"));

            await middleware.InvokeAsync(context);

            var (statusCode, error) = await LerResposta(context);
            statusCode.Should().Be(404);
            error.Should().Be("Recurso não encontrado");
        }

        [Fact]
        public async Task Dado_UnauthorizedAccessException_Quando_InvokeAsync_Entao_DeveRetornar401()
        {
            var context = CriarContexto();
            var middleware = CriarMiddleware(_ => throw new UnauthorizedAccessException("não autorizado"));

            await middleware.InvokeAsync(context);

            var (statusCode, error) = await LerResposta(context);
            statusCode.Should().Be(401);
            error.Should().Be("Acesso não autorizado");
        }

        [Fact]
        public async Task Dado_SecurityException_Quando_InvokeAsync_Entao_DeveRetornar403()
        {
            var context = CriarContexto();
            var middleware = CriarMiddleware(_ => throw new SecurityException("acesso negado"));

            await middleware.InvokeAsync(context);

            var (statusCode, error) = await LerResposta(context);
            statusCode.Should().Be(403);
            error.Should().Be("Acesso negado");
        }

        [Fact]
        public async Task Dado_ExcecaoGenerica_Quando_InvokeAsync_Entao_DeveRetornar500()
        {
            var context = CriarContexto();
            var middleware = CriarMiddleware(_ => throw new Exception("erro inesperado"));

            await middleware.InvokeAsync(context);

            var (statusCode, error) = await LerResposta(context);
            statusCode.Should().Be(500);
            error.Should().Be("Erro interno");
        }

        [Fact]
        public async Task Dado_SemExcecao_Quando_InvokeAsync_Entao_DevePassarParaProximoMiddleware()
        {
            var context = CriarContexto();
            var proximoFoiChamado = false;
            var middleware = CriarMiddleware(_ => { proximoFoiChamado = true; return Task.CompletedTask; });

            await middleware.InvokeAsync(context);

            proximoFoiChamado.Should().BeTrue();
        }

        [Fact]
        public async Task Dado_QualquerExcecao_Quando_InvokeAsync_Entao_RespostaDeveConterCorrelationId()
        {
            var context = CriarContexto("correlation-abc");
            var middleware = CriarMiddleware(_ => throw new Exception("erro"));

            await middleware.InvokeAsync(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(body);
            json.GetProperty("correlationId").GetString().Should().Be("correlation-abc");
        }
    }
}
