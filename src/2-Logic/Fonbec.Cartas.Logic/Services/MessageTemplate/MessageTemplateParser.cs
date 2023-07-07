using Fonbec.Cartas.DataAccess.Entities.Enums;
using System.Text.RegularExpressions;
using System.Text;
using Fonbec.Cartas.Logic.ExtensionMethods;

namespace Fonbec.Cartas.Logic.Services.MessageTemplate
{
    public interface IMessageTemplateParser
    {
        string FillPlaceholders(string markdown, MessageTemplateData data, bool highlight = false);
        string MarkdownToHtml(string markdown);
    }

    public class MessageTemplateParser : IMessageTemplateParser
    {
        private const string MesDeCarta = "{mes-de-carta}";

        private const string Documentos = "{documentos}";

        private const string Padrino = "{padrino}";
        private const string PadrinoNombre = "{padrino:nombre}";
        private const string PadrinoPalabra = @"{padrino:([^:]+):([^}]+)}";

        private const string Becario = "{ahijado}";
        private const string BecarioNombre = "{ahijado:nombre}";
        private const string BecarioPalabra = @"{ahijado:([^:]+):([^}]+)}";

        private const string Bold = @"\*(([^*]+))\*";
        private const string Italics = @"_(([^_]+))_";

        public string FillPlaceholders(string markdown, MessageTemplateData data, bool highlight = false)
        {
            var parsedText = Regex.Replace(markdown, MesDeCarta,
                WrapInHighlightedSpan(highlight,
                    data.Date.ToPlanName()));

            parsedText = Regex.Replace(parsedText, Documentos,
                WrapInHighlightedSpan(highlight, data.Documents));

            parsedText = Regex.Replace(parsedText, Padrino,
                data.Padrino.Gender is Gender.Unknown or Gender.Male
                    ? WrapInHighlightedSpan(highlight, "padrino")
                    : WrapInHighlightedSpan(highlight, "madrina"));
            parsedText = Regex.Replace(parsedText, PadrinoNombre,
                WrapInHighlightedSpan(highlight, data.Padrino.Name));
            parsedText = Regex.Replace(parsedText, PadrinoPalabra,
                data.Padrino.Gender is Gender.Unknown or Gender.Male
                    ? WrapInHighlightedSpan(highlight, "$1")
                    : WrapInHighlightedSpan(highlight, "$2"));

            parsedText = Regex.Replace(parsedText, Becario,
                data.Becario.Gender is Gender.Unknown or Gender.Male
                    ? WrapInHighlightedSpan(highlight, "ahijado")
                    : WrapInHighlightedSpan(highlight, "ahijada"));
            parsedText = Regex.Replace(parsedText, BecarioNombre,
                WrapInHighlightedSpan(highlight, data.Becario.Name));
            parsedText = Regex.Replace(parsedText, BecarioPalabra,
                data.Becario.Gender is Gender.Unknown or Gender.Male
                    ? WrapInHighlightedSpan(highlight, "$1")
                    : WrapInHighlightedSpan(highlight, "$2"));

            return parsedText;
        }

        public string MarkdownToHtml(string markdown)
        {
            var parsedText = Regex.Replace(markdown, Bold, "<strong>$1</strong>");
            parsedText = Regex.Replace(parsedText, Italics, "<em>$1</em>");

            var lines = parsedText.Split("\n", StringSplitOptions.TrimEntries);

            var sb = new StringBuilder();

            var index = 0;

            while (true)
            {
                if (index >= lines.Length)
                {
                    break;
                }

                var line = lines[index];

                if (string.IsNullOrWhiteSpace(line))
                {
                    ++index;
                    continue;
                }

                if (line.StartsWith("###"))
                {
                    var chopped = Regex.Replace(line, @"^###\s*", string.Empty);
                    sb.AppendLine("<div style=\"margin: 25px; padding: 35px 35px 35px 35px; position: relative; border: 1px solid #E8E8E8; border-bottom-right-radius: 60px 5px; display: inline-block; background: #ffff88; background: -moz-linear-gradient(-45deg, #ffff88 81%, #ffff88 82%, #ffff88 82%, #ffffc6 100%); background: -webkit-gradient(linear, left top, right bottom, color-stop(81%,#ffff88), color-stop(82%,#ffff88), color-stop(82%,#ffff88), color-stop(100%,#ffffc6)); background: -webkit-linear-gradient(-45deg, #ffff88 81%,#ffff88 82%,#ffff88 82%,#ffffc6 100%); background: -o-linear-gradient(-45deg, #ffff88 81%,#ffff88 82%,#ffff88 82%,#ffffc6 100%); background: -ms-linear-gradient(-45deg, #ffff88 81%,#ffff88 82%,#ffff88 82%,#ffffc6 100%); background: linear-gradient(135deg, #ffff88 81%,#ffff88 82%,#ffff88 82%,#ffffc6 100%); filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#ffff88', endColorstr='#ffffc6',GradientType=1 ); width: 500px;\">");
                    sb.AppendLine($"<p style=\"font-size: 1.4em; margin-top: 0; padding-bottom: .2em; border-bottom: 2px solid #0B9C8B;\">{chopped}</p>");
                    ++index;

                    var body = new StringBuilder();

                    while (index < lines.Length)
                    {
                        line = lines[index];
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            break;
                        }

                        body.AppendLine(line);
                        ++index;
                    }

                    var html = MarkdownToHtml(string.Join("\n", body))
                        .TrimEnd(Environment.NewLine.ToCharArray());
                    sb.AppendLine(html);
                    sb.AppendLine("</div>");

                }
                else if (line.StartsWith("##"))
                {
                    var chopped = Regex.Replace(line, @"^##\s*", string.Empty);
                    sb.AppendLine($"<p style=\"padding-bottom: 1em;\">{chopped}</p>");
                }
                else if (line.StartsWith("#"))
                {
                    var chopped = Regex.Replace(line, @"^#\s*", string.Empty);
                    sb.AppendLine($"<p style=\"font-size: 20pt;\">{chopped}</p>");
                }
                else if (line.StartsWith(">"))
                {
                    var chopped = Regex.Replace(line, @"^>\s*", string.Empty);
                    sb.AppendLine("<blockquote style=\"background: #f9f9f9; border-left: 10px solid #ccc; margin: 1.5em 10px; padding: 1.5em 10px; width: 600px;\">");
                    sb.AppendLine($"<p style=\"display: inline;\">{chopped}</p>");
                    sb.AppendLine("</blockquote>");
                }
                else if (line.StartsWith("-"))
                {
                    sb.AppendLine("<ul>");

                    while (index < lines.Length)
                    {
                        line = lines[index];
                        if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("-"))
                        {
                            --index;
                            break;
                        }

                        var chopped = Regex.Replace(line, @"^-\s*", string.Empty);
                        sb.AppendLine($"<li style=\"list-style: none;\"><span style=\"color: #DFE55D; font-size: 1.4em; padding-right: 0.3em; position: relative; top: 0.1em;\">&#x22C6;</span>{chopped}</li>");
                        ++index;
                    }

                    sb.AppendLine("</ul>");
                }
                else
                {
                    sb.AppendLine($"<p>{line}</p>");
                }

                ++index;
            }

            return sb.ToString();
        }

        private string WrapInHighlightedSpan(bool wrap, string text)
        {
            const string spanOpen = "<span style=\"background: yellow;\">";
            const string spanClose = "</span>";

            return wrap
                ? $"{spanOpen}{text}{spanClose}"
                : text;
        }
    }
}
