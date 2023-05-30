using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.ExtensionMethods;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Fonbec.Cartas.DataAccess
{
    public sealed class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
            ChangeTracker.StateChanged += UpdateTimestamps;
            ChangeTracker.Tracked += UpdateTimestamps;
        }

        public DbSet<Filial> Filiales => Set<Filial>();

        public DbSet<Coordinador> Coordinadores => Set<Coordinador>();

        public DbSet<Mediador> Mediadores => Set<Mediador>();

        public DbSet<Revisor> Revisores => Set<Revisor>();

        public DbSet<Padrino> Padrinos => Set<Padrino>();

        public DbSet<SendAlsoTo> SendAlsoTo => Set<SendAlsoTo>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("model");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Seed filiales - needs to run once all configurations have been applied
            modelBuilder.SeedFiliales();
        }

        private static void UpdateTimestamps(object? sender, EntityEntryEventArgs e)
        {
            if (e.Entry.Entity is not Auditable auditable)
            {
                return;
            }
            
            switch (e.Entry.State)
            {
                case EntityState.Added:
                    auditable.CreatedOnUtc = DateTimeOffset.UtcNow;
                    break;
                case EntityState.Modified:
                    if (auditable.IsDeleted)
                    {
                        auditable.SoftDeletedOnUtc = DateTimeOffset.UtcNow;
                    }
                    else
                    {
                        auditable.LastUpdatedOnUtc = DateTimeOffset.UtcNow;
                    }
                    break;
            }
        }
    }
}
