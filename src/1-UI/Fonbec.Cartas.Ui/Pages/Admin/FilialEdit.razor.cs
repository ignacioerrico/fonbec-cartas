using Fonbec.Cartas.Logic.Services.Admin;
using Fonbec.Cartas.Ui.Constants;
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

        private MudTextField<string> _mudTextFieldNombre = default!;

        private bool ModelHasChanged => !string.Equals(_filialName, _originalFilialName, StringComparison.Ordinal);

        private bool SaveButtonDisabled => _loading
                                           || !_formValidationSucceeded
                                           || !ModelHasChanged;

        [Parameter]
        public string FilialId { get; set; } = string.Empty;

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

                var searchResult = await FilialService.GetFilialNameAsync(filialId);

                _loading = false;

                if (!searchResult.IsFound)
                {
                    Snackbar.Add($"No se encontró filial con ID {filialId}.", Severity.Error);
                    NavigationManager.NavigateTo(NavRoutes.AdminFilialNew);
                    return;
                }

                _filialName = _originalFilialName = searchResult.Data!;
            }
            else
            {
                NavigationManager.NavigateTo(NavRoutes.AdminFiliales);
            }
        }

        private async Task Save()
        {
            if (_isNewFilial)
            {
                var result = await FilialService.CreateFilialAsync(_filialName);
                if (!result.IsSuccess)
                {
                    Snackbar.Add($"No se pudo crear la filial.", Severity.Error);
                }
            }
            else if (ModelHasChanged)
            {
                var id = int.Parse(FilialId);
                var result = await FilialService.UpdateFilialAsync(id, _filialName);
                if (!result.IsSuccess)
                {
                    Snackbar.Add($"No se pudo actualizar la filial.", Severity.Error);
                }
            }

            NavigationManager.NavigateTo(NavRoutes.AdminFiliales);
        }
    }
}
