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

        private PlannedCartaEditViewModel _viewModel = new();
        private PlannedCartaEditViewModel _originalViewModel = new();
        private int _plannedCartaId;

        private int _coordinadorId;

        private bool _loading;
        private bool _isNew;
        private string? _pageTitle;
        private string _saveButtonText = "Guardar";
        private bool _formValidationSucceeded;

        private List<DateTime> _takenCartasDates = new();

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
        public string PlannedCartaId { get; set; } = string.Empty;
        
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

            if (string.Equals(PlannedCartaId, NavRoutes.New, StringComparison.OrdinalIgnoreCase))
            {
                _isNew = true;

                _pageTitle = "Nueva planificación de cartas";
                _saveButtonText = "Crear";

                _takenCartasDates = await PlannedEventService.GetAllPlannedEventDates(authenticatedUserData.FilialId, PlannedEventType.CartaObligatoria);

                _viewModel.Subject = MessageTemplateGetterService.GetDefaultSubject();
                _viewModel.MessageMarkdown = MessageTemplateGetterService.GetDefaultMessageMarkdown();
                
                OnStartDateChanged();
            }
            else if (int.TryParse(PlannedCartaId, out _plannedCartaId) && _plannedCartaId > 0)
            {
                _isNew = false;

                var result = await PlannedEventService.GetPlannedCartaAsync(_plannedCartaId, authenticatedUserData.FilialId);

                if (!result.IsFound || result.Data is null)
                {
                    Snackbar.Add($"No se encontró planificación de carta con ID {_plannedCartaId}.", Severity.Error);
                    NavigationManager.NavigateTo(NavRoutes.CoordinadorPlanificaciónNew);
                    return;
                }

                _pageTitle = $"Editar planificación de cartas de {result.Data.Date.ToPlanName()}";
                _saveButtonText = "Actualizar";

                _viewModel = result.Data.Adapt<PlannedCartaEditViewModel>();
                _originalViewModel = result.Data.Adapt<PlannedCartaEditViewModel>();

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

                var result = await PlannedEventService.CreatePlannedCartaAsync(_viewModel);

                if (!result.AnyRowsAffected)
                {
                    Snackbar.Add("No se pudo crear esta planificación de cartas.", Severity.Error);
                }
            }
            else if (ModelHasChanged)
            {
                _viewModel.UpdatedByCoordinadorId = _coordinadorId;
                
                var result = await PlannedEventService.UpdatePlannedCartaAsync(_plannedCartaId, _viewModel);

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

            _viewModel.Date = _takenCartasDates.Any()
                ? _takenCartasDates.Max().AddMonths(1)
                : new DateTime(today.Year, today.Month, 1)
                    .AddMonths(1);
        }

        private bool IsDateDisabled(DateTime dateTime)
        {
            var today = DateTime.Today;

            var fechaDeCorte = new DateTime(today.Year, today.Month, 1);
            
            // It's possible to create "planned cartas" till the 10th of the month
            if (today.Day > 10)
            {
                fechaDeCorte = fechaDeCorte.AddMonths(1);
            }

            return dateTime < fechaDeCorte
                   || _takenCartasDates.Exists(taken => dateTime.Year == taken.Year && dateTime.Month == taken.Month);
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
