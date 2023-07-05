using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.DataModels.Admin;
using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Repositories.Admin;
using Fonbec.Cartas.Logic.Services.Admin;
using Fonbec.Cartas.Logic.Tests.ViewModels;
using Moq;

namespace Fonbec.Cartas.Logic.Tests.Services.Admin
{
    public class FilialServiceTests : MapsterTests
    {
        private readonly Mock<IFilialesRepository> _filialesRepositoryMock = new();
        private readonly FilialService _sut;

        public FilialServiceTests()
        {
            _sut = new FilialService(_filialesRepositoryMock.Object);
        }

        [Fact]
        public async Task ShouldReturnAllFilialesAsFilialListViewModels_When_GetAllFilialesAsync_IsCalled()
        {
            // Arrange
            var filiales = GetFiliales();

            _filialesRepositoryMock
                .Setup(x => x.GetAllFilialesAsync())
                .ReturnsAsync(filiales);

            // Act
            var result = await _sut.GetAllFilialesAsync();

            // Assert
            using (new AssertionScope())
            {
                result.Should().HaveCount(3);
                
                result[0].FilialId.Should().Be(1);
                result[0].FilialName.Should().Be("Filial-1");
                result[0].Coordinadores.Should().HaveCount(3);
                result[0].Coordinadores[0].Should().Be("Coordinador-1");
                result[0].Coordinadores[1].Should().Be("Coordinador-2");
                result[0].Coordinadores[2].Should().Be("Coordinador-3");
                result[0].QtyMediadores.Should().Be(3);
                result[0].QtyRevisores.Should().Be(14);
                result[0].QtyPadrinos.Should().Be(15);
                result[0].QtyBecarios.Should().Be(92);
                result[0].LastUpdatedOnUtc.Should().BeNull();

                result[1].FilialId.Should().Be(2);
                result[1].FilialName.Should().Be("Filial-2");
                result[1].Coordinadores.Should().HaveCount(1);
                result[1].Coordinadores[0].Should().Be("Coordinador-4");
                result[1].QtyMediadores.Should().Be(65);
                result[1].QtyRevisores.Should().Be(35);
                result[1].QtyPadrinos.Should().Be(89);
                result[1].QtyBecarios.Should().Be(79);
                result[1].LastUpdatedOnUtc.Should().NotBeNull();

                result[2].FilialId.Should().Be(3);
                result[2].FilialName.Should().Be("Filial-3");
                result[2].Coordinadores.Should().HaveCount(2);
                result[2].Coordinadores[0].Should().Be("Coordinador-5");
                result[2].Coordinadores[1].Should().Be("Coordinador-6");
                result[2].QtyMediadores.Should().Be(32);
                result[2].QtyRevisores.Should().Be(38);
                result[2].QtyPadrinos.Should().Be(46);
                result[2].QtyBecarios.Should().Be(26);
                result[2].LastUpdatedOnUtc.Should().BeNull();
            }
        }

        [Fact]
        public async Task ShouldReturnFilialName_When_GetFilialNameAsync_IsCalled_And_IdCorrespondsToAnExistingFilial()
        {
            _filialesRepositoryMock
                .Setup(x => x.GetFilialNameAsync(3))
                .ReturnsAsync("Filial-3");

            // Act
            var result = await _sut.GetFilialNameAsync(3);

            // Assert
            using (new AssertionScope())
            {
                result.IsFound.Should().BeTrue();
                result.Data.Should().Be("Filial-3");
            }
        }

        [Fact]
        public async Task ShouldReturnNull_When_GetFilialNameAsync_IsCalled_And_IdDoesNotCorrespondToAnExistingFilial()
        {
            _filialesRepositoryMock
                .Setup(x => x.GetFilialNameAsync(3))
                .ReturnsAsync((string?)null);

            // Act
            var result = await _sut.GetFilialNameAsync(3);

            // Assert
            result.IsFound.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldReturn_1_When_CreateFilialAsync_IsCalled_And_FilialIsCreated()
        {
            _filialesRepositoryMock
                .Setup(x => x.CreateFilialAsync(It.Is<Filial>(f => f.Name == "Filial-3")))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFilialAsync("Filial-3");

            // Assert
            using (new AssertionScope())
            {
                result.RowsAffected.Should().Be(1);
                result.AnyRowsAffected.Should().BeTrue();
            }
        }

        [Fact]
        public async Task ShouldReturn_1_When_UpdateFilialAsync_IsCalled_And_FilialIsUpdated()
        {
            _filialesRepositoryMock
                .Setup(x => x.UpdateFilialAsync(3, "Filial-3"))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateFilialAsync(3, "Filial-3");

            // Assert
            using (new AssertionScope())
            {
                result.RowsAffected.Should().Be(1);
                result.AnyRowsAffected.Should().BeTrue();
            }
        }

        [Fact]
        public async Task ShouldReturn_1_When_SoftDeleteAsync_IsCalled_And_FilialIsSoftDeleted()
        {
            _filialesRepositoryMock
                .Setup(x => x.SoftDeleteAsync(3))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.SoftDeleteAsync(3);

            // Assert
            using (new AssertionScope())
            {
                result.RowsAffected.Should().Be(1);
                result.AnyRowsAffected.Should().BeTrue();
            }
        }

        private static List<FilialesListDataModel> GetFiliales()
        {
            var filiales = new List<FilialesListDataModel>
            {
                new()
                {
                    FilialId = 1,
                    FilialName = "Filial-1",
                    Coordinadores = new()
                    {
                        "Coordinador-1",
                        "Coordinador-2",
                        "Coordinador-3",
                    },
                    QtyMediadores = 3,
                    QtyRevisores = 14,
                    QtyPadrinos = 15,
                    QtyBecarios = 92,
                    CreatedOnUtc = DateTimeOffset.UtcNow,
                    LastUpdatedOnUtc = null,
                },
                new()
                {
                    FilialId = 2,
                    FilialName = "Filial-2",
                    Coordinadores = new()
                    {
                        "Coordinador-4",
                    },
                    QtyMediadores = 65,
                    QtyRevisores = 35,
                    QtyPadrinos = 89,
                    QtyBecarios = 79,
                    CreatedOnUtc = DateTimeOffset.UtcNow,
                    LastUpdatedOnUtc = DateTimeOffset.UtcNow,
                },
                new()
                {
                    FilialId = 3,
                    FilialName = "Filial-3",
                    Coordinadores = new()
                    {
                        "Coordinador-5",
                        "Coordinador-6",
                    },
                    QtyMediadores = 32,
                    QtyRevisores = 38,
                    QtyPadrinos = 46,
                    QtyBecarios = 26,
                    CreatedOnUtc = DateTimeOffset.UtcNow,
                    LastUpdatedOnUtc = null,
                },
            };
            return filiales;
        }
    }
}
