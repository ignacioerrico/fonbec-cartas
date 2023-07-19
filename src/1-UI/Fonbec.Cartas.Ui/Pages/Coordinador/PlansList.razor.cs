using Fonbec.Cartas.Logic.ExtensionMethods;
using Microsoft.AspNetCore.Components;
using Fonbec.Cartas.Logic.Services.Coordinador;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Fonbec.Cartas.Ui.Components.Dialogs;
using MudBlazor;
using Fonbec.Cartas.Ui.Constants;
using Fonbec.Cartas.Logic.Models.Coordinador;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class PlansList : PerFilialComponentBase
    {
        private List<PlannedEventsListViewModel> _viewModels = new();

        private bool _loading;

        private int _coordinadorId;
        private int _filialId;

        private List<DateTime> _takenNotasDates = new();

        [Inject]
        public IPlannedEventService PlannedEventService { get; set; } = default!;

        [Inject]
        public IDialogService DialogService { get; set; } = default!;

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

            _filialId = authenticatedUserData.FilialId;

            _viewModels = await PlannedEventService.GetAllPlansAsync(authenticatedUserData.FilialId);

            _takenNotasDates = await PlannedEventService.GetAllPlannedEventDates(authenticatedUserData.FilialId);

            _loading = false;
        }

        private async Task OpenNewCorteRecepciónNotasDialog()
        {
            var parameters = new DialogParameters
            {
                ["TakenNotasDates"] = _takenNotasDates
            };
            var options = new DialogOptions
            {
                CloseButton = true,
                CloseOnEscapeKey = true
            };
            var dialog = await DialogService.ShowAsync<NewCorteRecepciónNotasDialog>("Crear corte de recepción de notas", parameters, options);
            var result = await dialog.Result;

            if (result.Canceled)
            {
                return;
            }

            var date = (DateTime)result.Data;

            var model = new PlannedEventsListPlannedCorteNotasModel
            {
                Date = date,
                FilialId = _filialId,
                CreatedByCoordinadorId = _coordinadorId,
            };

            await PlannedEventService.CreatePlannedCorteNotasAsync(model);

            NavigationManager.NavigateTo(NavRoutes.CoordinadorPlanificación, forceLoad: true);
        }

        private async Task OpenEditCorteRecepciónNotasDialog(int plannedCorteNotasId, DateTime selectedDate)
        {
            var parameters = new DialogParameters
            {
                ["TakenNotasDates"] = _takenNotasDates.Except(new[] { selectedDate }).ToList(),
                ["SelectedDate"] = selectedDate,
            };
            var options = new DialogOptions
            {
                CloseButton = true,
                CloseOnEscapeKey = true
            };
            var dialog = await DialogService.ShowAsync<NewCorteRecepciónNotasDialog>("Editar corte de recepción de notas", parameters, options);
            var result = await dialog.Result;

            if (result.Canceled)
            {
                return;
            }

            var date = (DateTime)result.Data;

            var model = new PlannedEventsListPlannedCorteNotasModel
            {
                PlannedCorteNotasId = plannedCorteNotasId,
                Date = date,
                FilialId = _filialId,
                CreatedByCoordinadorId = _coordinadorId,
            };

            await PlannedEventService.UpdatePlannedCorteNotasAsync(model);

            NavigationManager.NavigateTo(NavRoutes.CoordinadorPlanificación, forceLoad: true);
        }
    }
}
