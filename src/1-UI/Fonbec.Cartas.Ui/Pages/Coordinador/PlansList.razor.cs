using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.Services.Coordinador;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class PlansList
    {
        private readonly List<PlansListViewModel> _planes = new();

        private bool _loading;
        private string _searchString = string.Empty;

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationState { get; set; }

        [Inject]
        public IPlanService PlanService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            if (AuthenticationState is null)
            {
                return;
            }

            var user = (await AuthenticationState).User;
            if (user.Identity is not { IsAuthenticated: true })
            {
                return;
            }

            var filialId = user.FilialId();

            if (filialId is null)
            {
                return;
            }

            var all = await PlanService.GetAllPlansAsync(filialId.Value);
            _planes.AddRange(all);

            _loading = false;
        }

        private bool Filter(PlansListViewModel padrinosListViewModel)
        {
            return string.IsNullOrWhiteSpace(_searchString)
                   || padrinosListViewModel.PlanName.Contains(_searchString, StringComparison.OrdinalIgnoreCase);
        }
    }
}
