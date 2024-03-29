﻿using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Actors.Abstract;
using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.Services.Admin;
using Fonbec.Cartas.Logic.ViewModels.Admin;
using Fonbec.Cartas.Ui.Constants;
using Microsoft.AspNetCore.Components;

namespace Fonbec.Cartas.Ui.Components.Admin
{
    public partial class UsersWithAccountList<T>
        where T : UserWithAccount
    {
        private List<UsersWithAccountListViewModel> _viewModels = new();

        private readonly string _pageTitle;
        private readonly string _pathToNew;
        private readonly string _pathToEdit_0;

        private bool _loading;
        private string _searchString = string.Empty;
        private bool _includeAll;

        public UsersWithAccountList()
        {
            _pageTitle = $"{typeof(T).Name}es";

            switch (typeof(T))
            {
                case { } t when t == typeof(DataAccess.Entities.Actors.Coordinador):
                    _pathToNew = NavRoutes.AdminCoordinadorNew;
                    _pathToEdit_0 = NavRoutes.AdminCoordinadorEdit0;
                    break;
                case { } t when t == typeof(Mediador):
                    _pathToNew = NavRoutes.AdminMediadorNew;
                    _pathToEdit_0 = NavRoutes.AdminMediadorEdit0;
                    break;
                case { } t when t == typeof(Revisor):
                    _pathToNew = NavRoutes.AdminRevisorNew;
                    _pathToEdit_0 = NavRoutes.AdminRevisorEdit0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        $"{nameof(UserWithAccountEdit<T>)}.ctor: Unsupported type {typeof(T).Name} in switch statement.");
            }
        }

        [Inject]
        public IUserWithAccountService<T> UserWithAccountService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            _viewModels = await UserWithAccountService.GetAllUsersWithAccountAsync();

            _loading = false;
        }

        private bool Filter(UsersWithAccountListViewModel usersWithAccountListViewModel)
        {
            return string.IsNullOrWhiteSpace(_searchString)
                   || usersWithAccountListViewModel.UserWithAccountFullName.ContainsIgnoringAccents(_searchString)
                   || (_includeAll &&
                       (usersWithAccountListViewModel.FilialName.ContainsIgnoringAccents(_searchString)
                        || usersWithAccountListViewModel.Email.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                        || usersWithAccountListViewModel.Phone.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                        || usersWithAccountListViewModel.Username.ContainsIgnoringAccents(_searchString)));
        }
    }
}
