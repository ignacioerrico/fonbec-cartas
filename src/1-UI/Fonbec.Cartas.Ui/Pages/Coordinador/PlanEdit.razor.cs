using System.Globalization;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Microsoft.AspNetCore.Components;
using Fonbec.Cartas.Logic.Services.MessageTemplate;
using MudBlazor;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class PlanEdit
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

        private PersonData _selectedPadrino = Padrino;
        private PersonData _selectedBecario = Ahijado;

        private MudForm _mudForm = default!;
        private bool _formValidationSucceeded;

        private DateTime? _startDate = FirstDayOfFollowingMonth;

        private string _subject = "Carta de tu {ahijado} {ahijado:nombre} de {mes-de-carta}";
        private string _renderedSubject = default!;

        private string _messageBody = default!;
        private string _renderedMessageBody = default!;
        private bool _highlight = false;

        private bool SaveButtonDisabled => !_formValidationSucceeded
                                           || string.IsNullOrWhiteSpace(_messageBody);

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
        public IMessageTemplateGetterService MessageTemplateGetterService { get; set; } = default!;

        [Inject]
        public IMessageTemplateParser MessageTemplateParser { get; set; } = default!;

        protected override void OnAfterRender(bool firstRender)
        {
            _mudForm.Validate();
        }

        protected override void OnInitialized()
        {
            OnSubjectChanged(_subject);
            
            _messageBody = MessageTemplateGetterService.GetDefaultMessage();
            OnMessageBodyChanged(_messageBody);
        }

        private Task Save()
        {
            // TODO
            return Task.CompletedTask;
        }

        private void OnStartDateChanged(DateTime? startDate)
        {
            if (startDate is null)
            {
                return;
            }

            _startDate = startDate;
            _messageTemplateData.Date = startDate.Value;
            UpdatePreview();
        }

        private void OnSubjectChanged(string subject)
        {
            _subject = subject;
            _renderedSubject = MessageTemplateParser.FillPlaceholders(subject, _messageTemplateData, _highlight);
        }

        private void OnMessageBodyChanged(string messageBody)
        {
            _messageBody = messageBody;
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
            OnSubjectChanged(_subject);
            OnMessageBodyChanged(_messageBody);
        }
    }
}
