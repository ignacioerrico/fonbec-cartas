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

        [Fact]
        public void ToCommaSeparatedList_ListHasElements()
        {
            // Arrange
            var list = new List<string> { "one", "two", "three" };

            // Act
            var result = list.ToCommaSeparatedList();

            // Assert
            result.Should().Be("one, two, three");
        }

        [Fact]
        public void ToCommaSeparatedList_ListIsEmpty()
        {
            // Arrange
            var list = new List<string>();

            // Act
            var result = list.ToCommaSeparatedList();

            // Assert
            result.Should().Be(string.Empty);
        }
        
        [Fact]
        public void ToCommaSeparatedList_ListIsNull()
        {
            // Arrange
            List<string>? list = null;

            // Act
            var result = list.ToCommaSeparatedList();

            // Assert
            result.Should().Be(string.Empty);
        }
        
        [Fact]
        public void MDashIfEmpty_StringIsEmpty()
        {
            // Arrange
            var value = string.Empty;

            // Act
            var result = value.MDashIfEmpty();

            // Assert
            result.Should().Be("—");
        }

        [Fact]
        public void MDashIfEmpty_StringIsNotEmpty()
        {
            // Arrange
            var value = "something";

            // Act
            var result = value.MDashIfEmpty();

            // Assert
            result.Should().Be("something");
        }

        [Fact]
        public void MDashIfEmpty_StringIsNull()
        {
            // Arrange
            string? value = null;

            // Act
            var result = value.MDashIfEmpty();

            // Assert
            result.Should().Be(string.Empty);
        }
    }
}
