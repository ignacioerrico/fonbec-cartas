using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Components.Dialogs
{
    public partial class ConfirmDeleteDialog
    {
        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; } = default!;

        [Parameter]
        public string ItemToDelete { get; set; } = default!;

        private void ConfirmDelete() => MudDialog.Close();

        private void Cancel() => MudDialog.Cancel();
    }
}
