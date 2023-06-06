using System.Text;
using FluentAssertions;
using Fonbec.Cartas.Logic.Services.MessageTemplate;
using Fonbec.Cartas.DataAccess.Entities.Enums;

namespace Fonbec.Cartas.Logic.Tests.Services.MessageTemplate
{
    public class MessageTemplateParserTests
    {
        private readonly MessageTemplateParser _sut;

        public MessageTemplateParserTests()
        {
            _sut = new MessageTemplateParser();
        }

        #region FillPlaceholders()

        [Theory]
        [InlineData(Gender.Unknown, Gender.Unknown)]
        [InlineData(Gender.Unknown, Gender.Male)]
        [InlineData(Gender.Unknown, Gender.Female)]
        [InlineData(Gender.Male, Gender.Unknown)]
        [InlineData(Gender.Male, Gender.Male)]
        [InlineData(Gender.Male, Gender.Female)]
        [InlineData(Gender.Female, Gender.Unknown)]
        [InlineData(Gender.Female, Gender.Male)]
        [InlineData(Gender.Female, Gender.Female)]
        public void FillPlaceholders_ShouldParseDate(Gender genderPadrino, Gender genderBecario)
        {
            // Arrange
            const string markdown = "First line\nBeginning {mes-de-carta} End\nLast line";
            var data = new MessageTemplateData
            {
                Date = new DateTime(2023, 06, 12),
                Documents = "la carta y el boletín",
                Padrino = new PersonData("Padrino", genderPadrino),
                Becario = new PersonData("Becario", genderBecario)
            };

            // Act
            var result = _sut.FillPlaceholders(markdown, data);

            // Assert
            result.Should().Be("First line\nBeginning junio de 2023 End\nLast line");
        }

        [Theory]
        [InlineData(Gender.Unknown, Gender.Unknown)]
        [InlineData(Gender.Unknown, Gender.Male)]
        [InlineData(Gender.Unknown, Gender.Female)]
        [InlineData(Gender.Male, Gender.Unknown)]
        [InlineData(Gender.Male, Gender.Male)]
        [InlineData(Gender.Male, Gender.Female)]
        [InlineData(Gender.Female, Gender.Unknown)]
        [InlineData(Gender.Female, Gender.Male)]
        [InlineData(Gender.Female, Gender.Female)]
        public void FillPlaceholders_ShouldParseDocuments(Gender genderPadrino, Gender genderBecario)
        {
            // Arrange
            const string markdown = "First line\nBeginning {documentos} End\nLast line";
            var data = new MessageTemplateData
            {
                Date = new DateTime(2023, 06, 12),
                Documents = "la carta y el boletín",
                Padrino = new PersonData("Padrino", genderPadrino),
                Becario = new PersonData("Becario", genderBecario)
            };

            // Act
            var result = _sut.FillPlaceholders(markdown, data);

            // Assert
            result.Should().Be("First line\nBeginning la carta y el boletín End\nLast line");
        }

        [Theory]
        [InlineData(Gender.Unknown, Gender.Unknown, "padrino")]
        [InlineData(Gender.Unknown, Gender.Male, "padrino")]
        [InlineData(Gender.Unknown, Gender.Female, "padrino")]
        [InlineData(Gender.Male, Gender.Unknown, "padrino")]
        [InlineData(Gender.Male, Gender.Male, "padrino")]
        [InlineData(Gender.Male, Gender.Female, "padrino")]
        [InlineData(Gender.Female, Gender.Unknown, "madrina")]
        [InlineData(Gender.Female, Gender.Male, "madrina")]
        [InlineData(Gender.Female, Gender.Female, "madrina")]
        public void FillPlaceholders_ShouldParsePadrino(Gender genderPadrino, Gender genderBecario, string padrinoMadrina)
        {
            // Arrange
            const string markdown = "First line\nBeginning {padrino} End\nLast line";
            var data = new MessageTemplateData
            {
                Date = new DateTime(2023, 06, 12),
                Documents = "la carta y el boletín",
                Padrino = new PersonData("Padrino", genderPadrino),
                Becario = new PersonData("Becario", genderBecario),
            };

            // Act
            var result = _sut.FillPlaceholders(markdown, data);

            // Assert
            result.Should().Be($"First line\nBeginning {padrinoMadrina} End\nLast line");
        }

        [Theory]
        [InlineData(Gender.Unknown, Gender.Unknown, "Male name")]
        [InlineData(Gender.Unknown, Gender.Male, "Male name")]
        [InlineData(Gender.Unknown, Gender.Female, "Male name")]
        [InlineData(Gender.Male, Gender.Unknown, "Male name")]
        [InlineData(Gender.Male, Gender.Male, "Male name")]
        [InlineData(Gender.Male, Gender.Female, "Male name")]
        [InlineData(Gender.Female, Gender.Unknown, "Female name")]
        [InlineData(Gender.Female, Gender.Male, "Female name")]
        [InlineData(Gender.Female, Gender.Female, "Female name")]
        public void FillPlaceholders_ShouldParsePadrinoNombre(Gender genderPadrino, Gender genderBecario, string padrinoNombre)
        {
            // Arrange
            const string markdown = "First line\nBeginning {padrino:nombre} End\nLast line";
            var data = new MessageTemplateData
            {
                Date = new DateTime(2023, 06, 12),
                Documents = "la carta y el boletín",
                Padrino = new PersonData(padrinoNombre, genderPadrino),
                Becario = new PersonData("Becario", genderBecario),
            };

            // Act
            var result = _sut.FillPlaceholders(markdown, data);

            // Assert
            result.Should().Be($"First line\nBeginning {padrinoNombre} End\nLast line");
        }

        [Theory]
        [InlineData(Gender.Unknown, Gender.Unknown, "male noun")]
        [InlineData(Gender.Unknown, Gender.Male, "male noun")]
        [InlineData(Gender.Unknown, Gender.Female, "male noun")]
        [InlineData(Gender.Male, Gender.Unknown, "male noun")]
        [InlineData(Gender.Male, Gender.Male, "male noun")]
        [InlineData(Gender.Male, Gender.Female, "male noun")]
        [InlineData(Gender.Female, Gender.Unknown, "female noun")]
        [InlineData(Gender.Female, Gender.Male, "female noun")]
        [InlineData(Gender.Female, Gender.Female, "female noun")]
        public void FillPlaceholders_ShouldParsePadrinoPalabra(Gender genderPadrino, Gender genderBecario, string word)
        {
            // Arrange
            const string markdown = "First line\nBeginning {padrino:male noun:female noun} End\nLast line";
            var data = new MessageTemplateData
            {
                Date = new DateTime(2023, 06, 12),
                Documents = "la carta y el boletín",
                Padrino = new PersonData("Padrino", genderPadrino),
                Becario = new PersonData("Becario", genderBecario),
            };

            // Act
            var result = _sut.FillPlaceholders(markdown, data);

            // Assert
            result.Should().Be($"First line\nBeginning {word} End\nLast line");
        }

        [Theory]
        [InlineData(Gender.Unknown, Gender.Unknown, "ahijado")]
        [InlineData(Gender.Unknown, Gender.Male, "ahijado")]
        [InlineData(Gender.Unknown, Gender.Female, "ahijada")]
        [InlineData(Gender.Male, Gender.Unknown, "ahijado")]
        [InlineData(Gender.Male, Gender.Male, "ahijado")]
        [InlineData(Gender.Male, Gender.Female, "ahijada")]
        [InlineData(Gender.Female, Gender.Unknown, "ahijado")]
        [InlineData(Gender.Female, Gender.Male, "ahijado")]
        [InlineData(Gender.Female, Gender.Female, "ahijada")]
        public void FillPlaceholders_ShouldParseBecario(Gender genderPadrino, Gender genderBecario, string ahijadoAhijada)
        {
            // Arrange
            const string markdown = "First line\nBeginning {ahijado} End\nLast line";
            var data = new MessageTemplateData
            {
                Date = new DateTime(2023, 06, 12),
                Documents = "la carta y el boletín",
                Padrino = new PersonData("Padrino", genderPadrino),
                Becario = new PersonData("Becario", genderBecario),
            };

            // Act
            var result = _sut.FillPlaceholders(markdown, data);

            // Assert
            result.Should().Be($"First line\nBeginning {ahijadoAhijada} End\nLast line");
        }

        [Theory]
        [InlineData(Gender.Unknown, Gender.Unknown, "Male name")]
        [InlineData(Gender.Unknown, Gender.Male, "Male name")]
        [InlineData(Gender.Unknown, Gender.Female, "Female name")]
        [InlineData(Gender.Male, Gender.Unknown, "Male name")]
        [InlineData(Gender.Male, Gender.Male, "Male name")]
        [InlineData(Gender.Male, Gender.Female, "Female name")]
        [InlineData(Gender.Female, Gender.Unknown, "Male name")]
        [InlineData(Gender.Female, Gender.Male, "Male name")]
        [InlineData(Gender.Female, Gender.Female, "Female name")]
        public void FillPlaceholders_ShouldParseBecarioNombre(Gender genderPadrino, Gender genderBecario, string becarioNombre)
        {
            // Arrange
            const string markdown = "First line\nBeginning {ahijado:nombre} End\nLast line";
            var data = new MessageTemplateData
            {
                Date = new DateTime(2023, 06, 12),
                Documents = "la carta y el boletín",
                Padrino = new PersonData("Padrino", genderPadrino),
                Becario = new PersonData(becarioNombre, genderBecario),
            };

            // Act
            var result = _sut.FillPlaceholders(markdown, data);

            // Assert
            result.Should().Be($"First line\nBeginning {becarioNombre} End\nLast line");
        }

        [Theory]
        [InlineData(Gender.Unknown, Gender.Unknown, "male noun")]
        [InlineData(Gender.Unknown, Gender.Male, "male noun")]
        [InlineData(Gender.Unknown, Gender.Female, "female noun")]
        [InlineData(Gender.Male, Gender.Unknown, "male noun")]
        [InlineData(Gender.Male, Gender.Male, "male noun")]
        [InlineData(Gender.Male, Gender.Female, "female noun")]
        [InlineData(Gender.Female, Gender.Unknown, "male noun")]
        [InlineData(Gender.Female, Gender.Male, "male noun")]
        [InlineData(Gender.Female, Gender.Female, "female noun")]
        public void FillPlaceholders_ShouldParseBecarioPalabra(Gender genderPadrino, Gender genderBecario, string word)
        {
            // Arrange
            const string markdown = "First line\nBeginning {ahijado:male noun:female noun} End\nLast line";
            var data = new MessageTemplateData
            {
                Date = new DateTime(2023, 06, 12),
                Documents = "la carta y el boletín",
                Padrino = new PersonData("Padrino", genderPadrino),
                Becario = new PersonData("Becario", genderBecario),
            };

            // Act
            var result = _sut.FillPlaceholders(markdown, data);

            // Assert
            result.Should().Be($"First line\nBeginning {word} End\nLast line");
        }

        #endregion

        #region MarkdownToHtml()

        [Fact]
        public void MarkdownToHtml_ShouldParseTextInBold()
        {
            // Arrange
            const string markdown = "First line\nBeginning *text in bold* End\nLast line";

            var expected = new StringBuilder();
            expected.AppendLine("<p>First line</p>");
            expected.AppendLine("<p>Beginning <strong>text in bold</strong> End</p>");
            expected.AppendLine("<p>Last line</p>");

            // Act
            var result = _sut.MarkdownToHtml(markdown);

            // Assert
            result.Should().Be(expected.ToString());
        }

        [Fact]
        public void MarkdownToHtml_ShouldParseTextInItalics()
        {
            // Arrange
            const string markdown = "First line\nBeginning _text in italics_ End\nLast line";

            var expected = new StringBuilder();
            expected.AppendLine("<p>First line</p>");
            expected.AppendLine("<p>Beginning <em>text in italics</em> End</p>");
            expected.AppendLine("<p>Last line</p>");

            // Act
            var result = _sut.MarkdownToHtml(markdown);

            // Assert
            result.Should().Be(expected.ToString());
        }

        [Fact]
        public void MarkdownToHtml_ShouldParseLists()
        {
            // Arrange
            const string markdown = "This is a list\n- First item\n -   Second item\n-Third item\nThis is the last line";

            var expected = new StringBuilder();
            expected.AppendLine("<p>This is a list</p>");
            expected.AppendLine("<ul>");
            expected.AppendLine("<li style=\"list-style: none;\"><span style=\"color: #DFE55D; font-size: 1.4em; padding-right: 0.3em; position: relative; top: 0.1em;\">&#x22C6;</span>First item</li>");
            expected.AppendLine("<li style=\"list-style: none;\"><span style=\"color: #DFE55D; font-size: 1.4em; padding-right: 0.3em; position: relative; top: 0.1em;\">&#x22C6;</span>Second item</li>");
            expected.AppendLine("<li style=\"list-style: none;\"><span style=\"color: #DFE55D; font-size: 1.4em; padding-right: 0.3em; position: relative; top: 0.1em;\">&#x22C6;</span>Third item</li>");
            expected.AppendLine("</ul>");
            expected.AppendLine("<p>This is the last line</p>");

            // Act
            var result = _sut.MarkdownToHtml(markdown);

            // Assert
            result.Should().Be(expected.ToString());
        }

        [Theory]
        [InlineData("First line\n> This is a quote\nLast line")]
        [InlineData("First line\n > This is a quote\nLast line")]
        [InlineData("First line\n>This is a quote\nLast line")]
        [InlineData("First line\n   >   This is a quote\nLast line")]
        public void MarkdownToHtml_ShouldParseBlockquotes(string markdown)
        {
            // Arrange
            var expected = new StringBuilder();
            expected.AppendLine("<p>First line</p>");
            expected.AppendLine("<blockquote style=\"background: #f9f9f9; border-left: 10px solid #ccc; margin: 1.5em 10px; padding: 1.5em 10px; width: 600px;\">");
            expected.AppendLine("<p style=\"display: inline;\">This is a quote</p>");
            expected.AppendLine("</blockquote>");
            expected.AppendLine("<p>Last line</p>");

            // Act
            var result = _sut.MarkdownToHtml(markdown);

            // Assert
            result.Should().Be(expected.ToString());
        }

        [Theory]
        [InlineData("First line\n# This is a subtitle\nLast line")]
        [InlineData("First line\n # This is a subtitle\nLast line")]
        [InlineData("First line\n#This is a subtitle\nLast line")]
        [InlineData("First line\n   #   This is a subtitle\nLast line")]
        public void MarkdownToHtml_ShouldParseSubtitles(string markdown)
        {
            // Arrange
            var expected = new StringBuilder();
            expected.AppendLine("<p>First line</p>");
            expected.AppendLine("<p style=\"font-size: 20pt;\">This is a subtitle</p>");
            expected.AppendLine("<p>Last line</p>");

            // Act
            var result = _sut.MarkdownToHtml(markdown);

            // Assert
            result.Should().Be(expected.ToString());
        }

        [Theory]
        [InlineData("First line\n## Hello, world!\nLast line")]
        [InlineData("First line\n ## Hello, world!\nLast line")]
        [InlineData("First line\n##Hello, world!\nLast line")]
        [InlineData("First line\n   ##   Hello, world!\nLast line")]
        public void MarkdownToHtml_ShouldParseSalutations(string markdown)
        {
            // Arrange
            var expected = new StringBuilder();
            expected.AppendLine("<p>First line</p>");
            expected.AppendLine("<p style=\"padding-bottom: 1em;\">Hello, world!</p>");
            expected.AppendLine("<p>Last line</p>");

            // Act
            var result = _sut.MarkdownToHtml(markdown);

            // Assert
            result.Should().Be(expected.ToString());
        }

        [Theory]
        [InlineData("First line in message\n### Title\nFirst line in post it\nSecond line in post it\n- List item 1\n- List item 2\nLast line in post it\n\nLast line in message")]
        [InlineData("First line in message\n ### Title\nFirst line in post it\nSecond line in post it\n- List item 1\n- List item 2\nLast line in post it\n\nLast line in message")]
        [InlineData("First line in message\n###Title\nFirst line in post it\nSecond line in post it\n- List item 1\n- List item 2\nLast line in post it\n\nLast line in message")]
        [InlineData("First line in message\n   ###   Title\nFirst line in post it\nSecond line in post it\n- List item 1\n- List item 2\nLast line in post it\n\nLast line in message")]
        public void MarkdownToHtml_ShouldParsePostItNotes(string markdown)
        {
            // Arrange
            var expected = new StringBuilder();
            expected.AppendLine("<p>First line in message</p>");
            expected.AppendLine("<div style=\"margin: 25px; padding: 35px 35px 35px 35px; position: relative; border: 1px solid #E8E8E8; border-bottom-right-radius: 60px 5px; display: inline-block; background: #ffff88; background: -moz-linear-gradient(-45deg, #ffff88 81%, #ffff88 82%, #ffff88 82%, #ffffc6 100%); background: -webkit-gradient(linear, left top, right bottom, color-stop(81%,#ffff88), color-stop(82%,#ffff88), color-stop(82%,#ffff88), color-stop(100%,#ffffc6)); background: -webkit-linear-gradient(-45deg, #ffff88 81%,#ffff88 82%,#ffff88 82%,#ffffc6 100%); background: -o-linear-gradient(-45deg, #ffff88 81%,#ffff88 82%,#ffff88 82%,#ffffc6 100%); background: -ms-linear-gradient(-45deg, #ffff88 81%,#ffff88 82%,#ffff88 82%,#ffffc6 100%); background: linear-gradient(135deg, #ffff88 81%,#ffff88 82%,#ffff88 82%,#ffffc6 100%); filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#ffff88', endColorstr='#ffffc6',GradientType=1 ); width: 500px;\">");
            expected.AppendLine("<p style=\"font-size: 1.4em; margin-top: 0; padding-bottom: .2em; border-bottom: 2px solid #0B9C8B;\">Title</p>");
            expected.AppendLine("<p>First line in post it</p>");
            expected.AppendLine("<p>Second line in post it</p>");
            expected.AppendLine("<ul>");
            expected.AppendLine("<li style=\"list-style: none;\"><span style=\"color: #DFE55D; font-size: 1.4em; padding-right: 0.3em; position: relative; top: 0.1em;\">&#x22C6;</span>List item 1</li>");
            expected.AppendLine("<li style=\"list-style: none;\"><span style=\"color: #DFE55D; font-size: 1.4em; padding-right: 0.3em; position: relative; top: 0.1em;\">&#x22C6;</span>List item 2</li>");
            expected.AppendLine("</ul>");
            expected.AppendLine("<p>Last line in post it</p>");
            expected.AppendLine("</div>");
            expected.AppendLine("<p>Last line in message</p>");

            // Act
            var result = _sut.MarkdownToHtml(markdown);

            // Assert
            result.Should().Be(expected.ToString());
        }

        [Fact]
        public void MarkdownToHtml_ShouldParseParagraphsIgnoringWhitespace()
        {
            // Arrange
            const string markdown = "\n\n  First paragraph\n\n\n   Second paragraph   \nThird paragraph   \n\n\n\n";

            var expected = new StringBuilder();
            expected.AppendLine("<p>First paragraph</p>");
            expected.AppendLine("<p>Second paragraph</p>");
            expected.AppendLine("<p>Third paragraph</p>");

            // Act
            var result = _sut.MarkdownToHtml(markdown);

            // Assert
            result.Should().Be(expected.ToString());
        }

        #endregion
    }
}
