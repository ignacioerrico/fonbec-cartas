using System.Text.RegularExpressions;

namespace Fonbec.Cartas.Logic.Services.MessageTemplate
{
    public interface IMessageTemplateGetterService
    {
        string GetDefaultMessage();
        string GetHtmlMessage(string markdown, MessageTemplateData data, bool highlight = false);
    }

    public class MessageTemplateGetterService : IMessageTemplateGetterService
    {
        private const string RevisorNombre = "{revisor:nombre}";

        private const string FilialNombre = "{filial:nombre}";

        private const string MessageTemplate = "MessageTemplate.html";

        private const string DefaultMessageMarkdown = "DefaultMessageMarkdown.txt";

        private const string BodyPlaceholder = "{BODY}";

        private readonly IMessageTemplateParser _messageTemplateParser;
        private readonly IEmbeddedResourceFileReader _embeddedResourceFileReader;

        private readonly string _htmlTemplate;

        public MessageTemplateGetterService(IMessageTemplateParser messageTemplateParser, IEmbeddedResourceFileReader embeddedResourceFileReader)
        {
            _messageTemplateParser = messageTemplateParser;
            _embeddedResourceFileReader = embeddedResourceFileReader;

            _htmlTemplate = embeddedResourceFileReader.Read(MessageTemplate);
        }

        public string GetDefaultMessage()
        {
            var defaultMessage = _embeddedResourceFileReader.Read(DefaultMessageMarkdown);
            return defaultMessage;
        }

        public string GetHtmlMessage(string markdown, MessageTemplateData data, bool highlight = false)
        {
            var body = _messageTemplateParser.FillPlaceholders(markdown, data, highlight);
            body = _messageTemplateParser.MarkdownToHtml(body);

            var htmlTemplate = Regex.Replace(_htmlTemplate, BodyPlaceholder, body);
            htmlTemplate = Regex.Replace(htmlTemplate, RevisorNombre, data.RevisorNombre);
            htmlTemplate = Regex.Replace(htmlTemplate, FilialNombre, data.FilialNombre);

            return htmlTemplate;
        }
    }
}
