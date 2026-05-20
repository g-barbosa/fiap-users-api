using FiapCloudGames.Users.Domain.Usuarios.Entities;
using FiapCloudGames.Users.Domain.Usuarios.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace FiapCloudGames.Users.Infrastructure.Data.Persistence.Configurations
{
    [ExcludeFromCodeCoverage]
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Nome).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Tipo).IsRequired().HasDefaultValue(TipoUsuario.Comum);
            builder.Property(u => u.DataCriacao).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            builder.Property(u => u.DataAtualizacao).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            builder.OwnsOne(u => u.Email, email =>
            {
                email.Property(e => e.Endereco)
                     .HasColumnName("Email")
                     .IsRequired()
                     .HasMaxLength(200);
            });

            builder.Property(x => x.SenhaHash)
                .HasMaxLength(500)
                .IsRequired();
        }
    }
}
