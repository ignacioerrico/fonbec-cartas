using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Microsoft.AspNetCore.Components;
using Fonbec.Cartas.Logic.Services.Coordinador;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class BecariosList : PerFilialComponentBase
    {
        private List<BecariosListViewModel> _viewModels = new();

        private bool _loading;
        private string _searchString = string.Empty;
        private bool _includeAll;

        [Inject]
        public IBecarioService BecarioService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var authenticatedUserData = await GetAuthenticatedUserDataAsync();
            if (!authenticatedUserData.DataObtainedSuccessfully)
            {
                _loading = false;
                return;
            }

            _viewModels = await BecarioService.GetAllBecariosAsync(authenticatedUserData.FilialId);
            
            _loading = false;
        }

        private bool Filter(BecariosListViewModel becariosListViewModel)
        {
            return string.IsNullOrWhiteSpace(_searchString)
                   || becariosListViewModel.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                   || (_includeAll &&
                       (becariosListViewModel.Mediador.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                        || becariosListViewModel.PadrinosActivos.Any(pa => pa.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                        || becariosListViewModel.PadrinosFuturos.Any(pf => pf.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                        || becariosListViewModel.Email.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                        || becariosListViewModel.Phone.Contains(_searchString, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
