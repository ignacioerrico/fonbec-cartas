using System.ComponentModel.DataAnnotations;
using System.Text;
using Fonbec.Cartas.DataAccess.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Fonbec.Cartas.DataAccess.Constants;
using Fonbec.Cartas.Ui.Options;

namespace Fonbec.Cartas.Ui
{
    public static class Configure
    {
        public static void SeedAminUser(IServiceProvider services)
        {
            var adminUserOptions = services.GetService<IOptions<AdminUserOptions>>()?.Value
                                   ?? throw new NullReferenceException("AdminUserOptions could not be retrieved.");

            var attemptToCreateAdminUser = adminUserOptions.AttemptToCreateAdminUser;
            if (!attemptToCreateAdminUser)
            {
                return;
            }

            // Set in user secrets.
            var userName = adminUserOptions.UserName;
            var email = adminUserOptions.Email;
            var password = adminUserOptions.Password;

            if (userName is null || email is null || password is null)
            {
                throw new ValidationException("Some values in AdminUserOptions are not set.");
            }

            using var serviceScope = services.CreateScope();
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<FonbecUser>>();

            if (userManager.FindByNameAsync(userName).Result is not null)
            {
                return;
            }

            var adminUser = new FonbecUser
            {
                UserName = userName,
                NormalizedUserName = userName.ToUpper(),
                Email = email,
                NormalizedEmail = email.ToUpper()
            };

            var userCreationResult = userManager.CreateAsync(adminUser, password).Result;

            if (userCreationResult.Succeeded)
            {
                var roleAssignmentResult = userManager.AddToRoleAsync(adminUser, FonbecRoles.Admin).Result;
                if (!roleAssignmentResult.Succeeded)
                {
                    Halt("Could not add Admin role to Admin user.", roleAssignmentResult.Errors);
                }
            }
            else
            {
                Halt("Could not create admin user.", userCreationResult.Errors);
            }
        }

        private static void Halt(string errorMessage, IEnumerable<IdentityError> errors)
        {
            var sb = new StringBuilder(errorMessage);
            foreach (var error in errors)
            {
                sb.AppendLine(error.Description);
            }

            throw new ValidationException(sb.ToString());
        }
    }
}
