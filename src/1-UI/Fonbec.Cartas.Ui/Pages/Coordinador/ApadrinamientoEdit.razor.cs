using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.Models;
using Fonbec.Cartas.Logic.Models.Coordinador;
using Fonbec.Cartas.Logic.Services.Coordinador;
using Fonbec.Cartas.Logic.ViewModels.Components.Dialogs;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Fonbec.Cartas.Ui.Components.Dialogs;
using Fonbec.Cartas.Ui.Constants;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class ApadrinamientoEdit : PerFilialComponentBase
    {
        private ApadrinamientoEditViewModel _viewModel = new();
        
        private bool _loading;

        private int _coordinadorId;
        private string _coordinadorName = default!;
        
        private string? _pageTitle;
        private string _becarioFirstName = default!;
        private List<SelectableModel<int>> _padrinos = new();

        [Inject]
        public IApadrinamientoService ApadrinamientoService { get; set; } = default!;

        [Inject]
        public IDialogService DialogService { get; set; } = default!;

        [Parameter]
        public int BecarioId { get; set; }

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
            
            _coordinadorName = authenticatedUserData.User.NickName()
                               ?? throw new NullReferenceException("No claim NickName found");

            _viewModel = await ApadrinamientoService.GetApadrinamientoEditDataAsync(authenticatedUserData.FilialId, BecarioId);

            if (!_viewModel.BecarioExists)
            {
                _loading = false;
                Snackbar.Add("No existe el becario en esta filial.", Severity.Error);
                NavigationManager.NavigateTo(NavRoutes.CoordinadorBecarios);
                return;
            }

            _pageTitle = $"Padrinos de {_viewModel.BecarioFullName}";

            _becarioFirstName = _viewModel.BecarioFirstName!;

            _padrinos = _viewModel.PadrinosForBecario;

            _loading = false;
        }

        private async Task<AssignNewPadrinoDialogViewModel?> OpenAssignNewPadrinoDialogAndGetDataAsync(bool getNewData, int? padrinoId = null, DateTime? from = null, DateTime? to = null)
        {
            if (!getNewData && padrinoId is null)
            {
                throw new ArgumentNullException(nameof(padrinoId));
            }

            if (!getNewData && from is null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            var parameters = new DialogParameters
            {
                ["Padrinos"] = _padrinos,
                ["GetNewData"] = getNewData,
            };
            
            if (!getNewData)
            {
                parameters.Add("PadrinoId", padrinoId!.Value);
                parameters.Add("From", from!.Value);
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

            return result.Data as AssignNewPadrinoDialogViewModel;
        }

        private async Task OpenAssignNewPadrinoDialogAsync()
        {
            var nuevaAsignación = await OpenAssignNewPadrinoDialogAndGetDataAsync(getNewData: true);
            if (nuevaAsignación is null)
            {
                return;
            }

            // Is there any overlap with another assignment for the same Padrino?
            var overlapWithSamePadrino = _viewModel.ApadrinamientosViewModel.Any(asignación =>
                string.Equals(asignación.PadrinoFullName, nuevaAsignación.SelectedPadrino!.DisplayName, StringComparison.OrdinalIgnoreCase)
                && ((nuevaAsignación.Desde <= asignación.From && (!nuevaAsignación.Hasta.HasValue || asignación.From <= nuevaAsignación.Hasta.Value))
                    || (asignación.From <= nuevaAsignación.Desde && (!asignación.To.HasValue || nuevaAsignación.Desde <= asignación.To.Value))));
            if (overlapWithSamePadrino)
            {
                Snackbar.Add($"{nuevaAsignación.SelectedPadrino!.DisplayName} ya apadrina a {_becarioFirstName} en ese período.", Severity.Error);
                return;
            }

            var apadrinamientoEditAssignPadrinoToBecarioModel = new ApadrinamientoEditAssignPadrinoToBecarioModel
            {
                BecarioId = BecarioId,
                PadrinoId = nuevaAsignación.SelectedPadrino!.Id,
                From = nuevaAsignación.Desde!.Value,
                To = nuevaAsignación.Hasta,
                CreatedByCoordinadorId = _coordinadorId,
            };

            var dataResult = await ApadrinamientoService.AssignPadrinoToBecarioAsync(apadrinamientoEditAssignPadrinoToBecarioModel);

            if (!dataResult.AnyRowsAffected)
            {
                Snackbar.Add("No se pudo almacenar la asignación.", Severity.Error);
                return;
            }

            var apadrinamientoEditViewModel = new ApadrinamientoEditApadrinamientoViewModel(nuevaAsignación.Desde!.Value, nuevaAsignación.Hasta)
            {
                ApadrinamientoId = dataResult.Data,
                PadrinoId = nuevaAsignación.SelectedPadrino.Id,
                PadrinoFullName = nuevaAsignación.SelectedPadrino.DisplayName,
                CreatedOnUtc = DateTimeOffset.UtcNow,
                CreatedBy = _coordinadorName,
            };

            _viewModel.ApadrinamientosViewModel.Add(apadrinamientoEditViewModel);
        }

        private async Task OpenAssignNewPadrinoDialogForEditAsync(int apadrinamientoId, int padrinoId, DateTime from, DateTime? to)
        {
            var asignaciónModificada = await OpenAssignNewPadrinoDialogAndGetDataAsync(getNewData: false, padrinoId, from, to);
            if (asignaciónModificada is null)
            {
                return;
            }

            var result = await ApadrinamientoService.UpdateApadrinamientoAsync(apadrinamientoId, asignaciónModificada.Desde!.Value, asignaciónModificada.Hasta, _coordinadorId);
            if (!result.AnyRowsAffected)
            {
                Snackbar.Add("No se pudo almacenar el cambio", Severity.Error);
                return;
            }

            var newStatus = new ApadrinamientoEditApadrinamientoViewModel(asignaciónModificada.Desde!.Value, asignaciónModificada.Hasta)
                .Status;

            var updatedApadrinamiento = _viewModel.ApadrinamientosViewModel.Single(pa => pa.ApadrinamientoId == apadrinamientoId);
            updatedApadrinamiento.PadrinoFullName = asignaciónModificada.SelectedPadrino!.DisplayName;
            updatedApadrinamiento.From = asignaciónModificada.Desde!.Value;
            updatedApadrinamiento.To = asignaciónModificada.Hasta;
            updatedApadrinamiento.Status = newStatus;
            updatedApadrinamiento.LastUpdatedOnUtc = DateTimeOffset.UtcNow;
            updatedApadrinamiento.UpdatedBy = _coordinadorName; // TODO: This should be the full name, but we would need to get that in the user's claims
        }

        private async Task SetToDateToToday(int apadrinamientoId)
        {
            var result = await ApadrinamientoService.SetToDateToTodayAsync(apadrinamientoId, _coordinadorId);
            if (!result.AnyRowsAffected)
            {
                Snackbar.Add("No se pudo almacenar el cambio", Severity.Error);
                return;
            }

            var updatedApadrinamiento = _viewModel.ApadrinamientosViewModel.Single(pa => pa.ApadrinamientoId == apadrinamientoId);
            updatedApadrinamiento.To = DateTime.Today;
            updatedApadrinamiento.LastUpdatedOnUtc = DateTimeOffset.UtcNow;
            updatedApadrinamiento.UpdatedBy = _coordinadorName;
        }

        private async Task SetToDateToUknown(int apadrinamientoId)
        {
            var result = await ApadrinamientoService.SetToDateToUknownAsync(apadrinamientoId, _coordinadorId);
            if (!result.AnyRowsAffected)
            {
                Snackbar.Add("No se pudo almacenar el cambio", Severity.Error);
                return;
            }

            var updatedApadrinamiento = _viewModel.ApadrinamientosViewModel.Single(pa => pa.ApadrinamientoId == apadrinamientoId);
            updatedApadrinamiento.To = null;
            updatedApadrinamiento.LastUpdatedOnUtc = DateTimeOffset.UtcNow;
            updatedApadrinamiento.UpdatedBy = _coordinadorName;
        }
    }
}
