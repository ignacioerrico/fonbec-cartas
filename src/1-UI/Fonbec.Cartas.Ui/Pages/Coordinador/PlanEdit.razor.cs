using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.Services.MessageTemplate;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Fonbec.Cartas.Ui.Constants;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Globalization;
using Fonbec.Cartas.Logic.Services.Coordinador;

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

        private readonly PlanEditViewModel _plan = new();

        private bool _loading;

        private List<DateTime> _takenStartDates = new();

        private PersonData _selectedPadrino = Padrino;
        private PersonData _selectedBecario = Ahijado;

        private MudForm _mudForm = default!;
        private bool _formValidationSucceeded;

        private string _renderedSubject = default!;
        private string _renderedMessageBody = default!;
        private bool _highlight;

        private bool SaveButtonDisabled => !_formValidationSucceeded
                                           || string.IsNullOrWhiteSpace(_plan.MessageMarkdown);

        private readonly MessageTemplateData _messageTemplateData = new()
        {
            Date = FirstDayOfFollowingMonth,
            Documents = "la carta y el boletín",
            Padrino = Padrino,
            Becario = Ahijado,
            RevisorNombre = "Vicente",
            FilialNombre = "AMBA",
        };

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

            _plan.CreatedByCoordinadorId = authenticatedUserData.User.UserWithAccountId()
                                           ?? throw new NullReferenceException("No claim UserWithAccountId found");

            _plan.FilialId = authenticatedUserData.FilialId;

            _takenStartDates = await PlanService.GetAllPlansStartDates(authenticatedUserData.FilialId);

            OnSubjectChanged(_plan.Subject);
            
            _plan.MessageMarkdown = MessageTemplateGetterService.GetDefaultMessage();
            OnMessageBodyChanged(_plan.MessageMarkdown);

            _loading = false;
        }

        private async Task Save()
        {
            var result = await PlanService.CreatePlanAsync(_plan);

            if (!result.AnyRowsAffected)
            {
                Snackbar.Add("No se pudo crear el padrino.", Severity.Error);
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

            if (startDate.Value.AddDays(9) < DateTime.Today)
            {
                Snackbar.Add("La fecha de comienzo debe ser por lo menos el día 10 del mes corriente.", Severity.Error);
                _plan.StartDate = FirstDayOfFollowingMonth;
                return;
            }

            if (_takenStartDates.Contains(startDate.Value))
            {
                Snackbar.Add("Ya hay un plan para ese mes en esta filial.", Severity.Error);
                _plan.StartDate = startDate.Value.AddMonths(1);
                while (_takenStartDates.Contains(_plan.StartDate))
                {
                    _plan.StartDate = _plan.StartDate.AddMonths(1);
                }

                return;
            }

            _plan.StartDate = startDate.Value;
            _messageTemplateData.Date = startDate.Value;
            UpdatePreview();
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
