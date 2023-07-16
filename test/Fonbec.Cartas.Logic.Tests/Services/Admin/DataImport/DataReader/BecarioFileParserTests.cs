using System.Text;
using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.DataAccess.Repositories.Admin.DataImport;
using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader;
using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader.Payloads;
using Moq;

namespace Fonbec.Cartas.Logic.Tests.Services.Admin.DataImport.DataReader
{
    public class BecarioFileParserTests
    {
        private readonly BecarioFileParser _sut;

        public BecarioFileParserTests()
        {
            var dataImportRepositoryMock = new Mock<IDataImportRepository>();

            dataImportRepositoryMock
                .Setup(x => x.GetAllBecariosAsync())
                .ReturnsAsync(new List<Becario>());

            _sut = new BecarioFileParser(dataImportRepositoryMock.Object);
        }

        [Fact]
        public async Task ConvertToObjects_Success_MediadorFound()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,nivel,email,teléfono,mediador");
            fileContents.AppendLine("Nombre1 Becario1, Apellido1 Becario1, Apodo1, M, Secundario, becario1@email.com, Phone1, Nombre1 Mediador1 Apellido1 Medidador1");

            var becarioPayload = new BecarioPayload
            {
                FilialId = 42,
                CreatedByCoordinadorId = 78,
                ExistingMediadores = new()
                {
                    new()
                    {
                        Id = 5,
                        FirstName = "Nombre1 Mediador1",
                        LastName = "Apellido1 Medidador1",
                    },
                },
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), becarioPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().BeEmpty();

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("Nombre1 Becario1");
                result.Single().LastName.Should().Be("Apellido1 Becario1");
                result.Single().NickName.Should().Be("Apodo1");
                result.Single().Gender.Should().Be(Gender.Male);
                result.Single().NivelDeEstudio.Should().Be(NivelDeEstudio.Secundario);
                result.Single().Email.Should().Be("becario1@email.com");
                result.Single().Phone.Should().Be("Phone1");
                result.Single().MediadorId.Should().Be(5);
                result.Single().FilialId.Should().Be(42);
                result.Single().CreatedByCoordinadorId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_Success_MediadorIdFound()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,nivel,email,teléfono,mediador");
            fileContents.AppendLine("Nombre1 Becario1, Apellido1 Becario1, Apodo1, M, Secundario, becario1@email.com, Phone1, 1729");

            var becarioPayload = new BecarioPayload
            {
                FilialId = 42,
                CreatedByCoordinadorId = 78,
                ExistingMediadorIds = new() { 1727, 1728, 1729, 1730, 1731 },
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), becarioPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().BeEmpty();

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("Nombre1 Becario1");
                result.Single().LastName.Should().Be("Apellido1 Becario1");
                result.Single().NickName.Should().Be("Apodo1");
                result.Single().Gender.Should().Be(Gender.Male);
                result.Single().NivelDeEstudio.Should().Be(NivelDeEstudio.Secundario);
                result.Single().Email.Should().Be("becario1@email.com");
                result.Single().Phone.Should().Be("Phone1");
                result.Single().MediadorId.Should().Be(1729);
                result.Single().FilialId.Should().Be(42);
                result.Single().CreatedByCoordinadorId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_FirstNameCannotBeEmpty()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,nivel,email,teléfono,mediador");
            fileContents.AppendLine(" , Apellido1 Becario1, Apodo1, M, Secundario, becario1@email.com, Phone1, Nombre1 Mediador1 Apellido1 Medidador1");

            var becarioPayload = new BecarioPayload
            {
                FilialId = 42,
                CreatedByCoordinadorId = 78,
                ExistingMediadores = new()
                {
                    new()
                    {
                        Id = 5,
                        FirstName = "Nombre1 Mediador1",
                        LastName = "Apellido1 Medidador1",
                    },
                },
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), becarioPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("becarios.csv: line 2: first name cannot be empty");

                result.Should().ContainSingle();

                result.Single().FirstName.Should().BeEmpty();
                result.Single().LastName.Should().Be("Apellido1 Becario1");
                result.Single().NickName.Should().Be("Apodo1");
                result.Single().Gender.Should().Be(Gender.Male);
                result.Single().NivelDeEstudio.Should().Be(NivelDeEstudio.Secundario);
                result.Single().Email.Should().Be("becario1@email.com");
                result.Single().Phone.Should().Be("Phone1");
                result.Single().MediadorId.Should().Be(5);
                result.Single().FilialId.Should().Be(42);
                result.Single().CreatedByCoordinadorId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_GenderMustBeEitherMOrF()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,nivel,email,teléfono,mediador");
            fileContents.AppendLine("Nombre1 Becario1, Apellido1 Becario1, Apodo1, X, Secundario, becario1@email.com, Phone1, Nombre1 Mediador1 Apellido1 Medidador1");

            var becarioPayload = new BecarioPayload
            {
                FilialId = 42,
                CreatedByCoordinadorId = 78,
                ExistingMediadores = new()
                {
                    new()
                    {
                        Id = 5,
                        FirstName = "Nombre1 Mediador1",
                        LastName = "Apellido1 Medidador1",
                    },
                },
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), becarioPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("becarios.csv: line 2: gender must be either 'M' or 'F'; 'X' was found");

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("Nombre1 Becario1");
                result.Single().LastName.Should().Be("Apellido1 Becario1");
                result.Single().NickName.Should().Be("Apodo1");
                result.Single().Gender.Should().Be(Gender.Unknown);
                result.Single().NivelDeEstudio.Should().Be(NivelDeEstudio.Secundario);
                result.Single().Email.Should().Be("becario1@email.com");
                result.Single().Phone.Should().Be("Phone1");
                result.Single().MediadorId.Should().Be(5);
                result.Single().FilialId.Should().Be(42);
                result.Single().CreatedByCoordinadorId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_NivelDeEstudioMustBeAValueInTheCorrespondingEnum()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,nivel,email,teléfono,mediador");
            fileContents.AppendLine("Nombre1 Becario1, Apellido1 Becario1, Apodo1, F, Wrong Value, becario1@email.com, Phone1, Nombre1 Mediador1 Apellido1 Medidador1");

            var becarioPayload = new BecarioPayload
            {
                FilialId = 42,
                CreatedByCoordinadorId = 78,
                ExistingMediadores = new()
                {
                    new()
                    {
                        Id = 5,
                        FirstName = "Nombre1 Mediador1",
                        LastName = "Apellido1 Medidador1",
                    },
                },
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), becarioPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("becarios.csv: line 2: nivel de estudio 'Wrong Value' is invalid; it must be one of: 'Primario', 'Secundario', 'Universitario'");

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("Nombre1 Becario1");
                result.Single().LastName.Should().Be("Apellido1 Becario1");
                result.Single().NickName.Should().Be("Apodo1");
                result.Single().Gender.Should().Be(Gender.Female);
                result.Single().NivelDeEstudio.Should().Be(NivelDeEstudio.Primario);
                result.Single().Email.Should().Be("becario1@email.com");
                result.Single().Phone.Should().Be("Phone1");
                result.Single().MediadorId.Should().Be(5);
                result.Single().FilialId.Should().Be(42);
                result.Single().CreatedByCoordinadorId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_EmailMustBeAValidEmailAddress()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,nivel,email,teléfono,mediador");
            fileContents.AppendLine("Nombre1 Becario1, Apellido1 Becario1, Apodo1, F, Secundario, NotAValidEmailAddress, Phone1, Nombre1 Mediador1 Apellido1 Medidador1");

            var becarioPayload = new BecarioPayload
            {
                FilialId = 42,
                CreatedByCoordinadorId = 78,
                ExistingMediadores = new()
                {
                    new()
                    {
                        Id = 5,
                        FirstName = "Nombre1 Mediador1",
                        LastName = "Apellido1 Medidador1",
                    },
                },
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), becarioPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("becarios.csv: line 2: 'NotAValidEmailAddress' is not a valid email address");
                
                result.Should().ContainSingle();
                
                result.Single().FirstName.Should().Be("Nombre1 Becario1");
                result.Single().LastName.Should().Be("Apellido1 Becario1");
                result.Single().NickName.Should().Be("Apodo1");
                result.Single().Gender.Should().Be(Gender.Female);
                result.Single().NivelDeEstudio.Should().Be(NivelDeEstudio.Secundario);
                result.Single().Email.Should().Be("NotAValidEmailAddress");
                result.Single().Phone.Should().Be("Phone1");
                result.Single().MediadorId.Should().Be(5);
                result.Single().FilialId.Should().Be(42);
                result.Single().CreatedByCoordinadorId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_MediadorCannotBeEmpty()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,nivel,email,teléfono,mediador");
            fileContents.AppendLine("Nombre1 Becario1, Apellido1 Becario1, Apodo1, F, Secundario, becario1@email.com, Phone1, ");

            var becarioPayload = new BecarioPayload
            {
                FilialId = 42,
                CreatedByCoordinadorId = 78,
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), becarioPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("becarios.csv: line 2: mediador cannot be empty");

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("Nombre1 Becario1");
                result.Single().LastName.Should().Be("Apellido1 Becario1");
                result.Single().NickName.Should().Be("Apodo1");
                result.Single().Gender.Should().Be(Gender.Female);
                result.Single().NivelDeEstudio.Should().Be(NivelDeEstudio.Secundario);
                result.Single().Email.Should().Be("becario1@email.com");
                result.Single().Phone.Should().Be("Phone1");
                result.Single().MediadorId.Should().Be(0);
                result.Single().FilialId.Should().Be(42);
                result.Single().CreatedByCoordinadorId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_MediadorIdNotFound()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,nivel,email,teléfono,mediador");
            fileContents.AppendLine("Nombre1 Becario1, Apellido1 Becario1, Apodo1, M, Secundario, becario1@email.com, Phone1, 1729");

            var becarioPayload = new BecarioPayload
            {
                FilialId = 42,
                CreatedByCoordinadorId = 78,
                ExistingMediadorIds = new() { 10, 11, 12 },
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), becarioPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("becarios.csv: line 2: mediador ID 1729 does not exist in the database for the Filial specified");

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("Nombre1 Becario1");
                result.Single().LastName.Should().Be("Apellido1 Becario1");
                result.Single().NickName.Should().Be("Apodo1");
                result.Single().Gender.Should().Be(Gender.Male);
                result.Single().NivelDeEstudio.Should().Be(NivelDeEstudio.Secundario);
                result.Single().Email.Should().Be("becario1@email.com");
                result.Single().Phone.Should().Be("Phone1");
                result.Single().MediadorId.Should().Be(0);
                result.Single().FilialId.Should().Be(42);
                result.Single().CreatedByCoordinadorId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_MediadorNotFoundAmongExistingOnes()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,nivel,email,teléfono,mediador");
            fileContents.AppendLine("Nombre1 Becario1, Apellido1 Becario1, Apodo1, M, Secundario, becario1@email.com, Phone1, Nonexistent name");

            var becarioPayload = new BecarioPayload
            {
                FilialId = 42,
                CreatedByCoordinadorId = 78,
                ExistingMediadores = new()
                {
                    new()
                    {
                        Id = 5,
                        FirstName = "Nombre1 Mediador1",
                        LastName = "Apellido1 Medidador1",
                    },
                },
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), becarioPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("becarios.csv: line 2: mediador 'Nonexistent name' does not exist; make sure spelling matches that in mediadores.csv");

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("Nombre1 Becario1");
                result.Single().LastName.Should().Be("Apellido1 Becario1");
                result.Single().NickName.Should().Be("Apodo1");
                result.Single().Gender.Should().Be(Gender.Male);
                result.Single().NivelDeEstudio.Should().Be(NivelDeEstudio.Secundario);
                result.Single().Email.Should().Be("becario1@email.com");
                result.Single().Phone.Should().Be("Phone1");
                result.Single().MediadorId.Should().Be(0);
                result.Single().FilialId.Should().Be(42);
                result.Single().CreatedByCoordinadorId.Should().Be(78);
            }
        }
    }
}
