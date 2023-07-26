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
            if (nuevaAsignación?.Desde is null)
            {
                return;
            }

            // Is there any overlap with another assignment for the same Padrino?
            var overlapWithSamePadrino = _viewModel.ApadrinamientosViewModel.Exists(asignación =>
                asignación.PadrinoId == nuevaAsignación.SelectedPadrino!.Id
                && (asignación.From.IsBetween(nuevaAsignación.Desde.Value, nuevaAsignación.Hasta)
                    || nuevaAsignación.Desde.Value.IsBetween(asignación.From, asignación.To)));
                
            if (overlapWithSamePadrino)
            {
                Snackbar.Add($"{nuevaAsignación.SelectedPadrino!.DisplayName} ya apadrina a {_becarioFirstName} en ese período.", Severity.Error);
                return;
            }

            var apadrinamientoEditNewApadrinamientoModel = new ApadrinamientoEditNewApadrinamientoModel
            {
                BecarioId = BecarioId,
                PadrinoId = nuevaAsignación.SelectedPadrino!.Id,
                From = nuevaAsignación.Desde!.Value,
                To = nuevaAsignación.Hasta,
                CreatedByCoordinadorId = _coordinadorId,
            };

            var result = await ApadrinamientoService.CreateApadrinamientoAsync(apadrinamientoEditNewApadrinamientoModel);

            if (!result.AnyRowsAffected)
            {
                Snackbar.Add("No se pudo almacenar la asignación.", Severity.Error);
                return;
            }

            var url = string.Format(NavRoutes.CoordinadorBecario0AsignarPadrinos, BecarioId);
            NavigationManager.NavigateTo(url, forceLoad: true);
        }

        private async Task OpenAssignNewPadrinoDialogForEditAsync(int apadrinamientoId, int padrinoId, DateTime from, DateTime? to)
        {
            var asignaciónModificada = await OpenAssignNewPadrinoDialogAndGetDataAsync(getNewData: false, padrinoId, from, to);
            if (asignaciónModificada is null)
            {
                return;
            }

            var apadrinamientoEditUpdateApadrinamientoModel = new ApadrinamientoEditUpdateApadrinamientoModel
            {
                ApadrinamientoId = apadrinamientoId,
                From = asignaciónModificada.Desde!.Value,
                To = asignaciónModificada.Hasta,
                UpdatedByCoordinadorId = _coordinadorId,
            };

            var result = await ApadrinamientoService.UpdateApadrinamientoAsync(apadrinamientoEditUpdateApadrinamientoModel);
            if (!result.AnyRowsAffected)
            {
                Snackbar.Add("No se pudo almacenar el cambio", Severity.Error);
                return;
            }

            var url = string.Format(NavRoutes.CoordinadorBecario0AsignarPadrinos, BecarioId);
            NavigationManager.NavigateTo(url, forceLoad: true);
        }

        private async Task SetToDateToToday(int apadrinamientoId)
        {
            var result = await ApadrinamientoService.SetToDateToTodayAsync(apadrinamientoId, _coordinadorId);
            if (!result.AnyRowsAffected)
            {
                Snackbar.Add("No se pudo almacenar el cambio", Severity.Error);
                return;
            }

            var url = string.Format(NavRoutes.CoordinadorBecario0AsignarPadrinos, BecarioId);
            NavigationManager.NavigateTo(url, forceLoad: true);
        }

        private async Task SetToDateToUknown(int apadrinamientoId)
        {
            var result = await ApadrinamientoService.SetToDateToUknownAsync(apadrinamientoId, _coordinadorId);
            if (!result.AnyRowsAffected)
            {
                Snackbar.Add("No se pudo almacenar el cambio", Severity.Error);
                return;
            }

            var url = string.Format(NavRoutes.CoordinadorBecario0AsignarPadrinos, BecarioId);
            NavigationManager.NavigateTo(url, forceLoad: true);
        }
    }
}
