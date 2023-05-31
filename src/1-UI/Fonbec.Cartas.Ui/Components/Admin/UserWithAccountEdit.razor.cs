using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.Logic.Services.Admin;
using Fonbec.Cartas.Logic.ViewModels.Admin;
using Fonbec.Cartas.Ui.Constants;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Components.Admin
{
    public partial class UserWithAccountEdit<T>
        where T : EntityBase, IAmUserWithAccount, IHaveEmail
    {
        private readonly string _pathToList;
        private readonly string _pathToNew;
        private readonly string _pageTitleNew;
        private readonly string _pageTitleEdit;

        private readonly UserWithAccountEditViewModel _userWithAccount = new();
        private readonly UserWithAccountEditViewModel _originalUserWithAccount = new();
        private string _initialPassword = string.Empty;

        private bool _loading;
        private bool _isNew;
        private string? _pageTitle;
        private string _saveButtonText = "Guardar";
        private bool _formValidationSucceeded;
        private IEnumerable<string> _createErrors = Enumerable.Empty<string>();

        private MudTextField<string> _mudTextFieldNombre = default!;

        private List<FilialViewModel> _filiales = new();
        private FilialViewModel? _selectedFilial;

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
                case { } t when t == typeof(Coordinador):
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
        public UserWithAccountService<T> UserWithAccountService { get; set; } = default!;

        [Inject]
        public IFilialService FilialService { get; set; } = default!;

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

            _filiales = await FilialService.GetAllFilialesForSelectionAsync();

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

                var userWithAccount = await UserWithAccountService.GetAsync(userWithAccountId);

                if (userWithAccount is null)
                {
                    Snackbar.Add($"No se encontró {typeof(T).Name} con ID {userWithAccountId}.", Severity.Error);
                    NavigationManager.NavigateTo(_pathToNew);
                    return;
                }

                _selectedFilial = _filiales.Single(f => f.Id == userWithAccount.FilialId);

                _userWithAccount.FilialId = _originalUserWithAccount.FilialId = userWithAccount.FilialId;
                _userWithAccount.FirstName = _originalUserWithAccount.FirstName = userWithAccount.FirstName;
                _userWithAccount.LastName = _originalUserWithAccount.LastName = userWithAccount.LastName;
                _userWithAccount.NickName = _originalUserWithAccount.NickName = userWithAccount.NickName;
                _userWithAccount.Gender = _originalUserWithAccount.Gender = userWithAccount.Gender;
                _userWithAccount.Email = _originalUserWithAccount.Email = userWithAccount.Email;
                _userWithAccount.Phone = _originalUserWithAccount.Phone = userWithAccount.Phone;
                _userWithAccount.Username = _originalUserWithAccount.Username = userWithAccount.Username;

                _userWithAccount.AspNetUserId = userWithAccount.AspNetUserId;
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
                (var qtyAdded, _createErrors) = await UserWithAccountService.CreateAsync(_userWithAccount, _initialPassword);
                if (_createErrors.Any())
                {
                    return;
                }

                if (qtyAdded == 0)
                {
                    Snackbar.Add($"No se pudo crear el {typeof(T).Name}.", Severity.Error);
                }
            }
            else if (ModelHasChanged)
            {
                var id = int.Parse(UserWithAccountId);
                var qtyUpdated = await UserWithAccountService.UpdateAsync(id, _userWithAccount);
                if (qtyUpdated == 0)
                {
                    Snackbar.Add($"No se pudo actualizar el {typeof(T).Name}.", Severity.Error);
                }
                else if (qtyUpdated == -1)
                {
                    Snackbar.Add($"Se actualizó el {typeof(T).Name}, pero no se pudo actualizar su identidad.", Severity.Error);
                }
            }

            NavigationManager.NavigateTo(_pathToList);
        }
    }
}
