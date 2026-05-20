using FiapCloudGames.Users.Application.Usuarios.Interfaces;
using FiapCloudGames.Users.Domain.Usuarios.Entities;
using FiapCloudGames.Users.Infrastructure.Security.settings;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FiapCloudGames.Users.Infrastructure.Security
{
    [ExcludeFromCodeCoverage]
    public class TokenService(Microsoft.Extensions.Options.IOptions<JwtConfigs> configuracoes) : ITokenService
    {
        private readonly JwtConfigs _configuracoes = configuracoes.Value;

        public string GerarToken(Usuario usuario)
        {
            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuracoes.Key));
            var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

            var expiracao = DateTime.UtcNow.AddMinutes(_configuracoes.ExpiracaoMinutos);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, usuario.Email.Endereco),
                new(ClaimTypes.Name, usuario.Email.Endereco),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Role, usuario.Tipo.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuracoes.Issuer,
                audience: _configuracoes.Audience,
                claims: claims,
                expires: expiracao,
                signingCredentials: credenciais
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }
    }
}
