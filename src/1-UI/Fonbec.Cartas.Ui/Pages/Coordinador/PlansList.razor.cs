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

        private List<DateTime> _takenDeadlinesDates = new();

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

            _takenDeadlinesDates = await PlannedEventService.GetAllDeadlinesDatesAsync(authenticatedUserData.FilialId);

            _loading = false;
        }

        private async Task OpenNewDeadlineDialog()
        {
            var parameters = new DialogParameters
            {
                ["TakenDeadlinesDates"] = _takenDeadlinesDates
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

            var model = new PlannedEventsListDeadlineModel
            {
                Date = date,
                FilialId = _filialId,
                CoordinadorId = _coordinadorId,
            };

            await PlannedEventService.CreateDeadlineAsync(model);

            NavigationManager.NavigateTo(NavRoutes.CoordinadorPlanificación, forceLoad: true);
        }

        private async Task OpenEditDeadlineDialog(int deadlineId, DateTime selectedDate)
        {
            var parameters = new DialogParameters
            {
                ["TakenDeadlinesDates"] = _takenDeadlinesDates.Except(new[] { selectedDate }).ToList(),
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

            var model = new PlannedEventsListDeadlineModel
            {
                DeadlineId = deadlineId,
                Date = date,
                FilialId = _filialId,
                CoordinadorId = _coordinadorId,
            };

            await PlannedEventService.UpdateDeadlineAsync(model);

            NavigationManager.NavigateTo(NavRoutes.CoordinadorPlanificación, forceLoad: true);
        }
    }
}
