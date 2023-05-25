using Fonbec.Cartas.Logic.Services.Admin;
using Microsoft.AspNetCore.Components;

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

        private MudBlazor.MudForm _mudForm = default!;
        private MudBlazor.MudTextField<string> _mudTextFieldNombre = default!;

        private bool SaveButtonDisabled => _loading
                                           || !_formValidationSucceeded
                                           || string.Equals(_filialName, _originalFilialName, StringComparison.Ordinal);

        [Parameter]
        public string FilialId { get; set; } = string.Empty;

        [Inject] public IFilialService FilialService { get; set; } = default!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await _mudTextFieldNombre.FocusAsync();

            if (!_isNewFilial)
            {
                await _mudForm.Validate();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnInitializedAsync()
        {
            if (string.Equals(FilialId, "new", StringComparison.OrdinalIgnoreCase))
            {
                _isNewFilial = true;

                _pageTitle = "Filial nueva";
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
                    // TODO: Display message
                    NavigationManager.NavigateTo("/admin/filiales/new");
                }

                _filialName = _originalFilialName = filialName!;
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
                    // TODO: Display message
                }
            }
            else if (!string.Equals(_filialName, _originalFilialName, StringComparison.OrdinalIgnoreCase))
            {
                var id = int.Parse(FilialId);
                var filialesUpdated = await FilialService.UpdateFilialAsync(id, _filialName);
                if (filialesUpdated == 0)
                {
                    // TODO: Display message
                }
            }

            NavigationManager.NavigateTo("/admin/filiales");
        }
    }
}
