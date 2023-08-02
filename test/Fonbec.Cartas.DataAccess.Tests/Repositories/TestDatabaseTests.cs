using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fonbec.Cartas.DataAccess.Tests.Repositories
{
    [Collection("Sequential")]
    public class TestDatabaseTests
    {
        [Fact]
        public async Task CreateDbContextAsync_TestDataIsAsExpected()
        {
            // Arrange
            var testDatabase = new TestDatabase();

            // Act
            var result = testDatabase.GetAppDbContextFactory();

            await using var appDbContext = await result.CreateDbContextAsync();

            var filiales = await appDbContext.Filiales.ToListAsync();
            var coordinadores = await appDbContext.Coordinadores.ToListAsync();
            var mediadores = await appDbContext.Mediadores.ToListAsync();
            var revisores = await appDbContext.Revisores.ToListAsync();
            var padrinos = await appDbContext.Padrinos.ToListAsync();
            var becarios = await appDbContext.Becarios.ToListAsync();
            var apadrinamientos = await appDbContext.Apadrinamientos.ToListAsync();

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();

                filiales.Should().NotBeNull()
                    .And.HaveCount(2);

                filiales[0].Id.Should().Be(1);
                filiales[0].Name.Should().Be("Default");

                filiales[1].Id.Should().Be(2);
                filiales[1].Name.Should().Be("Córdoba");

                coordinadores.Should().NotBeNull().And.HaveCount(3);

                coordinadores[0].Id.Should().Be(1);
                coordinadores[0].FilialId.Should().Be(1);
                coordinadores[0].FirstName.Should().Be("Coordinador3-FirstName");
                coordinadores[0].LastName.Should().Be("Coordinador3-LastName");
                coordinadores[0].NickName.Should().Be("Coo3");
                coordinadores[0].Gender.Should().Be(Gender.Female);
                coordinadores[0].Email.Should().Be("coordinador3@email.com");
                coordinadores[0].Phone.Should().Be("Coordinador3-Phone");

                coordinadores[1].Id.Should().Be(2);
                coordinadores[1].FilialId.Should().Be(1);
                coordinadores[1].FirstName.Should().Be("Coordinador2-FirstName");
                coordinadores[1].LastName.Should().Be("Coordinador2-LastName");
                coordinadores[1].NickName.Should().Be("Coo2");
                coordinadores[1].Gender.Should().Be(Gender.Male);
                coordinadores[1].Email.Should().Be("coordinador2@email.com");
                coordinadores[1].Phone.Should().Be("Coordinador2-Phone");

                coordinadores[2].Id.Should().Be(3);
                coordinadores[2].FilialId.Should().Be(2);
                coordinadores[2].FirstName.Should().Be("Coordinador1-FirstName");
                coordinadores[2].LastName.Should().Be("Coordinador1-LastName");
                coordinadores[2].NickName.Should().Be("Coo1");
                coordinadores[2].Gender.Should().Be(Gender.Female);
                coordinadores[2].Email.Should().Be("coordinador1@email.com");
                coordinadores[2].Phone.Should().Be("Coordinador1-Phone");

                mediadores.Should().NotBeNull().And.HaveCount(3);

                mediadores[0].Id.Should().Be(1);
                mediadores[0].FilialId.Should().Be(1);
                mediadores[0].FirstName.Should().Be("Mediador3-FirstName");
                mediadores[0].LastName.Should().Be("Mediador3-LastName");
                mediadores[0].NickName.Should().Be("Med3");
                mediadores[0].Gender.Should().Be(Gender.Female);
                mediadores[0].Email.Should().Be("mediador3@email.com");
                mediadores[0].Phone.Should().Be("Mediador3-Phone");

                mediadores[1].Id.Should().Be(2);
                mediadores[1].FilialId.Should().Be(1);
                mediadores[1].FirstName.Should().Be("Mediador2-FirstName");
                mediadores[1].LastName.Should().Be("Mediador2-LastName");
                mediadores[1].NickName.Should().Be("Med2");
                mediadores[1].Gender.Should().Be(Gender.Male);
                mediadores[1].Email.Should().Be("mediador2@email.com");
                mediadores[1].Phone.Should().Be("Mediador2-Phone");

                mediadores[2].Id.Should().Be(3);
                mediadores[2].FilialId.Should().Be(2);
                mediadores[2].FirstName.Should().Be("Mediador1-FirstName");
                mediadores[2].LastName.Should().Be("Mediador1-LastName");
                mediadores[2].NickName.Should().Be("Med1");
                mediadores[2].Gender.Should().Be(Gender.Female);
                mediadores[2].Email.Should().Be("mediador1@email.com");
                mediadores[2].Phone.Should().Be("Mediador1-Phone");

                revisores.Should().NotBeNull().And.HaveCount(3);

                revisores[0].Id.Should().Be(1);
                revisores[0].FilialId.Should().Be(1);
                revisores[0].FirstName.Should().Be("Revisor3-FirstName");
                revisores[0].LastName.Should().Be("Revisor3-LastName");
                revisores[0].NickName.Should().Be("Rev3");
                revisores[0].Gender.Should().Be(Gender.Female);
                revisores[0].Email.Should().Be("revisor3@email.com");
                revisores[0].Phone.Should().Be("Revisor3-Phone");

                revisores[1].Id.Should().Be(2);
                revisores[1].FilialId.Should().Be(1);
                revisores[1].FirstName.Should().Be("Revisor2-FirstName");
                revisores[1].LastName.Should().Be("Revisor2-LastName");
                revisores[1].NickName.Should().Be("Rev2");
                revisores[1].Gender.Should().Be(Gender.Male);
                revisores[1].Email.Should().Be("revisor2@email.com");
                revisores[1].Phone.Should().Be("Revisor2-Phone");

                revisores[2].Id.Should().Be(3);
                revisores[2].FilialId.Should().Be(2);
                revisores[2].FirstName.Should().Be("Revisor1-FirstName");
                revisores[2].LastName.Should().Be("Revisor1-LastName");
                revisores[2].NickName.Should().Be("Rev1");
                revisores[2].Gender.Should().Be(Gender.Female);
                revisores[2].Email.Should().Be("revisor1@email.com");
                revisores[2].Phone.Should().Be("Revisor1-Phone");

                padrinos.Should().NotBeNull().And.HaveCount(3);

                padrinos[0].Id.Should().Be(1);
                padrinos[0].FilialId.Should().Be(1);
                padrinos[0].FirstName.Should().Be("Padrino3-FirstName");
                padrinos[0].LastName.Should().Be("Padrino3-LastName");
                padrinos[0].NickName.Should().Be("Pad3");
                padrinos[0].Gender.Should().Be(Gender.Female);
                padrinos[0].Email.Should().Be("padrino3@email.com");
                padrinos[0].Phone.Should().Be("Padrino3-Phone");
                padrinos[0].CreatedByCoordinadorId.Should().Be(1);

                padrinos[1].Id.Should().Be(2);
                padrinos[1].FilialId.Should().Be(1);
                padrinos[1].FirstName.Should().Be("Padrino2-FirstName");
                padrinos[1].LastName.Should().Be("Padrino2-LastName");
                padrinos[1].NickName.Should().Be("Pad2");
                padrinos[1].Gender.Should().Be(Gender.Male);
                padrinos[1].Email.Should().Be("padrino2@email.com");
                padrinos[1].Phone.Should().Be("Padrino2-Phone");
                padrinos[1].CreatedByCoordinadorId.Should().Be(2);

                padrinos[2].Id.Should().Be(3);
                padrinos[2].FilialId.Should().Be(2);
                padrinos[2].FirstName.Should().Be("Padrino1-FirstName");
                padrinos[2].LastName.Should().Be("Padrino1-LastName");
                padrinos[2].NickName.Should().Be("Pad1");
                padrinos[2].Gender.Should().Be(Gender.Female);
                padrinos[2].Email.Should().Be("padrino1@email.com");
                padrinos[2].Phone.Should().Be("Padrino1-Phone");
                padrinos[2].CreatedByCoordinadorId.Should().Be(3);

                becarios.Should().NotBeNull().And.HaveCount(5);

                becarios[0].Id.Should().Be(1);
                becarios[0].FilialId.Should().Be(1);
                becarios[0].FirstName.Should().Be("Becario5-FirstName");
                becarios[0].LastName.Should().Be("Becario5-LastName");
                becarios[0].NickName.Should().Be("Bec5");
                becarios[0].NivelDeEstudio.Should().Be(NivelDeEstudio.Primario);
                becarios[0].Gender.Should().Be(Gender.Female);
                becarios[0].Email.Should().Be("becario5@email.com");
                becarios[0].Phone.Should().Be("Becario5-Phone");
                becarios[0].CreatedByCoordinadorId.Should().Be(1);
                becarios[0].MediadorId.Should().Be(1);

                becarios[1].Id.Should().Be(2);
                becarios[1].FilialId.Should().Be(1);
                becarios[1].FirstName.Should().Be("Becario4-FirstName");
                becarios[1].LastName.Should().Be("Becario4-LastName");
                becarios[1].NickName.Should().Be("Bec4");
                becarios[1].NivelDeEstudio.Should().Be(NivelDeEstudio.Secundario);
                becarios[1].Gender.Should().Be(Gender.Male);
                becarios[1].Email.Should().Be("becario4@email.com");
                becarios[1].Phone.Should().Be("Becario4-Phone");
                becarios[1].CreatedByCoordinadorId.Should().Be(1);
                becarios[1].MediadorId.Should().Be(1);

                becarios[2].Id.Should().Be(3);
                becarios[2].FilialId.Should().Be(1);
                becarios[2].FirstName.Should().Be("Becario3-FirstName");
                becarios[2].LastName.Should().Be("Becario3-LastName");
                becarios[2].NickName.Should().Be("Bec3");
                becarios[2].NivelDeEstudio.Should().Be(NivelDeEstudio.Universitario);
                becarios[2].Gender.Should().Be(Gender.Female);
                becarios[2].Email.Should().Be("becario3@email.com");
                becarios[2].Phone.Should().Be("Becario3-Phone");
                becarios[2].CreatedByCoordinadorId.Should().Be(2);
                becarios[2].MediadorId.Should().Be(2);

                becarios[3].Id.Should().Be(4);
                becarios[3].FilialId.Should().Be(1);
                becarios[3].FirstName.Should().Be("Becario2-FirstName");
                becarios[3].LastName.Should().Be("Becario2-LastName");
                becarios[3].NickName.Should().Be("Bec2");
                becarios[3].NivelDeEstudio.Should().Be(NivelDeEstudio.Primario);
                becarios[3].Gender.Should().Be(Gender.Male);
                becarios[3].Email.Should().Be("becario2@email.com");
                becarios[3].Phone.Should().Be("Becario2-Phone");
                becarios[3].CreatedByCoordinadorId.Should().Be(2);
                becarios[3].MediadorId.Should().Be(2);

                becarios[4].Id.Should().Be(5);
                becarios[4].FilialId.Should().Be(2);
                becarios[4].FirstName.Should().Be("Becario1-FirstName");
                becarios[4].LastName.Should().Be("Becario1-LastName");
                becarios[4].NickName.Should().Be("Bec1");
                becarios[4].NivelDeEstudio.Should().Be(NivelDeEstudio.Secundario);
                becarios[4].Gender.Should().Be(Gender.Female);
                becarios[4].Email.Should().Be("becario1@email.com");
                becarios[4].Phone.Should().Be("Becario1-Phone");
                becarios[4].CreatedByCoordinadorId.Should().Be(3);
                becarios[4].MediadorId.Should().Be(3);

                apadrinamientos.Should().NotBeNull().And.HaveCount(4);

                apadrinamientos[0].Id.Should().Be(1);
                apadrinamientos[0].PadrinoId.Should().Be(1);
                apadrinamientos[0].BecarioId.Should().Be(1);
                apadrinamientos[0].From.Should().Be(new DateTime(2020, 1, 1));
                apadrinamientos[0].To.Should().Be(new DateTime(2020, 5, 30));
                apadrinamientos[0].CreatedByCoordinadorId.Should().Be(1);

                apadrinamientos[1].Id.Should().Be(2);
                apadrinamientos[1].PadrinoId.Should().Be(1);
                apadrinamientos[1].BecarioId.Should().Be(2);
                apadrinamientos[1].From.Should().Be(new DateTime(2020, 1, 1));
                apadrinamientos[1].To.Should().Be(new DateTime(2020, 5, 30));
                apadrinamientos[1].CreatedByCoordinadorId.Should().Be(1);

                apadrinamientos[2].Id.Should().Be(3);
                apadrinamientos[2].PadrinoId.Should().Be(2);
                apadrinamientos[2].BecarioId.Should().Be(3);
                apadrinamientos[2].From.Should().Be(new DateTime(2020, 6, 1));
                apadrinamientos[2].To.Should().Be(new DateTime(2020, 9, 30));
                apadrinamientos[2].CreatedByCoordinadorId.Should().Be(1);

                apadrinamientos[3].Id.Should().Be(4);
                apadrinamientos[3].PadrinoId.Should().Be(3);
                apadrinamientos[3].BecarioId.Should().Be(4);
                apadrinamientos[3].From.Should().Be(new DateTime(2020, 1, 1));
                apadrinamientos[3].To.Should().Be(new DateTime(2020, 9, 30));
                apadrinamientos[3].CreatedByCoordinadorId.Should().Be(3);
            }
        }
    }
}
