using Fonbec.Cartas.Logic.Services.Coordinador;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Microsoft.AspNetCore.Components;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class PadrinosList : PerFilialComponentBase
    {
        private List<PadrinosListViewModel> _viewModels = new();

        private bool _loading;
        private string _searchString = string.Empty;
        private bool _includeAll;

        [Inject]
        public IPadrinoService PadrinoService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var authenticatedUserData = await GetAuthenticatedUserDataAsync();
            if (!authenticatedUserData.DataObtainedSuccessfully)
            {
                _loading = false;
                return;
            }

            _viewModels = await PadrinoService.GetAllPadrinosAsync(authenticatedUserData.FilialId);

            _loading = false;
        }

        private bool Filter(PadrinosListViewModel padrinosListViewModel)
        {
            return string.IsNullOrWhiteSpace(_searchString)
                   || padrinosListViewModel.PadrinoFullName.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                   || (_includeAll &&
                       (padrinosListViewModel.PadrinoEmail.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                        || padrinosListViewModel.PadrinoPhone.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                        || padrinosListViewModel.BecariosActivos.Any(b => b.BecarioFullName.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                                                                          || (b.BecarioEmail is not null && b.BecarioEmail.Contains(_searchString, StringComparison.OrdinalIgnoreCase)))
                        || padrinosListViewModel.Cc.Any(cc => cc.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                        || padrinosListViewModel.Bcc.Any(cc => cc.Contains(_searchString, StringComparison.OrdinalIgnoreCase))));
        }
    }
}
