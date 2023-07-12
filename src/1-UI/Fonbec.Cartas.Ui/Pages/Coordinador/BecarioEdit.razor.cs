using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.Models;
using Fonbec.Cartas.Logic.Services.Coordinador;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Fonbec.Cartas.Ui.Constants;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Mapster;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class BecarioEdit : PerFilialComponentBase
    {
        private BecarioEditViewModel _becario = new();
        private BecarioEditViewModel _originalBecario = new();
        private int _becarioId;

        private int _coordinadorId;

        private bool _loading;
        private bool _isNew;
        private string? _pageTitle;
        private string _saveButtonText = "Guardar";
        private bool _formValidationSucceeded;

        private MudTextField<string> _mudTextFieldNombre = default!;

        private NivelDeEstudio _selectedNivelDeEstudio = NivelDeEstudio.Primario;

        private List<SelectableModel<int>> _mediadores = new();
        private SelectableModel<int>? _selectedMediador;

        private bool SaveButtonDisabled => _loading
                                           || !_formValidationSucceeded
                                           || !ModelHasChanged;

        private bool ModelHasChanged =>
            !string.Equals(_becario.FirstName, _originalBecario.FirstName, StringComparison.Ordinal)
            || !string.Equals(_becario.LastName, _originalBecario.LastName, StringComparison.Ordinal)
            || !string.Equals(_becario.NickName, _originalBecario.NickName, StringComparison.Ordinal)
            || _becario.Gender != _originalBecario.Gender
            || _selectedNivelDeEstudio != _originalBecario.NivelDeEstudio
            || !string.Equals(_becario.Email, _originalBecario.Email, StringComparison.Ordinal)
            || !string.Equals(_becario.Phone, _originalBecario.Phone, StringComparison.Ordinal)
            || (_selectedMediador is not null && _selectedMediador.Id != _originalBecario.MediadorId);

        [Parameter]
        public string BecarioId { get; set; } = string.Empty;

        [Inject]
        public IBecarioService BecarioService { get; set; } = default!;

        [Inject]
        public IDialogService DialogService { get; set; } = default!;

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

            var authenticatedUserData = await GetAuthenticatedUserDataAsync();
            if (!authenticatedUserData.DataObtainedSuccessfully)
            {
                _loading = false;
                return;
            }

            _coordinadorId = authenticatedUserData.User.UserWithAccountId()
                             ?? throw new NullReferenceException("No claim UserWithAccountId found");

            _mediadores = await BecarioService.GetAllMediadoresForSelectionAsync(authenticatedUserData.FilialId);

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
                
                _becario.FilialId = authenticatedUserData.FilialId;
            }
            else if (int.TryParse(BecarioId, out _becarioId) && _becarioId > 0)
            {
                _isNew = false;

                _pageTitle = "Editar Becario";
                _saveButtonText = "Actualizar";

                var result = await BecarioService.GetBecarioAsync(_becarioId, authenticatedUserData.FilialId);

                if (!result.IsFound || result.Data is null)
                {
                    Snackbar.Add($"No se encontró becario con ID {_becarioId}.", Severity.Error);
                    NavigationManager.NavigateTo(NavRoutes.CoordinadorPadrinoNew);
                    return;
                }

                _becario = result.Data.Adapt<BecarioEditViewModel>();
                _originalBecario = result.Data.Adapt<BecarioEditViewModel>();
                
                _selectedNivelDeEstudio = result.Data.NivelDeEstudio;

                _selectedMediador = _mediadores.Single(m => m.Id == result.Data.MediadorId);
            }
            else
            {
                NavigationManager.NavigateTo(NavRoutes.CoordinadorPadrinos);
            }

            _loading = false;
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
                _becario.CreatedByCoordinadorId = _coordinadorId;

                var result = await BecarioService.CreateAsync(_becario);

                if (!result.AnyRowsAffected)
                {
                    Snackbar.Add("No se pudo crear el padrino.", Severity.Error);
                }
            }
            else if (ModelHasChanged)
            {
                _becario.UpdatedByCoordinadorId = _coordinadorId;

                var result = await BecarioService.UpdateAsync(_becarioId, _becario);

                if (!result.AnyRowsAffected)
                {
                    Snackbar.Add("No se pudo actualizar el padrino.", Severity.Error);
                }
            }

            NavigationManager.NavigateTo(NavRoutes.CoordinadorBecarios);
        }

        private async Task<IEnumerable<SelectableModel<int>>> SearchMediador(string searchString)
        {
            await Task.Delay(5);

            if (string.IsNullOrWhiteSpace(searchString))
            {
                return _mediadores;
            }

            return _mediadores.Where(m => m.DisplayName.ContainsIgnoringAccents(searchString));
        }
    }
}
