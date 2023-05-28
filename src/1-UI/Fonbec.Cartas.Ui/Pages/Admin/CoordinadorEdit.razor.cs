using Fonbec.Cartas.Logic.Services.Admin;
using Fonbec.Cartas.Logic.ViewModels.Admin;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Pages.Admin
{
    public partial class CoordinadorEdit
    {
        private readonly CoordinadorEditViewModel _coordinador = new();
        private readonly CoordinadorEditViewModel _originalCoordinador = new();
        private string _initialPassword = string.Empty;

        private bool _loading;
        private bool _isNewCoordinador;
        private string? _pageTitle;
        private string _saveButtonText = "Guardar";
        private bool _formValidationSucceeded;
        private IEnumerable<string> _createCoordinadorErrors = Enumerable.Empty<string>();

        private MudForm _mudForm = default!;
        private MudTextField<string> _mudTextFieldNombre = default!;

        private List<FilialViewModel> _filiales = new();
        private FilialViewModel? _selectedFilial;

        private bool SaveButtonDisabled => _loading
                                           || !_formValidationSucceeded
                                           || !ModelHasChanged;

        private bool ModelHasChanged =>
            (_selectedFilial is not null && _originalCoordinador.FilialId != _selectedFilial.Id)
            || !string.Equals(_coordinador.FirstName, _originalCoordinador.FirstName, StringComparison.Ordinal)
            || !string.Equals(_coordinador.LastName, _originalCoordinador.LastName, StringComparison.Ordinal)
            || !string.Equals(_coordinador.NickName, _originalCoordinador.NickName, StringComparison.Ordinal)
            || _coordinador.Gender != _originalCoordinador.Gender
            || !string.Equals(_coordinador.Email, _originalCoordinador.Email, StringComparison.Ordinal)
            || !string.Equals(_coordinador.Phone, _originalCoordinador.Phone, StringComparison.Ordinal)
            || !string.Equals(_coordinador.Username, _originalCoordinador.Username, StringComparison.Ordinal);


        [Parameter]
        public string CoordinadorId { get; set; } = string.Empty;

        [Inject]
        public ICoordinadorService CoordinadorService { get; set; } = default!;

        [Inject]
        public IFilialService FilialService { get; set; } = default!;

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

            if (!_isNewCoordinador)
            {
                await _mudForm.Validate();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnInitializedAsync()
        {
            _filiales = await FilialService.GetAllFilialesForSelectionAsync();

            if (!_filiales.Any())
            {
                NavigationManager.NavigateTo("/admin/filiales");
            }

            if (string.Equals(CoordinadorId, "new", StringComparison.OrdinalIgnoreCase))
            {
                _isNewCoordinador = true;

                _pageTitle = "Alta de Coordinador";
                _saveButtonText = "Crear";

                _selectedFilial = _filiales.First();
            }
            else if (int.TryParse(CoordinadorId, out var coordinadorId) && coordinadorId > 0)
            {
                _isNewCoordinador = false;

                _pageTitle = "Editar coordinador";
                _saveButtonText = "Actualizar";

                _loading = true;
                var coordinador = await CoordinadorService.GetCoordinadorAsync(coordinadorId);
                _loading = false;

                if (coordinador is null)
                {
                    Snackbar.Add($"No se encontró coordinador con ID {coordinadorId}.", Severity.Error);
                    NavigationManager.NavigateTo("/admin/coordinadores/new");
                    return;
                }
                
                _selectedFilial = _filiales.Single(f => f.Id == coordinador.FilialId);

                _coordinador.FilialId = _originalCoordinador.FilialId = coordinador.FilialId;
                _coordinador.FirstName = _originalCoordinador.FirstName = coordinador.FirstName;
                _coordinador.LastName = _originalCoordinador.LastName = coordinador.LastName;
                _coordinador.NickName = _originalCoordinador.NickName = coordinador.NickName;
                _coordinador.Gender = _originalCoordinador.Gender = coordinador.Gender;
                _coordinador.Email = _originalCoordinador.Email = coordinador.Email;
                _coordinador.Phone = _originalCoordinador.Phone = coordinador.Phone;
                _coordinador.Username = _originalCoordinador.Username = coordinador.Username;

                _coordinador.AspNetUserId = coordinador.AspNetUserId;

                StateHasChanged();
            }
            else
            {
                NavigationManager.NavigateTo("/admin/coordinadores");
            }
        }

        private async Task Save()
        {
            if (_selectedFilial is null)
            {
                return;
            }

            _coordinador.FilialId = _selectedFilial.Id;

            if (_isNewCoordinador)
            {
                (var qtyCoordinadoresAdded, _createCoordinadorErrors) = await CoordinadorService.CreateCoordinadorAsync(_coordinador, _initialPassword);
                if (_createCoordinadorErrors.Any())
                {
                    return;
                }
                
                if (qtyCoordinadoresAdded == 0)
                {
                    Snackbar.Add("No se pudo crear el coordinador.", Severity.Error);
                }
            }
            else if (ModelHasChanged)
            {
                var id = int.Parse(CoordinadorId);
                var qtyCoordinadoresUpdated = await CoordinadorService.UpdateCoordinadorAsync(id, _coordinador);
                if (qtyCoordinadoresUpdated == 0)
                {
                    Snackbar.Add("No se pudo actualizar el coordinador.", Severity.Error);
                }
                else if (qtyCoordinadoresUpdated == -1)
                {
                    Snackbar.Add("Se actualizó el coordinador, pero no se pudo actualizar su identidad.", Severity.Error);
                }
            }

            NavigationManager.NavigateTo("/admin/coordinadores");
        }
    }
}
