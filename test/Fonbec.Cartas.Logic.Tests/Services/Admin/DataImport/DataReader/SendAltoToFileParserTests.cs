using FluentAssertions.Execution;
using FluentAssertions;
using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Repositories.Admin.DataImport;
using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader;
using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader.Payloads;
using Moq;
using System.Text;

namespace Fonbec.Cartas.Logic.Tests.Services.Admin.DataImport.DataReader
{
    public class SendAltoToFileParserTests
    {
        private readonly SendAltoToFileParser _sut;

        public SendAltoToFileParserTests()
        {
            var dataImportRepositoryMock = new Mock<IDataImportRepository>();

            dataImportRepositoryMock
                .Setup(x => x.GetAllSendAlsoTosAsync())
                .ReturnsAsync(new List<SendAlsoTo>());

            _sut = new SendAltoToFileParser(dataImportRepositoryMock.Object);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ConvertToObjects_Success_AllFieldsSpecified(bool bcc)
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("padrino,nombre-completo,email,bcc");
            fileContents.AppendLine($"PadrinoNombre PadrinoApellido, DestinatarioNombre DestinatarioApellido, destinatario@email.com, {bcc.ToString().ToLower()}");

            var sendAlsoToPayload = new SendAlsoToPayload
            {
                PadrinosToCreate = new()
                {
                    new()
                    {
                        FirstName = "PadrinoNombre",
                        LastName = "PadrinoApellido",
                    }
                }
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), sendAlsoToPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().BeEmpty();

                result.Should().ContainSingle();

                result.Single().RecipientFullName.Should().Be("DestinatarioNombre DestinatarioApellido");
                result.Single().RecipientEmail.Should().Be("destinatario@email.com");
                result.Single().SendAsBcc.Should().Be(bcc);

                sendAlsoToPayload.PadrinosToCreate.Single().SendAlsoTo.Should().NotBeNull();
                sendAlsoToPayload.PadrinosToCreate.Single().SendAlsoTo.Should().ContainSingle();
                var sendAlsoTo = sendAlsoToPayload.PadrinosToCreate.Single().SendAlsoTo!.Single();
                sendAlsoTo.RecipientFullName.Should().Be("DestinatarioNombre DestinatarioApellido");
                sendAlsoTo.RecipientEmail.Should().Be("destinatario@email.com");
                sendAlsoTo.SendAsBcc.Should().Be(bcc);
            }
        }

        [Fact]
        public async Task ConvertToObjects_Success_PadrinoIdFound()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("padrino,nombre-completo,email,bcc");
            fileContents.AppendLine("78, DestinatarioNombre DestinatarioApellido, destinatario@email.com, true");

            var sendAlsoToPayload = new SendAlsoToPayload
            {
                PadrinosToUpdate = new()
                {
                    new() { Id = 77 },
                    new() { Id = 78 },
                    new() { Id = 79 },
                    new() { Id = 80 },
                }
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), sendAlsoToPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().BeEmpty();

                result.Should().ContainSingle();

                result.Single().RecipientFullName.Should().Be("DestinatarioNombre DestinatarioApellido");
                result.Single().RecipientEmail.Should().Be("destinatario@email.com");
                result.Single().SendAsBcc.Should().BeTrue();

                sendAlsoToPayload.PadrinosToUpdate.Should().HaveCount(4);
                sendAlsoToPayload.PadrinosToUpdate[0].SendAlsoTo.Should().BeEmpty();
                sendAlsoToPayload.PadrinosToUpdate[1].SendAlsoTo.Should().ContainSingle();
                sendAlsoToPayload.PadrinosToUpdate[2].SendAlsoTo.Should().BeEmpty();
                sendAlsoToPayload.PadrinosToUpdate[3].SendAlsoTo.Should().BeEmpty();

                var sendAlsoTo = sendAlsoToPayload.PadrinosToUpdate[1].SendAlsoTo.Single();
                sendAlsoTo.RecipientFullName.Should().Be("DestinatarioNombre DestinatarioApellido");
                sendAlsoTo.RecipientEmail.Should().Be("destinatario@email.com");
                sendAlsoTo.SendAsBcc.Should().BeTrue();
            }
        }

        [Fact]
        public async Task ConvertToObjects_Success_PadrinoIdNotFound()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("padrino,nombre-completo,email,bcc");
            fileContents.AppendLine("99, DestinatarioNombre DestinatarioApellido, destinatario@email.com, true");

            var sendAlsoToPayload = new SendAlsoToPayload
            {
                PadrinosToUpdate = new()
                {
                    new() { Id = 77 },
                    new() { Id = 78 },
                    new() { Id = 79 },
                    new() { Id = 80 },
                }
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), sendAlsoToPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().BeEmpty();

                result.Should().ContainSingle();

                result.Single().RecipientFullName.Should().Be("DestinatarioNombre DestinatarioApellido");
                result.Single().RecipientEmail.Should().Be("destinatario@email.com");
                result.Single().SendAsBcc.Should().BeTrue();

                sendAlsoToPayload.PadrinosToUpdate.Should().HaveCount(5);
                sendAlsoToPayload.PadrinosToUpdate[0].SendAlsoTo.Should().BeEmpty();
                sendAlsoToPayload.PadrinosToUpdate[1].SendAlsoTo.Should().BeEmpty();
                sendAlsoToPayload.PadrinosToUpdate[2].SendAlsoTo.Should().BeEmpty();
                sendAlsoToPayload.PadrinosToUpdate[3].SendAlsoTo.Should().BeEmpty();
                sendAlsoToPayload.PadrinosToUpdate[4].SendAlsoTo.Should().ContainSingle();

                sendAlsoToPayload.PadrinosToUpdate[4].Id.Should().Be(99);

                var sendAlsoTo = sendAlsoToPayload.PadrinosToUpdate[4].SendAlsoTo.Single();
                sendAlsoTo.RecipientFullName.Should().Be("DestinatarioNombre DestinatarioApellido");
                sendAlsoTo.RecipientEmail.Should().Be("destinatario@email.com");
                sendAlsoTo.SendAsBcc.Should().BeTrue();
            }
        }

        [Fact]
        public async Task ConvertToObjects_PadrinoCannotBeEmpty()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("padrino,nombre-completo,email,bcc");
            fileContents.AppendLine(" , DestinatarioNombre DestinatarioApellido, destinatario@email.com, false");

            var sendAlsoToPayload = new SendAlsoToPayload();

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), sendAlsoToPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("enviar-copia.csv: line 2: padrino cannot be empty");

                result.Should().ContainSingle();

                result.Single().RecipientFullName.Should().Be("DestinatarioNombre DestinatarioApellido");
                result.Single().RecipientEmail.Should().Be("destinatario@email.com");
                result.Single().SendAsBcc.Should().BeFalse();
            }
        }

        [Fact]
        public async Task ConvertToObjects_RecipientNameCannotBeEmpty()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("padrino,nombre-completo,email,bcc");
            fileContents.AppendLine("PadrinoNombre PadrinoApellido, , destinatario@email.com, false");

            var sendAlsoToPayload = new SendAlsoToPayload
            {
                PadrinosToCreate = new()
                {
                    new()
                    {
                        FirstName = "PadrinoNombre",
                        LastName = "PadrinoApellido",
                    }
                }
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), sendAlsoToPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("enviar-copia.csv: line 2: recipient name cannot be empty");

                result.Should().ContainSingle();

                result.Single().RecipientFullName.Should().BeEmpty();
                result.Single().RecipientEmail.Should().Be("destinatario@email.com");
                result.Single().SendAsBcc.Should().BeFalse();
            }
        }

        [Fact]
        public async Task ConvertToObjects_EmailCannotBeEmpty()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("padrino,nombre-completo,email,bcc");
            fileContents.AppendLine("PadrinoNombre PadrinoApellido, DestinatarioNombre DestinatarioApellido, , false");

            var sendAlsoToPayload = new SendAlsoToPayload
            {
                PadrinosToCreate = new()
                {
                    new()
                    {
                        FirstName = "PadrinoNombre",
                        LastName = "PadrinoApellido",
                    }
                }
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), sendAlsoToPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("enviar-copia.csv: line 2: email cannot be empty");

                result.Should().ContainSingle();

                result.Single().RecipientFullName.Should().Be("DestinatarioNombre DestinatarioApellido");
                result.Single().RecipientEmail.Should().BeEmpty();
                result.Single().SendAsBcc.Should().BeFalse();
            }
        }

        [Fact]
        public async Task ConvertToObjects_EmailMustBeAValidEmailAddress()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("padrino,nombre-completo,email,bcc");
            fileContents.AppendLine("PadrinoNombre PadrinoApellido, DestinatarioNombre DestinatarioApellido, NotAValidEmailAddress, false");

            var sendAlsoToPayload = new SendAlsoToPayload
            {
                PadrinosToCreate = new()
                {
                    new()
                    {
                        FirstName = "PadrinoNombre",
                        LastName = "PadrinoApellido",
                    }
                }
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), sendAlsoToPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("enviar-copia.csv: line 2: 'NotAValidEmailAddress' is not a valid email address");

                result.Should().ContainSingle();

                result.Single().RecipientFullName.Should().Be("DestinatarioNombre DestinatarioApellido");
                result.Single().RecipientEmail.Should().Be("NotAValidEmailAddress");
                result.Single().SendAsBcc.Should().BeFalse();
            }
        }

        [Fact]
        public async Task ConvertToObjects_BccMustBeEitherTrueOrFalse()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("padrino,nombre-completo,email,bcc");
            fileContents.AppendLine("PadrinoNombre PadrinoApellido, DestinatarioNombre DestinatarioApellido, destinatario@email.com, dontknow");

            var sendAlsoToPayload = new SendAlsoToPayload
            {
                PadrinosToCreate = new()
                {
                    new()
                    {
                        FirstName = "PadrinoNombre",
                        LastName = "PadrinoApellido",
                    }
                }
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), sendAlsoToPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("enviar-copia.csv: line 2: bcc must be either 'true' or 'false'; 'dontknow' was found");

                result.Should().ContainSingle();

                result.Single().RecipientFullName.Should().Be("DestinatarioNombre DestinatarioApellido");
                result.Single().RecipientEmail.Should().Be("destinatario@email.com");
                result.Single().SendAsBcc.Should().BeFalse();
            }
        }

        [Fact]
        public async Task ConvertToObjects_PadrinoNotFoundInImportFiles()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("padrino,nombre-completo,email,bcc");
            fileContents.AppendLine("WrongNombre WrongApellido, DestinatarioNombre DestinatarioApellido, destinatario@email.com, true");

            var sendAlsoToPayload = new SendAlsoToPayload
            {
                PadrinosToCreate = new()
                {
                    new()
                    {
                        FirstName = "PadrinoNombre",
                        LastName = "PadrinoApellido",
                    }
                }
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), sendAlsoToPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("enviar-copia.csv: line 2: padrino 'WrongNombre WrongApellido' does not exist; make sure spelling matches that in padrinos.csv");

                result.Should().ContainSingle();

                result.Single().RecipientFullName.Should().Be("DestinatarioNombre DestinatarioApellido");
                result.Single().RecipientEmail.Should().Be("destinatario@email.com");
                result.Single().SendAsBcc.Should().BeTrue();

                sendAlsoToPayload.PadrinosToCreate.Single().SendAlsoTo.Should().BeNull();
            }
        }

    }
}
