using Microsoft.EntityFrameworkCore;
using VentusServer.Database.Entities;
using VentusServer.Models;

namespace VentusServer.Database
{
    public class PostgresDbContext : DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options) { }

        // DbSets para las entidades
        public DbSet<AccountEntity> Accounts { get; set; } = null!;
        public DbSet<PlayerEntity> Players { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la relación entre AccountEntity y PlayerEntity
            modelBuilder.Entity<AccountEntity>()
                .HasMany(a => a.Players) // Una cuenta puede tener muchos jugadores
                .WithOne(p => p.Account) // Un jugador tiene una sola cuenta
                .HasForeignKey(p => p.AccountId) // La FK está en PlayerEntity
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina la cuenta, se eliminan los jugadores
        }
    }
}
