using FluentAssertions;
using Fonbec.Cartas.Logic.Models;

namespace Fonbec.Cartas.Logic.Tests.Models
{
    public class RecipientTests
    {
        [Fact]
        public void ShouldUseEmailAddressForDisplayName_WhenCtorCalledWithEmailOnly()
        {
            var sut = new Recipient("john@doe.com");

            sut.EmailAddress.Should().Be("<john@doe.com>");
            sut.DisplayName.Should().Be("john@doe.com");
        }

        [Fact]
        public void ShouldNotUseEmailAddressForDisplayName_WhenCtorCalledWithEmailAndDisplayName()
        {
            var sut = new Recipient("john@doe.com", "John Doe");

            sut.EmailAddress.Should().Be("<john@doe.com>");
            sut.DisplayName.Should().Be("John Doe");
        }
    }
}
