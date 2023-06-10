using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.Services.ServicesCoordinador;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Fonbec.Cartas.Ui.Components.Dialogs;
using Fonbec.Cartas.Ui.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class ApadrinamientoEdit
    {
        private bool _loading;
        private string? _pageTitle;

        private List<ApadrinamientoEditViewModel> _padrinosAsignados = new();
        private string _becarioName = default!;

        private List<PadrinoViewModel> _padrinos = new();
        
        private int _coordinadorId;
        private string _coordinadorName = default!;

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationState { get; set; }

        [Inject]
        public IApadrinamientoService ApadrinamientoService { get; set; } = default!;

        [Inject]
        public IBecarioService BecarioService { get; set; } = default!;

        [Inject]
        public IDialogService DialogService { get; set; } = default!;

        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        [Parameter]
        public int BecarioId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            if (AuthenticationState is null)
            {
                Snackbar.Add("AuthenticationState is null.", Severity.Error);
                NavigationManager.NavigateTo(NavRoutes.CoordinadorBecarios);
                return;
            }

            var user = (await AuthenticationState).User;
            if (user.Identity is not { IsAuthenticated: true })
            {
                Snackbar.Add("Usuario no está autenticado.", Severity.Error);
                NavigationManager.NavigateTo(NavRoutes.CoordinadorBecarios);
                return;
            }

            _coordinadorId = user.UserWithAccountId() ?? throw new NullReferenceException("No claim UserWithAccountId found");
            _coordinadorName = user.NickName() ?? throw new NullReferenceException("No claim UserWithAccountId found");

            var filialId = user.FilialId();

            if (filialId is null)
            {
                Snackbar.Add("Filial no está en el claim.", Severity.Error);
                NavigationManager.NavigateTo(NavRoutes.CoordinadorBecarios);
                return;
            }

            var becarioName = await BecarioService.GetBecarioNameAsync(filialId.Value, BecarioId);

            if (becarioName is null)
            {
                Snackbar.Add("No existe el becario en esta filial.", Severity.Error);
                NavigationManager.NavigateTo(NavRoutes.CoordinadorBecarios);
                return;
            }

            _becarioName = becarioName.FirstName;

            _pageTitle = $"Padrinos de {becarioName.FullName}";

            _padrinos = await BecarioService.GetAllPadrinosForSelectionAsync(filialId.Value);

            _padrinosAsignados = await ApadrinamientoService.GetAllPadrinosForBecario(BecarioId);

            _loading = false;
        }

        private async Task<AssignNewPadrinoDialogModel?> OpenAssignNewPadrinoDialogAndGetDataAsync(bool getNewData, int? padrinoId = null, DateTime? from = null, DateTime? to = null)
        {
            var parameters = new DialogParameters
            {
                ["Padrinos"] = _padrinos,
                ["GetNewData"] = getNewData,
            };
            
            if (!getNewData && padrinoId.HasValue && from.HasValue)
            {
                parameters.Add("PadrinoId", padrinoId.Value);
                parameters.Add("From", from.Value);
                parameters.Add("To", to);
            }

            var options = new DialogOptions
            {
                CloseButton = true,
                CloseOnEscapeKey = true
            };
            var dialog = await DialogService.ShowAsync<AssignNewPadrinoDialog>("Nueva asignación", parameters, options);
            var result = await dialog.Result;

            if (result.Canceled)
            {
                return null;
            }

            if (result.Data is not AssignNewPadrinoDialogModel nuevaAsignación)
            {
                return null;
            }

            return nuevaAsignación;
        }

        private async Task OpenAssignNewPadrinoDialogAsync()
        {
            var nuevaAsignación = await OpenAssignNewPadrinoDialogAndGetDataAsync(getNewData: true);
            if (nuevaAsignación is null)
            {
                return;
            }

            // Is there any overlap with another assignment for the same Padrino?
            var overlapWithSamePadrino = _padrinosAsignados.Any(asignación =>
                string.Equals(asignación.PadrinoFullName, nuevaAsignación.PadrinoViewModel.Name, StringComparison.OrdinalIgnoreCase)
                && ((nuevaAsignación.Desde <= asignación.From && (!nuevaAsignación.Hasta.HasValue || asignación.From <= nuevaAsignación.Hasta.Value))
                    || (asignación.From <= nuevaAsignación.Desde && (!asignación.To.HasValue || nuevaAsignación.Desde <= asignación.To.Value))));
            if (overlapWithSamePadrino)
            {
                Snackbar.Add($"{nuevaAsignación.PadrinoViewModel.Name} ya apadrina a {_becarioName} en ese período.", Severity.Error);
                return;
            }

            var assignPadrinoToBecarioViewModel = new AssignPadrinoToBecarioViewModel
            {
                BecarioId = BecarioId,
                PadrinoId = nuevaAsignación.PadrinoViewModel.Id,
                From = nuevaAsignación.Desde,
                To = nuevaAsignación.Hasta,
                CreatedByCoordinadorId = _coordinadorId,
            };

            var apadrinamientoId = await ApadrinamientoService.AssignPadrinoToBecarioAsync(assignPadrinoToBecarioViewModel);

            if (apadrinamientoId == 0)
            {
                Snackbar.Add("No se pudo almacenar la asignación.", Severity.Error);
                return;
            }

            var apadrinamientoEditViewModel = new ApadrinamientoEditViewModel(nuevaAsignación.Desde, nuevaAsignación.Hasta)
            {
                ApadrinamientoId = apadrinamientoId,
                PadrinoId = nuevaAsignación.PadrinoViewModel.Id,
                PadrinoFullName = nuevaAsignación.PadrinoViewModel.Name,
                CreatedOnUtc = DateTimeOffset.UtcNow,
                CreatedBy = _coordinadorName,
            };

            _padrinosAsignados.Add(apadrinamientoEditViewModel);
        }

        private async Task SetToDateToUknown(int apadrinamientoId)
        {
            var qtyChanged = await ApadrinamientoService.SetToDateToUknownAsync(apadrinamientoId, _coordinadorId);
            if (qtyChanged == 0)
            {
                Snackbar.Add("No se pudo almacenar el cambio", Severity.Error);
                return;
            }

            var updatedApadrinamiento = _padrinosAsignados.Single(pa => pa.ApadrinamientoId == apadrinamientoId);
            updatedApadrinamiento.To = null;
            updatedApadrinamiento.LastUpdatedOnUtc = DateTimeOffset.UtcNow;
            updatedApadrinamiento.UpdatedBy = _coordinadorName;
        }

        private async Task OpenAssignNewPadrinoDialogForEditAsync(int apadrinamientoId, int padrinoId, DateTime from, DateTime? to)
        {
            var asignaciónModificada = await OpenAssignNewPadrinoDialogAndGetDataAsync(getNewData: false, padrinoId, from, to);
            if (asignaciónModificada is null)
            {
                return;
            }

            var qtyChanged = await ApadrinamientoService.UpdateApadrinamientoAsync(apadrinamientoId, asignaciónModificada.Desde, asignaciónModificada.Hasta, _coordinadorId);
            if (qtyChanged == 0)
            {
                Snackbar.Add("No se pudo almacenar el cambio", Severity.Error);
                return;
            }

            var newStatus = new ApadrinamientoEditViewModel(asignaciónModificada.Desde, asignaciónModificada.Hasta)
                .Status;

            var updatedApadrinamiento = _padrinosAsignados.Single(pa => pa.ApadrinamientoId == apadrinamientoId);
            updatedApadrinamiento.PadrinoFullName = asignaciónModificada.PadrinoViewModel.Name;
            updatedApadrinamiento.From = asignaciónModificada.Desde;
            updatedApadrinamiento.To = asignaciónModificada.Hasta;
            updatedApadrinamiento.Status = newStatus;
            updatedApadrinamiento.LastUpdatedOnUtc = DateTimeOffset.UtcNow;
            updatedApadrinamiento.UpdatedBy = _coordinadorName; // TODO: This should be the full name, but we would need to get that in the user's claims
        }

        private async Task SetToDateToToday(int apadrinamientoId)
        {
            var qtyChanged = await ApadrinamientoService.SetToDateToTodayAsync(apadrinamientoId, _coordinadorId);
            if (qtyChanged == 0)
            {
                Snackbar.Add("No se pudo almacenar el cambio", Severity.Error);
                return;
            }

            var updatedApadrinamiento = _padrinosAsignados.Single(pa => pa.ApadrinamientoId == apadrinamientoId);
            updatedApadrinamiento.To = DateTime.Today;
            updatedApadrinamiento.LastUpdatedOnUtc = DateTimeOffset.UtcNow;
            updatedApadrinamiento.UpdatedBy = _coordinadorName;
        }
    }
}
