using System.Reflection;
using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Actors.Abstract;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.DataAccess.Identity;
using Fonbec.Cartas.DataAccess.Repositories.Admin;
using Fonbec.Cartas.Logic.Services.Admin;
using Fonbec.Cartas.Logic.Tests.ViewModels;
using Fonbec.Cartas.Logic.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Fonbec.Cartas.Logic.Tests.Services.Admin
{
    public abstract class UserWithAccountServiceTests<TService, T> : MapsterTests
        where TService : IUserWithAccountService<T>
        where T : UserWithAccount, new()
    {
        private readonly TService _sut;

        private readonly Mock<IUserWithAccountRepositoryBase<T>> _userWithAccountRepositoryMock = new();
        private readonly Mock<UserManager<FonbecUser>> _userManagerMock;

        private readonly DateTimeOffset _createdOnUtc = DateTimeOffset.Now;

        protected UserWithAccountServiceTests()
        {
            var userStore = new UserStore<FonbecUser>(Mock.Of<DbContext>(), Mock.Of<IdentityErrorDescriber>());
            
            _userManagerMock = new(userStore,
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<FonbecUser>>(),
                Enumerable.Empty<IUserValidator<FonbecUser>>(),
                Enumerable.Empty<IPasswordValidator<FonbecUser>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<FonbecUser>>>());

            _sut = (TService)Activator.CreateInstance(typeof(TService),
                _userWithAccountRepositoryMock.Object,
                _userManagerMock.Object,
                userStore)!;
        }

        [Fact]
        public async Task GetAllFilialesForSelectionAsync_ReturnsAllFilialesAsSelectableModel()
        {
            // Arrange
            var filiales = new List<Filial>
            {
                new() { Id = 1, Name = "Filial-1" },
                new() { Id = 2, Name = "Filial-2" },
                new() { Id = 3, Name = "Filial-3" },
            };

            _userWithAccountRepositoryMock
                .Setup(x => x.GetAllFilialesAsync())
                .ReturnsAsync(filiales);

            // Act
            var result = await _sut.GetAllFilialesAsSelectableAsync();

            // Assert
            using (new AssertionScope())
            {
                result.Should().HaveCount(3);

                result[0].Id.Should().Be(1);
                result[0].DisplayName.Should().Be("Filial-1");

                result[1].Id.Should().Be(2);
                result[1].DisplayName.Should().Be("Filial-2");

                result[2].Id.Should().Be(3);
                result[2].DisplayName.Should().Be("Filial-3");
            }
        }

        [Fact]
        public async Task GetAllAsync_Success()
        {
            // Arrange
            var usersWithAccount = GetTs();

            _userWithAccountRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(usersWithAccount);

            // Act
            var result = await _sut.GetAllUsersWithAccountAsync();

            // Assert
            using (new AssertionScope())
            {
                result.Should().HaveCount(4);

                result[0].UserWithAccountId.Should().Be(42);
                result[0].UserWithAccountFullName.Should().Be("FirstName-1 LastName-1");
                result[0].Gender.Should().Be(Gender.Female);
                result[0].FilialName.Should().Be("Filial-1");
                result[0].Email.Should().Be("Email-1");
                result[0].Phone.Should().BeEmpty();
                result[0].Username.Should().Be("Username-1");
                result[0].CreatedOnUtc.Should().Be(_createdOnUtc);
                result[0].LastUpdatedOnUtc.Should().NotBeNull();
                result[0].LastUpdatedOnUtc.Should().Be(_createdOnUtc.AddDays(1));

                result[1].UserWithAccountId.Should().Be(43);
                result[1].UserWithAccountFullName.Should().Be("FirstName-2 LastName-2 (\"NickName\")");
                result[1].Gender.Should().Be(Gender.Male);
                result[1].FilialName.Should().Be("Filial-2");
                result[1].Email.Should().Be("Email-2");
                result[1].Phone.Should().BeEmpty();
                result[1].Username.Should().Be("Username-2");
                result[1].CreatedOnUtc.Should().Be(_createdOnUtc);
                result[1].LastUpdatedOnUtc.Should().BeNull();

                result[2].UserWithAccountId.Should().Be(44);
                result[2].UserWithAccountFullName.Should().Be("FirstName-3 LastName-3");
                result[2].Gender.Should().Be(Gender.Unknown);
                result[2].FilialName.Should().Be("Filial-3");
                result[2].Email.Should().Be("Email-3");
                result[2].Phone.Should().Be("Phone");
                result[2].Username.Should().Be("Username-3");
                result[2].CreatedOnUtc.Should().Be(_createdOnUtc);
                result[2].LastUpdatedOnUtc.Should().NotBeNull();
                result[2].LastUpdatedOnUtc.Should().Be(_createdOnUtc.AddDays(2));

                result[3].UserWithAccountId.Should().Be(45);
                result[3].UserWithAccountFullName.Should().Be("FirstName-4 LastName-4 (\"NickName\")");
                result[3].Gender.Should().Be(Gender.Female);
                result[3].FilialName.Should().Be("Filial-4");
                result[3].Email.Should().Be("Email-4");
                result[3].Phone.Should().Be("Phone");
                result[3].Username.Should().Be("Username-4");
                result[3].CreatedOnUtc.Should().Be(_createdOnUtc);
                result[3].LastUpdatedOnUtc.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetAsync_Found()
        {
            // Arrange
            var userWithAccount = GetTs().First();

            _userWithAccountRepositoryMock
                .Setup(x => x.GetAsync(42))
                .ReturnsAsync(userWithAccount);

            // Act
            var result = await _sut.GetUserWithAccountAsync(42);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();

                result.IsFound.Should().BeTrue();

                result.Data.Should().NotBeNull();
                result.Data!.FilialId.Should().Be(123);
                result.Data.FirstName.Should().Be("FirstName-1");
                result.Data.LastName.Should().Be("LastName-1");
                result.Data.NickName.Should().BeEmpty();
                result.Data.Gender.Should().Be(Gender.Female);
                result.Data.Email.Should().Be("Email-1");
                result.Data.Phone.Should().BeEmpty();
                result.Data.Username.Should().Be("Username-1");
                result.Data.AspNetUserId.Should().Be("AspNetUserId-1");
            }
        }

        [Fact]
        public async Task GetAsync_NotFound()
        {
            // Arrange
            _userWithAccountRepositoryMock
                .Setup(x => x.GetAsync(42))
                .ReturnsAsync((T?)null);

            // Act
            var result = await _sut.GetUserWithAccountAsync(42);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();

                result.IsFound.Should().BeFalse();
            }
        }

        [Fact]
        public async Task CreateAsync_Success()
        {
            // Arrange
            var identityResultSuccess = GetIdentityResult(isSuccess: true);

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<FonbecUser>(), "Password-1"))
                .ReturnsAsync(identityResultSuccess);

            _userManagerMock
                .Setup(x => x.AddToRoleAsync(It.IsAny<FonbecUser>(), typeof(T).Name))
                .ReturnsAsync(identityResultSuccess);

            _userManagerMock
                .Setup(x => x.GetUserIdAsync(It.IsAny<FonbecUser>()))
                .ReturnsAsync("Username-1");

            _userWithAccountRepositoryMock
                .Setup(x => x.CreateAsync(It.Is<T>(t =>
                    t.FilialId == 42
                    && t.FirstName == "FirstName-1"
                    && t.LastName == "LastName-1"
                    && t.NickName == "NickName-1"
                    && t.Gender == Gender.Female
                    && t.Email == "Email-1"
                    && t.Phone == "Phone-1"
                    && t.Username == "Username-1")))
                .ReturnsAsync(1);

            var userWithAccountEditViewModel = new UserWithAccountEditViewModel
            {
                FilialId = 42,
                FirstName = "FirstName-1",
                LastName = "LastName-1",
                NickName = "NickName-1",
                Gender = Gender.Female,
                Email = "Email-1",
                Phone = "Phone-1",
                Username = "Username-1",
                AspNetUserId = "AspNetUserId-1"
            };

            // Act
            var result = await _sut.CreateUserWithAccountAsync(userWithAccountEditViewModel, "Password-1");

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();

                result.AnyRowsAffected.Should().BeTrue();
                result.RowsAffected.Should().Be(1);

                result.AnyErrors.Should().BeFalse();
                result.Errors.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task CreateAsync_FailsToCreateUsersAccount()
        {
            // Arrange
            var identityResultFailed = GetIdentityResult(isSuccess: false, new [] { "Failed to create user's account" });

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<FonbecUser>(), "Password-1"))
                .ReturnsAsync(identityResultFailed);

            var userWithAccountEditViewModel = new UserWithAccountEditViewModel
            {
                FilialId = 42,
                FirstName = "FirstName-1",
                LastName = "LastName-1",
                NickName = "NickName-1",
                Gender = Gender.Female,
                Email = "Email-1",
                Phone = "Phone-1",
                Username = "Username-1",
                AspNetUserId = "AspNetUserId-1"
            };

            // Act
            var result = await _sut.CreateUserWithAccountAsync(userWithAccountEditViewModel, "Password-1");

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();

                result.AnyRowsAffected.Should().BeFalse();
                result.RowsAffected.Should().Be(0);

                result.AnyErrors.Should().BeTrue();
                result.Errors.Should().NotBeEmpty();
                result.Errors[0].Should().Be("Failed to create user's account");
            }
        }

        [Fact]
        public async Task CreateAsync_FailsToAddUserToRole()
        {
            // Arrange
            var identityResultSuccess = GetIdentityResult(isSuccess: true);
            
            var identityResultFailed = GetIdentityResult(isSuccess: false, new[] { "Failed to add user to role" });

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<FonbecUser>(), "Password-1"))
                .ReturnsAsync(identityResultSuccess);

            _userManagerMock
                .Setup(x => x.AddToRoleAsync(It.IsAny<FonbecUser>(), typeof(T).Name))
                .ReturnsAsync(identityResultFailed);

            var userWithAccountEditViewModel = new UserWithAccountEditViewModel
            {
                FilialId = 42,
                FirstName = "FirstName-1",
                LastName = "LastName-1",
                NickName = "NickName-1",
                Gender = Gender.Female,
                Email = "Email-1",
                Phone = "Phone-1",
                Username = "Username-1",
                AspNetUserId = "AspNetUserId-1"
            };

            // Act
            var result = await _sut.CreateUserWithAccountAsync(userWithAccountEditViewModel, "Password-1");

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();

                result.AnyRowsAffected.Should().BeFalse();
                result.RowsAffected.Should().Be(0);

                result.AnyErrors.Should().BeTrue();
                result.Errors.Should().NotBeEmpty();
                result.Errors[0].Should().Be("Failed to add user to role");
            }
        }

        [Fact]
        public async Task UpdateAsync_Success_NeitherUsernameNorEmailChanged()
        {
            // Arrange
            _userWithAccountRepositoryMock
                .Setup(x => x.UpdateAsync(42,
                    It.Is<T>(t =>
                        t.FilialId == 42
                        && t.FirstName == "FirstName-1"
                        && t.LastName == "LastName-1"
                        && t.NickName == "NickName-1"
                        && t.Gender == Gender.Female
                        && t.Email == "Email-1"
                        && t.Phone == "Phone-1"
                        && t.Username == "Username-1")))
                .ReturnsAsync(1);

            var fonbecUser = new FonbecUser
            {
                UserName = "Username-1",
                Email = "Email-1",
            };

            _userManagerMock
                .Setup(x => x.FindByIdAsync("AspNetUserId-1"))
                .ReturnsAsync(fonbecUser);

            var userWithAccountEditViewModel = new UserWithAccountEditViewModel
            {
                FilialId = 42,
                FirstName = "FirstName-1",
                LastName = "LastName-1",
                NickName = "NickName-1",
                Gender = Gender.Female,
                Email = "Email-1",
                Phone = "Phone-1",
                Username = "Username-1",
                AspNetUserId = "AspNetUserId-1"
            };

            // Act
            var result = await _sut.UpdateUserWithAccountAsync(42, userWithAccountEditViewModel);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();

                result.AnyRowsAffected.Should().BeTrue();
                result.RowsAffected.Should().Be(1);

                result.AnyErrors.Should().BeFalse();
                result.Errors.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task UpdateAsync_Success_OnlyUsernameChanged()
        {
            // Arrange
            _userWithAccountRepositoryMock
                .Setup(x => x.UpdateAsync(42,
                    It.Is<T>(t =>
                        t.FilialId == 42
                        && t.FirstName == "FirstName-1"
                        && t.LastName == "LastName-1"
                        && t.NickName == "NickName-1"
                        && t.Gender == Gender.Female
                        && t.Email == "Email-1"
                        && t.Phone == "Phone-1"
                        && t.Username == "Username-Changed")))
                .ReturnsAsync(1);

            var fonbecUser = new FonbecUser
            {
                UserName = "Username-1",
                Email = "Email-1",
            };

            _userManagerMock
                .Setup(x => x.FindByIdAsync("AspNetUserId-1"))
                .ReturnsAsync(fonbecUser);

            var identityResultSuccess = GetIdentityResult(isSuccess: true);

            _userManagerMock
                .Setup(x => x.UpdateAsync(It.Is<FonbecUser>(u =>
                    u.UserName == "Username-Changed"
                    && u.Email == "Email-1")))
                .ReturnsAsync(identityResultSuccess);

            var userWithAccountEditViewModel = new UserWithAccountEditViewModel
            {
                FilialId = 42,
                FirstName = "FirstName-1",
                LastName = "LastName-1",
                NickName = "NickName-1",
                Gender = Gender.Female,
                Email = "Email-1",
                Phone = "Phone-1",
                Username = "Username-Changed",
                AspNetUserId = "AspNetUserId-1"
            };

            // Act
            var result = await _sut.UpdateUserWithAccountAsync(42, userWithAccountEditViewModel);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();

                result.AnyRowsAffected.Should().BeTrue();
                result.RowsAffected.Should().Be(1);

                result.AnyErrors.Should().BeFalse();
                result.Errors.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task UpdateAsync_Success_OnlyEmailChanged()
        {
            // Arrange
            _userWithAccountRepositoryMock
                .Setup(x => x.UpdateAsync(42,
                    It.Is<T>(t =>
                        t.FilialId == 42
                        && t.FirstName == "FirstName-1"
                        && t.LastName == "LastName-1"
                        && t.NickName == "NickName-1"
                        && t.Gender == Gender.Female
                        && t.Email == "Email-Changed"
                        && t.Phone == "Phone-1"
                        && t.Username == "Username-1")))
                .ReturnsAsync(1);

            var fonbecUser = new FonbecUser
            {
                UserName = "Username-1",
                Email = "Email-1",
            };

            _userManagerMock
                .Setup(x => x.FindByIdAsync("AspNetUserId-1"))
                .ReturnsAsync(fonbecUser);

            var identityResultSuccess = GetIdentityResult(isSuccess: true);

            _userManagerMock
                .Setup(x => x.UpdateAsync(It.Is<FonbecUser>(u =>
                    u.UserName == "Username-1"
                    && u.Email == "Email-Changed")))
                .ReturnsAsync(identityResultSuccess);

            var userWithAccountEditViewModel = new UserWithAccountEditViewModel
            {
                FilialId = 42,
                FirstName = "FirstName-1",
                LastName = "LastName-1",
                NickName = "NickName-1",
                Gender = Gender.Female,
                Email = "Email-Changed",
                Phone = "Phone-1",
                Username = "Username-1",
                AspNetUserId = "AspNetUserId-1"
            };

            // Act
            var result = await _sut.UpdateUserWithAccountAsync(42, userWithAccountEditViewModel);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();

                result.AnyRowsAffected.Should().BeTrue();
                result.RowsAffected.Should().Be(1);

                result.AnyErrors.Should().BeFalse();
                result.Errors.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task UpdateAsync_Success_BothUsernameAndEmailChanged()
        {
            // Arrange
            _userWithAccountRepositoryMock
                .Setup(x => x.UpdateAsync(42,
                    It.Is<T>(t =>
                        t.FilialId == 42
                        && t.FirstName == "FirstName-1"
                        && t.LastName == "LastName-1"
                        && t.NickName == "NickName-1"
                        && t.Gender == Gender.Female
                        && t.Email == "Email-Changed"
                        && t.Phone == "Phone-1"
                        && t.Username == "Username-Changed")))
                .ReturnsAsync(1);

            var fonbecUser = new FonbecUser
            {
                UserName = "Username-1",
                Email = "Email-1",
            };

            _userManagerMock
                .Setup(x => x.FindByIdAsync("AspNetUserId-1"))
                .ReturnsAsync(fonbecUser);

            var identityResultSuccess = GetIdentityResult(isSuccess: true);

            _userManagerMock
                .Setup(x => x.UpdateAsync(It.Is<FonbecUser>(u =>
                    u.UserName == "Username-Changed"
                    && u.Email == "Email-Changed")))
                .ReturnsAsync(identityResultSuccess);

            var userWithAccountEditViewModel = new UserWithAccountEditViewModel
            {
                FilialId = 42,
                FirstName = "FirstName-1",
                LastName = "LastName-1",
                NickName = "NickName-1",
                Gender = Gender.Female,
                Email = "Email-Changed",
                Phone = "Phone-1",
                Username = "Username-Changed",
                AspNetUserId = "AspNetUserId-1"
            };

            // Act
            var result = await _sut.UpdateUserWithAccountAsync(42, userWithAccountEditViewModel);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();

                result.AnyRowsAffected.Should().BeTrue();
                result.RowsAffected.Should().Be(1);

                result.AnyErrors.Should().BeFalse();
                result.Errors.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task UpdateAsync_FailsToUpdateUserWithAccount()
        {
            // Arrange
            _userWithAccountRepositoryMock
                .Setup(x => x.UpdateAsync(42,
                    It.Is<T>(t =>
                        t.FilialId == 42
                        && t.FirstName == "FirstName-1"
                        && t.LastName == "LastName-1"
                        && t.NickName == "NickName-1"
                        && t.Gender == Gender.Female
                        && t.Email == "Email-Changed"
                        && t.Phone == "Phone-1"
                        && t.Username == "Username-Changed")))
                .ReturnsAsync(0);

            var userWithAccountEditViewModel = new UserWithAccountEditViewModel
            {
                FilialId = 42,
                FirstName = "FirstName-1",
                LastName = "LastName-1",
                NickName = "NickName-1",
                Gender = Gender.Female,
                Email = "Email-Changed",
                Phone = "Phone-1",
                Username = "Username-Changed",
                AspNetUserId = "AspNetUserId-1"
            };

            // Act
            var result = await _sut.UpdateUserWithAccountAsync(42, userWithAccountEditViewModel);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();

                result.AnyRowsAffected.Should().BeFalse();
                result.RowsAffected.Should().Be(0);

                result.AnyErrors.Should().BeTrue();
                result.Errors.Should().NotBeEmpty();
                result.Errors[0].Should().Be($"No se pudo actualizar el {typeof(T).Name}.");
            }
        }

        [Fact]
        public async Task UpdateAsync_FailsToFindIdentityUser()
        {
            // Arrange
            _userWithAccountRepositoryMock
                .Setup(x => x.UpdateAsync(42,
                    It.Is<T>(t =>
                        t.FilialId == 42
                        && t.FirstName == "FirstName-1"
                        && t.LastName == "LastName-1"
                        && t.NickName == "NickName-1"
                        && t.Gender == Gender.Female
                        && t.Email == "Email-Changed"
                        && t.Phone == "Phone-1"
                        && t.Username == "Username-Changed")))
                .ReturnsAsync(1);

            _userManagerMock
                .Setup(x => x.FindByIdAsync("AspNetUserId-1"))
                .ReturnsAsync((FonbecUser?)null);

            var userWithAccountEditViewModel = new UserWithAccountEditViewModel
            {
                FilialId = 42,
                FirstName = "FirstName-1",
                LastName = "LastName-1",
                NickName = "NickName-1",
                Gender = Gender.Female,
                Email = "Email-Changed",
                Phone = "Phone-1",
                Username = "Username-Changed",
                AspNetUserId = "AspNetUserId-1"
            };

            // Act
            var result = await _sut.UpdateUserWithAccountAsync(42, userWithAccountEditViewModel);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();

                result.AnyRowsAffected.Should().BeFalse();
                result.RowsAffected.Should().Be(0);

                result.AnyErrors.Should().BeTrue();
                result.Errors.Should().NotBeEmpty();
                result.Errors[0].Should().Be($"Se actualizó el {typeof(T).Name}, pero no se encontró principal con ID AspNetUserId-1.");
            }
        }

        [Fact]
        public async Task UpdateAsync_FailsToUpdateIdentityUser()
        {
            // Arrange
            _userWithAccountRepositoryMock
                .Setup(x => x.UpdateAsync(42,
                    It.Is<T>(t =>
                        t.FilialId == 42
                        && t.FirstName == "FirstName-1"
                        && t.LastName == "LastName-1"
                        && t.NickName == "NickName-1"
                        && t.Gender == Gender.Female
                        && t.Email == "Email-Changed"
                        && t.Phone == "Phone-1"
                        && t.Username == "Username-Changed")))
                .ReturnsAsync(1);

            var fonbecUser = new FonbecUser
            {
                UserName = "Username-1",
                Email = "Email-1",
            };

            _userManagerMock
                .Setup(x => x.FindByIdAsync("AspNetUserId-1"))
                .ReturnsAsync(fonbecUser);

            var identityResultFailed = GetIdentityResult(isSuccess: false, new [] { "Failed to update Identity User" });

            _userManagerMock
                .Setup(x => x.UpdateAsync(It.Is<FonbecUser>(u =>
                    u.UserName == "Username-Changed"
                    && u.Email == "Email-Changed")))
                .ReturnsAsync(identityResultFailed);

            var userWithAccountEditViewModel = new UserWithAccountEditViewModel
            {
                FilialId = 42,
                FirstName = "FirstName-1",
                LastName = "LastName-1",
                NickName = "NickName-1",
                Gender = Gender.Female,
                Email = "Email-Changed",
                Phone = "Phone-1",
                Username = "Username-Changed",
                AspNetUserId = "AspNetUserId-1"
            };

            // Act
            var result = await _sut.UpdateUserWithAccountAsync(42, userWithAccountEditViewModel);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();

                result.AnyRowsAffected.Should().BeFalse();
                result.RowsAffected.Should().Be(0);

                result.AnyErrors.Should().BeTrue();
                result.Errors.Should().NotBeEmpty();
                result.Errors.Should().HaveCount(2);
                result.Errors[0].Should().Be($"Se actualizó el {typeof(T).Name}, pero no se pudo actualizar su identidad.");
                result.Errors[1].Should().Be("Failed to update Identity User");
            }
        }

        private List<T> GetTs()
        {
            var usersWithAccount = new List<T>
            {
                new()
                {
                    Id = 42,
                    FilialId = 78,
                    Filial = new() { Id = 123, Name = "Filial-1" },
                    FirstName = "FirstName-1",
                    LastName = "LastName-1",
                    NickName = null,
                    Gender = Gender.Female,
                    Phone = null,
                    AspNetUserId = "AspNetUserId-1",
                    Username = "Username-1",
                    Email = "Email-1",
                    CreatedOnUtc = _createdOnUtc,
                    LastUpdatedOnUtc = _createdOnUtc.AddDays(1),
                },
                new()
                {
                    Id = 43,
                    FilialId = 79,
                    Filial = new() { Id = 124, Name = "Filial-2" },
                    FirstName = "FirstName-2",
                    LastName = "LastName-2",
                    NickName = "NickName",
                    Gender = Gender.Male,
                    Phone = null,
                    AspNetUserId = "AspNetUserId-2",
                    Username = "Username-2",
                    Email = "Email-2",
                    CreatedOnUtc = _createdOnUtc,
                },
                new()
                {
                    Id = 44,
                    FilialId = 80,
                    Filial = new() { Id = 125, Name = "Filial-3" },
                    FirstName = "FirstName-3",
                    LastName = "LastName-3",
                    NickName = null,
                    Gender = Gender.Unknown,
                    Phone = "Phone",
                    AspNetUserId = "AspNetUserId-3",
                    Username = "Username-3",
                    Email = "Email-3",
                    CreatedOnUtc = _createdOnUtc,
                    LastUpdatedOnUtc = _createdOnUtc.AddDays(2),
                },
                new()
                {
                    Id = 45,
                    FilialId = 81,
                    Filial = new() { Id = 126, Name = "Filial-4" },
                    FirstName = "FirstName-4",
                    LastName = "LastName-4",
                    NickName = "NickName",
                    Gender = Gender.Female,
                    Phone = "Phone",
                    AspNetUserId = "AspNetUserId-4",
                    Username = "Username-4",
                    Email = "Email-4",
                    CreatedOnUtc = _createdOnUtc,
                },
            };
            return usersWithAccount;
        }

        private IdentityResult GetIdentityResult(bool isSuccess, string[]? errors = null)
        {
            if (!isSuccess && (errors is null || !errors.Any()))
            {
                throw new ArgumentException($"{nameof(errors)} must have at least one item");
            }

            var identityResult = new IdentityResult();

            // Set the Succeeded protected property to true
            var propertySucceeded = identityResult.GetType().GetProperty("Succeeded", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            propertySucceeded.Should().NotBeNull();
            propertySucceeded!.SetValue(identityResult, isSuccess);

            if (!isSuccess)
            {
                // Set the Errors private property to a single error
                var propertyErrors = identityResult.GetType().GetProperty("Errors", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                propertyErrors.Should().NotBeNull();
                var identityResultErrors = propertyErrors!.GetValue(identityResult) as List<IdentityError>;
                identityResultErrors.Should().NotBeNull();
                identityResultErrors!.AddRange(errors!.Select(e => new IdentityError { Description = e}));
            }

            return identityResult;
        }
    }

    public class CoordinadorServiceTests : UserWithAccountServiceTests<CoordinadorService, Coordinador>
    {
    }
    
    public class MediadorServiceTests : UserWithAccountServiceTests<MediadorService, Mediador>
    {
    }
    
    public class RevisorServiceTests : UserWithAccountServiceTests<RevisorService, Revisor>
    {
    }
}
