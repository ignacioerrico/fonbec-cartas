using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Logic.Properties;
using System.Text.RegularExpressions;

namespace Fonbec.Cartas.Logic.Services.MessageTemplate
{
    public interface IMessageTemplateGetterService
    {
        string GetDefaultSubject();
        string GetDefaultMessageMarkdown();
        string GetHtmlMessage(string markdown, MessageTemplateData data, bool highlight = false);
    }

    public class MessageTemplateGetterService : IMessageTemplateGetterService
    {
        private const string RevisorNombre = "{revisor:nombre}";

        private const string VoluntarioCapitalized = "{Voluntario}";

        private const string FilialNombre = "{filial:nombre}";

        private const string BodyPlaceholder = "{BODY}";

        private readonly IMessageTemplateParser _messageTemplateParser;
        private readonly IResourcesWrapper _resourcesWrapper;

        public MessageTemplateGetterService(IMessageTemplateParser messageTemplateParser, IResourcesWrapper resourcesWrapper)
        {
            _messageTemplateParser = messageTemplateParser;
            _resourcesWrapper = resourcesWrapper;
        }

        public string GetDefaultSubject()
        {
            var defaultSubject = _resourcesWrapper.DefaultSubject;
            defaultSubject = Regex.Replace(defaultSubject, @"\t|\n|\r", " ");
            return Regex.Replace(defaultSubject, @"\s+", " ").Trim();
        }

        public string GetDefaultMessageMarkdown()
        {
            return _resourcesWrapper.DefaultMessageMarkdown;
        }

        public string GetHtmlMessage(string markdown, MessageTemplateData data, bool highlight = false)
        {
            var body = _messageTemplateParser.FillPlaceholders(markdown, data, highlight);
            body = _messageTemplateParser.MarkdownToHtml(body);

            var htmlTemplate = _resourcesWrapper.MessageTemplate;
            htmlTemplate = Regex.Replace(htmlTemplate, BodyPlaceholder, body);
            htmlTemplate = Regex.Replace(htmlTemplate, RevisorNombre, data.Revisor.Name);
            htmlTemplate = Regex.Replace(htmlTemplate, VoluntarioCapitalized, data.Revisor.Gender is Gender.Unknown or Gender.Male ? "Voluntario" : "Voluntaria");
            htmlTemplate = Regex.Replace(htmlTemplate, FilialNombre, data.FilialNombre);

            return htmlTemplate;
        }
    }
}
