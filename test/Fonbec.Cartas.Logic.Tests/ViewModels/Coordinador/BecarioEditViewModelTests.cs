using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Mapster;

namespace Fonbec.Cartas.Logic.Tests.ViewModels.Coordinador
{
    public class BecarioEditViewModelTests : MapsterTests
    {
        [Fact]
        public void Map_Becario_To_BecarioEditViewModel_AllPropertiesHaveValues()
        {
            var becario = new Becario
            {
                FilialId = 42,
                MediadorId = 78,
                NivelDeEstudio = NivelDeEstudio.Universitario,
                FirstName = "FirstName-Becario",
                LastName = "LastName-Becario",
                NickName = "NickName-Becario",
                Gender = Gender.Female,
                Email = "Email",
                Phone = "Phone",
                CreatedByCoordinadorId = 128,
                UpdatedByCoordinadorId = 512,
            };

            var result = becario.Adapt<BecarioEditViewModel>();

            using (new AssertionScope())
            {
                result.FilialId.Should().Be(42);
                result.MediadorId.Should().Be(78);
                result.NivelDeEstudio.Should().Be(NivelDeEstudio.Universitario);
                result.FirstName.Should().Be("FirstName-Becario");
                result.LastName.Should().Be("LastName-Becario");
                result.NickName.Should().Be("NickName-Becario");
                result.Gender.Should().Be(Gender.Female);
                result.Email.Should().Be("Email");
                result.Phone.Should().Be("Phone");
                result.CreatedByCoordinadorId.Should().Be(128);
                result.UpdatedByCoordinadorId.Should().Be(512);
            }
        }

        [Fact]
        public void Map_Becario_To_BecarioEditViewModel_NullablePropertiesMappedAsEmpty()
        {
            var becario = new Becario();

            var result = becario.Adapt<BecarioEditViewModel>();

            using (new AssertionScope())
            {
                result.LastName.Should().BeEmpty();
                result.NickName.Should().BeEmpty();
                result.Email.Should().BeEmpty();
                result.Phone.Should().BeEmpty();
            }
        }

        [Fact]
        public void Map_BecarioEditViewModel_To_Becario_AllPropertiesHaveValues()
        {
            var becario = new BecarioEditViewModel
            {
                FilialId = 42,
                MediadorId = 78,
                NivelDeEstudio = NivelDeEstudio.Universitario,
                FirstName = "FirstName-Becario",
                LastName = "LastName-Becario",
                NickName = "NickName-Becario",
                Gender = Gender.Female,
                Email = "Email",
                Phone = "Phone",
                CreatedByCoordinadorId = 128,
                UpdatedByCoordinadorId = 512,
            };

            var result = becario.Adapt<Becario>();

            using (new AssertionScope())
            {
                result.FilialId.Should().Be(42);
                result.MediadorId.Should().Be(78);
                result.NivelDeEstudio.Should().Be(NivelDeEstudio.Universitario);
                result.FirstName.Should().Be("FirstName-Becario");
                result.LastName.Should().Be("LastName-Becario");
                result.NickName.Should().Be("NickName-Becario");
                result.Gender.Should().Be(Gender.Female);
                result.Email.Should().Be("Email");
                result.Phone.Should().Be("Phone");
                result.CreatedByCoordinadorId.Should().Be(128);
                result.UpdatedByCoordinadorId.Should().Be(512);
            }
        }

        [Fact]
        public void Map_BecarioEditViewModel_To_Becario_NullablePropertiesMappedAsEmpty()
        {
            var becario = new BecarioEditViewModel();

            var result = becario.Adapt<Becario>();

            using (new AssertionScope())
            {
                result.LastName.Should().BeNull();
                result.NickName.Should().BeNull();
                result.Email.Should().BeNull();
                result.Phone.Should().BeNull();
            }
        }
    }
}
