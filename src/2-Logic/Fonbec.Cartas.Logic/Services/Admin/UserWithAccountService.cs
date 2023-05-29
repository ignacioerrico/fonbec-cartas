using Fonbec.Cartas.DataAccess.Constants;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Identity;
using Fonbec.Cartas.DataAccess.Repositories;
using Fonbec.Cartas.Logic.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;

namespace Fonbec.Cartas.Logic.Services.Admin
{
 public abstract class UserWithAccountService<T>
        where T : EntityBase, IAmUserWithAccount, IHaveEmail
    {
        private readonly UserWithAccountRepositoryBase<T> _userWithAccountRepository;
        private readonly UserManager<FonbecUser> _userManager;
        private readonly IUserStore<FonbecUser> _userStore;

        protected UserWithAccountService(UserWithAccountRepositoryBase<T> userWithAccountRepository,
            UserManager<FonbecUser> userManager,
            IUserStore<FonbecUser> userStore)
        {
            _userWithAccountRepository = userWithAccountRepository;
            _userManager = userManager;
            _userStore = userStore;
        }

        public async Task<List<UsersWithAccountListViewModel>> GetAllAsync()
        {
            var all = await _userWithAccountRepository.GetAllAsync();

            return all.Select(c => new UsersWithAccountListViewModel
            {
                Id = c.Id,
                Name = c.FullName(includeNickName: true),
                Gender = c.Gender,
                Filial = c.Filial.Name,
                Email = c.Email,
                Phone = c.Phone ?? string.Empty,
                Username = c.Username,
                CreatedOnUtc = c.CreatedOnUtc,
                LastUpdatedOnUtc = c.LastUpdatedOnUtc
            }).ToList();
        }

        public async Task<UserWithAccountEditViewModel?> GetAsync(int id)
        {
            var single = await _userWithAccountRepository.GetAsync(id);

            if (single is null)
            {
                return null;
            }

            var editViewModel = new UserWithAccountEditViewModel
            {
                FilialId = single.FilialId,
                FirstName = single.FirstName,
                LastName = single.LastName,
                NickName = single.NickName ?? string.Empty,
                Gender = single.Gender,
                Email = single.Email,
                Phone = single.Phone ?? string.Empty,
                Username = single.Username,
                AspNetUserId = single.AspNetUserId,
            };

            return editViewModel;
        }

        public async Task<(int, IEnumerable<string>)> CreateAsync(UserWithAccountEditViewModel editViewModel, string initialPassword)
        {
            var user = new FonbecUser();

            await _userStore.SetUserNameAsync(user, editViewModel.Username, CancellationToken.None);
            await ((IUserEmailStore<FonbecUser>)_userStore).SetEmailAsync(user, editViewModel.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, initialPassword);

            if (!result.Succeeded)
            {
                return (0, result.Errors.Select(e => e.Description));
            }

            var role = typeof(T) switch
            {
                { } t when t == typeof(Coordinador) => FonbecRoles.Coordinador,
                { } t when t == typeof(Mediador) => FonbecRoles.Mediador,
                { } t when t == typeof(Revisor) => FonbecRoles.Revisor,
                _ => throw new ArgumentOutOfRangeException($"{nameof(UserWithAccountService<T>)}.{nameof(CreateAsync)}: Unsupported type {typeof(T).Name} in switch statement.")
            };

            result = await _userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
            {
                return (0, result.Errors.Select(e => e.Description));
            }

            var userId = await _userManager.GetUserIdAsync(user);

            T userWithAccount;

            try
            {
                userWithAccount = Activator.CreateInstance<T>();
            }
            catch (Exception ex)
            {
                return (0, new [] { ex.Message });
            }

            userWithAccount.FilialId = editViewModel.FilialId;
            userWithAccount.FirstName = editViewModel.FirstName;
            userWithAccount.LastName = editViewModel.LastName;
            userWithAccount.NickName = string.IsNullOrWhiteSpace(editViewModel.NickName) ? null : editViewModel.NickName;
            userWithAccount.Gender = editViewModel.Gender;
            userWithAccount.Email = editViewModel.Email;
            userWithAccount.Phone = editViewModel.Phone;
            userWithAccount.Username = editViewModel.Username;
            userWithAccount.AspNetUserId = userId;

            var rowsAffected = await _userWithAccountRepository.CreateAsync(userWithAccount);

            return (rowsAffected, Enumerable.Empty<string>());
        }

        public async Task<int> UpdateAsync(int id, UserWithAccountEditViewModel editViewModel)
        {
            T userWithAccount;

            try
            {
                userWithAccount = Activator.CreateInstance<T>();
            }
            catch
            {
                return 0;
            }

            userWithAccount.FilialId = editViewModel.FilialId;
            userWithAccount.FirstName = editViewModel.FirstName;
            userWithAccount.LastName = editViewModel.LastName;
            userWithAccount.NickName = string.IsNullOrWhiteSpace(editViewModel.NickName) ? null : editViewModel.NickName;
            userWithAccount.Gender = editViewModel.Gender;
            userWithAccount.Email = editViewModel.Email;
            userWithAccount.Phone = editViewModel.Phone;
            userWithAccount.Username = editViewModel.Username;

            var affectedRows = await _userWithAccountRepository.UpdateAsync(id, userWithAccount);

            if (affectedRows == 0)
            {
                return 0;
            }

            var user = await _userManager.FindByIdAsync(editViewModel.AspNetUserId);

            if (user is null)
            {
                return -1;
            }

            var usernameHasChanged = !string.Equals(user.UserName, editViewModel.Username, StringComparison.OrdinalIgnoreCase);
            if (usernameHasChanged)
            {
                await _userStore.SetUserNameAsync(user, editViewModel.Username, CancellationToken.None);
            }

            var emailHasChanged = !string.Equals(user.Email, editViewModel.Email, StringComparison.OrdinalIgnoreCase);
            if (emailHasChanged)
            {
                await ((IUserEmailStore<FonbecUser>)_userStore).SetEmailAsync(user, editViewModel.Email, CancellationToken.None);
            }

            if (!usernameHasChanged && !emailHasChanged)
            {
                return affectedRows;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return -1;
            }

            return affectedRows;
        }
    }
}
