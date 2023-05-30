using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Components.Dialogs
{
    public partial class AddSendAlsoToDialog
    {
        private string _fullName = string.Empty;
        private string _email = string.Empty;
        private bool _sendAsBcc;

        private bool _formValidationSucceeded;
        private MudTextField<string> _mudTextFieldNombre = new();

        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; } = default!;

        [Parameter]
        public string? FullName { get; set; }

        [Parameter]
        public string? Email { get; set; }

        [Parameter]
        public bool SendAsBcc { get; set; }

        public bool AddButtonDisabled => !_formValidationSucceeded;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                return;
            }

            await _mudTextFieldNombre.FocusAsync();

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override void OnParametersSet()
        {
            if (FullName is not null)
            {
                _fullName = FullName;
            }

            if (Email is not null)
            {
                _email = Email;
            }

            _sendAsBcc = SendAsBcc;

            base.OnParametersSet();
        }

        private void Add()
        {
            var addSendAlsoToDialogModel = new AddSendAlsoToDialogModel(_fullName, _email, _sendAsBcc);
            MudDialog.Close(DialogResult.Ok(addSendAlsoToDialogModel));
        }

        private void Cancel() => MudDialog.Cancel();
    }

    public class AddSendAlsoToDialogModel
    {
        public AddSendAlsoToDialogModel(string fullName, string email, bool sendAsBcc)
        {
            FullName = fullName;
            Email = email;
            SendAsBcc = sendAsBcc;
        }

        public string FullName { get; }

        public string Email { get; }

        public bool SendAsBcc { get; }
    }
}
