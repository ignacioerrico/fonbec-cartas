using Fonbec.Cartas.Logic.Services.Admin;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Pages.Admin
{
    public partial class FilialEdit
    {
        private string _filialName = string.Empty;
        private string _originalFilialName = string.Empty;

        private bool _loading;
        private bool _isNewFilial;
        private string? _pageTitle;
        private string _saveButtonText = "Guardar";
        private bool _formValidationSucceeded;

        private MudForm _mudForm = default!;
        private MudTextField<string> _mudTextFieldNombre = default!;

        private bool SaveButtonDisabled => _loading
                                           || !_formValidationSucceeded
                                           || string.Equals(_filialName, _originalFilialName, StringComparison.Ordinal);

        [Parameter]
        public string FilialId { get; set; } = string.Empty;

        [Inject] public IFilialService FilialService { get; set; } = default!;

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
            if (string.Equals(FilialId, "new", StringComparison.OrdinalIgnoreCase))
            {
                _isNewFilial = true;

                _pageTitle = "Alta de Filial";
                _saveButtonText = "Crear";
            }
            else if (int.TryParse(FilialId, out var filialId) && filialId > 0)
            {
                _isNewFilial = false;

                _pageTitle = "Editar filial";
                _saveButtonText = "Actualizar";

                _loading = true;
                var filialName = await FilialService.GetFilialNameAsync(filialId);
                _loading = false;

                if (filialName is null)
                {
                    Snackbar.Add($"No se encontró filial con ID {filialId}.", Severity.Error);
                    NavigationManager.NavigateTo("/admin/filiales/new");
                    return;
                }

                _filialName = _originalFilialName = filialName;
            }
            else
            {
                NavigationManager.NavigateTo("/admin/filiales");
            }
        }

        private async Task Save()
        {
            if (_isNewFilial)
            {
                var filialesAdded = await FilialService.CreateFilialAsync(_filialName);
                if (filialesAdded == 0)
                {
                    Snackbar.Add($"No se pudo crear la filial.", Severity.Error);
                }
            }
            else if (!string.Equals(_filialName, _originalFilialName, StringComparison.OrdinalIgnoreCase))
            {
                var id = int.Parse(FilialId);
                var filialesUpdated = await FilialService.UpdateFilialAsync(id, _filialName);
                if (filialesUpdated == 0)
                {
                    Snackbar.Add($"No se pudo actualizar la filial.", Severity.Error);
                }
            }

            NavigationManager.NavigateTo("/admin/filiales");
        }
    }
}
