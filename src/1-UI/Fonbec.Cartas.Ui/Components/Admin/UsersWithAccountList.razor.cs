using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.Logic.Services.Admin;
using Fonbec.Cartas.Logic.ViewModels.Admin;
using Fonbec.Cartas.Ui.Constants;
using Microsoft.AspNetCore.Components;

namespace Fonbec.Cartas.Ui.Components.Admin
{
    public partial class UsersWithAccountList<T>
        where T : EntityBase, IAmUserWithAccount, IHaveEmail
    {
        private readonly string _pageTitle;
        private readonly string _pathToNew;
        private readonly string _pathToEdit_0;

        private readonly List<UsersWithAccountListViewModel> _usersWithAccount = new();

        private bool _loading;
        private string _searchString = string.Empty;
        private bool _includeAll;

        public UsersWithAccountList()
        {
            _pageTitle = $"{typeof(T).Name}es";

            switch (typeof(T))
            {
                case { } t when t == typeof(Coordinador):
                    _pathToNew = NavRoutes.AdminCoordinadorNew;
                    _pathToEdit_0 = NavRoutes.AdminCoordinadorEdit_0;
                    break;
                case { } t when t == typeof(Mediador):
                    _pathToNew = NavRoutes.AdminMediadorNew;
                    _pathToEdit_0 = NavRoutes.AdminMediadorEdit_0;
                    break;
                case { } t when t == typeof(Revisor):
                    _pathToNew = NavRoutes.AdminRevisorNew;
                    _pathToEdit_0 = NavRoutes.AdminRevisorEdit_0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        $"{nameof(UserWithAccountEdit<T>)}.ctor: Unsupported type {typeof(T).Name} in switch statement.");
            }
        }

        [Inject]
        public UserWithAccountService<T> UserWithAccountService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var all = await UserWithAccountService.GetAllAsync();
            _usersWithAccount.AddRange(all);

            _loading = false;
        }

        private bool Filter(UsersWithAccountListViewModel usersWithAccountListViewModel)
        {
            return string.IsNullOrWhiteSpace(_searchString)
                   || usersWithAccountListViewModel.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                   || (_includeAll &&
                       (usersWithAccountListViewModel.Filial.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                        || usersWithAccountListViewModel.Email.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                        || usersWithAccountListViewModel.Phone.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                        || usersWithAccountListViewModel.Username.Contains(_searchString, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
