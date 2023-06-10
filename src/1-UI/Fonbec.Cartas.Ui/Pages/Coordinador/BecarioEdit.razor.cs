using System.Security.Claims;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Fonbec.Cartas.Logic.Services.ServicesCoordinador;
using Fonbec.Cartas.Ui.Constants;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class BecarioEdit
    {
        private ClaimsPrincipal _user = default!;

        private readonly BecarioEditViewModel _becario = new();
        private readonly BecarioEditViewModel _originalBecario = new();

        private bool _loading;
        private bool _isNew;
        private string? _pageTitle;
        private string _saveButtonText = "Guardar";
        private bool _formValidationSucceeded;

        private MudTextField<string> _mudTextFieldNombre = default!;

        private NivelDeEstudio _selectedNivelDeEstudio = NivelDeEstudio.Primario;

        private List<MediadorViewModel> _mediadores = new();
        private MediadorViewModel? _selectedMediador;

        private bool SaveButtonDisabled => _loading
                                           || !_formValidationSucceeded
                                           || !ModelHasChanged;

        private bool ModelHasChanged =>
            !string.Equals(_becario.FirstName, _originalBecario.FirstName, StringComparison.Ordinal)
            || !string.Equals(_becario.LastName, _originalBecario.LastName, StringComparison.Ordinal)
            || !string.Equals(_becario.NickName, _originalBecario.NickName, StringComparison.Ordinal)
            || _becario.Gender != _originalBecario.Gender
            || !string.Equals(_becario.Email, _originalBecario.Email, StringComparison.Ordinal)
            || !string.Equals(_becario.Phone, _originalBecario.Phone, StringComparison.Ordinal);

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationState { get; set; }

        [Parameter]
        public string BecarioId { get; set; } = string.Empty;

        [Inject]
        public IBecarioService BecarioService { get; set; } = default!;

        [Inject]
        public IDialogService DialogService { get; set; } = default!;

        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                return;
            }

            await _mudTextFieldNombre.FocusAsync();

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            if (AuthenticationState is null)
            {
                Snackbar.Add("AuthenticationState is null.", Severity.Error);
                NavigationManager.NavigateTo(NavRoutes.CoordinadorPadrinos);
                return;
            }

            var user = (await AuthenticationState).User;
            if (user.Identity is not { IsAuthenticated: true })
            {
                Snackbar.Add("Usuario no está autenticado.", Severity.Error);
                NavigationManager.NavigateTo(NavRoutes.CoordinadorPadrinos);
                return;
            }

            _user = user;

            var filialId = user.FilialId();

            if (filialId is null)
            {
                Snackbar.Add("Filial no está en el claim.", Severity.Error);
                NavigationManager.NavigateTo(NavRoutes.CoordinadorPadrinos);
                return;
            }

            _becario.FilialId = filialId.Value;

            _mediadores = await BecarioService.GetAllMediadoresForSelectionAsync(filialId.Value);

            if (!_mediadores.Any())
            {
                Snackbar.Add("No hay mediadores.");
                NavigationManager.NavigateTo(NavRoutes.CoordinadorBecarios);
                return;
            }

            if (string.Equals(BecarioId, NavRoutes.New, StringComparison.OrdinalIgnoreCase))
            {
                _isNew = true;

                _pageTitle = "Alta de Becario";
                _saveButtonText = "Crear";
            }
            else if (int.TryParse(BecarioId, out var becarioId) && becarioId > 0)
            {
                _isNew = false;

                _pageTitle = "Editar Becario";
                _saveButtonText = "Actualizar";

                var becario = await BecarioService.GetBecarioAsync(becarioId, filialId.Value);

                if (becario is null)
                {
                    Snackbar.Add($"No se encontró becario con ID {becarioId}.", Severity.Error);
                    NavigationManager.NavigateTo(NavRoutes.CoordinadorPadrinoNew);
                    return;
                }

                _selectedNivelDeEstudio = becario.NivelDeEstudio;
                
                _selectedMediador = _mediadores.Single(m => m.Id == becario.MediadorId);

                _becario.FirstName = _originalBecario.FirstName = becario.FirstName;
                _becario.LastName = _originalBecario.LastName = becario.LastName;
                _becario.NickName = _originalBecario.NickName = becario.NickName;
                _becario.Gender = _originalBecario.Gender = becario.Gender;
                _becario.Email = _originalBecario.Email = becario.Email;
                _becario.Phone = _originalBecario.Phone = becario.Phone;
            }
            else
            {
                NavigationManager.NavigateTo(NavRoutes.CoordinadorPadrinos);
            }

            _loading = false;
        }

        private async Task<IEnumerable<MediadorViewModel>> SearchMediador(string searchString)
        {
            await Task.Delay(5);

            if (string.IsNullOrWhiteSpace(searchString))
            {
                return _mediadores;
            }

            return _mediadores.Where(m => m.Name.ContainsIgnoringAccents(searchString));
        }

        private async Task Save()
        {
            if (_selectedMediador is null)
            {
                return;
            }
            
            _becario.MediadorId = _selectedMediador.Id;
            _becario.NivelDeEstudio = _selectedNivelDeEstudio;

            if (_isNew)
            {
                _becario.CreatedByCoordinadorId = _user.UserWithAccountId() ?? throw new NullReferenceException("No claim UserWithAccountId found");

                var qtyAdded = await BecarioService.CreateAsync(_becario);

                if (qtyAdded == 0)
                {
                    Snackbar.Add("No se pudo crear el padrino.", Severity.Error);
                }
            }
            else if (ModelHasChanged)
            {
                _becario.UpdatedByCoordinadorId = _user.UserWithAccountId() ?? throw new NullReferenceException("No claim UserWithAccountId found");

                var becarioId = int.Parse(BecarioId);

                var qtyUpdated = await BecarioService.UpdateAsync(becarioId, _becario);

                if (qtyUpdated == 0)
                {
                    Snackbar.Add("No se pudo actualizar el padrino.", Severity.Error);
                }
            }

            NavigationManager.NavigateTo(NavRoutes.CoordinadorBecarios);
        }
    }
}
