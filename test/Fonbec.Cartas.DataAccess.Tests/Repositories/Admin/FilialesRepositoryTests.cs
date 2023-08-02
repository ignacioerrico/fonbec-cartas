using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Repositories.Admin;

namespace Fonbec.Cartas.DataAccess.Tests.Repositories.Admin
{
    [Collection("Sequential")]
    public class FilialesRepositoryTests
    {
        [Fact]
        public async Task GetAllFilialesAsync_Success()
        {
            // Arrange
            var testDatabase = new TestDatabase();

            // Act
            var result = await GetSut(testDatabase).GetAllFilialesAsync();

            // Assert
            using (new AssertionScope())
            {
                result.Should().HaveCount(2);

                result[0].FilialId.Should().Be(1);
                result[0].FilialName.Should().Be("Default");
                result[0].Coordinadores.Should().HaveCount(2);
                result[0].Coordinadores[0].Should().Be("Coordinador2-FirstName Coordinador2-LastName");
                result[0].Coordinadores[1].Should().Be("Coordinador3-FirstName Coordinador3-LastName");
                result[0].QtyMediadores.Should().Be(2);
                result[0].QtyRevisores.Should().Be(2);
                result[0].QtyPadrinos.Should().Be(2);
                result[0].QtyBecarios.Should().Be(4);
                result[0].LastUpdatedOnUtc.Should().BeNull();

                result[1].FilialId.Should().Be(2);
                result[1].FilialName.Should().Be("Córdoba");
                result[1].Coordinadores.Should().ContainSingle();
                result[1].Coordinadores[0].Should().Be("Coordinador1-FirstName Coordinador1-LastName");
                result[1].QtyMediadores.Should().Be(1);
                result[1].QtyRevisores.Should().Be(1);
                result[1].QtyPadrinos.Should().Be(1);
                result[1].QtyBecarios.Should().Be(1);
                result[1].LastUpdatedOnUtc.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetAllFilialesForSelectionAsync_ReturnsAllFilialesOrderedByName()
        {
            // Arrange
            var testDatabase = new TestDatabase();
            
            // Act
            var result = await GetSut(testDatabase).GetAllFilialesForSelectionAsync();

            // Assert
            using (new AssertionScope())
            {
                result.Should().HaveCount(2);

                result[0].Id.Should().Be(2);
                result[0].Name.Should().Be("Córdoba");

                result[1].Id.Should().Be(1);
                result[1].Name.Should().Be("Default");
            }
        }

        [Theory]
        [InlineData(1, "Default")]
        [InlineData(2, "Córdoba")]
        public async Task GetFilialNameAsync_ValidId(int filialId, string filialName)
        {
            // Arrange
            var testDatabase = new TestDatabase();
            
            // Act
            var result = await GetSut(testDatabase).GetFilialNameAsync(filialId);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull()
                    .And.Be(filialName);
            }
        }

        [Fact]
        public async Task GetFilialNameAsync_InvalidId()
        {
            // Arrange
            var testDatabase = new TestDatabase();

            // Act
            var result = await GetSut(testDatabase).GetFilialNameAsync(99);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateFilialAsync_Success()
        {
            // Arrange
            var testDatabase = new TestDatabase();

            var filial = new Filial
            {
                Name = "Jujuy",
                CreatedOnUtc = new DateTime(2020, 1, 3),
            };

            // Act
            var result = await GetSut(testDatabase).CreateFilialAsync(filial);

            var filialName = await GetSut(testDatabase).GetFilialNameAsync(3);

            // Assert
            using (new AssertionScope())
            {
                result.Should().Be(1);
                filialName.Should().NotBeNull()
                    .And.Be("Jujuy");
            }
        }

        [Fact]
        public async Task UpdateFilialAsync_Success()
        {
            // Arrange
            var testDatabase = new TestDatabase();

            // Act
            var result = await GetSut(testDatabase).UpdateFilialAsync(1, "Buenos Aires");

            var filialName = await GetSut(testDatabase).GetFilialNameAsync(1);

            // Assert
            using (new AssertionScope())
            {
                result.Should().Be(1);
                filialName.Should().NotBeNull()
                    .And.Be("Buenos Aires");
            }
        }

        private static FilialesRepository GetSut(TestDatabase testDatabase)
        {
            var appDbContextFactory = testDatabase.GetAppDbContextFactory();
            var filialesRepository = new FilialesRepository(appDbContextFactory);
            return filialesRepository;
        }
    }
}
