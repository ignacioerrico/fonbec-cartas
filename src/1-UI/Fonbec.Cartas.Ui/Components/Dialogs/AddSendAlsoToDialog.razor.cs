using Fonbec.Cartas.Logic.ViewModels.Components.Dialogs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Components.Dialogs
{
    public partial class AddSendAlsoToDialog
    {
        private readonly AddSendAlsoToDialogViewModel _viewModel = new();

        private bool _formValidationSucceeded;
        private bool AddButtonDisabled => !_formValidationSucceeded;
        
        private MudTextField<string> _mudTextFieldNombre = new();

        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; } = default!;

        [Parameter]
        public string? FullName { get; set; }

        [Parameter]
        public string? Email { get; set; }

        [Parameter]
        public bool SendAsBcc { get; set; }

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
                _viewModel.FullName = FullName;
            }

            if (Email is not null)
            {
                _viewModel.Email = Email;
            }

            _viewModel.SendAsBcc = SendAsBcc;

            base.OnParametersSet();
        }

        private void Add() => MudDialog.Close(DialogResult.Ok(_viewModel));

        private void Cancel() => MudDialog.Cancel();
    }
}
