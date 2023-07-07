using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Mapster;

namespace Fonbec.Cartas.Logic.Tests.ViewModels.Coordinador
{
    public class BecariosListViewModelTests : MapsterTests
    {
        [Fact]
        public void Map_Becario_To_BecariosListViewModel()
        {
            // Arrange
            var createdOnUtc = DateTimeOffset.Now;

            var becario = new Becario
            {
                CreatedOnUtc = createdOnUtc,
                LastUpdatedOnUtc = createdOnUtc.AddDays(1),
                Id = 42,
                FirstName = "FirstName-Becario",
                LastName = "LastName-Becario",
                NickName = "NickName-Becario",
                Gender = Gender.Female,
                Phone = "Phone",
                CreatedByCoordinador = new DataAccess.Entities.Actors.Coordinador
                {
                    FirstName = "FirsName-CoordinadorCreated",
                    LastName = "LastName-CoordinadorCreated",
                    NickName = "NickName-CoordinadorCreated",
                },
                UpdatedByCoordinador = new DataAccess.Entities.Actors.Coordinador
                {
                    FirstName = "FirsName-CoordinadorUpdated",
                    LastName = "LastName-CoordinadorUpdated",
                    NickName = "NickName-CoordinadorUpdated",
                },
                Mediador = new Mediador
                {
                    FirstName = "FirstName-Mediador",
                    LastName = "LastName-Mediador",
                    NickName = "NickName-Mediador",
                },
                Apadrinamientos = new()
                {
                    new()
                    {
                        From = DateTime.Now.AddDays(-1),
                        To = DateTime.Now.AddDays(1),
                        Padrino = new()
                        {
                            FirstName = "FirstName-Padrino-1",
                            LastName = "LastName-Padrino-1",
                            NickName = "NickName-Padrino-1",
                        }
                    },
                    new()
                    {
                        From = DateTime.Now.AddDays(11),
                        To = DateTime.Now.AddDays(20),
                        Padrino = new()
                        {
                            FirstName = "FirstName-Padrino-2",
                            LastName = "LastName-Padrino-2",
                            NickName = "NickName-Padrino-2",
                        }
                    },
                    new()
                    {
                        From = DateTime.Now.AddDays(-1),
                        To = DateTime.Now.AddDays(10),
                        Padrino = new()
                        {
                            FirstName = "FirstName-Padrino-3",
                            LastName = "LastName-Padrino-3",
                            NickName = "NickName-Padrino-3",
                        }
                    },
                },
                NivelDeEstudio = NivelDeEstudio.Universitario,
                Email = "Email",
            };

            // Act
            var result = becario.Adapt<BecariosListViewModel>();

            // Assert
            using (new AssertionScope())
            {
                result.Id.Should().Be(42);
                result.Mediador.Should().Be("FirstName-Mediador LastName-Mediador");
                result.PadrinosActivos.Should().HaveCount(2);
                result.PadrinosActivos[0].Should().Be("FirstName-Padrino-1 LastName-Padrino-1");
                result.PadrinosActivos[1].Should().Be("FirstName-Padrino-3 LastName-Padrino-3");
                result.PadrinosFuturos.Should().HaveCount(1);
                result.PadrinosFuturos[0].Should().Be("FirstName-Padrino-2 LastName-Padrino-2");
                result.LatestActiveAssignmentEndsOn.Should().NotBeNull();
                result.LatestActiveAssignmentEndsOn!.Value.Date.Should().Be(DateTime.Now.AddDays(10).Date);
                result.NivelDeEstudio.Should().Be("Universitario");
                result.Name.Should().Be("FirstName-Becario LastName-Becario (\"NickName-Becario\")");
                result.Gender.Should().Be(Gender.Female);
                result.Email.Should().Be("Email");
                result.Phone.Should().Be("Phone");
                result.CreatedOnUtc.Should().Be(createdOnUtc);
                result.CreatedBy.Should().Be("FirsName-CoordinadorCreated LastName-CoordinadorCreated");
                result.UpdatedBy.Should().Be("FirsName-CoordinadorUpdated LastName-CoordinadorUpdated");
            }
        }
    }
}
