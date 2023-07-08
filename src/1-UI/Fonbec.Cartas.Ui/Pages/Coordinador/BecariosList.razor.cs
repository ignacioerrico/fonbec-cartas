using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Logic.ExtensionMethods;
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

        private bool _displayFilterByBecario = true;
        private SelectableModel<FilterBy> _selectedFilter = Todos;
        private readonly List<SelectableModel<FilterBy>> _filters = new();

        private static readonly SelectableModel<NivelDeEstudio> Primario = new(NivelDeEstudio.Primario, "Primarios");
        private static readonly SelectableModel<NivelDeEstudio> Secundario = new(NivelDeEstudio.Secundario, "Secundarios");
        private static readonly SelectableModel<NivelDeEstudio> Universitario = new(NivelDeEstudio.Universitario, "Universitarios");

        private bool _displayFilterByNivelDeEstudio = true;
        private int _totalNivelesDisponibles = 0;
        private readonly List<SelectableModel<NivelDeEstudio>> _niveles = new();
        private List<SelectableModel<NivelDeEstudio>> _selectedNiveles = new();

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

            AddBecariosFilters();

            AddNivelesDeEstudioFilters();

            OnSelectedStatusChanged(_selectedFilter);
            
            _loading = false;
        }

        private bool Filter(BecariosListViewModel becariosListViewModel)
        {
            return string.IsNullOrWhiteSpace(_searchString)
                   || becariosListViewModel.Name.ContainsIgnoringAccents(_searchString)
                   || (_includeAll &&
                       (becariosListViewModel.Mediador.ContainsIgnoringAccents(_searchString)
                        || becariosListViewModel.PadrinosActivos.Any(pa => pa.ContainsIgnoringAccents(_searchString))
                        || becariosListViewModel.PadrinosFuturos.Any(pf => pf.ContainsIgnoringAccents(_searchString))
                        || becariosListViewModel.Email.Contains(_searchString, StringComparison.OrdinalIgnoreCase)
                        || becariosListViewModel.Phone.Contains(_searchString, StringComparison.OrdinalIgnoreCase)));
        }

        private void AddBecariosFilters()
        {
            var totalBecarios = _viewModels.Count;

            var totalSinPadrinoHoy = _viewModels.Count(vm => !vm.PadrinosActivos.Any());
            var totalConPadrinoHoy = _viewModels.Count(vm => vm.PadrinosActivos.Any());
            var totalAlMenosDosPadrinosHoy = _viewModels.Count(vm => vm.PadrinosActivos.Count > 1);
            var totalSinPadrinoFuturo = _viewModels.Count(vm => !vm.PadrinosFuturos.Any());
            var totalConPadrinoFuturo = _viewModels.Count(vm => vm.PadrinosFuturos.Any());

            var onlySingleFilter = totalSinPadrinoHoy == totalBecarios
                                           || totalConPadrinoHoy == totalBecarios
                                           || totalAlMenosDosPadrinosHoy == totalBecarios
                                           || totalSinPadrinoFuturo == totalBecarios
                                           || totalConPadrinoFuturo == totalBecarios;
            if (onlySingleFilter)
            {
                _displayFilterByBecario = false;
                return;
            }

            var showSinPadrinoHoy = totalSinPadrinoHoy > 0 && totalSinPadrinoHoy < totalBecarios;
            var showConPadrinoHoy = totalConPadrinoHoy > 0 && totalConPadrinoHoy < totalBecarios;
            var showAlMenosDosPadrinosHoy = totalAlMenosDosPadrinosHoy > 0 && totalAlMenosDosPadrinosHoy < totalBecarios;
            var showSinPadrinoFuturo = totalSinPadrinoFuturo > 0 && totalSinPadrinoFuturo < totalBecarios;
            var showConPadrinoFuturo = totalConPadrinoFuturo > 0 && totalConPadrinoFuturo < totalBecarios;

            AddBecarioFilter(Todos, totalBecarios);
            _selectedFilter = _filters.First();

            if (showSinPadrinoHoy)
            {
                AddBecarioFilter(SinPadrinoHoy, totalSinPadrinoHoy);
            }

            if (showConPadrinoHoy)
            {
                AddBecarioFilter(ConPadrinoHoy, totalConPadrinoHoy);
            }

            if (showAlMenosDosPadrinosHoy)
            {
                AddBecarioFilter(AlMenosDosPadrinosHoy, totalAlMenosDosPadrinosHoy);
            }

            if (showSinPadrinoFuturo)
            {
                AddBecarioFilter(SinPadrinoFuturo, totalSinPadrinoFuturo);
            }
            
            if (showConPadrinoFuturo)
            {
                AddBecarioFilter(ConPadrinoFuturo, totalConPadrinoFuturo);
            }
        }

        private void AddBecarioFilter(SelectableModel<FilterBy> option, int total)
        {
            var totalBecarios = _viewModels.Count;
            var optionWithTotalDisplayName = total == totalBecarios
                ? $"{option.DisplayName} ({total})"
                : $"{option.DisplayName} ({total} de {totalBecarios})";

            var optionWithTotal = new SelectableModel<FilterBy>(option.Id, optionWithTotalDisplayName);
            _filters.Add(optionWithTotal);
        }

        private void AddNivelesDeEstudioFilters()
        {
            var totalBecarios = _viewModels.Count;

            var totalPrimarios = _viewModels.Count(vm => vm.NivelDeEstudio == NivelDeEstudio.Primario);
            var totalSecundarios = _viewModels.Count(vm => vm.NivelDeEstudio == NivelDeEstudio.Secundario);
            var totalUniversitarios = _viewModels.Count(vm => vm.NivelDeEstudio == NivelDeEstudio.Universitario);

            var onlySingleNivelDeEstudio = totalPrimarios == totalBecarios
                                           || totalSecundarios == totalBecarios
                                           || totalUniversitarios == totalBecarios;
            if (onlySingleNivelDeEstudio)
            {
                _displayFilterByNivelDeEstudio = false;
                return;
            }

            var showPrimarios = totalPrimarios > 0 && totalPrimarios < totalBecarios;
            var showSecundarios = totalSecundarios > 0 && totalSecundarios < totalBecarios;
            var showUniversitarios = totalUniversitarios > 0 && totalUniversitarios < totalBecarios;

            if (showPrimarios)
            {
                AddNivelDeEstudioFilter(Primario, totalPrimarios);
                _totalNivelesDisponibles++;
            }

            if (showSecundarios)
            {
                AddNivelDeEstudioFilter(Secundario, totalSecundarios);
                _totalNivelesDisponibles++;
            }

            if (showUniversitarios)
            {
                AddNivelDeEstudioFilter(Universitario, totalUniversitarios);
                _totalNivelesDisponibles++;
            }
        }

        private void AddNivelDeEstudioFilter(SelectableModel<NivelDeEstudio> option, int total)
        {
            var optionWithTotalDisplayName = $"{option.DisplayName} ({total})";

            var optionWithTotal = new SelectableModel<NivelDeEstudio>(option.Id, optionWithTotalDisplayName);
            _niveles.Add(optionWithTotal);
            _selectedNiveles.Add(optionWithTotal);
        }

        private void OnSelectedStatusChanged(SelectableModel<FilterBy> selectedFilter)
        {
            _selectedFilter = selectedFilter;

            ApplyFilters(_selectedFilter, _selectedNiveles);
        }

        private void OnSelectedNivelesChanged(IEnumerable<SelectableModel<NivelDeEstudio>> selectedNiveles)
        {
            _selectedNiveles = selectedNiveles.ToList();

            ApplyFilters(_selectedFilter, _selectedNiveles);
        }

        private string GetSelectedNivelesText(List<string> selectedNiveles)
        {
            if (!selectedNiveles.Any())
            {
                return "Seleccioná por lo menos un nivel de estudio";
            }

            if (selectedNiveles.Count == _totalNivelesDisponibles)
            {
                return "Todos";
            }

            return $"Solamente {string.Join(" y ", selectedNiveles)}";
        }

        private void ApplyFilters(SelectableModel<FilterBy> selectedFilter, IEnumerable<SelectableModel<NivelDeEstudio>> selectedNiveles)
        {
            var filteredViewModels = selectedFilter.Id switch
            {
                FilterBy.Todos => _viewModels.ToList(),
                FilterBy.SinPadrinoHoy => _viewModels.Where(vm => !vm.PadrinosActivos.Any()).ToList(),
                FilterBy.ConPadrinoHoy => _viewModels.Where(vm => vm.PadrinosActivos.Any()).ToList(),
                FilterBy.AlMenosDosPadrinosHoy => _viewModels.Where(vm => vm.PadrinosActivos.Count > 1).ToList(),
                FilterBy.SinPadrinoFuturo => _viewModels.Where(vm => !vm.PadrinosFuturos.Any()).ToList(),
                FilterBy.ConPadrinoFuturo => _viewModels.Where(vm => vm.PadrinosFuturos.Any()).ToList(),
                _ => throw new InvalidOperationException($"Select options wrongly set in {nameof(PadrinosList)}")
            };

            filteredViewModels = filteredViewModels
                .Where(vm => selectedNiveles.Select(sn => sn.Id).Contains(vm.NivelDeEstudio))
                .ToList();

            _filteredViewModels = filteredViewModels;
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
