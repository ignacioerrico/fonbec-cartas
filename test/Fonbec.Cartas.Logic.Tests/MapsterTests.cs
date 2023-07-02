using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.DataModels.Admin;
using Fonbec.Cartas.Logic.ViewModels.Admin;
using Mapster;
using System.Reflection;

namespace Fonbec.Cartas.Logic.Tests
{
    public class MapsterTests
    {
        private readonly TypeAdapterConfig _config;

        public MapsterTests()
        {
            _config = new TypeAdapterConfig();
            
            var logicAssembly = Assembly.Load("Fonbec.Cartas.Logic");
            _config.Scan(logicAssembly);
        }

        [Fact]
        public void Map_FilialesListDataModel_To_FilialesListViewModel()
        {
            // Arrange
            var createdOnUtc = DateTimeOffset.Now;
            var lastUpdatedOnUtc = createdOnUtc.AddMinutes(78);

            var filialesListDataModel = new FilialesListDataModel
            {
                FilialId = 42,
                FilialName = "FilialName",
                Coordinadores = new List<string> { "One", "Two", "Three" },
                QtyMediadores = 3,
                QtyRevisores = 14,
                QtyPadrinos = 15,
                QtyBecarios = 92,
                CreatedOnUtc = createdOnUtc,
                LastUpdatedOnUtc = lastUpdatedOnUtc
            };

            // Act
            var result = filialesListDataModel.Adapt<FilialesListViewModel>(_config);

            // Assert
            using (new AssertionScope())
            {
                result.FilialId.Should().Be(42);
                result.FilialName.Should().Be("FilialName");
                result.Coordinadores.Should().HaveCount(3);
                result.Coordinadores[0].Should().Be("One");
                result.Coordinadores[1].Should().Be("Two");
                result.Coordinadores[2].Should().Be("Three");
                result.QtyMediadores.Should().Be(3);
                result.QtyRevisores.Should().Be(14);
                result.QtyPadrinos.Should().Be(15);
                result.QtyBecarios.Should().Be(92);
                result.CreatedOnUtc.Should().Be(createdOnUtc);
                result.LastUpdatedOnUtc.Should().Be(lastUpdatedOnUtc);
            }
        }
    }
}
