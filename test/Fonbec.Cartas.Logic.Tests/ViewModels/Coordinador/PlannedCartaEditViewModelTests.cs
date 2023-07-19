﻿using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.DataAccess.Entities.Planning;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Mapster;

namespace Fonbec.Cartas.Logic.Tests.ViewModels.Coordinador
{
    public class PlannedCartaEditViewModelTests : MapsterTests
    {
        [Fact]
        public void Map_PlannedEvent_To_PlanEditViewModel()
        {
            // Arrange
            var startDate = new DateTime(2020, 6, 1);

            var plannedEvent = new PlannedEvent
            {
                FilialId = 42,
                Date = startDate,
                CartaObligatoria = new CartaObligatoria
                {
                    Subject = "Subject",
                    MessageMarkdown = "MessageMarkdown",
                },
                CreatedByCoordinadorId = 78,
                UpdatedByCoordinadorId = 123,
            };

            // Act
            var result = plannedEvent.Adapt<PlannedCartaEditViewModel>();

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

            var planEditViewModel = new PlannedCartaEditViewModel
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
                result.CartaObligatoria.Should().NotBeNull();
                
                result.FilialId.Should().Be(42);
                result.Date.Should().Be(startDate);
                result.Type.Should().Be(PlannedEventType.CartaObligatoria);
                result.CartaObligatoria!.Subject.Should().Be("Subject");
                result.CartaObligatoria.MessageMarkdown.Should().Be("MessageMarkdown");
                result.CreatedByCoordinadorId.Should().Be(78);
                result.UpdatedByCoordinadorId.Should().Be(123);
            }
        }
    }
}
