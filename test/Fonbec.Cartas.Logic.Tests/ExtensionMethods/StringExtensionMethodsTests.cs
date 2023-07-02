using FluentAssertions;
using Fonbec.Cartas.Logic.ExtensionMethods;

namespace Fonbec.Cartas.Logic.Tests.ExtensionMethods
{
    public class StringExtensionMethodsTests
    {
        [Theory]
        [InlineData("María Elena", "ría", true)]
        [InlineData("María Elena", "ria", true)]
        [InlineData("Marí aelena", "ria", true)]
        [InlineData("MAR Í äelena", "ria", true)]
        [InlineData("Mar/Í/À\\elena", "ria", true)]
        public void ContainsIgnoringAccents_(string source, string subString, bool expected)
        {
            // Act
            var actual = source.ContainsIgnoringAccents(subString);

            // Assert
            expected.Should().Be(actual);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ToCommaSeparatedList_Success(bool useMDashIfEmpty)
        {
            // Arrange
            var list = new List<string> { "one", "two", "three" };

            // Act
            var result = list.ToCommaSeparatedList(useMDashIfEmpty);

            // Assert
            result.Should().Be("one, two, three");
        }

        [Fact]
        public void ToCommaSeparatedList_Empty_DoNotUseMDash()
        {
            // Arrange
            var list = new List<string>();

            // Act
            var result = list.ToCommaSeparatedList(useMDashIfEmpty: false);

            // Assert
            result.Should().Be(string.Empty);
        }
        
        [Fact]
        public void ToCommaSeparatedList_Empty_UseMDash()
        {
            // Arrange
            var list = new List<string>();

            // Act
            var result = list.ToCommaSeparatedList(useMDashIfEmpty: true);

            // Assert
            result.Should().Be("—");
        }
    }
}
