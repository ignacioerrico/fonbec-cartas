using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.Services.Coordinador;
using Fonbec.Cartas.Logic.ViewModels.Components.Dialogs;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Fonbec.Cartas.Ui.Components.Dialogs;
using Fonbec.Cartas.Ui.Constants;
using Mapster;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class PadrinoEdit : PerFilialComponentBase
    {
        private PadrinoEditViewModel _padrino = new();
        private PadrinoEditViewModel _originalPadrino = new();
        private int _padrinoId;

        private int _coordinadorId;
        
        private bool _loading;
        private bool _isNew;
        private string? _pageTitle;
        private string _saveButtonText = "Guardar";
        private bool _formValidationSucceeded;

        private MudTextField<string> _mudTextFieldNombre = default!;

        private bool SaveButtonDisabled => _loading
                                           || !_formValidationSucceeded
                                           || !ModelHasChanged;

        private bool ModelHasChanged =>
            !string.Equals(_padrino.FirstName, _originalPadrino.FirstName, StringComparison.Ordinal)
            || !string.Equals(_padrino.LastName, _originalPadrino.LastName, StringComparison.Ordinal)
            || !string.Equals(_padrino.NickName, _originalPadrino.NickName, StringComparison.Ordinal)
            || _padrino.Gender != _originalPadrino.Gender
            || !string.Equals(_padrino.Email, _originalPadrino.Email, StringComparison.Ordinal)
            || !string.Equals(_padrino.Phone, _originalPadrino.Phone, StringComparison.Ordinal)
            || _padrino.SendAlsoTo.Count != _originalPadrino.SendAlsoTo.Count
            || _padrino.SendAlsoTo.Any(sat =>
                _originalPadrino.SendAlsoTo.All(orig =>
                    orig.RecipientFullName != sat.RecipientFullName
                    || orig.RecipientEmail != sat.RecipientEmail
                    || orig.SendAsBcc != sat.SendAsBcc));

        [Parameter]
        public string PadrinoId { get; set; } = string.Empty;

        [Inject]
        public IPadrinoService PadrinoService { get; set; } = default!;

        [Inject]
        public IDialogService DialogService { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                return;
            }

            await _mudTextFieldNombre.FocusAsync();

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var authenticatedUserData = await GetAuthenticatedUserDataAsync();
            if (!authenticatedUserData.DataObtainedSuccessfully)
            {
                _loading = false;
                return;
            }

            _coordinadorId = authenticatedUserData.User.UserWithAccountId()
                             ?? throw new NullReferenceException("No claim UserWithAccountId found");

            if (string.Equals(PadrinoId, NavRoutes.New, StringComparison.OrdinalIgnoreCase))
            {
                _isNew = true;

                _pageTitle = "Alta de Padrino";
                _saveButtonText = "Crear";

                _padrino.FilialId = authenticatedUserData.FilialId;
            }
            else if (int.TryParse(PadrinoId, out _padrinoId) && _padrinoId > 0)
            {
                _isNew = false;

                _pageTitle = "Editar Padrino";
                _saveButtonText = "Actualizar";

                var result = await PadrinoService.GetPadrinoAsync(_padrinoId, authenticatedUserData.FilialId);

                if (!result.IsFound || result.Data is null)
                {
                    Snackbar.Add($"No se encontró padrino con ID {_padrinoId}.", Severity.Error);
                    NavigationManager.NavigateTo(NavRoutes.CoordinadorPadrinoNew);
                    return;
                }

                _padrino = result.Data.Adapt<PadrinoEditViewModel>();
                _originalPadrino = result.Data.Adapt<PadrinoEditViewModel>();
            }
            else
            {
                NavigationManager.NavigateTo(NavRoutes.CoordinadorPadrinos);
            }

            _loading = false;
        }

        private async Task Save()
        {
            if (_isNew)
            {
                _padrino.CreatedByCoordinadorId = _coordinadorId;

                var result = await PadrinoService.CreateAsync(_padrino);
                
                if (!result.AnyRowsAffected)
                {
                    Snackbar.Add("No se pudo crear el padrino.", Severity.Error);
                }
            }
            else if (ModelHasChanged)
            {
                _padrino.UpdatedByCoordinadorId = _coordinadorId;

                var result = await PadrinoService.UpdateAsync(_padrinoId, _padrino);
                
                if (!result.AnyRowsAffected)
                {
                    Snackbar.Add("No se pudo actualizar el padrino.", Severity.Error);
                }
            }

            NavigationManager.NavigateTo(NavRoutes.CoordinadorPadrinos);
        }

        private async Task OpenSendAlsoToDialogAsync(int? currentIndex = null)
        {

            var parameters = new DialogParameters();

            if (currentIndex is not null)
            {
                parameters["FullName"] = _padrino.SendAlsoTo[currentIndex.Value].RecipientFullName;
                parameters["Email"] = _padrino.SendAlsoTo[currentIndex.Value].RecipientEmail;
                parameters["SendAsBcc"] = _padrino.SendAlsoTo[currentIndex.Value].SendAsBcc;
            }

            var options = new DialogOptions
            {
                CloseButton = true,
                CloseOnEscapeKey = true
            };
            var dialog = await DialogService.ShowAsync<AddSendAlsoToDialog>("Enviar también a", parameters, options);
            var result = await dialog.Result;

            if (result.Canceled)
            {
                return;
            }

            if (result.Data is not AddSendAlsoToDialogViewModel data)
            {
                return;
            }

            var sendAlsoTo = new PadrinoEditSendAlsoToViewModel
            {
                RecipientFullName = data.FullName,
                RecipientEmail = data.Email,
                SendAsBcc = data.SendAsBcc
            };

            if (currentIndex is null)
            {
                _padrino.SendAlsoTo.Add(sendAlsoTo);
            }
            else
            {
                _padrino.SendAlsoTo[currentIndex.Value] = sendAlsoTo;
            }
        }
    }
}
