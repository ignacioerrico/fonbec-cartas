using Fonbec.Cartas.DataAccess.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.ExtensionMethods
{
    public static class ModelBuilderExtensionMethods
    {
        public static void SeedRoles(this ModelBuilder builder)
        {
            var roles = new[]
            {
                FonbecRoles.Admin,
                FonbecRoles.Coordinador,
                FonbecRoles.Mediador,
                FonbecRoles.Voluntario
            };

            var fonbecRoles = roles.Select(role =>
                    new IdentityRole
                    {
                        Name = role,
                        NormalizedName = role.ToUpper()
                    })
                .ToList();

            builder.Entity<IdentityRole>()
                .HasData(fonbecRoles);
        }
    }
}
