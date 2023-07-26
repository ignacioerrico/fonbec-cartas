using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.DataAccess.Entities.Planning;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Mapster;

namespace Fonbec.Cartas.Logic.Tests.ViewModels.Coordinador
{
    public class PlannedEventsListViewModelTests : MapsterTests
    {
        [Fact]
        public void Map_Plan_To_PlansListViewModel_HasBeenUpdated()
        {
            // Arrange
            var createdOnUtc = DateTimeOffset.Now;

            var startDate = new DateTime(2020, 6, 1);

            var plan = new PlannedEvent
            {
                Id = 42,
                FilialId = 314,
                Date = startDate,
                Type = PlannedEventType.CartaObligatoria,
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
            var result = plan.Adapt<PlannedEventsListViewModel>();

            // Assert
            using (new AssertionScope())
            {
                result.Id.Should().Be(42);
                result.Date.Should().Be(startDate);
                result.Type.Should().Be(PlannedEventType.CartaObligatoria);
                result.EventName.Should().Be("Carta de junio de 2020");
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

            var plan = new PlannedEvent
            {
                Id = 42,
                FilialId = 314,
                Date = startDate,
                Type = PlannedEventType.Notas,
                CreatedByCoordinadorId = 78,
                CreatedByCoordinador = new DataAccess.Entities.Actors.Coordinador
                {
                    FirstName = "FirstName-1",
                    LastName = "LastName-1"
                },
                CreatedOnUtc = createdOnUtc,
            };

            // Act
            var result = plan.Adapt<PlannedEventsListViewModel>();

            // Assert
            using (new AssertionScope())
            {
                result.Id.Should().Be(42);
                result.Date.Should().Be(startDate);
                result.Type.Should().Be(PlannedEventType.Notas);
                result.EventName.Should().Be("junio de 2020: corte para recepción de notas");
                result.CreatedOnUtc.Should().Be(createdOnUtc);
                result.CreatedBy.Should().Be("FirstName-1 LastName-1");
                result.UpdatedBy.Should().BeNull();
            }
        }

        [Theory]
        [InlineData(PlannedEventType.CartaObligatoria, "Carta de junio de 2020")]
        [InlineData(PlannedEventType.Notas, "junio de 2020: corte para recepción de notas")]
        public void Map_Plan_To_PlansListViewModel_PlannedEventTypes(PlannedEventType plannedEventType, string expectedEventName)
        {
            // Arrange
            var plan = new PlannedEvent
            {
                Date = new DateTime(2020, 6, 1),
                Type = plannedEventType,
                CreatedByCoordinador = new DataAccess.Entities.Actors.Coordinador
                {
                    FirstName = "FirstName-1",
                    LastName = "LastName-1"
                },
            };

            // Act
            var result = plan.Adapt<PlannedEventsListViewModel>();

            // Assert
            using (new AssertionScope())
            {
                result.Type.Should().Be(plannedEventType);
                result.EventName.Should().Be(expectedEventName);
            }
        }

        [Theory]
        [InlineData(PlannedEventType.CartaObligatoria, "La recepción de cartas comenzó el")]
        [InlineData(PlannedEventType.Notas, "Se recibieron notas hasta el")]
        public void Map_Plan_To_PlansListViewModel_ExplanationsForPastEvents(PlannedEventType plannedEventType, string expectedExplanation0)
        {
            // Arrange
            var pastDate = DateTime.Today.AddMonths(-1);

            var plan = new PlannedEvent
            {
                Date = pastDate,
                Type = plannedEventType,
                CreatedByCoordinador = new DataAccess.Entities.Actors.Coordinador
                {
                    FirstName = "FirstName-1",
                    LastName = "LastName-1"
                },
            };

            // Act
            var result = plan.Adapt<PlannedEventsListViewModel>();

            // Assert
            using (new AssertionScope())
            {
                result.Type.Should().Be(plannedEventType);
                result.Explanation.Should().Be($"{expectedExplanation0} {pastDate.Date:d/M/yy}");
            }
        }

        [Theory]
        [InlineData(PlannedEventType.CartaObligatoria, "La recepción de cartas se habilitará el")]
        [InlineData(PlannedEventType.Notas, "Se recibirán notas hasta el")]
        public void Map_Plan_To_PlansListViewModel_ExplanationsForFutureEvents(PlannedEventType plannedEventType, string expectedExplanation0)
        {
            // Arrange
            var futureDate = DateTime.Today.AddMonths(1);

            var plan = new PlannedEvent
            {
                Date = futureDate,
                Type = plannedEventType,
                CreatedByCoordinador = new DataAccess.Entities.Actors.Coordinador
                {
                    FirstName = "FirstName-1",
                    LastName = "LastName-1"
                },
            };

            // Act
            var result = plan.Adapt<PlannedEventsListViewModel>();

            // Assert
            using (new AssertionScope())
            {
                result.Type.Should().Be(plannedEventType);
                result.Explanation.Should().Be($"{expectedExplanation0} {futureDate.Date:d/M/yy}");
            }
        }

        [Fact]
        public void Map_Plan_To_PlansListViewModel_Statistics()
        {
            // Arrange
            var plan = new PlannedEvent
            {
                PlannedDeliveries = new()
                {
                    new(),
                    new() { SentOn = DateTimeOffset.Now },
                    new(),
                    new() { SentOn = DateTimeOffset.Now },
                    new() { SentOn = DateTimeOffset.Now },
                },
                CreatedByCoordinador = new DataAccess.Entities.Actors.Coordinador
                {
                    FirstName = "FirstName-1",
                    LastName = "LastName-1"
                },
            };

            // Act
            var result = plan.Adapt<PlannedEventsListViewModel>();

            // Assert
            using (new AssertionScope())
            {
                result.TotalToSend.Should().Be(5);
                result.AlreadySent.Should().Be(3);
                result.Percentage.Should().BeGreaterThan(.59)
                    .And.BeLessThan(.61); // So about 3/5
            }
        }
    }
}
