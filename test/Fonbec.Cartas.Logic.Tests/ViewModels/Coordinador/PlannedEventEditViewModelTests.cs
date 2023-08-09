using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.Entities.Planning;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Mapster;

namespace Fonbec.Cartas.Logic.Tests.ViewModels.Coordinador
{
    public class PlannedEventEditViewModelTests : MapsterTests
    {
        [Fact]
        public void Map_PlannedEvent_To_PlanEditViewModel()
        {
            // Arrange
            var startDate = new DateTime(2020, 6, 1);

            var plannedEvent = new PlannedEvent
            {
                FilialId = 42,
                StartsOn = startDate,
                Subject = "Subject",
                MessageMarkdown = "MessageMarkdown",
                CreatedByCoordinadorId = 78,
                UpdatedByCoordinadorId = 123,
            };

            // Act
            var result = plannedEvent.Adapt<PlannedEventEditViewModel>();

            // Assert
            using (new AssertionScope())
            {
                result.FilialId.Should().Be(42);
                result.Date.Should().Be(startDate);
                result.Subject.Should().Be("Subject");
                result.MessageMarkdown.Should().Be("MessageMarkdown");
                result.CreatedByCoordinadorId.Should().Be(78);
                result.UpdatedByCoordinadorId.Should().Be(123);
            }
        }

        [Fact]
        public void Map_PlanEditViewModel_To_PlannedEvent()
        {
            // Arrange
            var startDate = new DateTime(2020, 6, 1);

            var planEditViewModel = new PlannedEventEditViewModel
            {
                FilialId = 42,
                Date = startDate,
                Subject = "Subject",
                MessageMarkdown = "MessageMarkdown",
                CreatedByCoordinadorId = 78,
                UpdatedByCoordinadorId = 123,
            };

            // Act
            var result = planEditViewModel.Adapt<PlannedEvent>();

            // Assert
            using (new AssertionScope())
            {
                result.FilialId.Should().Be(42);
                result.StartsOn.Should().Be(startDate);
                result.Subject.Should().Be("Subject");
                result.MessageMarkdown.Should().Be("MessageMarkdown");
                result.CreatedByCoordinadorId.Should().Be(78);
                result.UpdatedByCoordinadorId.Should().Be(123);
            }
        }
    }
}
