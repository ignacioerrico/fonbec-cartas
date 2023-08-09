using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.Services.MessageTemplate;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Fonbec.Cartas.Ui.Constants;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Globalization;
using Fonbec.Cartas.Logic.Services.Coordinador;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Ui.Components.Coordinador;
using Mapster;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class PlanEdit : PerFilialComponentBase
    {
        private static readonly CultureInfo EsArCultureInfo = CultureInfo.GetCultureInfo("es-AR");

        private PlannedEventEditViewModel _viewModel = new();
        private PlannedEventEditViewModel _originalViewModel = new();

        private int _coordinadorId;

        private bool _loading;
        private bool _isNew;
        private string? _pageTitle;
        private string _saveButtonText = "Guardar";
        private bool _formValidationSucceeded;

        private List<DateTime> _takenPlannedEventDates = new();

        private readonly MessageTemplateData _messageTemplateData = new()
        {
            Documents = "la carta y el boletín",
            Padrino = PersonSelectorForPreview.Padrino,
            Becario = PersonSelectorForPreview.Ahijado,
            Revisor = new PersonData("Victoria", Gender.Female),
        };

        private bool _highlight;

        private string _renderedSubject = default!;
        private string _renderedMessageBody = default!;

        private bool ModelHasChanged => !string.Equals(_viewModel.Subject, _originalViewModel.Subject, StringComparison.Ordinal)
                                        || !string.Equals(_viewModel.MessageMarkdown, _originalViewModel.MessageMarkdown, StringComparison.Ordinal);

        private bool SaveButtonDisabled => _loading
                                           || !_formValidationSucceeded
                                           || !ModelHasChanged;

        [Parameter]
        public string PlannedEventId { get; set; } = string.Empty;
        
        [Inject]
        public IPlannedEventService PlannedEventService { get; set; } = default!;

        [Inject]
        public IMessageTemplateGetterService MessageTemplateGetterService { get; set; } = default!;

        [Inject]
        public IMessageTemplateParser MessageTemplateParser { get; set; } = default!;

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

            _viewModel.FilialId = authenticatedUserData.FilialId;

            _messageTemplateData.FilialNombre = authenticatedUserData.User.FilialName() ?? "AMBA";

            if (string.Equals(PlannedEventId, NavRoutes.New, StringComparison.OrdinalIgnoreCase))
            {
                _isNew = true;

                _pageTitle = "Nueva planificación de cartas";
                _saveButtonText = "Crear";

                _takenPlannedEventDates = await PlannedEventService.GetAllPlannedEventDatesAsync(authenticatedUserData.FilialId);

                _viewModel.Subject = MessageTemplateGetterService.GetDefaultSubject();
                _viewModel.MessageMarkdown = MessageTemplateGetterService.GetDefaultMessageMarkdown();
                
                OnStartDateChanged();
            }
            else if (int.TryParse(PlannedEventId, out var plannedEventId) && plannedEventId > 0)
            {
                _isNew = false;

                var result = await PlannedEventService.GetPlannedEventAsync(plannedEventId, authenticatedUserData.FilialId);

                if (!result.IsFound || result.Data is null)
                {
                    Snackbar.Add($"No se encontró planificación de carta con ID {plannedEventId}.", Severity.Error);
                    NavigationManager.NavigateTo(NavRoutes.CoordinadorPlanificaciónNew);
                    return;
                }

                _pageTitle = $"Editar planificación de cartas de {result.Data.Date.ToPlanName()}";
                _saveButtonText = "Actualizar";

                _viewModel = result.Data.Adapt<PlannedEventEditViewModel>();
                _originalViewModel = result.Data.Adapt<PlannedEventEditViewModel>();

                OnStartDateChanged(_viewModel.Date);
            }
            else
            {
                NavigationManager.NavigateTo(NavRoutes.CoordinadorPlanificación);
            }

            _loading = false;
        }

        private async Task Save()
        {
            if (_isNew)
            {
                _viewModel.CreatedByCoordinadorId = _coordinadorId;

                var result = await PlannedEventService.CreatePlannedEventAsync(_viewModel);

                if (!result.AnyRowsAffected)
                {
                    Snackbar.Add("No se pudo crear esta planificación de cartas.", Severity.Error);
                }
            }
            else if (ModelHasChanged)
            {
                _viewModel.UpdatedByCoordinadorId = _coordinadorId;
                
                var result = await PlannedEventService.UpdatePlannedEventAsync(_viewModel);

                if (!result.AnyRowsAffected)
                {
                    Snackbar.Add("No se pudo actualizar esta planificación de cartas.", Severity.Error);
                }
            }

            NavigationManager.NavigateTo(NavRoutes.CoordinadorPlanificación);
        }

        private void SetInitialStartDate()
        {
            var today = DateTime.Today;

            _viewModel.Date = _takenPlannedEventDates.Any()
                ? _takenPlannedEventDates.Max().AddMonths(1)
                : new DateTime(today.Year, today.Month, 1)
                    .AddMonths(1);
        }

        private bool IsDateDisabled(DateTime dateTime)
        {
            var today = DateTime.Today;

            var earliestSelectableDate = new DateTime(today.Year, today.Month, 1);
            
            // It's possible to create a planned event till the 10th of the month
            if (today.Day > 10)
            {
                earliestSelectableDate = earliestSelectableDate.AddMonths(1);
            }

            return dateTime < earliestSelectableDate
                   || _takenPlannedEventDates.Exists(taken =>
                       dateTime.Year == taken.Year
                       && dateTime.Month == taken.Month);
        }

        private void UpdatePreview()
        {
            _renderedSubject = MessageTemplateParser.FillPlaceholders(_viewModel.Subject, _messageTemplateData, _highlight);
            _renderedMessageBody = MessageTemplateGetterService.GetHtmlMessage(_viewModel.MessageMarkdown, _messageTemplateData, _highlight);
        }

        private void OnStartDateChanged(DateTime? startDate = null)
        {
            if (startDate is null)
            {
                SetInitialStartDate();
            }
            else
            {
                _viewModel.Date = startDate.Value;
            }

            _messageTemplateData.Date = _viewModel.Date;
            UpdatePreview();
        }

        private void OnSubjectChanged(string subject)
        {
            _viewModel.Subject = subject;
            UpdatePreview();
        }

        private void OnMessageMarkdownChanged(string messageMarkdown)
        {
            _viewModel.MessageMarkdown = messageMarkdown;
            UpdatePreview();
        }
    }
}
