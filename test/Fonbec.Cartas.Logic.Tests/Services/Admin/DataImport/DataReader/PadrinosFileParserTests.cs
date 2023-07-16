using FluentAssertions.Execution;
using FluentAssertions;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader.Payloads;
using System.Text;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Repositories.Admin.DataImport;
using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader;
using Moq;

namespace Fonbec.Cartas.Logic.Tests.Services.Admin.DataImport.DataReader
{
    public class PadrinosFileParserTests
    {
        private readonly PadrinosFileParser _sut;

        public PadrinosFileParserTests()
        {
            var dataImportRepositoryMock = new Mock<IDataImportRepository>();

            dataImportRepositoryMock
                .Setup(x => x.GetAllPadrinosAsync())
                .ReturnsAsync(new List<Padrino>());

            _sut = new PadrinosFileParser(dataImportRepositoryMock.Object);
        }

        [Fact]
        public async Task ConvertToObjects_Success_AllFieldsSpecified()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,email,teléfono");
            fileContents.AppendLine("PadrinoNombre, PadrinoApellido, PadrinoApodo, F, padrino@email.com, Phone");

            var padrinoPayload = new PadrinoPayload
            {
                FilialId = 42,
                CreatedByCoordinadorId = 78,
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), padrinoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().BeEmpty();
                
                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("PadrinoNombre");
                result.Single().LastName.Should().Be("PadrinoApellido");
                result.Single().NickName.Should().Be("PadrinoApodo");
                result.Single().Gender.Should().Be(Gender.Female);
                result.Single().Email.Should().Be("padrino@email.com");
                result.Single().Phone.Should().Be("Phone");
                result.Single().FilialId.Should().Be(42);
                result.Single().CreatedByCoordinadorId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_Success_NickNameNotSpecified()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,email,teléfono");
            fileContents.AppendLine("PadrinoNombre, PadrinoApellido, , M, padrino@email.com, Phone");

            var padrinoPayload = new PadrinoPayload
            {
                FilialId = 42,
                CreatedByCoordinadorId = 78,
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), padrinoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().BeEmpty();

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("PadrinoNombre");
                result.Single().LastName.Should().Be("PadrinoApellido");
                result.Single().NickName.Should().BeNull();
                result.Single().Gender.Should().Be(Gender.Male);
                result.Single().Email.Should().Be("padrino@email.com");
                result.Single().Phone.Should().Be("Phone");
                result.Single().FilialId.Should().Be(42);
                result.Single().CreatedByCoordinadorId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_Success_LastNameNotSpecified()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,email,teléfono");
            fileContents.AppendLine("PadrinoNombre, , Apodo, M, padrino@email.com, Phone");

            var padrinoPayload = new PadrinoPayload
            {
                FilialId = 42,
                CreatedByCoordinadorId = 78,
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), padrinoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().BeEmpty();

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("PadrinoNombre");
                result.Single().LastName.Should().BeNull();
                result.Single().NickName.Should().Be("Apodo");
                result.Single().Gender.Should().Be(Gender.Male);
                result.Single().Email.Should().Be("padrino@email.com");
                result.Single().Phone.Should().Be("Phone");
                result.Single().FilialId.Should().Be(42);
                result.Single().CreatedByCoordinadorId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_FirstNameCannotBeEmpty()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,email,teléfono");
            fileContents.AppendLine(" , PadrinoApellido, , F, padrino@email.com, Phone");

            var padrinoPayload = new PadrinoPayload
            {
                FilialId = 42,
                CreatedByCoordinadorId = 78,
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), padrinoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("padrinos.csv: line 2: first name cannot be empty");

                result.Should().ContainSingle();

                result.Single().FirstName.Should().BeEmpty();
                result.Single().LastName.Should().Be("PadrinoApellido");
                result.Single().NickName.Should().BeNull();
                result.Single().Gender.Should().Be(Gender.Female);
                result.Single().Email.Should().Be("padrino@email.com");
                result.Single().Phone.Should().Be("Phone");
                result.Single().FilialId.Should().Be(42);
                result.Single().CreatedByCoordinadorId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_GenderMustBeEitherMOrF()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,email,teléfono");
            fileContents.AppendLine("PadrinoNombre, PadrinoApellido, , X, padrino@email.com, Phone");

            var padrinoPayload = new PadrinoPayload
            {
                FilialId = 42,
                CreatedByCoordinadorId = 78,
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), padrinoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("padrinos.csv: line 2: gender must be either 'M' or 'F'; 'X' was found");

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("PadrinoNombre");
                result.Single().LastName.Should().Be("PadrinoApellido");
                result.Single().NickName.Should().BeNull();
                result.Single().Gender.Should().Be(Gender.Unknown);
                result.Single().Email.Should().Be("padrino@email.com");
                result.Single().Phone.Should().Be("Phone");
                result.Single().FilialId.Should().Be(42);
                result.Single().CreatedByCoordinadorId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_EmailCannotBeEmpty()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,email,teléfono");
            fileContents.AppendLine("PadrinoNombre, PadrinoApellido, , F, , Phone");

            var padrinoPayload = new PadrinoPayload
            {
                FilialId = 42,
                CreatedByCoordinadorId = 78,
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), padrinoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("padrinos.csv: line 2: email cannot be empty");

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("PadrinoNombre");
                result.Single().LastName.Should().Be("PadrinoApellido");
                result.Single().NickName.Should().BeNull();
                result.Single().Gender.Should().Be(Gender.Female);
                result.Single().Email.Should().BeEmpty();
                result.Single().Phone.Should().Be("Phone");
                result.Single().FilialId.Should().Be(42);
                result.Single().CreatedByCoordinadorId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_EmailMustBeAValidEmailAddress()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,email,teléfono");
            fileContents.AppendLine("PadrinoNombre, PadrinoApellido, , F, NotAValidEmailAddress, Phone");

            var padrinoPayload = new PadrinoPayload
            {
                FilialId = 42,
                CreatedByCoordinadorId = 78,
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), padrinoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("padrinos.csv: line 2: 'NotAValidEmailAddress' is not a valid email address");

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("PadrinoNombre");
                result.Single().LastName.Should().Be("PadrinoApellido");
                result.Single().NickName.Should().BeNull();
                result.Single().Gender.Should().Be(Gender.Female);
                result.Single().Email.Should().Be("NotAValidEmailAddress");
                result.Single().Phone.Should().Be("Phone");
                result.Single().FilialId.Should().Be(42);
                result.Single().CreatedByCoordinadorId.Should().Be(78);
            }
        }
    }
}
