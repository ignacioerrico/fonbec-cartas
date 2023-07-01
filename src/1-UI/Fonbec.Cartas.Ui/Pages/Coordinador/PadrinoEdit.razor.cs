using System.Security.Claims;
using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.Services.Coordinador;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Fonbec.Cartas.Ui.Components.Dialogs;
using Fonbec.Cartas.Ui.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class PadrinoEdit
    {
        private ClaimsPrincipal _user = default!;

        private readonly PadrinoEditViewModel _padrino = new();
        private readonly PadrinoEditViewModel _originalPadrino = new();

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
            || _padrino.SendAlsoTo.Select(sat =>
                _originalPadrino.SendAlsoTo.All(orig =>
                    orig.RecipientFullName != sat.RecipientFullName
                    || orig.RecipientEmail != sat.RecipientEmail
                    || orig.SendAsBcc != sat.SendAsBcc))
                .Any(notFound => notFound);

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationState { get; set; }

        [Parameter]
        public string PadrinoId { get; set; } = string.Empty;

        [Inject]
        public IPadrinoService PadrinoService { get; set; } = default!;

        [Inject]
        public IDialogService DialogService { get; set; } = default!;

        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

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

            if (AuthenticationState is null)
            {
                Snackbar.Add("AuthenticationState is null.", Severity.Error);
                NavigationManager.NavigateTo(NavRoutes.CoordinadorPadrinos);
                return;
            }

            var user = (await AuthenticationState).User;
            if (user.Identity is not { IsAuthenticated: true })
            {
                Snackbar.Add("Usuario no está autenticado.", Severity.Error);
                NavigationManager.NavigateTo(NavRoutes.CoordinadorPadrinos);
                return;
            }

            _user = user;

            if (string.Equals(PadrinoId, NavRoutes.New, StringComparison.OrdinalIgnoreCase))
            {
                _isNew = true;

                _pageTitle = "Alta de Padrino";
                _saveButtonText = "Crear";
            }
            else if (int.TryParse(PadrinoId, out var padrinoId) && padrinoId > 0)
            {
                _isNew = false;

                _pageTitle = "Editar Padrino";
                _saveButtonText = "Actualizar";

                var filialId = _user.FilialId();

                if (filialId is null)
                {
                    Snackbar.Add("Filial no está en el claim.", Severity.Error);
                    NavigationManager.NavigateTo(NavRoutes.CoordinadorPadrinos);
                    return;
                }

                var padrino = await PadrinoService.GetPadrinoAsync(padrinoId, filialId.Value);

                if (padrino is null)
                {
                    Snackbar.Add($"No se encontró padrino con ID {padrinoId}.", Severity.Error);
                    NavigationManager.NavigateTo(NavRoutes.CoordinadorPadrinoNew);
                    return;
                }

                _padrino.FirstName = _originalPadrino.FirstName = padrino.FirstName;
                _padrino.LastName = _originalPadrino.LastName = padrino.LastName;
                _padrino.NickName = _originalPadrino.NickName = padrino.NickName;
                _padrino.Gender = _originalPadrino.Gender = padrino.Gender;
                _padrino.Email = _originalPadrino.Email = padrino.Email;
                _padrino.Phone = _originalPadrino.Phone = padrino.Phone;

                foreach (var satFromDb in padrino.SendAlsoTo)
                {
                    var sat = new PadrinoEditSendAlsoToViewModel
                    {
                        RecipientFullName = satFromDb.RecipientFullName,
                        RecipientEmail = satFromDb.RecipientEmail,
                        SendAsBcc = satFromDb.SendAsBcc
                    };
                    _padrino.SendAlsoTo.Add(sat);

                    var originalSat = new PadrinoEditSendAlsoToViewModel
                    {
                        RecipientFullName = satFromDb.RecipientFullName,
                        RecipientEmail = satFromDb.RecipientEmail,
                        SendAsBcc = satFromDb.SendAsBcc
                    };
                    _originalPadrino.SendAlsoTo.Add(originalSat);
                }
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
                _padrino.FilialId = _user.FilialId() ?? throw new NullReferenceException("No claim FilialId found");
                _padrino.CreatedByCoordinadorId = _user.UserWithAccountId() ?? throw new NullReferenceException("No claim UserWithAccountId found");

                var qtyAdded = await PadrinoService.CreateAsync(_padrino);
                
                if (qtyAdded == 0)
                {
                    Snackbar.Add("No se pudo crear el padrino.", Severity.Error);
                }
            }
            else if (ModelHasChanged)
            {
                var id = int.Parse(PadrinoId);
                _padrino.UpdatedByCoordinadorId = _user.UserWithAccountId() ?? throw new NullReferenceException("No claim UserWithAccountId found");

                var qtyUpdated = await PadrinoService.UpdateAsync(id, _padrino);
                
                if (qtyUpdated == 0)
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

            if (result.Data is not AddSendAlsoToDialogModel data)
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
