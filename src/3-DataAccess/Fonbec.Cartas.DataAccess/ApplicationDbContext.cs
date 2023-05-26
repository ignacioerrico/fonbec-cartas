using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.ExtensionMethods;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Filial> Filiales => Set<Filial>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("model");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Seed filiales - needs to run once all configurations have been applied
            modelBuilder.SeedFiliales();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            var addedEntities = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity);
            foreach (var addedEntity in addedEntities)
            {
                if (addedEntity is Auditable auditableEntity)
                {
                    auditableEntity.CreatedOnUtc = DateTimeOffset.UtcNow;
                }
            }

            var modifiedEntities = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified)
                .Select(e => e.Entity);
            foreach (var modifiedEntity in modifiedEntities)
            {
                switch (modifiedEntity)
                {
                    case Auditable { IsDeleted: true } auditableEntity:
                        auditableEntity.SoftDeletedOnUtc = DateTimeOffset.UtcNow;
                        break;
                    case Auditable auditableEntity:
                        auditableEntity.LastUpdatedOnUtc = DateTimeOffset.UtcNow;
                        break;
                }
            }
            
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
