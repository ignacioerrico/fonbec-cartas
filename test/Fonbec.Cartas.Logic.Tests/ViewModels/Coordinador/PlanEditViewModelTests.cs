using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Mapster;

namespace Fonbec.Cartas.Logic.Tests.ViewModels.Coordinador
{
    public class PlanEditViewModelTests : MapsterTests
    {
        [Fact]
        public void Map_PlanEditViewModel_To_Plan()
        {
            // Arrange
            var startDate = new DateTime(2020, 6, 1);

            var planEditViewModel = new PlanEditViewModel
            {
                FilialId = 42,
                StartDate = startDate,
                Subject = "Subject",
                MessageMarkdown = "MessageMarkdown",
                CreatedByCoordinadorId = 78,
                UpdatedByCoordinadorId = 123,
            };

            // Act
            var result = planEditViewModel.Adapt<Plan>();

            // Assert
            using (new AssertionScope())
            {
                result.FilialId.Should().Be(42);
                result.StartDate.Should().Be(startDate);
                result.Subject.Should().Be("Subject");
                result.MessageMarkdown.Should().Be("MessageMarkdown");
                result.CreatedByCoordinadorId.Should().Be(78);
                result.UpdatedByCoordinadorId.Should().Be(123);
            }
        }
    }
}
