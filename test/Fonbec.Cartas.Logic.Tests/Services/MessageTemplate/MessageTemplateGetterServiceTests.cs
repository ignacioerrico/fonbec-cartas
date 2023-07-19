using System.Text;
using FluentAssertions;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Logic.Properties;
using Fonbec.Cartas.Logic.Services.MessageTemplate;
using Moq;

namespace Fonbec.Cartas.Logic.Tests.Services.MessageTemplate
{
    public class MessageTemplateGetterServiceTests
    {
        private readonly Mock<IMessageTemplateParser> _messageTemplateParserMock;
        private readonly Mock<IResourcesWrapper> _resourcesWrapperMock;
        
        private readonly MessageTemplateGetterService _sut;

        public MessageTemplateGetterServiceTests()
        {
            _messageTemplateParserMock = new Mock<IMessageTemplateParser>();
            _resourcesWrapperMock = new Mock<IResourcesWrapper>();

            var messageTempleate = new StringBuilder();
            messageTempleate.AppendLine("<html>");
            messageTempleate.AppendLine("<head>");
            messageTempleate.AppendLine("<title>Title</title>");
            messageTempleate.AppendLine("</head>");
            messageTempleate.AppendLine("<body>");
            messageTempleate.AppendLine("{BODY}");
            messageTempleate.AppendLine("</body>");
            messageTempleate.AppendLine("</html>");

            _resourcesWrapperMock
                .Setup(x => x.MessageTemplate)
                .Returns(messageTempleate.ToString());

            _sut = new MessageTemplateGetterService(_messageTemplateParserMock.Object, _resourcesWrapperMock.Object);
        }

        [Fact]
        public void GetDefaultSubject_ShouldReturnDefaultSubjectWithoutNewLinesAndSingleSpaced()
        {
            // Arrange
            _resourcesWrapperMock
                .Setup(x => x.DefaultSubject)
                .Returns("Default    subject\non a single\r\n   line   ");

            // Act
            var result = _sut.GetDefaultSubject();

            // Assert
            result.Should().Be("Default subject on a single line");
        }

        [Fact]
        public void GetDefaultMessageMarkdown_ShouldReturnDefaultMessageInMarkdown()
        {
            // Arrange
            _resourcesWrapperMock
                .Setup(x => x.DefaultMessageMarkdown)
                .Returns("Default message");

            // Act
            var result = _sut.GetDefaultMessageMarkdown();

            // Assert
            result.Should().Be("Default message");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void GetHtmlMessage_ShouldReturnContentsOfFile(bool highlight)
        {
            const string markdown = "First Line\nSecond *bold* line\nLast line";

            // Arrange
            _messageTemplateParserMock
                .Setup(x => x.FillPlaceholders(markdown,
                    It.Is<MessageTemplateData>(d =>
                        d.Revisor.Name == "Revisor"
                        && d.Revisor.Gender == Gender.Female
                        && d.FilialNombre == "Filial"),
                    It.IsAny<bool>()))
                .Returns("First Line\nSecond <strong>bold</strong> line\nSigned by {revisor:nombre} ({Voluntario}) in {filial:nombre} just for you\nLast line");

            var html = new StringBuilder();
            html.AppendLine("<p>First Line</p>");
            html.AppendLine("<p>Second <strong>bold</strong> line</p>");
            html.AppendLine("<p>Signed by {revisor:nombre} ({Voluntario}) in {filial:nombre} just for you</p>");
            html.AppendLine("<p>Last line</p>");

            _messageTemplateParserMock
                .Setup(x => x.MarkdownToHtml("First Line\nSecond <strong>bold</strong> line\nSigned by {revisor:nombre} ({Voluntario}) in {filial:nombre} just for you\nLast line"))
                .Returns(html.ToString);

            var expected = new StringBuilder();
            expected.AppendLine("<html>");
            expected.AppendLine("<head>");
            expected.AppendLine("<title>Title</title>");
            expected.AppendLine("</head>");
            expected.AppendLine("<body>");
            expected.AppendLine("<p>First Line</p>");
            expected.AppendLine("<p>Second <strong>bold</strong> line</p>");
            expected.AppendLine("<p>Signed by Revisor (Voluntaria) in Filial just for you</p>");
            expected.AppendLine("<p>Last line</p>");
            expected.AppendLine();
            expected.AppendLine("</body>");
            expected.AppendLine("</html>");

            var messageTemplateData = new MessageTemplateData
            {
                Revisor = new("Revisor", Gender.Female),
                FilialNombre = "Filial",
            };

            // Act
            var result = _sut.GetHtmlMessage(markdown, messageTemplateData, highlight);

            // Assert
            result.Should().Be(expected.ToString());
        }
    }
}
