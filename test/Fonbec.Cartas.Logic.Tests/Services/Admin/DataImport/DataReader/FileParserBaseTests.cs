using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader;
using System.Text;
using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Moq;
using Fonbec.Cartas.DataAccess.Repositories.Admin.DataImport;
using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader.Payloads;

namespace Fonbec.Cartas.Logic.Tests.Services.Admin.DataImport.DataReader
{
    public class FileParserBaseTests
    {
        private readonly BecarioFileParser _sut;

        public FileParserBaseTests()
        {
            var dataImportRepositoryMock = new Mock<IDataImportRepository>();

            dataImportRepositoryMock
                .Setup(x => x.GetAllBecariosAsync())
                .ReturnsAsync(new List<Becario>());
            
            _sut = new BecarioFileParser(dataImportRepositoryMock.Object);
        }

        [Fact]
        public async Task BecarioFileParser_ConvertToObjects_Success()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,nivel,email,teléfono,mediador");
            fileContents.AppendLine("Nombre1 Becario1, Apellido1 Becario1, Apodo1, M, Secundario, becario1@email.com, Phone1, Nombre1 Mediador1 Apellido1 Medidador1");
            fileContents.AppendLine("Nombre2 Becario2, Apellido2 Becario2, , F, Universitario, , , Nombre1 Mediador1 Apellido1 Medidador1");
            fileContents.AppendLine("Nombre3 Becario3, Apellido3 Becario3, , F, Secundario, becario3@email.com, , Nombre4 Mediador4 Apellido4 Medidador4");
            fileContents.AppendLine("Nombre4 Becario4, Apellido4 Becario4, , M, Secundario, , , Nombre2 Mediador2 Apellido2 Medidador2");

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
                    new()
                    {
                        Id = 7,
                        FirstName = "Nombre2 Mediador2",
                        LastName = "Apellido2 Medidador2",
                    },
                    new()
                    {
                        Id = 11,
                        FirstName = "Nombre3 Mediador3",
                        LastName = "Apellido3 Medidador3",
                    },
                    new()
                    {
                        Id = 13,
                        FirstName = "Nombre4 Mediador4",
                        LastName = "Apellido4 Medidador4",
                    },
                    new()
                    {
                        Id = 17,
                        FirstName = "Nombre5 Mediador5",
                        LastName = "Apellido5 Medidador5",
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

                result.Should().HaveCount(4);

                result[0].FirstName.Should().Be("Nombre1 Becario1");
                result[0].LastName.Should().Be("Apellido1 Becario1");
                result[0].NickName.Should().Be("Apodo1");
                result[0].Gender.Should().Be(Gender.Male);
                result[0].NivelDeEstudio.Should().Be(NivelDeEstudio.Secundario);
                result[0].Email.Should().Be("becario1@email.com");
                result[0].Phone.Should().Be("Phone1");
                result[0].MediadorId.Should().Be(5);
                result[0].FilialId.Should().Be(42);
                result[0].CreatedByCoordinadorId.Should().Be(78);

                result[1].FirstName.Should().Be("Nombre2 Becario2");
                result[1].LastName.Should().Be("Apellido2 Becario2");
                result[1].NickName.Should().BeNull();
                result[1].Gender.Should().Be(Gender.Female);
                result[1].NivelDeEstudio.Should().Be(NivelDeEstudio.Universitario);
                result[1].Email.Should().BeNull();
                result[1].Phone.Should().BeNull();
                result[1].MediadorId.Should().Be(5);
                result[1].FilialId.Should().Be(42);
                result[1].CreatedByCoordinadorId.Should().Be(78);

                result[2].FirstName.Should().Be("Nombre3 Becario3");
                result[2].LastName.Should().Be("Apellido3 Becario3");
                result[2].NickName.Should().BeNull();
                result[2].Gender.Should().Be(Gender.Female);
                result[2].NivelDeEstudio.Should().Be(NivelDeEstudio.Secundario);
                result[2].Email.Should().Be("becario3@email.com");
                result[2].Phone.Should().BeNull();
                result[2].MediadorId.Should().Be(13);
                result[2].FilialId.Should().Be(42);
                result[2].CreatedByCoordinadorId.Should().Be(78);

                result[3].FirstName.Should().Be("Nombre4 Becario4");
                result[3].LastName.Should().Be("Apellido4 Becario4");
                result[3].NickName.Should().BeNull();
                result[3].Gender.Should().Be(Gender.Male);
                result[3].NivelDeEstudio.Should().Be(NivelDeEstudio.Secundario);
                result[3].Email.Should().BeNull();
                result[3].Phone.Should().BeNull();
                result[3].MediadorId.Should().Be(7);
                result[3].FilialId.Should().Be(42);
                result[3].CreatedByCoordinadorId.Should().Be(78);
            }
        }

        [Fact]
        public async Task BecarioFileParser_ConvertToObjects_Success_EmptyLinesAreIgnored()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine();
            fileContents.AppendLine();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,nivel,email,teléfono,mediador");
            fileContents.AppendLine();
            fileContents.AppendLine("Nombre1 Becario1, Apellido1 Becario1, Apodo1, M, Secundario, becario1@email.com, Phone1, Nombre1 Mediador1 Apellido1 Medidador1");
            fileContents.AppendLine("Nombre2 Becario2, Apellido2 Becario2, , F, Universitario, , , Nombre1 Mediador1 Apellido1 Medidador1");
            fileContents.AppendLine();
            fileContents.AppendLine();
            fileContents.AppendLine("Nombre3 Becario3, Apellido3 Becario3, , F, Secundario, becario3@email.com, , Nombre4 Mediador4 Apellido4 Medidador4");
            fileContents.AppendLine("Nombre4 Becario4, Apellido4 Becario4, , M, Secundario, , , Nombre2 Mediador2 Apellido2 Medidador2");
            fileContents.AppendLine();
            fileContents.AppendLine();

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
                    new()
                    {
                        Id = 7,
                        FirstName = "Nombre2 Mediador2",
                        LastName = "Apellido2 Medidador2",
                    },
                    new()
                    {
                        Id = 11,
                        FirstName = "Nombre3 Mediador3",
                        LastName = "Apellido3 Medidador3",
                    },
                    new()
                    {
                        Id = 13,
                        FirstName = "Nombre4 Mediador4",
                        LastName = "Apellido4 Medidador4",
                    },
                    new()
                    {
                        Id = 17,
                        FirstName = "Nombre5 Mediador5",
                        LastName = "Apellido5 Medidador5",
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

                result.Should().HaveCount(4);

                result[0].FirstName.Should().Be("Nombre1 Becario1");
                result[0].LastName.Should().Be("Apellido1 Becario1");
                result[0].NickName.Should().Be("Apodo1");
                result[0].Gender.Should().Be(Gender.Male);
                result[0].NivelDeEstudio.Should().Be(NivelDeEstudio.Secundario);
                result[0].Email.Should().Be("becario1@email.com");
                result[0].Phone.Should().Be("Phone1");
                result[0].MediadorId.Should().Be(5);
                result[0].FilialId.Should().Be(42);
                result[0].CreatedByCoordinadorId.Should().Be(78);

                result[1].FirstName.Should().Be("Nombre2 Becario2");
                result[1].LastName.Should().Be("Apellido2 Becario2");
                result[1].NickName.Should().BeNull();
                result[1].Gender.Should().Be(Gender.Female);
                result[1].NivelDeEstudio.Should().Be(NivelDeEstudio.Universitario);
                result[1].Email.Should().BeNull();
                result[1].Phone.Should().BeNull();
                result[1].MediadorId.Should().Be(5);
                result[1].FilialId.Should().Be(42);
                result[1].CreatedByCoordinadorId.Should().Be(78);

                result[2].FirstName.Should().Be("Nombre3 Becario3");
                result[2].LastName.Should().Be("Apellido3 Becario3");
                result[2].NickName.Should().BeNull();
                result[2].Gender.Should().Be(Gender.Female);
                result[2].NivelDeEstudio.Should().Be(NivelDeEstudio.Secundario);
                result[2].Email.Should().Be("becario3@email.com");
                result[2].Phone.Should().BeNull();
                result[2].MediadorId.Should().Be(13);
                result[2].FilialId.Should().Be(42);
                result[2].CreatedByCoordinadorId.Should().Be(78);

                result[3].FirstName.Should().Be("Nombre4 Becario4");
                result[3].LastName.Should().Be("Apellido4 Becario4");
                result[3].NickName.Should().BeNull();
                result[3].Gender.Should().Be(Gender.Male);
                result[3].NivelDeEstudio.Should().Be(NivelDeEstudio.Secundario);
                result[3].Email.Should().BeNull();
                result[3].Phone.Should().BeNull();
                result[3].MediadorId.Should().Be(7);
                result[3].FilialId.Should().Be(42);
                result[3].CreatedByCoordinadorId.Should().Be(78);
            }
        }

        [Fact]
        public async Task BecarioFileParser_ConvertToObjects_Success_HeaderMustBeFirstNonEmptyLine()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine();
            fileContents.AppendLine("Nombre1 Becario1, Apellido1 Becario1, Apodo1, M, Secundario, becario1@email.com, Phone1, Nombre1 Mediador1 Apellido1 Medidador1");
            fileContents.AppendLine();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,nivel,email,teléfono,mediador");
            fileContents.AppendLine();
            fileContents.AppendLine("Nombre2 Becario2, Apellido2 Becario2, , F, Universitario, , , Nombre1 Mediador1 Apellido1 Medidador1");
            fileContents.AppendLine();
            fileContents.AppendLine();
            fileContents.AppendLine("Nombre3 Becario3, Apellido3 Becario3, , F, Secundario, becario3@email.com, , Nombre4 Mediador4 Apellido4 Medidador4");
            fileContents.AppendLine("Nombre4 Becario4, Apellido4 Becario4, , M, Secundario, , , Nombre2 Mediador2 Apellido2 Medidador2");
            fileContents.AppendLine();
            fileContents.AppendLine();

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
                    new()
                    {
                        Id = 7,
                        FirstName = "Nombre2 Mediador2",
                        LastName = "Apellido2 Medidador2",
                    },
                    new()
                    {
                        Id = 11,
                        FirstName = "Nombre3 Mediador3",
                        LastName = "Apellido3 Medidador3",
                    },
                    new()
                    {
                        Id = 13,
                        FirstName = "Nombre4 Mediador4",
                        LastName = "Apellido4 Medidador4",
                    },
                    new()
                    {
                        Id = 17,
                        FirstName = "Nombre5 Mediador5",
                        LastName = "Apellido5 Medidador5",
                    },
                },
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), becarioPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().HaveCount(5);

                errors[0].Should().Be("becarios.csv: line 2: wrong header or header missing");
                errors[1].Should().Be("becarios.csv: line 4: gender must be either 'M' or 'F'; 'sexo' was found");
                errors[2].Should().Be("becarios.csv: line 4: nivel de estudio 'nivel' is invalid; it must be one of: 'Primario', 'Secundario', 'Universitario'");
                errors[3].Should().Be("becarios.csv: line 4: 'email' is not a valid email address");
                errors[4].Should().Be("becarios.csv: line 4: mediador 'mediador' does not exist; make sure spelling matches that in mediadores.csv");

                result.Should().HaveCount(4);

                result[0].FirstName.Should().Be("nombre");
                result[0].LastName.Should().Be("apellido");
                result[0].NickName.Should().Be("apodo");
                result[0].Gender.Should().Be(Gender.Unknown);
                result[0].NivelDeEstudio.Should().Be(NivelDeEstudio.Primario);
                result[0].Email.Should().Be("email");
                result[0].Phone.Should().Be("teléfono");
                result[0].MediadorId.Should().Be(0);
                result[0].FilialId.Should().Be(42);
                result[0].CreatedByCoordinadorId.Should().Be(78);

                result[1].FirstName.Should().Be("Nombre2 Becario2");
                result[1].LastName.Should().Be("Apellido2 Becario2");
                result[1].NickName.Should().BeNull();
                result[1].Gender.Should().Be(Gender.Female);
                result[1].NivelDeEstudio.Should().Be(NivelDeEstudio.Universitario);
                result[1].Email.Should().BeNull();
                result[1].Phone.Should().BeNull();
                result[1].MediadorId.Should().Be(5);
                result[1].FilialId.Should().Be(42);
                result[1].CreatedByCoordinadorId.Should().Be(78);

                result[2].FirstName.Should().Be("Nombre3 Becario3");
                result[2].LastName.Should().Be("Apellido3 Becario3");
                result[2].NickName.Should().BeNull();
                result[2].Gender.Should().Be(Gender.Female);
                result[2].NivelDeEstudio.Should().Be(NivelDeEstudio.Secundario);
                result[2].Email.Should().Be("becario3@email.com");
                result[2].Phone.Should().BeNull();
                result[2].MediadorId.Should().Be(13);
                result[2].FilialId.Should().Be(42);
                result[2].CreatedByCoordinadorId.Should().Be(78);

                result[3].FirstName.Should().Be("Nombre4 Becario4");
                result[3].LastName.Should().Be("Apellido4 Becario4");
                result[3].NickName.Should().BeNull();
                result[3].Gender.Should().Be(Gender.Male);
                result[3].NivelDeEstudio.Should().Be(NivelDeEstudio.Secundario);
                result[3].Email.Should().BeNull();
                result[3].Phone.Should().BeNull();
                result[3].MediadorId.Should().Be(7);
                result[3].FilialId.Should().Be(42);
                result[3].CreatedByCoordinadorId.Should().Be(78);
            }
        }

        [Fact]
        public async Task BecarioFileParser_ConvertToObjects_LinesWithMissingOrExtraFieldsAreReportedAsErrors()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,nivel,email,teléfono,mediador");
            fileContents.AppendLine("Nombre1 Becario1, Apellido1 Becario1, Apodo1");
            fileContents.AppendLine("Nombre2 Becario2, Apellido2 Becario2, , F, Universitario, , , Nombre1 Mediador1 Apellido1 Medidador1, Extra field, Another extra field");

            var becarioPayload = new BecarioPayload();

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), becarioPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().HaveCount(2);

                errors[0].Should().Be("becarios.csv: line 2: wrong number of fields (3 instead of the expected 8)");
                errors[1].Should().Be("becarios.csv: line 3: wrong number of fields (10 instead of the expected 8)");

                result.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task BecarioFileParser_ConvertToObjects_FirstNameAndLastNameAreUsedToDetermineDuplicates()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,nivel,email,teléfono,mediador");
            fileContents.AppendLine("Nombre1 Becario1, Apellido1 Becario1, Apodo1, M, Secundario, becario1@email.com, Phone1, Nombre1 Mediador1 Apellido1 Medidador1");
            fileContents.AppendLine("Nombre1 Becario1, Apellido1 Becario1, Apodo2, F, Universitario, becario2@email.com, Phone2, Nombre2 Mediador2 Apellido2 Medidador2");

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
                    new()
                    {
                        Id = 7,
                        FirstName = "Nombre2 Mediador2",
                        LastName = "Apellido2 Medidador2",
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

                errors[0].Should().Be("becarios.csv: line 3: 'Nombre1 Becario1 Apellido1 Becario1' is a duplicate of 'Nombre1 Becario1 Apellido1 Becario1' in becarios.csv");

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
        public async Task BecarioFileParser_ConvertToObjects_SimilarFirstNamesAndLastNamesCanBeDuplicatesToo_DuplicateInImportFile()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,nivel,email,teléfono,mediador");
            fileContents.AppendLine("Nombre1 Becario1, Apellido1 Becario1, Apodo1, M, Secundario, becario1@email.com, Phone1, Nombre1 Mediador1 Apellido1 Medidador1");
            fileContents.AppendLine("nombrË 1 beca Rio 1, apellidÖ 1 béca Rio 1, Apodo2, F, Universitario, becario2@email.com, Phone2, Nombre2 Mediador2 Apellido2 Medidador2");

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
                    new()
                    {
                        Id = 7,
                        FirstName = "Nombre2 Mediador2",
                        LastName = "Apellido2 Medidador2",
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

                errors[0].Should().Be("becarios.csv: line 3: 'nombrË 1 beca Rio 1 apellidÖ 1 béca Rio 1' is a duplicate of 'Nombre1 Becario1 Apellido1 Becario1' in becarios.csv");

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
        public async Task BecarioFileParser_ConvertToObjects_SimilarFirstNamesAndLastNamesCanBeDuplicatesToo_DuplicateInDatabase()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,nivel,email,teléfono,mediador");
            fileContents.AppendLine("nombrË 1 beca Rio 1, apellidÖ 1 béca Rio 1, Apodo2, F, Universitario, becario2@email.com, Phone2, Nombre2 Mediador2 Apellido2 Medidador2");

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
                    new()
                    {
                        Id = 7,
                        FirstName = "Nombre2 Mediador2",
                        LastName = "Apellido2 Medidador2",
                    },
                },
            };

            var errors = new List<string>();

            var dataImportRepositoryMock = new Mock<IDataImportRepository>();

            var becarios = new List<Becario>
            {
                new()
                {
                    FirstName = "Nombre1 Becario1",
                    LastName = "Apellido1 Becario1",
                    NickName = "Apodo1",
                    Gender = Gender.Male,
                    NivelDeEstudio = NivelDeEstudio.Secundario,
                    Email = "becario1@email.com",
                    Phone = "Phone1",
                    MediadorId = 5,
                },
            };

            dataImportRepositoryMock
                .Setup(x => x.GetAllBecariosAsync())
                .ReturnsAsync(becarios);

            // This SUT has a different dependency, which returns the single Becario defined above
            var sut = new BecarioFileParser(dataImportRepositoryMock.Object);

            // Act
            var result = await sut.ConvertToObjects(fileContents.ToString(), becarioPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors[0].Should().Be("becarios.csv: line 2: 'nombrË 1 beca Rio 1 apellidÖ 1 béca Rio 1' is a duplicate of 'Nombre1 Becario1 Apellido1 Becario1' in the database");

                result.Should().BeEmpty();
            }
        }
    }
}
