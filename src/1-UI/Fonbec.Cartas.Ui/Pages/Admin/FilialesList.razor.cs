using Fonbec.Cartas.Logic.Services.Admin;
using Fonbec.Cartas.Logic.ViewModels.Admin;
using Microsoft.AspNetCore.Components;

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
    }
}
