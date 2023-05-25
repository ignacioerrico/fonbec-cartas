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

            // Seed filiales
            modelBuilder.SeedFiliales();
        }
    }
}
