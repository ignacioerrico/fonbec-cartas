using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.Services.MessageTemplate;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Fonbec.Cartas.Ui.Constants;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Globalization;
using Fonbec.Cartas.Logic.Services.Coordinador;
using Mapster;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class PlanEdit : PerFilialComponentBase
    {
        private static readonly CultureInfo EsArCultureInfo = CultureInfo.GetCultureInfo("es-AR");

        private static readonly DateTime FirstDayOfFollowingMonth =
            DateTime.Today
                .AddDays(1 - DateTime.Today.Day) // Go back to the first day
                .AddMonths(1);

        private static readonly PersonData Padrino = new("Patricio", Gender.Male);
        private static readonly PersonData Madrina = new("María", Gender.Female);

        private static readonly PersonData Ahijado = new("Benjamín", Gender.Male);
        private static readonly PersonData Ahijada = new("Beatriz", Gender.Female);

        private PlanEditViewModel _plan = new();
        private PlanEditViewModel _originalPlan = new();
        private int _planId;

        private int _coordinadorId;

        private bool _loading;
        private bool _isNew;
        private string? _pageTitle;
        private string _saveButtonText = "Guardar";

        private List<DateTime> _takenStartDates = new();

        private PersonData _selectedPadrino = Padrino;
        private PersonData _selectedBecario = Ahijado;

        private MudForm _mudForm = default!;
        private bool _formValidationSucceeded;

        private string _renderedSubject = default!;
        private string _renderedMessageBody = default!;
        private bool _highlight;

        private bool ModelHasChanged =>
            !string.Equals(_plan.Subject, _originalPlan.Subject, StringComparison.Ordinal)
            || !string.Equals(_plan.MessageMarkdown, _originalPlan.MessageMarkdown, StringComparison.Ordinal);

        private bool SaveButtonDisabled => _loading
                                           || !_formValidationSucceeded
                                           || string.IsNullOrWhiteSpace(_plan.Subject)
                                           || string.IsNullOrWhiteSpace(_plan.MessageMarkdown)
                                           || !ModelHasChanged;

        private readonly MessageTemplateData _messageTemplateData = new()
        {
            Date = FirstDayOfFollowingMonth,
            Documents = "la carta y el boletín",
            Padrino = Padrino,
            Becario = Ahijado,
            Revisor = new PersonData("Victoria", Gender.Female),
            FilialNombre = "AMBA",
        };

        [Parameter]
        public string PlanId { get; set; } = string.Empty;
        
        [Inject]
        public IPlanService PlanService { get; set; } = default!;

        [Inject]
        public IMessageTemplateGetterService MessageTemplateGetterService { get; set; } = default!;

        [Inject]
        public IMessageTemplateParser MessageTemplateParser { get; set; } = default!;

        public PlanEdit()
        {
            _plan.StartDate = FirstDayOfFollowingMonth;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            _mudForm.Validate();
        }

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

        private void OnStartDateChanged(DateTime? startDate)
        {
            if (startDate is null)
            {
                _plan.StartDate = FirstDayOfFollowingMonth;
                return;
            }

            _plan.StartDate = startDate.Value;
            _messageTemplateData.Date = startDate.Value;
            UpdatePreview();
        }

        private bool IsDateDisabled(DateTime dateTime)
        {
            return dateTime.Year < DateTime.Today.Year
                   || dateTime.Month < DateTime.Today.Month
                   || (dateTime.Month == DateTime.Today.Month && DateTime.Today.Day >= 10)
                   || _takenStartDates.Any(taken => dateTime.Year == taken.Year && dateTime.Month == taken.Month);
        }

        private void OnSubjectChanged(string subject)
        {
            _plan.Subject = subject;
            _renderedSubject = MessageTemplateParser.FillPlaceholders(subject, _messageTemplateData, _highlight);
        }

        private void OnMessageBodyChanged(string messageBody)
        {
            _plan.MessageMarkdown = messageBody;
            _renderedMessageBody = MessageTemplateGetterService.GetHtmlMessage(messageBody, _messageTemplateData, _highlight);
        }

        private void OnSelectedPadrinoChanged(PersonData padrinoData)
        {
            _selectedPadrino = padrinoData;
            _messageTemplateData.Padrino = padrinoData;
            UpdatePreview();
        }

        private void OnSelectedBecarioChanged(PersonData becarioData)
        {
            _selectedBecario = becarioData;
            _messageTemplateData.Becario = becarioData;
            UpdatePreview();
        }

        private void OnHighlightChanged(bool highlight)
        {
            _highlight = highlight;
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            OnSubjectChanged(_plan.Subject);
            OnMessageBodyChanged(_plan.MessageMarkdown);
        }
    }
}
