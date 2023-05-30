using Fonbec.Cartas.Logic.Services.ServicesCoordinador;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Fonbec.Cartas.Ui.Areas.Identity.ExtensionMethods;
using Fonbec.Cartas.Ui.Components.Dialogs;
using Fonbec.Cartas.Ui.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class PadrinoEdit
    {
        private readonly PadrinoEditViewModel _padrino = new();
        private readonly PadrinoEditViewModel _originalPadrino = new();

        private readonly List<AddSendAlsoToDialogModel> _sendAlsoTo = new();

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
            || !string.Equals(_padrino.Phone, _originalPadrino.Phone, StringComparison.Ordinal);

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

                _loading = true;
                var padrino = await PadrinoService.GetPadrinoAsync(padrinoId);
                _loading = false;

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
            }
            else
            {
                NavigationManager.NavigateTo(NavRoutes.CoordinadorPadrinos);
            }
        }

        private async Task Save()
        {
            if (AuthenticationState is null)
            {
                return;
            }

            var user = (await AuthenticationState).User;
            if (user.Identity is not { IsAuthenticated: true })
            {
                return;
            }

            _padrino.SendAlsoTo = _sendAlsoTo.Select(sat =>
                new PadrinoEditSendAlsoToViewModel
                {
                    RecipientFullName = sat.FullName,
                    RecipientEmail = sat.Email,
                    SendAsBcc = sat.SendAsBcc,
                }).ToList();

            if (_isNew)
            {
                _padrino.FilialId = user.FilialId() ?? throw new NullReferenceException("No claim FilialId found");
                _padrino.CreatedByCoordinadorId = user.UserWithAccountId() ?? throw new NullReferenceException("No claim UserWithAccountId found");

                var qtyAdded = await PadrinoService.CreateAsync(_padrino);
                
                if (qtyAdded == 0)
                {
                    Snackbar.Add("No se pudo crear el padrino.", Severity.Error);
                }
            }
            else if (ModelHasChanged)
            {
                var id = int.Parse(PadrinoId);
                _padrino.UpdatedByCoordinadorId = user.UserWithAccountId() ?? throw new NullReferenceException("No claim UserWithAccountId found");

                var qtyUpdated = await PadrinoService.UpdateAsync(id, _padrino);
                
                if (qtyUpdated == 0)
                {
                    Snackbar.Add("No se pudo actualizar el padrino.", Severity.Error);
                }
            }

            NavigationManager.NavigateTo(NavRoutes.CoordinadorPadrinos);
        }

        private async Task OpenSendAlsoToDialogAsync(string? fullName = null, string? email = null, bool sendAsBcc = false)
        {
            var options = new DialogOptions
            {
                CloseButton = true,
                CloseOnEscapeKey = true
            };
            var dialog = await DialogService.ShowAsync<AddSendAlsoToDialog>("Enviar también a", options);
            var result = await dialog.Result;

            if (result.Canceled)
            {
                return;
            }

            if (result.Data is not AddSendAlsoToDialogModel data)
            {
                return;
            }

            _sendAlsoTo.Add(data);
        }

        private async Task OpenSendAlsoToDialogAsync(int currentIndex)
        {
            var parameters = new DialogParameters
            {
                { "FullName", _sendAlsoTo[currentIndex].FullName },
                { "Email", _sendAlsoTo[currentIndex].Email },
                { "SendAsBcc", _sendAlsoTo[currentIndex].SendAsBcc }
            };
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

            _sendAlsoTo[currentIndex] = data;
        }

    }
}
