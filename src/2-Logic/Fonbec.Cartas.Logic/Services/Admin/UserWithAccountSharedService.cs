using Fonbec.Cartas.DataAccess.Identity;
using Microsoft.AspNetCore.Identity;

namespace Fonbec.Cartas.Logic.Services.Admin;

public interface IUserWithAccountSharedService
{
    bool UsernameExists(string username);
    Task<(bool, List<string>)> ValidatePassword(string userName, string password);
}

public class UserWithAccountSharedService : IUserWithAccountSharedService
{
    private readonly UserManager<FonbecUser> _userManager;

    public UserWithAccountSharedService(UserManager<FonbecUser> userManager)
    {
        _userManager = userManager;
    }

    public bool UsernameExists(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return false;
        }

        return _userManager.Users.AsEnumerable()
            .Any(u => string.Equals(u.UserName, username, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<(bool, List<string>)> ValidatePassword(string userName, string password)
    {
        var errors = new List<string>();
        var isValid = true;

        var user = new FonbecUser { UserName = userName };

        foreach (var passwordValidator in _userManager.PasswordValidators)
        {
            var result = await passwordValidator.ValidateAsync(_userManager, user, password);
            if (result.Succeeded)
            {
                continue;
            }

            if (result.Errors.Any())
            {
                errors.AddRange(result.Errors.Select(e => e.Description));
            }

            isValid = false;
        }

        return (isValid, errors);
    }
}