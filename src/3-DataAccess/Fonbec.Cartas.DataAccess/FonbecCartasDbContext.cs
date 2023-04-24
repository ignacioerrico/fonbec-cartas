using Fonbec.Cartas.DataAccess.ExtensionMethods;
using Fonbec.Cartas.DataAccess.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess;

public class FonbecCartasDbContext : IdentityDbContext<FonbecUser>
{
    public FonbecCartasDbContext(DbContextOptions<FonbecCartasDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Seed roles
        builder.SeedRoles();
    }
}
