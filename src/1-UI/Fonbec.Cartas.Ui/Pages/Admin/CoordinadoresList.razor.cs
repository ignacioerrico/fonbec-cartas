using Fonbec.Cartas.Logic.Services.Admin;
using Fonbec.Cartas.Logic.ViewModels.Admin;
using Microsoft.AspNetCore.Components;

namespace Fonbec.Cartas.Ui.Pages.Admin
{
    public partial class CoordinadoresList
    {
        private readonly List<CoordinadoresListViewModel> _coordinadores = new();

        private bool _loading;
        private string _searchString = string.Empty;
        private bool _includeAll;

        [Inject]
        public ICoordinadorService CoordinadorService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var coordinadores = await CoordinadorService.GetAllCoordinadoresAsync();
            _coordinadores.AddRange(coordinadores);

            _loading = false;
        }

        private bool Filter(CoordinadoresListViewModel coordinadoresListViewModel)
        {
            return string.IsNullOrWhiteSpace(_searchString)
                   || coordinadoresListViewModel.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                   || (_includeAll &&
                       (coordinadoresListViewModel.Filial.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                       || coordinadoresListViewModel.Email.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                       || coordinadoresListViewModel.Phone.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                       || coordinadoresListViewModel.Username.Contains(_searchString, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
