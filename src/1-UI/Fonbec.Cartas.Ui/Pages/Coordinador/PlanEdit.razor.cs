using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.Services.MessageTemplate;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Fonbec.Cartas.Ui.Constants;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Globalization;
using Fonbec.Cartas.Logic.Services.Coordinador;
using Mapster;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Ui.Components.Coordinador;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class PlanEdit : PerFilialComponentBase
    {
        private static readonly CultureInfo EsArCultureInfo = CultureInfo.GetCultureInfo("es-AR");

        private PlanEditViewModel _plan = new();
        private PlanEditViewModel _originalPlan = new();
        private int _planId;

        private int _coordinadorId;

        private bool _loading;
        private bool _isNew;
        private string? _pageTitle;
        private string _saveButtonText = "Guardar";
        private bool _formValidationSucceeded;

        private List<DateTime> _takenStartDates = new();

        private readonly MessageTemplateData _messageTemplateData = new()
        {
            Documents = "la carta y el boletín",
            Padrino = PersonSelectorForPreview.Padrino,
            Becario = PersonSelectorForPreview.Ahijado,
            Revisor = new PersonData("Victoria", Gender.Female),
            FilialNombre = "AMBA",
        };

        private bool _highlight;

        private string _renderedSubject = default!;
        private string _renderedMessageBody = default!;

        private bool ModelHasChanged => !string.Equals(_plan.Subject, _originalPlan.Subject, StringComparison.Ordinal)
                                        || !string.Equals(_plan.MessageMarkdown, _originalPlan.MessageMarkdown, StringComparison.Ordinal);

        private bool SaveButtonDisabled => _loading
                                           || !_formValidationSucceeded
                                           || !ModelHasChanged;

        [Parameter]
        public string PlanId { get; set; } = string.Empty;
        
        [Inject]
        public IPlanService PlanService { get; set; } = default!;

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

            _plan.FilialId = authenticatedUserData.FilialId;

            if (string.Equals(PlanId, NavRoutes.New, StringComparison.OrdinalIgnoreCase))
            {
                _isNew = true;

                _pageTitle = "Plan nuevo";
                _saveButtonText = "Crear";

                _takenStartDates = await PlanService.GetAllPlansStartDates(authenticatedUserData.FilialId);

                SetInitialStartDate();
                _plan.MessageMarkdown = MessageTemplateGetterService.GetDefaultMessage();
            }
            else if (int.TryParse(PlanId, out _planId) && _planId > 0)
            {
                _isNew = false;

                var result = await PlanService.GetPlanAsync(_planId, authenticatedUserData.FilialId);

                if (!result.IsFound || result.Data is null)
                {
                    Snackbar.Add($"No se encontró plan con ID {_planId}.", Severity.Error);
                    NavigationManager.NavigateTo(NavRoutes.CoordinadorPlanNew);
                    return;
                }

                _pageTitle = $"Editar plan de {result.Data.StartDate.ToPlanName()}";
                _saveButtonText = "Actualizar";

                _plan = result.Data.Adapt<PlanEditViewModel>();
                _originalPlan = result.Data.Adapt<PlanEditViewModel>();
            }
            else
            {
                NavigationManager.NavigateTo(NavRoutes.CoordinadorPlanes);
            }

            UpdatePreview();

            _loading = false;
        }

        private async Task Save()
        {
            if (_isNew)
            {
                _plan.CreatedByCoordinadorId = _coordinadorId;

                var result = await PlanService.CreatePlanAsync(_plan);

                if (!result.AnyRowsAffected)
                {
                    Snackbar.Add("No se pudo crear el plan.", Severity.Error);
                }
            }
            else if (ModelHasChanged)
            {
                _plan.UpdatedByCoordinadorId = _coordinadorId;
                
                var result = await PlanService.UpdatePlanAsync(_planId, _plan);

                if (!result.AnyRowsAffected)
                {
                    Snackbar.Add("No se pudo actualizar el plan.", Severity.Error);
                }
            }

            NavigationManager.NavigateTo(NavRoutes.CoordinadorPlanes);
        }

        private void SetInitialStartDate()
        {
            _plan.StartDate = _takenStartDates.Any()
                ? _takenStartDates.Max().AddMonths(1)
                : new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        }

        private bool IsDateDisabled(DateTime dateTime)
        {
            return dateTime.Year < DateTime.Today.Year
                   || dateTime.Month < DateTime.Today.Month
                   || (dateTime.Month == DateTime.Today.Month && DateTime.Today.Day >= 10)
                   || _takenStartDates.Any(taken => dateTime.Year == taken.Year && dateTime.Month == taken.Month);
        }

        private void UpdatePreview()
        {
            _renderedSubject = MessageTemplateParser.FillPlaceholders(_plan.Subject, _messageTemplateData, _highlight);
            _renderedMessageBody = MessageTemplateGetterService.GetHtmlMessage(_plan.MessageMarkdown, _messageTemplateData, _highlight);
        }

        private void OnStartDateChanged(DateTime? startDate)
        {
            if (startDate is null)
            {
                SetInitialStartDate();
            }
            else
            {
                _plan.StartDate = startDate.Value;
            }

            _messageTemplateData.Date = _plan.StartDate;
            UpdatePreview();
        }

        private void OnSubjectChanged(string subject)
        {
            _plan.Subject = subject;
            UpdatePreview();
        }

        private void OnMessageMarkdownChanged(string messageMarkdown)
        {
            _plan.MessageMarkdown = messageMarkdown;
            UpdatePreview();
        }
    }
}
