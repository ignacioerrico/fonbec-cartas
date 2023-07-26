using System.Globalization;
using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.Models;
using Fonbec.Cartas.Logic.ViewModels.Components.Dialogs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Components.Dialogs
{
    public partial class AssignNewPadrinoDialog
    {
        private static readonly CultureInfo EsArCultureInfo = CultureInfo.GetCultureInfo("es-AR");

        private readonly AssignNewPadrinoDialogViewModel _viewModel = new();

        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; } = default!;

        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        [Parameter]
        public List<SelectableModel<int>> Padrinos { get; set; } = default!;

        [Parameter]
        public bool GetNewData { get; set; }

        [Parameter]
        public int PadrinoId { get; set; }

        [Parameter]
        public DateTime From { get; set; }

        [Parameter]
        public DateTime? To { get; set; }

        protected override void OnParametersSet()
        {
            if (GetNewData)
            {
                return;
            }

            _viewModel.SelectedPadrino = Padrinos.Single(p => p.Id == PadrinoId);
            _viewModel.Desde = From;
            _viewModel.Hasta = To;
            _viewModel.KnownEndDate = To.HasValue;
        }

        private async Task<IEnumerable<SelectableModel<int>>> SearchPadrino(string searchString)
        {
            await Task.Delay(5);

            if (string.IsNullOrWhiteSpace(searchString))
            {
                return Padrinos;
            }

            return Padrinos.Where(m => m.DisplayName.ContainsIgnoringAccents(searchString));
        }

        private bool IsToDateDisabled(DateTime to)
        {
            if (!_viewModel.Desde.HasValue)
            {
                return true;
            }

            return to.Date <= _viewModel.Desde.Value.Date;
        }

        private void OnKnownEndDateChanged(bool knownEndDate)
        {
            _viewModel.KnownEndDate = knownEndDate;
            _viewModel.Hasta = knownEndDate
                ? _viewModel.Desde!.Value.AddMonths(1)
                : null;
        }

        private void Add()
        {
            if (_viewModel.Hasta.HasValue && _viewModel.Desde >= _viewModel.Hasta)
            {
                Snackbar.Add("La fecha de finalización debe ser posterior a la de comienzo", Severity.Error);
                _viewModel.Hasta = _viewModel.Desde!.Value.AddMonths(1);
                return;
            }

            if (!_viewModel.KnownEndDate)
            {
                _viewModel.Hasta = null;
            }

            MudDialog.Close(DialogResult.Ok(_viewModel));
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
