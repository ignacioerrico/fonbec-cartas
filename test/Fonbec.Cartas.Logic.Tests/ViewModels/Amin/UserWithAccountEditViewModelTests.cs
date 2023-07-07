using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Actors.Abstract;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Logic.ViewModels.Admin;
using Mapster;

namespace Fonbec.Cartas.Logic.Tests.ViewModels.Amin
{
    public abstract class UserWithAccountEditViewModelTests<T> : MapsterTests
        where T : UserWithAccount, new()
    {
        [Theory]
        [InlineData("Nickname", "Phone", "Nickname", "Phone")]
        [InlineData("", "", "", "")]
        [InlineData(null, null, "", "")]
        protected void Map_UserWithAccount_To_UserWithAccountEditViewModel(string? actualNickName, string? actualPhone, string expectedNickName, string expectedPhone)
        {
            // Arrange
            var userWithAccount = new T
            {
                Filial = new Filial { Id = 42 },
                FirstName = "FirstName",
                LastName = "LastName",
                NickName = actualNickName,
                Gender = Gender.Female,
                Email = "Email",
                Phone = actualPhone,
                Username = "Username",
                AspNetUserId = "AspNetUserId"
            };

            // Act
            var result = userWithAccount.Adapt<T, UserWithAccountEditViewModel>();

            // Assert
            using (new AssertionScope())
            {
                result.FilialId.Should().Be(42);
                result.FirstName.Should().Be("FirstName");
                result.LastName.Should().Be("LastName");
                result.NickName.Should().Be(expectedNickName);
                result.Gender.Should().Be(Gender.Female);
                result.Email.Should().Be("Email");
                result.Phone.Should().Be(expectedPhone);
                result.Username.Should().Be("Username");
                result.AspNetUserId.Should().Be("AspNetUserId");
            }
        }

        [Theory]
        [InlineData("Nickname", "Phone", "Nickname", "Phone")]
        [InlineData("", "", null, null)]
        protected void Map_UserWithAccountEditViewModel_To_UserWithAccount_UsingParameter(string actualNickName, string actualPhone, string? expectedNickName, string? expectedPhone)
        {
            // Arrange
            var userWithAccountEditViewModel = new UserWithAccountEditViewModel
            {
                FilialId = 42,
                FirstName = "FirstName",
                LastName = "LastName",
                NickName = actualNickName,
                Gender = Gender.Female,
                Email = "Email",
                Phone = actualPhone,
                AspNetUserId = "AspNetUserId"
            };

            // Act
            var result = userWithAccountEditViewModel.BuildAdapter()
                .AddParameters("userId", "Username")
                .AdaptToType<T>();

            // Assert
            using (new AssertionScope())
            {
                result.FilialId.Should().Be(42);
                result.FirstName.Should().Be("FirstName");
                result.LastName.Should().Be("LastName");
                result.NickName.Should().Be(expectedNickName);
                result.Gender.Should().Be(Gender.Female);
                result.Email.Should().Be("Email");
                result.Phone.Should().Be(expectedPhone);
                result.Username.Should().Be("Username");
                result.AspNetUserId.Should().Be("AspNetUserId");
            }
        }

        [Fact]
        protected void Map_UserWithAccountEditViewModel_To_UserWithAccount_NoParameter()
        {
            // Arrange
            var userWithAccountEditViewModel = new UserWithAccountEditViewModel
            {
                FilialId = 42,
                FirstName = "FirstName",
                LastName = "LastName",
                NickName = string.Empty,
                Gender = Gender.Female,
                Email = "Email",
                Phone = string.Empty,
                AspNetUserId = "AspNetUserId"
            };

            // Act
            var result = userWithAccountEditViewModel.Adapt<UserWithAccountEditViewModel, T>();

            // Assert
            using (new AssertionScope())
            {
                result.FilialId.Should().Be(42);
                result.FirstName.Should().Be("FirstName");
                result.LastName.Should().Be("LastName");
                result.NickName.Should().BeNull();
                result.Gender.Should().Be(Gender.Female);
                result.Email.Should().Be("Email");
                result.Phone.Should().BeNull();
                result.Username.Should().BeEmpty();
                result.AspNetUserId.Should().Be("AspNetUserId");
            }
        }
    }

    public class CoordinadorUserWithAccountEditViewModelTests : UserWithAccountEditViewModelTests<DataAccess.Entities.Actors.Coordinador>
    {
    }
    
    public class MediadorUserWithAccountEditViewModelTests : UserWithAccountEditViewModelTests<Mediador>
    {
    }
    
    public class RevisorUserWithAccountEditViewModelTests : UserWithAccountEditViewModelTests<Revisor>
    {
    }
}
