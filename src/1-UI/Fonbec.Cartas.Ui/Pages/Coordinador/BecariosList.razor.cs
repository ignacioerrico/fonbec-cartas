using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Microsoft.AspNetCore.Components;
using Fonbec.Cartas.Logic.Services.Coordinador;
using Fonbec.Cartas.Logic.Models;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class BecariosList : PerFilialComponentBase
    {
        private List<BecariosListViewModel> _viewModels = new();
        private List<BecariosListViewModel> _filteredViewModels = new();

        private bool _loading;
        private string _searchString = string.Empty;
        private bool _includeAll;

        private static readonly SelectableModel<FilterBy> Todos = new(FilterBy.Todos, "Todos");
        private static readonly SelectableModel<FilterBy> SinPadrinoHoy = new(FilterBy.SinPadrinoHoy, "Actualmente SIN padrino");
        private static readonly SelectableModel<FilterBy> ConPadrinoHoy = new(FilterBy.ConPadrinoHoy, "Actualmente CON padrino");
        private static readonly SelectableModel<FilterBy> AlMenosDosPadrinosHoy = new(FilterBy.AlMenosDosPadrinosHoy, "Actualmente con al menos dos padrinos");
        private static readonly SelectableModel<FilterBy> SinPadrinoFuturo = new(FilterBy.SinPadrinoFuturo, "SIN padrino a futuro");
        private static readonly SelectableModel<FilterBy> ConPadrinoFuturo = new(FilterBy.ConPadrinoFuturo, "CON padrino a futuro");

        private SelectableModel<FilterBy> _selectedFilter = Todos;
        private readonly List<SelectableModel<FilterBy>> _filters = new();

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

            AddFilters();

            OnSelectedStatusChanged(_selectedFilter);
            
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

        private void AddFilters()
        {
            var totalBecarios = _viewModels.Count;

            var totalSinPadrinoHoy = _viewModels.Count(vm => !vm.PadrinosActivos.Any());
            var totalConPadrinoHoy = _viewModels.Count(vm => vm.PadrinosActivos.Any());
            var totalAlMenosDosPadrinosHoy = _viewModels.Count(vm => vm.PadrinosActivos.Count > 1);
            var totalSinPadrinoFuturo = _viewModels.Count(vm => !vm.PadrinosFuturos.Any());
            var totalConPadrinoFuturo = _viewModels.Count(vm => vm.PadrinosFuturos.Any());

            var showSinPadrinoHoy = totalSinPadrinoHoy > 0 && totalSinPadrinoHoy < totalBecarios;
            var showConPadrinoHoy = totalConPadrinoHoy > 0 && totalConPadrinoHoy < totalBecarios;
            var showAlMenosDosPadrinosHoy = totalAlMenosDosPadrinosHoy > 0 && totalAlMenosDosPadrinosHoy < totalBecarios;
            var showSinPadrinoFuturo = totalSinPadrinoFuturo > 0 && totalSinPadrinoFuturo < totalBecarios;
            var showConPadrinoFuturo = totalConPadrinoFuturo > 0 && totalConPadrinoFuturo < totalBecarios;

            AddFilter(Todos, totalBecarios);
            _selectedFilter = _filters.First();

            if (showSinPadrinoHoy)
            {
                AddFilter(SinPadrinoHoy, totalSinPadrinoHoy);
            }

            if (showConPadrinoHoy)
            {
                AddFilter(ConPadrinoHoy, totalConPadrinoHoy);
            }

            if (showAlMenosDosPadrinosHoy)
            {
                AddFilter(AlMenosDosPadrinosHoy, totalAlMenosDosPadrinosHoy);
            }

            if (showSinPadrinoFuturo)
            {
                AddFilter(SinPadrinoFuturo, totalSinPadrinoFuturo);
            }
            
            if (showConPadrinoFuturo)
            {
                AddFilter(ConPadrinoFuturo, totalConPadrinoFuturo);
            }
        }

        private void AddFilter(SelectableModel<FilterBy> option, int total)
        {
            var totalBecarios = _viewModels.Count;
            var optionWithTotalDisplayName = total == totalBecarios
                ? $"{option.DisplayName} ({total})"
                : $"{option.DisplayName} ({total} de {totalBecarios})";

            var optionWithTotal = new SelectableModel<FilterBy>(option.Id, optionWithTotalDisplayName);
            _filters.Add(optionWithTotal);
        }

        private void OnSelectedStatusChanged(SelectableModel<FilterBy> selectedFilter)
        {
            _selectedFilter = selectedFilter;

            _filteredViewModels = selectedFilter.Id switch
            {
                FilterBy.Todos => _viewModels.ToList(),
                FilterBy.SinPadrinoHoy => _viewModels.Where(vm => !vm.PadrinosActivos.Any()).ToList(),
                FilterBy.ConPadrinoHoy => _viewModels.Where(vm => vm.PadrinosActivos.Any()).ToList(),
                FilterBy.AlMenosDosPadrinosHoy => _viewModels.Where(vm => vm.PadrinosActivos.Count > 1).ToList(),
                FilterBy.SinPadrinoFuturo => _viewModels.Where(vm => !vm.PadrinosFuturos.Any()).ToList(),
                FilterBy.ConPadrinoFuturo => _viewModels.Where(vm => vm.PadrinosFuturos.Any()).ToList(),
                _ => throw new InvalidOperationException($"Select options wrongly set in {nameof(PadrinosList)}")
            };
        }
        
        private enum FilterBy : byte
        {
            Todos,
            SinPadrinoHoy,
            ConPadrinoHoy,
            AlMenosDosPadrinosHoy,
            SinPadrinoFuturo,
            ConPadrinoFuturo,
        }
    }
}
