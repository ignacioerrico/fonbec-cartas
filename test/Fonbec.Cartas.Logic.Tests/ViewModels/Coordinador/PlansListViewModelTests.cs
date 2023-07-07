using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Mapster;

namespace Fonbec.Cartas.Logic.Tests.ViewModels.Coordinador
{
    public class PlansListViewModelTests : MapsterTests
    {
        [Fact]
        public void Map_Plan_To_PlansListViewModel_HasBeenUpdated()
        {
            // Arrange
            var createdOnUtc = DateTimeOffset.Now;

            var startDate = new DateTime(2020, 6, 1);

            var plan = new Plan
            {
                Id = 42,
                FilialId = 314,
                StartDate = startDate,
                CreatedByCoordinadorId = 78,
                CreatedByCoordinador = new DataAccess.Entities.Actors.Coordinador
                {
                    FirstName = "FirstName-1",
                    LastName = "LastName-1"
                },
                CreatedOnUtc = createdOnUtc,
                UpdatedByCoordinadorId = 96,
                UpdatedByCoordinador = new DataAccess.Entities.Actors.Coordinador
                {
                    FirstName = "FirstName-2",
                    LastName = "LastName-2"
                },
                LastUpdatedOnUtc = createdOnUtc.AddDays(1),
            };

            // Act
            var result = plan.Adapt<PlansListViewModel>();

            // Assert
            using (new AssertionScope())
            {
                result.Id.Should().Be(42);
                result.StartDate.Should().Be(startDate);
                result.PlanName.Should().Be("Carta de junio de 2020");
                result.TotalLettersToSend.Should().Be(0); // TODO
                result.LettersSent.Should().Be(0); // TODO
                result.Percentage.Should().Be(0); // TODO
                result.CreatedOnUtc.Should().Be(createdOnUtc);
                result.CreatedBy.Should().Be("FirstName-1 LastName-1");
                result.LastUpdatedOnUtc.Should().Be(createdOnUtc.AddDays(1));
                result.UpdatedBy.Should().Be("FirstName-2 LastName-2");
            }
        }

        [Fact]
        public void Map_Plan_To_PlansListViewModel_HasNotBeenUpdated()
        {
            // Arrange
            var createdOnUtc = DateTimeOffset.Now;

            var startDate = new DateTime(2020, 6, 1);

            var plan = new Plan
            {
                Id = 42,
                FilialId = 314,
                StartDate = startDate,
                CreatedByCoordinadorId = 78,
                CreatedByCoordinador = new DataAccess.Entities.Actors.Coordinador
                {
                    FirstName = "FirstName-1",
                    LastName = "LastName-1"
                },
                CreatedOnUtc = createdOnUtc,
            };

            // Act
            var result = plan.Adapt<PlansListViewModel>();

            // Assert
            using (new AssertionScope())
            {
                result.Id.Should().Be(42);
                result.StartDate.Should().Be(startDate);
                result.PlanName.Should().Be("Carta de junio de 2020");
                result.TotalLettersToSend.Should().Be(0); // TODO
                result.LettersSent.Should().Be(0); // TODO
                result.Percentage.Should().Be(0); // TODO
                result.CreatedOnUtc.Should().Be(createdOnUtc);
                result.CreatedBy.Should().Be("FirstName-1 LastName-1");
                result.UpdatedBy.Should().BeNull();
            }
        }
    }
}
