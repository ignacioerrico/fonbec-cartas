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
    public class ApadrinamientosFileParserTests
    {
        private readonly ApadrinamientosFileParser _sut;

        public ApadrinamientosFileParserTests()
        {
            var dataImportRepositoryMock = new Mock<IDataImportRepository>();

            dataImportRepositoryMock
                .Setup(x => x.GetAllApadrinamientosAsync())
                .ReturnsAsync(new List<Apadrinamiento>());

            _sut = new ApadrinamientosFileParser(dataImportRepositoryMock.Object);
        }

        [Fact]
        public async Task ConvertToObjects_Success_HastaNotSpecified()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("becario,padrino,desde,hasta");
            fileContents.AppendLine("BecarioNombre BecarioApellido, PadrinoNombre PadrinoApellido, 2002-02-20, ");

            var apadrinamientoPayload = new ApadrinamientoPayload
            {
                CreatedByCoordinadorId = 1729,
                ExistingBecarios = new()
                {
                    new()
                    {
                        Id = 42,
                        FirstName = "BecarioNombre",
                        LastName = "BecarioApellido",
                    },
                },
                ExistingPadrinos = new()
                {
                    new()
                    {
                        Id = 78,
                        FirstName = "PadrinoNombre",
                        LastName = "PadrinoApellido",
                    },
                },
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), apadrinamientoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().BeEmpty();

                result.Should().ContainSingle();

                result.Single().CreatedByCoordinadorId.Should().Be(1729);
                result.Single().From.Should().Be(new DateTime(2002, 2, 20));
                result.Single().To.Should().BeNull();
                result.Single().BecarioId.Should().Be(42);
                result.Single().PadrinoId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_Success_HastaSpecified()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("becario,padrino,desde,hasta");
            fileContents.AppendLine("BecarioNombre BecarioApellido, PadrinoNombre PadrinoApellido, 2002-02-20, 2002-02-25");

            var apadrinamientoPayload = new ApadrinamientoPayload
            {
                CreatedByCoordinadorId = 1729,
                ExistingBecarios = new()
                {
                    new()
                    {
                        Id = 42,
                        FirstName = "BecarioNombre",
                        LastName = "BecarioApellido",
                    },
                },
                ExistingPadrinos = new()
                {
                    new()
                    {
                        Id = 78,
                        FirstName = "PadrinoNombre",
                        LastName = "PadrinoApellido",
                    },
                },
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), apadrinamientoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().BeEmpty();

                result.Should().ContainSingle();

                result.Single().CreatedByCoordinadorId.Should().Be(1729);
                result.Single().From.Should().Be(new DateTime(2002, 2, 20));
                result.Single().To.Should().Be(new DateTime(2002, 2, 25));
                result.Single().BecarioId.Should().Be(42);
                result.Single().PadrinoId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_Success_BecarioIdIsSpecified()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("becario,padrino,desde,hasta");
            fileContents.AppendLine("42, PadrinoNombre PadrinoApellido, 2002-02-20, ");

            var apadrinamientoPayload = new ApadrinamientoPayload
            {
                CreatedByCoordinadorId = 1729,
                ExistingBecarioIds = new() { 40, 41, 42, 43, 44 },
                ExistingPadrinos = new()
                {
                    new()
                    {
                        Id = 78,
                        FirstName = "PadrinoNombre",
                        LastName = "PadrinoApellido",
                    },
                },
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), apadrinamientoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().BeEmpty();

                result.Should().ContainSingle();

                result.Single().CreatedByCoordinadorId.Should().Be(1729);
                result.Single().From.Should().Be(new DateTime(2002, 2, 20));
                result.Single().To.Should().BeNull();
                result.Single().BecarioId.Should().Be(42);
                result.Single().PadrinoId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_Success_PadrinoIdIsSpecified()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("becario,padrino,desde,hasta");
            fileContents.AppendLine("BecarioNombre BecarioApellido, 78, 2002-02-20, 2002-02-25");

            var apadrinamientoPayload = new ApadrinamientoPayload
            {
                CreatedByCoordinadorId = 1729,
                ExistingBecarios = new()
                {
                    new()
                    {
                        Id = 42,
                        FirstName = "BecarioNombre",
                        LastName = "BecarioApellido",
                    },
                },
                ExistingPadrinoIds = new() { 76, 77, 78, 79, 80},
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), apadrinamientoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().BeEmpty();

                result.Should().ContainSingle();

                result.Single().CreatedByCoordinadorId.Should().Be(1729);
                result.Single().From.Should().Be(new DateTime(2002, 2, 20));
                result.Single().To.Should().Be(new DateTime(2002, 2, 25));
                result.Single().BecarioId.Should().Be(42);
                result.Single().PadrinoId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_DesdeCannotBeEmpty()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("becario,padrino,desde,hasta");
            fileContents.AppendLine("BecarioNombre BecarioApellido, PadrinoNombre PadrinoApellido, , ");

            var apadrinamientoPayload = new ApadrinamientoPayload
            {
                CreatedByCoordinadorId = 1729,
                ExistingBecarios = new()
                {
                    new()
                    {
                        Id = 42,
                        FirstName = "BecarioNombre",
                        LastName = "BecarioApellido",
                    },
                },
                ExistingPadrinos = new()
                {
                    new()
                    {
                        Id = 78,
                        FirstName = "PadrinoNombre",
                        LastName = "PadrinoApellido",
                    },
                },
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), apadrinamientoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("apadrinamientos.csv: line 2: desde cannot be empty");

                result.Should().ContainSingle();

                result.Single().CreatedByCoordinadorId.Should().Be(1729);
                result.Single().From.Should().Be(DateTime.MinValue);
                result.Single().To.Should().BeNull();
                result.Single().BecarioId.Should().Be(42);
                result.Single().PadrinoId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_DesdeMustBeSpecifiedAsYyyyMmDd()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("becario,padrino,desde,hasta");
            fileContents.AppendLine("BecarioNombre BecarioApellido, PadrinoNombre PadrinoApellido, 20-02-2002, ");

            var apadrinamientoPayload = new ApadrinamientoPayload
            {
                CreatedByCoordinadorId = 1729,
                ExistingBecarios = new()
                {
                    new()
                    {
                        Id = 42,
                        FirstName = "BecarioNombre",
                        LastName = "BecarioApellido",
                    },
                },
                ExistingPadrinos = new()
                {
                    new()
                    {
                        Id = 78,
                        FirstName = "PadrinoNombre",
                        LastName = "PadrinoApellido",
                    },
                },
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), apadrinamientoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("apadrinamientos.csv: line 2: desde is '20-02-2002', but must be specified as a date with this format: 'YYYY-MM-DD'");

                result.Should().ContainSingle();

                result.Single().CreatedByCoordinadorId.Should().Be(1729);
                result.Single().From.Should().Be(DateTime.MinValue);
                result.Single().To.Should().BeNull();
                result.Single().BecarioId.Should().Be(42);
                result.Single().PadrinoId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_HastaMustBeSpecifiedAsYyyyMmDdOrLeftBlank()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("becario,padrino,desde,hasta");
            fileContents.AppendLine("BecarioNombre BecarioApellido, PadrinoNombre PadrinoApellido, 2002-02-20, 20-02-2002");

            var apadrinamientoPayload = new ApadrinamientoPayload
            {
                CreatedByCoordinadorId = 1729,
                ExistingBecarios = new()
                {
                    new()
                    {
                        Id = 42,
                        FirstName = "BecarioNombre",
                        LastName = "BecarioApellido",
                    },
                },
                ExistingPadrinos = new()
                {
                    new()
                    {
                        Id = 78,
                        FirstName = "PadrinoNombre",
                        LastName = "PadrinoApellido",
                    },
                },
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), apadrinamientoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("apadrinamientos.csv: line 2: hasta is '20-02-2002'; if known, it must be specified as a date with this format: 'YYYY-MM-DD'; otherwise leave blank");

                result.Should().ContainSingle();

                result.Single().CreatedByCoordinadorId.Should().Be(1729);
                result.Single().From.Should().Be(new DateTime(2002, 2, 20));
                result.Single().To.Should().BeNull();
                result.Single().BecarioId.Should().Be(42);
                result.Single().PadrinoId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_BecarioNameNotFound()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("becario,padrino,desde,hasta");
            fileContents.AppendLine("WrongNombre WrongApellido, PadrinoNombre PadrinoApellido, 2002-02-20, ");

            var apadrinamientoPayload = new ApadrinamientoPayload
            {
                CreatedByCoordinadorId = 1729,
                ExistingBecarios = new()
                {
                    new()
                    {
                        Id = 42,
                        FirstName = "BecarioNombre",
                        LastName = "BecarioApellido",
                    },
                },
                ExistingPadrinos = new()
                {
                    new()
                    {
                        Id = 78,
                        FirstName = "PadrinoNombre",
                        LastName = "PadrinoApellido",
                    },
                },
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), apadrinamientoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("apadrinamientos.csv: line 2: becario 'WrongNombre WrongApellido' does not exist; make sure spelling matches that in becarios.csv");

                result.Should().ContainSingle();

                result.Single().CreatedByCoordinadorId.Should().Be(1729);
                result.Single().From.Should().Be(new DateTime(2002, 2, 20));
                result.Single().To.Should().BeNull();
                result.Single().BecarioId.Should().Be(0);
                result.Single().PadrinoId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_PadrinoNameNotFound()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("becario,padrino,desde,hasta");
            fileContents.AppendLine("BecarioNombre BecarioApellido, WrongNombre WrongApellido, 2002-02-20, ");

            var apadrinamientoPayload = new ApadrinamientoPayload
            {
                CreatedByCoordinadorId = 1729,
                ExistingBecarios = new()
                {
                    new()
                    {
                        Id = 42,
                        FirstName = "BecarioNombre",
                        LastName = "BecarioApellido",
                    },
                },
                ExistingPadrinos = new()
                {
                    new()
                    {
                        Id = 78,
                        FirstName = "PadrinoNombre",
                        LastName = "PadrinoApellido",
                    },
                },
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), apadrinamientoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("apadrinamientos.csv: line 2: padrino 'WrongNombre WrongApellido' does not exist; make sure spelling matches that in padrinos.csv");

                result.Should().ContainSingle();

                result.Single().CreatedByCoordinadorId.Should().Be(1729);
                result.Single().From.Should().Be(new DateTime(2002, 2, 20));
                result.Single().To.Should().BeNull();
                result.Single().BecarioId.Should().Be(42);
                result.Single().PadrinoId.Should().Be(0);
            }
        }

        [Fact]
        public async Task ConvertToObjects_BecarioIdNotFound()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("becario,padrino,desde,hasta");
            fileContents.AppendLine("99, PadrinoNombre PadrinoApellido, 2002-02-20, ");

            var apadrinamientoPayload = new ApadrinamientoPayload
            {
                CreatedByCoordinadorId = 1729,
                ExistingBecarioIds = new() { 10, 11, 12 },
                ExistingPadrinos = new()
                {
                    new()
                    {
                        Id = 78,
                        FirstName = "PadrinoNombre",
                        LastName = "PadrinoApellido",
                    },
                },
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), apadrinamientoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("apadrinamientos.csv: line 2: becario ID 99 does not exist in the database for the Filial specified");

                result.Should().ContainSingle();

                result.Single().CreatedByCoordinadorId.Should().Be(1729);
                result.Single().From.Should().Be(new DateTime(2002, 2, 20));
                result.Single().To.Should().BeNull();
                result.Single().BecarioId.Should().Be(0);
                result.Single().PadrinoId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ConvertToObjects_PadrinoIdNotFound()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("becario,padrino,desde,hasta");
            fileContents.AppendLine("BecarioNombre BecarioApellido, 99, 2002-02-20, ");

            var apadrinamientoPayload = new ApadrinamientoPayload
            {
                CreatedByCoordinadorId = 1729,
                ExistingBecarios = new()
                {
                    new()
                    {
                        Id = 42,
                        FirstName = "BecarioNombre",
                        LastName = "BecarioApellido",
                    },
                },
                ExistingPadrinoIds = new() { 10, 11, 12 },
            };

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), apadrinamientoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be("apadrinamientos.csv: line 2: padrino ID 99 does not exist in the database for the Filial specified");

                result.Should().ContainSingle();

                result.Single().CreatedByCoordinadorId.Should().Be(1729);
                result.Single().From.Should().Be(new DateTime(2002, 2, 20));
                result.Single().To.Should().BeNull();
                result.Single().BecarioId.Should().Be(42);
                result.Single().PadrinoId.Should().Be(0);
            }
        }
    }
}
