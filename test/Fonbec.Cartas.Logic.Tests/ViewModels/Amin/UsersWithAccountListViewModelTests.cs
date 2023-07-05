using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Actors.Abstract;
using Fonbec.Cartas.Logic.ViewModels.Admin;
using Mapster;

namespace Fonbec.Cartas.Logic.Tests.ViewModels.Amin
{
    public abstract class UsersWithAccountListViewModelTests<T> : MapsterTests
        where T : UserWithAccount, new()
    {
        [Theory]
        [InlineData("Nickname", "Phone", "FirstName LastName (\"Nickname\")", "Phone")]
        [InlineData("", "", "FirstName LastName", "")]
        [InlineData(null, null, "FirstName LastName", "")]
        public void Map_UserWithAccount_To_UsersWithAccountListViewModelTests(string? actualNickName, string? actualPhone, string expectedFullName, string expectedPhone)
        {
            // Arrange
            var createdOnUtc = DateTimeOffset.Now;
            var lastUpdatedOnUtc = createdOnUtc.AddMinutes(78);

            var userWithAccount = new T
            {
                Id = 42,
                Filial = new Filial { Name = "FilialName" },
                FirstName = "FirstName",
                LastName = "LastName",
                NickName = actualNickName,
                Email = "Email",
                Phone = actualPhone,
                Username = "Username",
                CreatedOnUtc = createdOnUtc,
                LastUpdatedOnUtc = lastUpdatedOnUtc
            };

            // Act
            var result = userWithAccount.Adapt<T, UsersWithAccountListViewModel>();

            // Assert
            using (new AssertionScope())
            {
                result.UserWithAccountId.Should().Be(42);
                result.UserWithAccountFullName.Should().Be(expectedFullName);
                result.FilialName.Should().Be("FilialName");
                result.Email.Should().Be("Email");
                result.Phone.Should().Be(expectedPhone);
                result.Username.Should().Be("Username");
                result.CreatedOnUtc.Should().Be(createdOnUtc);
                result.LastUpdatedOnUtc.Should().Be(lastUpdatedOnUtc);
            }
        }
    }

    public class CoordinadorUsersWithAccountListViewModelTests : UsersWithAccountListViewModelTests<Coordinador>
    {
    }

    public class MediadorUsersWithAccountListViewModelTests : UsersWithAccountListViewModelTests<Mediador>
    {
    }

    public class RevisorUsersWithAccountListViewModelTests : UsersWithAccountListViewModelTests<Revisor>
    {
    }
}
