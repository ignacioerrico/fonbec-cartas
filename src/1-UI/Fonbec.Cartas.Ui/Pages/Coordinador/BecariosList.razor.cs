using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.Services.ServicesCoordinador;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class BecariosList
    {
        private readonly List<BecariosListViewModel> _becarios = new();

        private bool _loading;
        private string _searchString = string.Empty;
        private bool _includeAll;

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationState { get; set; }

        [Inject]
        public IBecarioService BecarioService { get; set; } = default!;

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

            var all = await BecarioService.GetAllBecariosAsync(filialId.Value);
            _becarios.AddRange(all);

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
