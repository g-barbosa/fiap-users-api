using FiapCloudGames.Users.Domain.Usuarios.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FiapCloudGames.Users.Infrastructure.Data.Persistence
{
    [ExcludeFromCodeCoverage]
    public class FiapCloudGamesDbContext(DbContextOptions<FiapCloudGamesDbContext> options) : DbContext(options)
    {
        public DbSet<Usuario> Usuarios => Set<Usuario>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FiapCloudGamesDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
