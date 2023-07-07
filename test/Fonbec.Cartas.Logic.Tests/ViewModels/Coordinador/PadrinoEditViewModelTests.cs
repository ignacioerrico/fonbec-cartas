using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Mapster;

namespace Fonbec.Cartas.Logic.Tests.ViewModels.Coordinador
{
    public class PadrinoEditViewModelTests : MapsterTests
    {
        [Fact]
        public void Map_Padrino_To_PadrinoEditViewModel()
        {
            // Arrange
            var padrino = new Padrino
            {
                FirstName = "FirstName",
                LastName = "LastName",
                NickName = "NickName",
                Gender = Gender.Female,
                Email = "Email",
                SendAlsoTo = new()
                {
                    new()
                    {
                        RecipientFullName = "SendAlsoTo-RecipientFullName-1",
                        RecipientEmail = "SendAlsoTo-RecipientEmail-1",
                        SendAsBcc = false,
                    },
                    new()
                    {
                        RecipientFullName = "SendAlsoTo-RecipientFullName-2",
                        RecipientEmail = "SendAlsoTo-RecipientEmail-2",
                        SendAsBcc = true,
                    },
                },
                Phone = "Phone",
                CreatedByCoordinadorId = 3,
                UpdatedByCoordinadorId = 14,
            };

            // Act
            var result = padrino.Adapt<PadrinoEditViewModel>();

            // Assert
            using (new AssertionScope())
            {
                result.FirstName.Should().Be("FirstName");
                result.LastName.Should().Be("LastName");
                result.NickName.Should().Be("NickName");
                result.Gender.Should().Be(Gender.Female);
                result.Email.Should().Be("Email");
                
                result.SendAlsoTo.Should().HaveCount(2);
                
                result.SendAlsoTo[0].RecipientFullName.Should().Be("SendAlsoTo-RecipientFullName-1");
                result.SendAlsoTo[0].RecipientEmail.Should().Be("SendAlsoTo-RecipientEmail-1");
                result.SendAlsoTo[0].SendAsBcc.Should().BeFalse();

                result.SendAlsoTo[1].RecipientFullName.Should().Be("SendAlsoTo-RecipientFullName-2");
                result.SendAlsoTo[1].RecipientEmail.Should().Be("SendAlsoTo-RecipientEmail-2");
                result.SendAlsoTo[1].SendAsBcc.Should().BeTrue();

                result.Phone.Should().Be("Phone");
                result.CreatedByCoordinadorId.Should().Be(3);
                result.UpdatedByCoordinadorId.Should().Be(14);
            }
        }

        [Fact]
        public void Map_PadrinoEditViewModel_To_Padrino()
        {
            // Arrange
            var padrinoEditViewModel = new PadrinoEditViewModel
            {
                FirstName = "FirstName",
                LastName = "LastName",
                NickName = "NickName",
                Gender = Gender.Female,
                Email = "Email",
                SendAlsoTo = new()
                {
                    new()
                    {
                        RecipientFullName = "SendAlsoTo-RecipientFullName-1",
                        RecipientEmail = "SendAlsoTo-RecipientEmail-1",
                        SendAsBcc = false,
                    },
                    new()
                    {
                        RecipientFullName = "SendAlsoTo-RecipientFullName-2",
                        RecipientEmail = "SendAlsoTo-RecipientEmail-2",
                        SendAsBcc = true,
                    },
                },
                Phone = "Phone",
                FilialId = 42,
                CreatedByCoordinadorId = 3,
                UpdatedByCoordinadorId = 14
            };

            // Act
            var result = padrinoEditViewModel.Adapt<Padrino>();

            // Assert
            using (new AssertionScope())
            {
                result.FilialId.Should().Be(42);
                result.FirstName.Should().Be("FirstName");
                result.LastName.Should().Be("LastName");
                result.NickName.Should().Be("NickName");
                result.Gender.Should().Be(Gender.Female);
                result.Email.Should().Be("Email");

                result.SendAlsoTo.Should().NotBeNull();
                result.SendAlsoTo.Should().HaveCount(2);

                result.SendAlsoTo![0].RecipientFullName.Should().Be("SendAlsoTo-RecipientFullName-1");
                result.SendAlsoTo[0].RecipientEmail.Should().Be("SendAlsoTo-RecipientEmail-1");
                result.SendAlsoTo[0].SendAsBcc.Should().BeFalse();

                result.SendAlsoTo[1].RecipientFullName.Should().Be("SendAlsoTo-RecipientFullName-2");
                result.SendAlsoTo[1].RecipientEmail.Should().Be("SendAlsoTo-RecipientEmail-2");
                result.SendAlsoTo[1].SendAsBcc.Should().BeTrue();

                result.Phone.Should().Be("Phone");
                result.CreatedByCoordinadorId.Should().Be(3);
                result.UpdatedByCoordinadorId.Should().Be(14);
            }
        }
    }
}
