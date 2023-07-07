using Microsoft.AspNetCore.Components;
using Fonbec.Cartas.Logic.Services.Coordinador;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class PlansList : PerFilialComponentBase
    {
        private List<PlansListViewModel> _viewModels = new();

        private bool _loading;
        private string _searchString = string.Empty;

        [Inject]
        public IPlanService PlanService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var authenticatedUserData = await GetAuthenticatedUserDataAsync();
            if (!authenticatedUserData.DataObtainedSuccessfully)
            {
                _loading = false;
                return;
            }

            _viewModels = await PlanService.GetAllPlansAsync(authenticatedUserData.FilialId);

            _loading = false;
        }

        private bool Filter(PlansListViewModel padrinosListViewModel)
        {
            return string.IsNullOrWhiteSpace(_searchString)
                   || padrinosListViewModel.PlanName.Contains(_searchString, StringComparison.OrdinalIgnoreCase);
        }
    }
}
