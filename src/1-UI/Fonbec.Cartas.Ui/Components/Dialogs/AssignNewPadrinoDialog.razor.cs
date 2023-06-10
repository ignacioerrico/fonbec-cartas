using System.Globalization;
using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Components.Dialogs
{
    public partial class AssignNewPadrinoDialog
    {
        private static readonly CultureInfo EsArCultureInfo = CultureInfo.GetCultureInfo("es-AR");

        private PadrinoViewModel? _selectedPadrino;

        private DateTime? _desde = DateTime.Today;
        
        private DateTime? _hasta;

        private bool _knownEndDate;

        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; } = default!;

        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        [Parameter]
        public List<PadrinoViewModel> Padrinos { get; set; } = default!;

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
            if (!GetNewData)
            {
                _selectedPadrino = Padrinos.Single(p => p.Id == PadrinoId);
                _desde = From;
                _hasta = To;
                _knownEndDate = To.HasValue;
            }
        }

        private async Task<IEnumerable<PadrinoViewModel>> SearchPadrino(string searchString)
        {
            await Task.Delay(5);

            if (string.IsNullOrWhiteSpace(searchString))
            {
                return Padrinos;
            }

            return Padrinos.Where(m => m.Name.ContainsIgnoringAccents(searchString));
        }

        private void Add()
        {
            if (_desde.HasValue
                && _hasta.HasValue
                && _desde.Value >= _hasta.Value)
            {
                Snackbar.Add("La fecha de finalización debe ser posterior a la de comienzo", Severity.Error);
                _hasta = _desde.Value.AddMonths(1);
                return;
            }

            var assignNewPadrinoDialogModel = new AssignNewPadrinoDialogModel(_selectedPadrino!, _desde!.Value, _knownEndDate ? _hasta!.Value : null);
            MudDialog.Close(DialogResult.Ok(assignNewPadrinoDialogModel));
        }

        private void Cancel() => MudDialog.Cancel();

        private void OnKnownEndDateChanged(bool knownEndDate)
        {
            _knownEndDate = knownEndDate;
            _hasta = knownEndDate
                ? _desde!.Value.AddMonths(1)
                : null;
        }
    }

    public class AssignNewPadrinoDialogModel
    {
        public AssignNewPadrinoDialogModel(PadrinoViewModel padrinoViewModel, DateTime desde, DateTime? hasta)
        {
            PadrinoViewModel = padrinoViewModel;
            Desde = desde;
            Hasta = hasta;
        }

        public PadrinoViewModel PadrinoViewModel { get; }
        
        public DateTime Desde { get; }
        
        public DateTime? Hasta { get; }
    }
}
