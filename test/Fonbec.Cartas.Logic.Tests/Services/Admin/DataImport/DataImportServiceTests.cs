using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.DataImport;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.DataAccess.Repositories.Admin;
using Fonbec.Cartas.DataAccess.Repositories.Admin.DataImport;
using Fonbec.Cartas.Logic.Models.Admin.DataImport;
using Fonbec.Cartas.Logic.Services.Admin;
using Fonbec.Cartas.Logic.Services.Admin.DataImport;
using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader;
using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader.Payloads;
using Fonbec.Cartas.Logic.Tests.ViewModels;
using Mapster;
using Moq;

namespace Fonbec.Cartas.Logic.Tests.Services.Admin.DataImport
{
    public class DataImportServiceTests : MapsterTests
    {
        private readonly DataImportService _sut;

        public DataImportServiceTests()
        {
            var userWithAccountSharedServiceMock = new Mock<IUserWithAccountSharedService>();

            var allCoordinadoresInFilial = new List<Coordinador>
            {
                new()
                {
                    Id = 7,
                    FirstName = "FirstName1",
                    LastName = "LastName1",
                },
                new()
                {
                    Id = 11,
                    FirstName = "FirstName2",
                    LastName = "LastName2",
                    NickName = "NickName2"
                },
                new()
                {
                    Id = 13,
                    FirstName = "FirstName3",
                    LastName = "LastName3",
                },
            };

            var coordinadoresToCreate = new List<UserWithAccountToCreate>
            {
                new()
                {
                    FirstName = "CoordinadorNombre1",
                    LastName = "CoordinadorApellido1",
                    NickName = "CoordinadorApodo1",
                    Gender = Gender.Female,
                    Email = "coordinador1@email.com",
                    Phone = "CoordinadorPhone1",
                    Username = "coordinador1",
                    Password = "Password1!",
                },
                new()
                {
                    FirstName = "CoordinadorNombre2",
                    LastName = "CoordinadorApellido2",
                    Gender = Gender.Female,
                    Email = "coordinador2@email.com",
                    Username = "coordinador2",
                    Password = "Password1!",
                },
            };

            var mediadoresToCreate = new List<UserWithAccountToCreate>
            {
                new()
                {
                    FirstName = "MediadorNombre1",
                    LastName = "MediadorApellido1",
                    NickName = "MediadorApodo1",
                    Gender = Gender.Female,
                    Email = "mediador1@email.com",
                    Phone = "MediadorPhone1",
                    Username = "mediador1",
                    Password = "Password1!",
                },
                new()
                {
                    FirstName = "MediadorNombre2",
                    LastName = "MediadorApellido2",
                    Gender = Gender.Female,
                    Email = "mediador2@email.com",
                    Username = "mediador2",
                    Password = "Password1!",
                },
            };

            var revisoresToCreate = new List<UserWithAccountToCreate>
            {
                new()
                {
                    FirstName = "RevisorNombre1",
                    LastName = "RevisorApellido1",
                    NickName = "RevisorApodo1",
                    Gender = Gender.Female,
                    Email = "revisor1@email.com",
                    Phone = "RevisorPhone1",
                    Username = "revisor1",
                    Password = "Password1!",
                },
                new()
                {
                    FirstName = "RevisorNombre2",
                    LastName = "RevisorApellido2",
                    Gender = Gender.Female,
                    Email = "revisor2@email.com",
                    Username = "revisor2",
                    Password = "Password1!",
                },
            };

            var padrinosToCreate = new List<Padrino>
            {
                new()
                {
                    FirstName = "PadrinoNombre1",
                    LastName = "PadrinoApellido1",
                    NickName = "PadrinoApodo1",
                    Gender = Gender.Female,
                    Email = "padrino1@email.com",
                    Phone = "PadrinoPhone1",
                    FilialId = 42,
                    CreatedByCoordinadorId = 78,
                },
                new()
                {
                    FirstName = "PadrinoNombre2",
                    LastName = "PadrinoApellido2",
                    Gender = Gender.Female,
                    Email = "padrino2@email.com",
                    FilialId = 42,
                    CreatedByCoordinadorId = 78,
                },
            };

            var padrinosToUpdate = new List<PadrinoToUpdate>
            {
                new()
                {
                    Id = 900,
                    SendAlsoTo = new()
                    {
                        new()
                        {
                            RecipientFullName = "RecipientNombre RecipientApellido",
                            RecipientEmail = "recipient@email.com",
                            SendAsBcc = true
                        }
                    }
                }
            };

            // This return value is discarded
            var sendAlsoTosToCreate = new List<SendAlsoTo>();

            var becariosToCreate = new List<Becario>
            {
                new()
                {
                    FirstName = "BecarioNombre1",
                    LastName = "BecarioApellido1",
                    NickName = "BecarioApodo1",
                    Gender = Gender.Female,
                    NivelDeEstudio = NivelDeEstudio.Universitario,
                    Email = "becario1@email.com",
                    Phone = "BecarioPhone1",
                    MediadorId = 128,
                    FilialId = 42,
                    CreatedByCoordinadorId = 78,
                },
                new()
                {
                    FirstName = "BecarioNombre2",
                    LastName = "BecarioApellido2",
                    Gender = Gender.Male,
                    NivelDeEstudio = NivelDeEstudio.Secundario,
                    Email = "becario2@email.com",
                    MediadorId = 512,
                    FilialId = 42,
                    CreatedByCoordinadorId = 78,
                },
                new()
                {
                    FirstName = "BecarioNombre3",
                    LastName = "BecarioApellido3",
                    Gender = Gender.Female,
                    NivelDeEstudio = NivelDeEstudio.Primario,
                    Email = "becario3@email.com",
                    Phone = "BecarioPhone3",
                    MediadorId = 512,
                    FilialId = 42,
                    CreatedByCoordinadorId = 78,
                },
            };

            var apadrinamientosToCreate = new List<Apadrinamiento>
            {
                new()
                {
                    BecarioId = 100,
                    PadrinoId = 200,
                    From = new DateTime(2002, 2, 20),
                    CreatedByCoordinadorId = 78,
                },
                new()
                {
                    BecarioId = 102,
                    PadrinoId = 202,
                    From = new DateTime(2002, 2, 22),
                    To = new DateTime(2002, 3, 22),
                    CreatedByCoordinadorId = 78,
                },
                new()
                {
                    BecarioId = 110,
                    PadrinoId = 220,
                    From = new DateTime(2002, 2, 25),
                    CreatedByCoordinadorId = 78,
                },
            };

            var coordinadoresCreated = coordinadoresToCreate.Adapt<List<Coordinador>>();
            
            coordinadoresCreated[0].Id = 510;
            coordinadoresCreated[0].FilialId = 42;
            coordinadoresCreated[0].AspNetUserId = "AspNetUserId1";
            
            coordinadoresCreated[1].Id = 520;
            coordinadoresCreated[1].FilialId = 42;
            coordinadoresCreated[1].AspNetUserId = "AspNetUserId2";

            var mediadoresCreated = mediadoresToCreate.Adapt<List<Mediador>>();

            mediadoresCreated[0].Id = 610;
            mediadoresCreated[0].FilialId = 42;
            mediadoresCreated[0].AspNetUserId = "AspNetUserId3";

            mediadoresCreated[1].Id = 620;
            mediadoresCreated[1].FilialId = 42;
            mediadoresCreated[1].AspNetUserId = "AspNetUserId4";

            var revisoresCreated = revisoresToCreate.Adapt<List<Revisor>>();

            revisoresCreated[0].Id = 710;
            revisoresCreated[0].FilialId = 42;
            revisoresCreated[0].AspNetUserId = "AspNetUserId5";

            revisoresCreated[1].Id = 720;
            revisoresCreated[1].FilialId = 42;
            revisoresCreated[1].AspNetUserId = "AspNetUserId6";

            var existingMediadorIds = new List<int> { 10, 11, 12 };

            var userWithAccountRepositoryBase = new Mock<IUserWithAccountRepositoryBase<Coordinador>>();

            userWithAccountRepositoryBase
                .Setup(x => x.GetAllInFilialAsync(42))
                .ReturnsAsync(allCoordinadoresInFilial);

            var coordinadoresFileParserMock = new Mock<UserWithAccountFileParser<Coordinador>>(null);

            coordinadoresFileParserMock
                .Setup(x => x.ConvertToObjects(string.Empty, It.IsAny<UserWithAccountPayload<Coordinador>>(), It.IsAny<List<string>>()))
                .ReturnsAsync(coordinadoresToCreate);

            var mediadoresFileParserMock = new Mock<UserWithAccountFileParser<Mediador>>(null);

            mediadoresFileParserMock
                .Setup(x => x.ConvertToObjects(string.Empty, It.IsAny<UserWithAccountPayload<Mediador>>(), It.IsAny<List<string>>()))
                .ReturnsAsync(mediadoresToCreate);

            var revisoresFileParserMock = new Mock<UserWithAccountFileParser<Revisor>>(null);

            revisoresFileParserMock
                .Setup(x => x.ConvertToObjects(string.Empty, It.IsAny<UserWithAccountPayload<Revisor>>(), It.IsAny<List<string>>()))
                .ReturnsAsync(revisoresToCreate);

            var padrinosFileParserMock = new Mock<PadrinosFileParser>(null);

            padrinosFileParserMock
                .Setup(x => x.ConvertToObjects(string.Empty, It.IsAny<PadrinoPayload>(), It.IsAny<List<string>>()))
                .ReturnsAsync(padrinosToCreate);

            var sendAltoToFileParserMock = new Mock<SendAltoToFileParser>(null);

            sendAltoToFileParserMock
                .Setup(x => x.ConvertToObjects(string.Empty, It.IsAny<SendAlsoToPayload>(), It.IsAny<List<string>>()))
                .ReturnsAsync(sendAlsoTosToCreate);

            var becarioFileParserMock = new Mock<BecarioFileParser>(null);

            becarioFileParserMock
                .Setup(x => x.ConvertToObjects(string.Empty, It.IsAny<BecarioPayload>(), It.IsAny<List<string>>()))
                .ReturnsAsync(becariosToCreate);

            var apadrinamientosFileParserMock = new Mock<ApadrinamientosFileParser>(null);

            apadrinamientosFileParserMock
                .Setup(x => x.ConvertToObjects(string.Empty, It.IsAny<ApadrinamientoPayload>(), It.IsAny<List<string>>()))
                .ReturnsAsync(apadrinamientosToCreate);

            var createCoordinadorServiceMock = new Mock<ICreateUserWithAccountService<Coordinador>>();

            createCoordinadorServiceMock
                .Setup(x => x.CreateAsync(42,
                    It.Is<List<UserWithAccountToCreate>>(l =>
                        l.Count == 2
                        
                        && l[0].FirstName == "CoordinadorNombre1"
                        && l[0].LastName == "CoordinadorApellido1"
                        && l[0].NickName == "CoordinadorApodo1"
                        && l[0].Gender == Gender.Female
                        && l[0].Email == "coordinador1@email.com"
                        && l[0].Phone == "CoordinadorPhone1"
                        && l[0].Username == "coordinador1"
                        && l[0].Password == "Password1!"
                        
                        && l[1].FirstName == "CoordinadorNombre2"
                        && l[1].LastName == "CoordinadorApellido2"
                        && l[1].Gender == Gender.Female
                        && l[1].Email == "coordinador2@email.com"
                        && l[1].Username == "coordinador2"
                        && l[1].Password == "Password1!"),
                    It.IsAny<List<string>>()))
                .ReturnsAsync(coordinadoresCreated);

            var createMediadorServiceMock = new Mock<ICreateUserWithAccountService<Mediador>>();

            createMediadorServiceMock
                .Setup(x => x.CreateAsync(42,
                    It.Is<List<UserWithAccountToCreate>>(l =>
                        l.Count == 2

                        && l[0].FirstName == "MediadorNombre1"
                        && l[0].LastName == "MediadorApellido1"
                        && l[0].NickName == "MediadorApodo1"
                        && l[0].Gender == Gender.Female
                        && l[0].Email == "mediador1@email.com"
                        && l[0].Phone == "MediadorPhone1"
                        && l[0].Username == "mediador1"
                        && l[0].Password == "Password1!"

                        && l[1].FirstName == "MediadorNombre2"
                        && l[1].LastName == "MediadorApellido2"
                        && l[1].Gender == Gender.Female
                        && l[1].Email == "mediador2@email.com"
                        && l[1].Username == "mediador2"
                        && l[1].Password == "Password1!"),
                    It.IsAny<List<string>>()))
                .ReturnsAsync(mediadoresCreated);

            var createRevisorServiceMock = new Mock<ICreateUserWithAccountService<Revisor>>();

            createRevisorServiceMock
                .Setup(x => x.CreateAsync(42,
                    It.Is<List<UserWithAccountToCreate>>(l =>
                        l.Count == 2

                        && l[0].FirstName == "RevisorNombre1"
                        && l[0].LastName == "RevisorApellido1"
                        && l[0].NickName == "RevisorApodo1"
                        && l[0].Gender == Gender.Female
                        && l[0].Email == "revisor1@email.com"
                        && l[0].Phone == "RevisorPhone1"
                        && l[0].Username == "revisor1"
                        && l[0].Password == "Password1!"

                        && l[1].FirstName == "RevisorNombre2"
                        && l[1].LastName == "RevisorApellido2"
                        && l[1].Gender == Gender.Female
                        && l[1].Email == "revisor2@email.com"
                        && l[1].Username == "revisor2"
                        && l[1].Password == "Password1!"),
                    It.IsAny<List<string>>()))
                .ReturnsAsync(revisoresCreated);

            var dataImportRepositoryMock = new Mock<IDataImportRepository>();

            dataImportRepositoryMock
                .Setup(x => x.GetExistingMediadorIdsAsync(42))
                .ReturnsAsync(existingMediadorIds);

            dataImportRepositoryMock
                .Setup(x => x.UpdatePadrinosAsync(It.Is<List<PadrinoToUpdate>>(l => !l.Any()), It.IsAny<List<string>>()))
                .Callback<List<PadrinoToUpdate>, List<string>>((ptu, _) => ptu.AddRange(padrinosToUpdate));

            _sut = new DataImportService(userWithAccountSharedServiceMock.Object,
                userWithAccountRepositoryBase.Object,
                coordinadoresFileParserMock.Object,
                mediadoresFileParserMock.Object,
                revisoresFileParserMock.Object,
                padrinosFileParserMock.Object,
                sendAltoToFileParserMock.Object,
                becarioFileParserMock.Object,
                apadrinamientosFileParserMock.Object,
                createCoordinadorServiceMock.Object,
                createMediadorServiceMock.Object,
                createRevisorServiceMock.Object,
                dataImportRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllCoordinadoresForSelectionAsync_Success()
        {
            var result = await _sut.GetAllCoordinadoresForSelectionAsync(42);

            using (new AssertionScope())
            {
                result.Should().HaveCount(3);

                result[0].Id.Should().Be(7);
                result[0].DisplayName.Should().Be("FirstName1 LastName1");
                
                result[1].Id.Should().Be(11);
                result[1].DisplayName.Should().Be("FirstName2 LastName2");

                result[2].Id.Should().Be(13);
                result[2].DisplayName.Should().Be("FirstName3 LastName3");
            }
        }

        [Fact]
        public void CorrectFilesHaveBeenSelected_Success()
        {
            // Arrange
            var fileNames = new[]
            {
                "apadrinamientos.csv",
                "becarios.csv",
                "coordinadores.csv",
                "enviar-copia.csv",
                "mediadores.csv",
                "padrinos.csv",
                "revisores.csv",
            };

            // Act
            var result = _sut.CorrectFilesHaveBeenSelected(fileNames);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ImportData_Success_DryRun()
        {
            // Arrange
            var importDataStreamsInputModel = new ImportDataStreamsInputModel
            {
                IsDryRun = true,
                FilialId = 42,
                CreatedByCoordinadorId = 78,
                Coordinadores = Stream.Null,
                Mediadores = Stream.Null,
                Revisores = Stream.Null,
                Padrinos = Stream.Null,
                EnviarCopia = Stream.Null,
                Becarios = Stream.Null,
                Apadrinamientos = Stream.Null,
            };

            // Act
            var result = await _sut.ImportData(importDataStreamsInputModel);

            // Assert
            using (new AssertionScope())
            {
                result.CoordinadoresCreated.Should().HaveCount(2);

                result.CoordinadoresCreated[0].FirstName.Should().Be("CoordinadorNombre1");
                result.CoordinadoresCreated[0].LastName.Should().Be("CoordinadorApellido1");
                result.CoordinadoresCreated[0].NickName.Should().Be("CoordinadorApodo1");
                result.CoordinadoresCreated[0].Gender.Should().Be(Gender.Female);
                result.CoordinadoresCreated[0].Email.Should().Be("coordinador1@email.com");
                result.CoordinadoresCreated[0].Phone.Should().Be("CoordinadorPhone1");
                result.CoordinadoresCreated[0].Username.Should().Be("coordinador1");

                result.CoordinadoresCreated[1].FirstName.Should().Be("CoordinadorNombre2");
                result.CoordinadoresCreated[1].LastName.Should().Be("CoordinadorApellido2");
                result.CoordinadoresCreated[1].NickName.Should().BeNull();
                result.CoordinadoresCreated[1].Gender.Should().Be(Gender.Female);
                result.CoordinadoresCreated[1].Email.Should().Be("coordinador2@email.com");
                result.CoordinadoresCreated[1].Phone.Should().BeNull();
                result.CoordinadoresCreated[1].Username.Should().Be("coordinador2");

                result.MediadoresCreated.Should().HaveCount(2);

                result.MediadoresCreated[0].FirstName.Should().Be("MediadorNombre1");
                result.MediadoresCreated[0].LastName.Should().Be("MediadorApellido1");
                result.MediadoresCreated[0].NickName.Should().Be("MediadorApodo1");
                result.MediadoresCreated[0].Gender.Should().Be(Gender.Female);
                result.MediadoresCreated[0].Email.Should().Be("mediador1@email.com");
                result.MediadoresCreated[0].Phone.Should().Be("MediadorPhone1");
                result.MediadoresCreated[0].Username.Should().Be("mediador1");

                result.MediadoresCreated[1].FirstName.Should().Be("MediadorNombre2");
                result.MediadoresCreated[1].LastName.Should().Be("MediadorApellido2");
                result.MediadoresCreated[1].NickName.Should().BeNull();
                result.MediadoresCreated[1].Gender.Should().Be(Gender.Female);
                result.MediadoresCreated[1].Email.Should().Be("mediador2@email.com");
                result.MediadoresCreated[1].Phone.Should().BeNull();
                result.MediadoresCreated[1].Username.Should().Be("mediador2");

                result.RevisoresCreated.Should().HaveCount(2);

                result.RevisoresCreated[0].FirstName.Should().Be("RevisorNombre1");
                result.RevisoresCreated[0].LastName.Should().Be("RevisorApellido1");
                result.RevisoresCreated[0].NickName.Should().Be("RevisorApodo1");
                result.RevisoresCreated[0].Gender.Should().Be(Gender.Female);
                result.RevisoresCreated[0].Email.Should().Be("revisor1@email.com");
                result.RevisoresCreated[0].Phone.Should().Be("RevisorPhone1");
                result.RevisoresCreated[0].Username.Should().Be("revisor1");

                result.RevisoresCreated[1].FirstName.Should().Be("RevisorNombre2");
                result.RevisoresCreated[1].LastName.Should().Be("RevisorApellido2");
                result.RevisoresCreated[1].NickName.Should().BeNull();
                result.RevisoresCreated[1].Gender.Should().Be(Gender.Female);
                result.RevisoresCreated[1].Email.Should().Be("revisor2@email.com");
                result.RevisoresCreated[1].Phone.Should().BeNull();
                result.RevisoresCreated[1].Username.Should().Be("revisor2");

                result.PadrinosCreated.Should().HaveCount(2);

                result.PadrinosCreated[0].FirstName.Should().Be("PadrinoNombre1");
                result.PadrinosCreated[0].LastName.Should().Be("PadrinoApellido1");
                result.PadrinosCreated[0].NickName.Should().Be("PadrinoApodo1");
                result.PadrinosCreated[0].Gender.Should().Be(Gender.Female);
                result.PadrinosCreated[0].Email.Should().Be("padrino1@email.com");
                result.PadrinosCreated[0].Phone.Should().Be("PadrinoPhone1");
                result.PadrinosCreated[0].FilialId.Should().Be(42);
                result.PadrinosCreated[0].CreatedByCoordinadorId.Should().Be(78);

                result.PadrinosCreated[1].FirstName.Should().Be("PadrinoNombre2");
                result.PadrinosCreated[1].LastName.Should().Be("PadrinoApellido2");
                result.PadrinosCreated[1].NickName.Should().BeNull();
                result.PadrinosCreated[1].Gender.Should().Be(Gender.Female);
                result.PadrinosCreated[1].Email.Should().Be("padrino2@email.com");
                result.PadrinosCreated[1].Phone.Should().BeNull();
                result.PadrinosCreated[1].FilialId.Should().Be(42);
                result.PadrinosCreated[1].CreatedByCoordinadorId.Should().Be(78);

                result.PadrinosUpdated.Should().BeEmpty();

                result.BecariosCreated.Should().HaveCount(3);

                result.BecariosCreated[0].FirstName.Should().Be("BecarioNombre1");
                result.BecariosCreated[0].LastName.Should().Be("BecarioApellido1");
                result.BecariosCreated[0].NickName.Should().Be("BecarioApodo1");
                result.BecariosCreated[0].NivelDeEstudio.Should().Be(NivelDeEstudio.Universitario);
                result.BecariosCreated[0].Gender.Should().Be(Gender.Female);
                result.BecariosCreated[0].Email.Should().Be("becario1@email.com");
                result.BecariosCreated[0].Phone.Should().Be("BecarioPhone1");
                result.BecariosCreated[0].FilialId.Should().Be(42);
                result.BecariosCreated[0].CreatedByCoordinadorId.Should().Be(78);

                result.BecariosCreated[1].FirstName.Should().Be("BecarioNombre2");
                result.BecariosCreated[1].LastName.Should().Be("BecarioApellido2");
                result.BecariosCreated[1].NickName.Should().BeNull();
                result.BecariosCreated[1].NivelDeEstudio.Should().Be(NivelDeEstudio.Secundario);
                result.BecariosCreated[1].Gender.Should().Be(Gender.Male);
                result.BecariosCreated[1].Email.Should().Be("becario2@email.com");
                result.BecariosCreated[1].Phone.Should().BeNull();
                result.BecariosCreated[1].FilialId.Should().Be(42);
                result.BecariosCreated[1].CreatedByCoordinadorId.Should().Be(78);

                result.BecariosCreated[2].FirstName.Should().Be("BecarioNombre3");
                result.BecariosCreated[2].LastName.Should().Be("BecarioApellido3");
                result.BecariosCreated[2].NickName.Should().BeNull();
                result.BecariosCreated[2].NivelDeEstudio.Should().Be(NivelDeEstudio.Primario);
                result.BecariosCreated[2].Gender.Should().Be(Gender.Female);
                result.BecariosCreated[2].Email.Should().Be("becario3@email.com");
                result.BecariosCreated[2].Phone.Should().Be("BecarioPhone3");
                result.BecariosCreated[2].FilialId.Should().Be(42);
                result.BecariosCreated[2].CreatedByCoordinadorId.Should().Be(78);

                result.ApadrinamientosCreated.Should().HaveCount(3);

                result.ApadrinamientosCreated[0].BecarioId.Should().Be(100);
                result.ApadrinamientosCreated[0].PadrinoId.Should().Be(200);
                result.ApadrinamientosCreated[0].From.Should().Be(new DateTime(2002, 2, 20));
                result.ApadrinamientosCreated[0].To.Should().BeNull();
                result.ApadrinamientosCreated[0].CreatedByCoordinadorId.Should().Be(78);

                result.ApadrinamientosCreated[1].BecarioId.Should().Be(102);
                result.ApadrinamientosCreated[1].PadrinoId.Should().Be(202);
                result.ApadrinamientosCreated[1].From.Should().Be(new DateTime(2002, 2, 22));
                result.ApadrinamientosCreated[1].To.Should().Be(new DateTime(2002, 3, 22));
                result.ApadrinamientosCreated[1].CreatedByCoordinadorId.Should().Be(78);

                result.ApadrinamientosCreated[2].BecarioId.Should().Be(110);
                result.ApadrinamientosCreated[2].PadrinoId.Should().Be(220);
                result.ApadrinamientosCreated[2].From.Should().Be(new DateTime(2002, 2, 25));
                result.ApadrinamientosCreated[2].To.Should().BeNull();
                result.ApadrinamientosCreated[2].CreatedByCoordinadorId.Should().Be(78);
            }
        }

        [Fact]
        public async Task ImportData_Success_NoDryRun()
        {
            // Arrange
            var importDataStreamsInputModel = new ImportDataStreamsInputModel
            {
                IsDryRun = false,
                FilialId = 42,
                CreatedByCoordinadorId = 78,
                Coordinadores = Stream.Null,
                Mediadores = Stream.Null,
                Revisores = Stream.Null,
                Padrinos = Stream.Null,
                EnviarCopia = Stream.Null,
                Becarios = Stream.Null,
                Apadrinamientos = Stream.Null,
            };

            // Act
            var result = await _sut.ImportData(importDataStreamsInputModel);

            // Assert
            using (new AssertionScope())
            {
                result.CoordinadoresCreated.Should().HaveCount(2);

                result.CoordinadoresCreated[0].Id.Should().Be(510);
                result.CoordinadoresCreated[0].FilialId.Should().Be(42);
                result.CoordinadoresCreated[0].AspNetUserId.Should().Be("AspNetUserId1");

                result.CoordinadoresCreated[1].Id.Should().Be(520);
                result.CoordinadoresCreated[1].FilialId.Should().Be(42);
                result.CoordinadoresCreated[1].AspNetUserId.Should().Be("AspNetUserId2");

                result.MediadoresCreated.Should().HaveCount(2);

                result.MediadoresCreated[0].Id.Should().Be(610);
                result.MediadoresCreated[0].FilialId.Should().Be(42);
                result.MediadoresCreated[0].AspNetUserId.Should().Be("AspNetUserId3");

                result.MediadoresCreated[1].Id.Should().Be(620);
                result.MediadoresCreated[1].FilialId.Should().Be(42);
                result.MediadoresCreated[1].AspNetUserId.Should().Be("AspNetUserId4");

                result.RevisoresCreated.Should().HaveCount(2);

                result.RevisoresCreated[0].Id.Should().Be(710);
                result.RevisoresCreated[0].FilialId.Should().Be(42);
                result.RevisoresCreated[0].AspNetUserId.Should().Be("AspNetUserId5");

                result.RevisoresCreated[1].Id.Should().Be(720);
                result.RevisoresCreated[1].FilialId.Should().Be(42);
                result.RevisoresCreated[1].AspNetUserId.Should().Be("AspNetUserId6");

                result.PadrinosUpdated.Should().ContainSingle();

                result.PadrinosUpdated.Single().Id.Should().Be(900);
                result.PadrinosUpdated.Single().SendAlsoTo.Should().ContainSingle();
                result.PadrinosUpdated.Single().SendAlsoTo.Single().RecipientFullName.Should().Be("RecipientNombre RecipientApellido");
                result.PadrinosUpdated.Single().SendAlsoTo.Single().RecipientEmail.Should().Be("recipient@email.com");
                result.PadrinosUpdated.Single().SendAlsoTo.Single().SendAsBcc.Should().BeTrue();
            }
        }
    }
}
