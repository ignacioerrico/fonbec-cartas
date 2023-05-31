using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.Services.ServicesCoordinador;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class PadrinosList
    {
        private readonly List<PadrinosListViewModel> _padrinos = new();

        private bool _loading;
        private string _searchString = string.Empty;
        private bool _includeAll;

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationState { get; set; }

        [Inject]
        public IPadrinoService PadrinoService { get; set; } = default!;

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

            var all = await PadrinoService.GetAllPadrinosAsync(filialId.Value);
            _padrinos.AddRange(all);

            _loading = false;
        }

        private bool Filter(PadrinosListViewModel padrinosListViewModel)
        {
            return string.IsNullOrWhiteSpace(_searchString)
                   || padrinosListViewModel.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                   || (_includeAll &&
                       (padrinosListViewModel.Email.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                        || padrinosListViewModel.Phone.Contains(_searchString, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
