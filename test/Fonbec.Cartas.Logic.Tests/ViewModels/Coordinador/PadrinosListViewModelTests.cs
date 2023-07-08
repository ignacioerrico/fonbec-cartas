using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Mapster;

namespace Fonbec.Cartas.Logic.Tests.ViewModels.Coordinador
{
    public class PadrinosListViewModelTests : MapsterTests
    {
        [Fact]
        public void Map_Padrino_To_PadrinosListViewModel()
        {
            // Arrange
            var createdOnUtc = DateTimeOffset.Now;

            var padrino = new Padrino
            {
                CreatedOnUtc = createdOnUtc,
                LastUpdatedOnUtc = createdOnUtc.AddDays(1),
                Id = 42,
                FirstName = "FirstName",
                LastName = "LastName",
                NickName = "NickName",
                Gender = Gender.Female,
                Email = "Email",
                Phone = "Phone",
                CreatedByCoordinador = new()
                {
                    FirstName = "CreatedByCoordinador-FirstName",
                    LastName = "CreatedByCoordinador-LastName",
                    NickName = "CreatedByCoordinador-NickName",
                },
                UpdatedByCoordinador = new()
                {
                    FirstName = "UpdatedByCoordinador-FirstName",
                    LastName = "UpdatedByCoordinador-LastName",
                    NickName = "UpdatedByCoordinador-NickName",
                },
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
                        SendAsBcc = false,
                    },
                    new()
                    {
                        RecipientFullName = "SendAlsoTo-RecipientFullName-3",
                        RecipientEmail = "SendAlsoTo-RecipientEmail-3",
                        SendAsBcc = true,
                    },
                },
                Apadrinamientos = new()
                {
                    new()
                    {
                        Becario = new()
                        {
                            FirstName = "Becario-FirstName-1",
                            LastName = "Becario-LastName-1",
                            NickName = "Becario-NickName-1",
                            Email = "Becario-Email-1",
                        },
                    },
                    new()
                    {
                        Becario = new()
                        {
                            FirstName = "Becario-FirstName-2",
                            LastName = "Becario-LastName-2",
                            NickName = "Becario-NickName-2",
                            Email = "Becario-Email-2",
                        },
                    },
                },
            };

            // Act
            var result = padrino.Adapt<PadrinosListViewModel>();

            // Assert
            using (new AssertionScope())
            {
                result.PadrinoId.Should().Be(42);
                
                result.BecariosActivos.Should().HaveCount(2);
                result.BecariosActivos[0].BecarioFullName.Should().Be("Becario-FirstName-1 Becario-LastName-1");
                result.BecariosActivos[0].BecarioEmail.Should().Be("Becario-Email-1");
                result.BecariosActivos[1].BecarioFullName.Should().Be("Becario-FirstName-2 Becario-LastName-2");
                result.BecariosActivos[1].BecarioEmail.Should().Be("Becario-Email-2");

                result.PadrinoFullName.Should().Be("FirstName LastName (\"NickName\")");
                result.PadrinoGender.Should().Be(Gender.Female);
                result.PadrinoEmail.Should().Be("Email");
                
                result.Cc.Should().HaveCount(2);
                result.Cc[0].Should().Be("SendAlsoTo-RecipientFullName-1 <SendAlsoTo-RecipientEmail-1>");
                result.Cc[1].Should().Be("SendAlsoTo-RecipientFullName-2 <SendAlsoTo-RecipientEmail-2>");

                result.Bcc.Should().ContainSingle();
                result.Bcc[0].Should().Be("SendAlsoTo-RecipientFullName-3 <SendAlsoTo-RecipientEmail-3>");

                result.PadrinoPhone.Should().Be("Phone");
                result.CreatedOnUtc.Should().Be(createdOnUtc);
                result.LastUpdatedOnUtc.Should().Be(createdOnUtc.AddDays(1));
                result.CreatedBy.Should().Be("CreatedByCoordinador-FirstName CreatedByCoordinador-LastName");
                result.UpdatedBy.Should().Be("UpdatedByCoordinador-FirstName UpdatedByCoordinador-LastName");
            }
        }
    }
}
