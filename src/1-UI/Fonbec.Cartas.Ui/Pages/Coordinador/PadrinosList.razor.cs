using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.Models;
using Fonbec.Cartas.Logic.Services.Coordinador;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Microsoft.AspNetCore.Components;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class PadrinosList : PerFilialComponentBase
    {
        private List<PadrinosListViewModel> _viewModels = new();
        private List<PadrinosListViewModel> _filteredViewModels = new();

        private bool _loading;
        private string _searchString = string.Empty;
        private bool _includeAll;

        private static readonly SelectableModel<FilterBy> Todos = new(FilterBy.Todos, "Todos");
        private static readonly SelectableModel<FilterBy> SinBecario = new(FilterBy.SinBecario, "Solo los que NO tienen becario asignado");
        private static readonly SelectableModel<FilterBy> ConBecario = new(FilterBy.ConBecario, "Solo los que SÍ tienen becario asignado");
        private static readonly SelectableModel<FilterBy> AlMenosDosBecarios = new(FilterBy.AlMenosDosBecarios, "Solo los que tienen al menos dos becarios asignados");
        private static readonly SelectableModel<FilterBy> ConCcOBcc = new(FilterBy.ConCcOBcc, "Solo los que requieren copia a otra persona");

        private SelectableModel<FilterBy> _selectedFilter = Todos;
        private readonly List<SelectableModel<FilterBy>> _filters = new();

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

            AddFilters();

            OnSelectedStatusChanged(_selectedFilter);

            _loading = false;
        }

        private bool Filter(PadrinosListViewModel padrinosListViewModel)
        {
            return string.IsNullOrWhiteSpace(_searchString)
                   || padrinosListViewModel.PadrinoFullName.ContainsIgnoringAccents(_searchString)
                   || (_includeAll &&
                       (padrinosListViewModel.PadrinoEmail.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                        || padrinosListViewModel.PadrinoPhone.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                        || padrinosListViewModel.BecariosActivos.Any(b => b.BecarioFullName.ContainsIgnoringAccents(_searchString)
                                                                          || (b.BecarioEmail is not null && b.BecarioEmail.Contains(_searchString, StringComparison.OrdinalIgnoreCase)))
                        || padrinosListViewModel.Cc.Any(cc => cc.ContainsIgnoringAccents(_searchString))
                        || padrinosListViewModel.Bcc.Any(cc => cc.ContainsIgnoringAccents(_searchString))));
        }

        private void AddFilters()
        {
            var totalPadrinos = _viewModels.Count;

            var totalSinBecario = _viewModels.Count(vm => !vm.BecariosActivos.Any());
            var totalConBecario = _viewModels.Count(vm => vm.BecariosActivos.Any());
            var totalAlMenosDosBecarios = _viewModels.Count(vm => vm.BecariosActivos.Count > 1);
            var totalConCcOBcc = _viewModels.Count(vm => vm.Cc.Any() || vm.Bcc.Any());

            var showSinBecario = totalSinBecario > 0 && totalSinBecario < totalPadrinos;
            var showConBecario = totalConBecario > 0 && totalConBecario < totalPadrinos;
            var showAlMenosDosBecarios = totalAlMenosDosBecarios > 0 && totalAlMenosDosBecarios < totalPadrinos;
            var showConCcOBcc = totalConCcOBcc > 0 && totalConCcOBcc < totalPadrinos;

            AddFilter(Todos, totalPadrinos);
            _selectedFilter = _filters.First();

            if (showSinBecario)
            {
                AddFilter(SinBecario, totalSinBecario);
            }

            if (showConBecario)
            {
                AddFilter(ConBecario, totalConBecario);
            }

            if (showAlMenosDosBecarios)
            {
                AddFilter(AlMenosDosBecarios, totalAlMenosDosBecarios);
            }

            if (showConCcOBcc)
            {
                AddFilter(ConCcOBcc, totalConCcOBcc);
            }
        }

        private void AddFilter(SelectableModel<FilterBy> option, int total)
        {
            var totalPadrinos = _viewModels.Count;
            var optionWithTotalDisplayName = total == totalPadrinos
                ? $"{option.DisplayName} ({total})"
                : $"{option.DisplayName} ({total} de {totalPadrinos})";
            
            var optionWithTotal = new SelectableModel<FilterBy>(option.Id, optionWithTotalDisplayName);
            _filters.Add(optionWithTotal);
        }

        private void OnSelectedStatusChanged(SelectableModel<FilterBy> selectedFilter)
        {
            _selectedFilter = selectedFilter;

            _filteredViewModels = selectedFilter.Id switch
            {
                FilterBy.Todos => _viewModels.ToList(),
                FilterBy.SinBecario => _viewModels.Where(vm => !vm.BecariosActivos.Any()).ToList(),
                FilterBy.ConBecario => _viewModels.Where(vm => vm.BecariosActivos.Any()).ToList(),
                FilterBy.AlMenosDosBecarios => _viewModels.Where(vm => vm.BecariosActivos.Count > 1).ToList(),
                FilterBy.ConCcOBcc => _viewModels.Where(vm => vm.Cc.Any() || vm.Bcc.Any()).ToList(),
                _ => throw new InvalidOperationException($"Select options wrongly set in {nameof(PadrinosList)}")
            };
        }

        private enum FilterBy : byte
        {
            Todos,
            SinBecario,
            ConBecario,
            AlMenosDosBecarios,
            ConCcOBcc,
        }
    }
}
