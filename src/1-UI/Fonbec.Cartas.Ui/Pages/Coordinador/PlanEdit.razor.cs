using System.Globalization;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Microsoft.AspNetCore.Components;
using Fonbec.Cartas.Logic.Services.MessageTemplate;

namespace Fonbec.Cartas.Ui.Pages.Coordinador
{
    public partial class PlanEdit
    {
        private static readonly CultureInfo EsArCultureInfo = CultureInfo.GetCultureInfo("es-AR");

        private static readonly PersonData Padrino = new("Patricio", Gender.Male);
        private static readonly PersonData Madrina = new("María", Gender.Female);

        private static readonly PersonData Ahijado = new("Benjamín", Gender.Male);
        private static readonly PersonData Ahijada = new("Beatriz", Gender.Female);

        private PersonData _selectedPadrino = Padrino;
        private PersonData _selectedBecario = Ahijado;

        private DateTime? _name;

        private const string _subject = "Carta de tu {ahijado} {ahijado:nombre} de {mes-de-carta}";
        private string _renderedSubject = default!;

        private string _messageBody = default!;
        private string _renderedMessageBody = default!;

        private bool SaveButtonDisabled => !_name.HasValue
                                           || string.IsNullOrWhiteSpace(_subject)
                                           || string.IsNullOrWhiteSpace(_messageBody);

        private readonly MessageTemplateData _messageTemplateData = new()
        {
            Date = DateTime.Today,
            Documents = "la carta y el boletín",
            Padrino = Padrino,
            Becario = Ahijado,
        };

        [Inject]
        public IMessageTemplateGetterService MessageTemplateGetterService { get; set; } = default!;

        [Inject]
        public IMessageTemplateParser MessageTemplateParser { get; set; } = default!;

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

        private void OnSelectedPadrinoChanged(PersonData padrinoData)
        {
            _selectedPadrino = padrinoData;
            _messageTemplateData.Padrino = padrinoData;
            OnSubjectChanged(_subject);
            OnMessageBodyChanged(_messageBody);
        }

        private void OnSelectedBecarioChanged(PersonData becarioData)
        {
            _selectedBecario = becarioData;
            _messageTemplateData.Becario = becarioData;
            OnSubjectChanged(_subject);
            OnMessageBodyChanged(_messageBody);
        }

        private void OnMessageBodyChanged(string text)
        {
            _renderedMessageBody = MessageTemplateGetterService.GetHtmlMessage(text, _messageTemplateData);
        }

        private void OnSubjectChanged(string text)
        {
            _renderedSubject = MessageTemplateParser.FillPlaceholders(text, _messageTemplateData);
        }
    }
}
