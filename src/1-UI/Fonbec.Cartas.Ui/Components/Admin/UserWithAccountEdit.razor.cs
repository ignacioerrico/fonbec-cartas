using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Actors.Abstract;
using Fonbec.Cartas.Logic.Models;
using Fonbec.Cartas.Logic.Services.Admin;
using Fonbec.Cartas.Logic.ViewModels.Admin;
using Fonbec.Cartas.Ui.Constants;
using Mapster;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Components.Admin
{
    public partial class UserWithAccountEdit<T>
        where T : UserWithAccount
    {
        private readonly string _pathToList;
        private readonly string _pathToNew;
        private readonly string _pageTitleNew;
        private readonly string _pageTitleEdit;

        private UserWithAccountEditViewModel _userWithAccount = new();
        private UserWithAccountEditViewModel _originalUserWithAccount = new();
        private string _initialPassword = string.Empty;

        private bool _loading;
        private bool _isNew;
        private string? _pageTitle;
        private string _saveButtonText = "Guardar";
        private bool _formValidationSucceeded;
        private IEnumerable<string> _createErrors = Enumerable.Empty<string>();

        private MudTextField<string> _mudTextFieldNombre = default!;

        private List<SelectableModel<int>> _filiales = new();
        private SelectableModel<int>? _selectedFilial;

        private bool SaveButtonDisabled => _loading
                                           || !_formValidationSucceeded
                                           || !ModelHasChanged;

        private bool ModelHasChanged =>
            (_selectedFilial is not null && _originalUserWithAccount.FilialId != _selectedFilial.Id)
            || !string.Equals(_userWithAccount.FirstName, _originalUserWithAccount.FirstName, StringComparison.Ordinal)
            || !string.Equals(_userWithAccount.LastName, _originalUserWithAccount.LastName, StringComparison.Ordinal)
            || !string.Equals(_userWithAccount.NickName, _originalUserWithAccount.NickName, StringComparison.Ordinal)
            || _userWithAccount.Gender != _originalUserWithAccount.Gender
            || !string.Equals(_userWithAccount.Email, _originalUserWithAccount.Email, StringComparison.Ordinal)
            || !string.Equals(_userWithAccount.Phone, _originalUserWithAccount.Phone, StringComparison.Ordinal)
            || !string.Equals(_userWithAccount.Username, _originalUserWithAccount.Username, StringComparison.Ordinal);

        public UserWithAccountEdit()
        {
            switch (typeof(T))
            {
                case { } t when t == typeof(DataAccess.Entities.Actors.Coordinador):
                    _pathToList = NavRoutes.AdminCoordinadores;
                    _pathToNew = NavRoutes.AdminCoordinadorNew;
                    _pageTitleNew = "Alta de Coordinador";
                    _pageTitleEdit = "Editar Coordinador";
                    break;
                case { } t when t == typeof(Mediador):
                    _pathToList = NavRoutes.AdminMediadores;
                    _pathToNew = NavRoutes.AdminMediadorNew;
                    _pageTitleNew = "Alta de Mediador";
                    _pageTitleEdit = "Editar Mediador";
                    break;
                case { } t when t == typeof(Revisor):
                    _pathToList = NavRoutes.AdminRevisores;
                    _pathToNew = NavRoutes.AdminRevisorNew;
                    _pageTitleNew = "Alta de Revisor";
                    _pageTitleEdit = "Editar Revisor";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        $"{nameof(UserWithAccountEdit<T>)}.ctor: Unsupported type {typeof(T).Name} in switch statement.");
            }
        }

        [Parameter]
        public string UserWithAccountId { get; set; } = string.Empty;

        [Inject]
        public IUserWithAccountService<T> UserWithAccountService { get; set; } = default!;

        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _mudTextFieldNombre.FocusAsync();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            _filiales = await UserWithAccountService.GetAllFilialesAsSelectableAsync();

            if (!_filiales.Any())
            {
                Snackbar.Add("No hay filiales.");
                NavigationManager.NavigateTo(NavRoutes.AdminFiliales);
                return;
            }

            if (string.Equals(UserWithAccountId, NavRoutes.New, StringComparison.OrdinalIgnoreCase))
            {
                _isNew = true;

                _pageTitle = _pageTitleNew;
                _saveButtonText = "Crear";

                _selectedFilial = _filiales.First();
            }
            else if (int.TryParse(UserWithAccountId, out var userWithAccountId) && userWithAccountId > 0)
            {
                _isNew = false;

                _pageTitle = _pageTitleEdit;
                _saveButtonText = "Actualizar";

                var result = await UserWithAccountService.GetUserWithAccountAsync(userWithAccountId);

                if (!result.IsFound)
                {
                    Snackbar.Add($"No se encontró {typeof(T).Name} con ID {userWithAccountId}.", Severity.Error);
                    NavigationManager.NavigateTo(_pathToNew);
                    return;
                }

                _selectedFilial = _filiales.Single(f => f.Id == result.Data!.FilialId);

                _userWithAccount = result.Data!.Adapt<UserWithAccountEditViewModel>();
                _originalUserWithAccount = result.Data!.Adapt<UserWithAccountEditViewModel>();
            }
            else
            {
                NavigationManager.NavigateTo(_pathToList);
            }

            _loading = false;
        }

        private async Task Save()
        {
            if (_selectedFilial is null)
            {
                return;
            }

            _userWithAccount.FilialId = _selectedFilial.Id;

            if (_isNew)
            {
                var result = await UserWithAccountService.CreateUserWithAccountAsync(_userWithAccount, _initialPassword);

                _createErrors = result.Errors;

                if (result.AnyErrors)
                {
                    return;
                }

                if (!result.AnyRowsAffected)
                {
                    Snackbar.Add($"No se pudo crear el {typeof(T).Name}.", Severity.Error);
                }
            }
            else if (ModelHasChanged)
            {
                var id = int.Parse(UserWithAccountId);
                var result = await UserWithAccountService.UpdateUserWithAccountAsync(id, _userWithAccount);
                
                foreach (var error in result.Errors)
                {
                    Snackbar.Add(error, Severity.Error);
                }
            }

            NavigationManager.NavigateTo(_pathToList);
        }

        public IEnumerable<string> ValidateUsername(string username)
        {
            var usernameExists = UserWithAccountService.UsernameExists(username);
            if (usernameExists)
            {
                yield return "Ya existe un usuario con ese nombre";
            }
        }
    }
}
