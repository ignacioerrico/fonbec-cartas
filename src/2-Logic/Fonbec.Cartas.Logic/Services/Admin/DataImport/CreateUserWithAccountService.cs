using Fonbec.Cartas.DataAccess.Constants;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Actors.Abstract;
using Fonbec.Cartas.DataAccess.Identity;
using Fonbec.Cartas.DataAccess.Repositories.Admin.DataImport;
using Fonbec.Cartas.Logic.Models.Admin.DataImport;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace Fonbec.Cartas.Logic.Services.Admin.DataImport
{
    public interface ICreateUserWithAccountService<T> where T : UserWithAccount
    {
        Task<List<T>> CreateAsync(int filialId, List<UserWithAccountToCreate> users, List<string> errors);
    }

    public class CreateUserWithAccountService<T> : ICreateUserWithAccountService<T> where T : UserWithAccount
    {
        private readonly UserManager<FonbecUser> _userManager;
        private readonly IUserStore<FonbecUser> _userStore;
        private readonly ICreateUserWithAccountRepository<T> _createUserWithAccountRepository;

        public CreateUserWithAccountService(UserManager<FonbecUser> userManager,
            IUserStore<FonbecUser> userStore,
            ICreateUserWithAccountRepository<T> createUserWithAccountRepository)
        {
            _userManager = userManager;
            _userStore = userStore;
            _createUserWithAccountRepository = createUserWithAccountRepository;
        }

        public async Task<List<T>> CreateAsync(int filialId, List<UserWithAccountToCreate> users, List<string> errors)
        {
            List<T> usersCreated = new();

            foreach (var user in users)
            {
                var userDb = await _userManager.FindByNameAsync(user.Username);
                if (userDb is not null)
                {
                     errors.Add($"A user with the username '{user.Username}' already exists");
                     continue;
                }

                var fonbecUser = new FonbecUser();

                await _userStore.SetUserNameAsync(fonbecUser, user.Username, CancellationToken.None);
                await ((IUserEmailStore<FonbecUser>)_userStore).SetEmailAsync(fonbecUser, user.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(fonbecUser, user.Password);

                if (!result.Succeeded)
                {
                    errors.AddRange(result.Errors.Select(e => e.Description));
                    continue;
                }

                var role = typeof(T) switch
                {
                    { } t when t == typeof(DataAccess.Entities.Actors.Coordinador) => FonbecRoles.Coordinador,
                    { } t when t == typeof(Mediador) => FonbecRoles.Mediador,
                    { } t when t == typeof(Revisor) => FonbecRoles.Revisor,
                    _ => throw new ArgumentOutOfRangeException($"{nameof(UserWithAccountService<T>)}.{nameof(CreateAsync)}: Unsupported type {typeof(T).Name} in switch statement.")
                };

                result = await _userManager.AddToRoleAsync(fonbecUser, role);

                if (!result.Succeeded)
                {
                    errors.AddRange(result.Errors.Select(e => e.Description));
                    continue;
                }

                var userId = await _userManager.GetUserIdAsync(fonbecUser);

                var userWithAccount = user.BuildAdapter()
                    .AddParameters("filialId", filialId)
                    .AddParameters("userId", userId)
                    .AdaptToType<T>();

                usersCreated.Add(userWithAccount);
            }

            usersCreated = await _createUserWithAccountRepository.CreateAsync(usersCreated);

            return usersCreated;
        }
    }
}
