using Fonbec.Cartas.Logic.Services.Admin;
using Fonbec.Cartas.Logic.ViewModels.Admin;
using Fonbec.Cartas.Ui.Components.Dialogs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Pages.Admin
{
    public partial class FilialesList
    {
        private readonly List<FilialesListViewModel> _filiales = new();

        private bool _loading;
        private string _searchString = string.Empty;
        private bool _includeCoordinadores;

        [Inject]
        public IFilialService FilialService { get; set; } = default!;

        [Inject]
        public IDialogService DialogService { get; set; } = default!;

        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var filiales = await FilialService.GetAllFilialesAsync();
            _filiales.AddRange(filiales);

            _loading = false;
        }

        private bool Filter(FilialesListViewModel filialesListViewModel)
        {
            return string.IsNullOrWhiteSpace(_searchString)
                   || filialesListViewModel.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                   || (_includeCoordinadores
                       && filialesListViewModel.Coordinadores.Any(c =>
                           c.Contains(_searchString, StringComparison.OrdinalIgnoreCase)));
        }

        private async Task OpenDeleteDialog(int id, string filialName)
        {
            var parameters = new DialogParameters { ["ItemToDelete"] = filialName };
            var options = new DialogOptions
            {
                CloseButton = true,
                CloseOnEscapeKey = true
            };
            var dialog = await DialogService.ShowAsync<ConfirmDeleteDialog>("¡Atención!", parameters, options);
            var result = await dialog.Result;

            if (result.Canceled)
            {
                return;
            }

            var qtyFilialesSoftDeleted = await FilialService.SoftDeleteAsync(id);
            if (qtyFilialesSoftDeleted == 0)
            {
                Snackbar.Add($"Error al borrar '{filialName}'", Severity.Error);
                return;
            }

            Snackbar.Add($"'{filialName}' fue borrado", Severity.Success);

            var indexToDelete = _filiales.FindIndex(f => f.Id == id);
            _filiales.RemoveAt(indexToDelete);
        }
    }
}
