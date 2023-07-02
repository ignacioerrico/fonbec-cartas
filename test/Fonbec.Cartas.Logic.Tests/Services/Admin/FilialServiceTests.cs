using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.DataModels.Admin;
using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Repositories.Admin;
using Fonbec.Cartas.Logic.Services.Admin;
using Moq;

namespace Fonbec.Cartas.Logic.Tests.Services.Admin
{
    public class FilialServiceTests
    {
        private readonly Mock<IFilialesRepository> _filialesRepository = new();
        private readonly FilialService _sut;

        public FilialServiceTests()
        {
            _sut = new FilialService(_filialesRepository.Object);
        }

        [Fact]
        public async Task ShouldReturnAllFilialesAsFilialListViewModels_When_GetAllFilialesAsync_IsCalled()
        {
            // Arrange
            var filiales = GetFiliales();

            _filialesRepository
                .Setup(x => x.GetAllFilialesAsync())
                .ReturnsAsync(filiales);

            // Act
            var result = await _sut.GetAllFilialesAsync();

            // Assert
            using (new AssertionScope())
            {
                result.Count.Should().Be(3);
                
                result[0].FilialId.Should().Be(1);
                result[0].FilialName.Should().Be("Filial-1");
                result[0].LastUpdatedOnUtc.Should().BeNull();

                result[1].FilialId.Should().Be(2);
                result[1].FilialName.Should().Be("Filial-2");
                result[1].LastUpdatedOnUtc.Should().NotBeNull();

                result[2].FilialId.Should().Be(3);
                result[2].FilialName.Should().Be("Filial-3");
                result[2].LastUpdatedOnUtc.Should().BeNull();
            }
        }

        [Fact]
        public async Task ShouldReturnAllFilialesAsFilialViewModels_When_GetAllFilialesAsync_IsCalled()
        {
            // Arrange
            var filiales = GetFiliales();

            _filialesRepository
                .Setup(x => x.GetAllFilialesAsync())
                .ReturnsAsync(filiales);

            // Act
            var result = await _sut.GetAllFilialesForSelectionAsync();

            // Assert
            using (new AssertionScope())
            {
                result.Count.Should().Be(3);

                result[0].Id.Should().Be(1);
                result[0].Name.Should().Be("Filial-1");

                result[1].Id.Should().Be(2);
                result[1].Name.Should().Be("Filial-2");

                result[2].Id.Should().Be(3);
                result[2].Name.Should().Be("Filial-3");
            }
        }

        [Fact]
        public async Task ShouldReturnFilialName_When_GetFilialNameAsync_IsCalled_And_IdCorrespondsToAnExistingFilial()
        {
            _filialesRepository
                .Setup(x => x.GetFilialNameAsync(3))
                .ReturnsAsync("Filial-3");

            // Act
            var result = await _sut.GetFilialNameAsync(3);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().Be("Filial-3");
            }
        }

        [Fact]
        public async Task ShouldReturnNull_When_GetFilialNameAsync_IsCalled_And_IdDoesNotCorrespondToAnExistingFilial()
        {
            _filialesRepository
                .Setup(x => x.GetFilialNameAsync(3));

            // Act
            var result = await _sut.GetFilialNameAsync(3);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ShouldReturn_1_When_CreateFilialAsync_IsCalled_And_FilialIsCreated()
        {
            _filialesRepository
                .Setup(x => x.CreateFilialAsync(It.Is<Filial>(f => f.Name == "Filial-3")))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFilialAsync("Filial-3");

            // Assert
            result.Should().Be(1);
        }

        [Fact]
        public async Task ShouldReturn_1_When_UpdateFilialAsync_IsCalled_And_FilialIsUpdated()
        {
            _filialesRepository
                .Setup(x => x.UpdateFilialAsync(3, "Filial-3"))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateFilialAsync(3, "Filial-3");

            // Assert
            result.Should().Be(1);
        }

        [Fact]
        public async Task ShouldReturn_1_When_SoftDeleteAsync_IsCalled_And_FilialIsSoftDeleted()
        {
            _filialesRepository
                .Setup(x => x.SoftDeleteAsync(3))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.SoftDeleteAsync(3);

            // Assert
            result.Should().Be(1);
        }

        private static List<FilialesListDataModel> GetFiliales()
        {
            var filiales = new List<FilialesListDataModel>
            {
                new()
                {
                    FilialId = 1,
                    FilialName = "Filial-1",
                    CreatedOnUtc = DateTimeOffset.UtcNow,
                    LastUpdatedOnUtc = null,
                },
                new()
                {
                    FilialId = 2,
                    FilialName = "Filial-2",
                    CreatedOnUtc = DateTimeOffset.UtcNow,
                    LastUpdatedOnUtc = DateTimeOffset.UtcNow,
                },
                new()
                {
                    FilialId = 3,
                    FilialName = "Filial-3",
                    CreatedOnUtc = DateTimeOffset.UtcNow,
                    LastUpdatedOnUtc = null,
                },
            };
            return filiales;
        }
    }
}
