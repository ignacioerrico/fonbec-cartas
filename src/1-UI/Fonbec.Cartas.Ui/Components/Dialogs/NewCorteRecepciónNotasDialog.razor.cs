using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Components.Dialogs
{
    public partial class NewCorteRecepciónNotasDialog
    {
        private DateTime? _date;

        private string _saveButtonText = "Guardar";

        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; } = default!;

        [Parameter]
        public List<DateTime> TakenNotasDates { get; set; } = new();

        [Parameter]
        public DateTime? SelectedDate { get; set; }

        protected override void OnParametersSet()
        {
            if (SelectedDate is not null)
            {
                _date = SelectedDate.Value;
            }
        }

        protected override void OnInitialized()
        {
            _saveButtonText = SelectedDate is null
                ? "Crear"
                : "Actualizar";
        }

        private bool IsDateDisabled(DateTime dateTime)
        {
            return dateTime < DateTime.Today
                   || TakenNotasDates.Exists(taken =>
                       dateTime.Year == taken.Year
                       && dateTime.Month == taken.Month
                       && dateTime.Day == taken.Day);
        }

        private void Create() => MudDialog.Close(_date);

        private void Cancel() => MudDialog.Cancel();
    }
}
