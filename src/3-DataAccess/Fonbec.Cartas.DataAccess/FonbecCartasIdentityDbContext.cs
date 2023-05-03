using Fonbec.Cartas.DataAccess.ExtensionMethods;
using Fonbec.Cartas.DataAccess.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess;

public class FonbecCartasIdentityDbContext : IdentityDbContext<FonbecUser>
{
    public FonbecCartasIdentityDbContext(DbContextOptions<FonbecCartasIdentityDbContext> options)
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
