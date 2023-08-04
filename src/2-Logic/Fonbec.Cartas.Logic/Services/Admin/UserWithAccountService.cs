using Fonbec.Cartas.DataAccess.Constants;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Actors.Abstract;
using Fonbec.Cartas.DataAccess.Identity;
using Fonbec.Cartas.DataAccess.Repositories.Admin;
using Fonbec.Cartas.Logic.Models;
using Fonbec.Cartas.Logic.Models.Results;
using Fonbec.Cartas.Logic.ViewModels.Admin;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace Fonbec.Cartas.Logic.Services.Admin
{
    public interface IUserWithAccountService<T> where T : UserWithAccount
    {
        Task<List<SelectableModel<int>>> GetAllFilialesAsSelectableAsync();
        Task<List<UsersWithAccountListViewModel>> GetAllUsersWithAccountAsync();
        Task<SearchResult<UserWithAccountEditViewModel>> GetUserWithAccountAsync(int id);
        Task<CrudErrorResult> CreateUserWithAccountAsync(UserWithAccountEditViewModel editViewModel, string initialPassword);
        Task<CrudErrorResult> UpdateUserWithAccountAsync(int id, UserWithAccountEditViewModel editViewModel);
    }

    public abstract class UserWithAccountService<T> : IUserWithAccountService<T> where T : UserWithAccount
    {
        private readonly IUserWithAccountRepositoryBase<T> _userWithAccountRepository;
        private readonly UserManager<FonbecUser> _userManager;
        private readonly IUserStore<FonbecUser> _userStore;

        protected UserWithAccountService(IUserWithAccountRepositoryBase<T> userWithAccountRepository,
            UserManager<FonbecUser> userManager,
            IUserStore<FonbecUser> userStore)
        {
            _userWithAccountRepository = userWithAccountRepository;
            _userManager = userManager;
            _userStore = userStore;
        }

        public async Task<List<SelectableModel<int>>> GetAllFilialesAsSelectableAsync()
        {
            var filiales = await _userWithAccountRepository.GetAllFilialesAsync();
            return filiales.Select(f => new SelectableModel<int>(f.Id, f.Name)).ToList();
        }

        public async Task<List<UsersWithAccountListViewModel>> GetAllUsersWithAccountAsync()
        {
            var usersWithAccount = await _userWithAccountRepository.GetAllAsync();
            var usersWithAccountListViewModel = usersWithAccount.Adapt<List<UsersWithAccountListViewModel>>();
            return usersWithAccountListViewModel;
        }

        public async Task<SearchResult<UserWithAccountEditViewModel>> GetUserWithAccountAsync(int id)
        {
            var userWithAccount = await _userWithAccountRepository.GetAsync(id);
            var userWithAccountEditViewModel = userWithAccount?.Adapt<UserWithAccountEditViewModel>();
            return new SearchResult<UserWithAccountEditViewModel>(userWithAccountEditViewModel);
        }

        public async Task<CrudErrorResult> CreateUserWithAccountAsync(UserWithAccountEditViewModel editViewModel, string initialPassword)
        {
            var crudResult = new CrudErrorResult();

            var user = new FonbecUser();

            await _userStore.SetUserNameAsync(user, editViewModel.Username, CancellationToken.None);
            await ((IUserEmailStore<FonbecUser>)_userStore).SetEmailAsync(user, editViewModel.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, initialPassword);

            if (!result.Succeeded)
            {
                return crudResult.AddErrors(result.Errors.Select(e => e.Description));
            }

            var role = typeof(T) switch
            {
                { } t when t == typeof(DataAccess.Entities.Actors.Coordinador) => FonbecRoles.Coordinador,
                { } t when t == typeof(DataAccess.Entities.Actors.Mediador) => FonbecRoles.Mediador,
                { } t when t == typeof(Revisor) => FonbecRoles.Revisor,
                _ => throw new ArgumentOutOfRangeException($"{nameof(UserWithAccountService<T>)}.{nameof(CreateUserWithAccountAsync)}: Unsupported type {typeof(T).Name} in switch statement.")
            };

            result = await _userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
            {
                return crudResult.AddErrors(result.Errors.Select(e => e.Description));
            }

            var userId = await _userManager.GetUserIdAsync(user);

            var userWithAccount = editViewModel.BuildAdapter()
                .AddParameters("userId", userId)
                .AdaptToType<T>();

            var rowsAffected = await _userWithAccountRepository.CreateAsync(userWithAccount);

            return crudResult.SetRowsAffected(rowsAffected);
        }

        public async Task<CrudErrorResult> UpdateUserWithAccountAsync(int id, UserWithAccountEditViewModel editViewModel)
        {
            var crudResult = new CrudErrorResult();

            var userWithAccount = editViewModel.BuildAdapter()
                .AddParameters("userId", editViewModel.Username)
                .AdaptToType<T>();

            var rowsAffected = await _userWithAccountRepository.UpdateAsync(id, userWithAccount);

            if (rowsAffected == 0)
            {
                return crudResult.AddError($"No se pudo actualizar el {typeof(T).Name}.");
            }

            var user = await _userManager.FindByIdAsync(editViewModel.AspNetUserId);

            if (user is null)
            {
                return crudResult.AddError($"Se actualizó el {typeof(T).Name}, pero no se encontró principal con ID {editViewModel.AspNetUserId}.");
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
                return crudResult.SetRowsAffected(rowsAffected);
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                crudResult.AddError($"Se actualizó el {typeof(T).Name}, pero no se pudo actualizar su identidad.");
                return crudResult.AddErrors(result.Errors.Select(e => e.Description));
            }

            return crudResult.SetRowsAffected(rowsAffected);
        }
    }
}
