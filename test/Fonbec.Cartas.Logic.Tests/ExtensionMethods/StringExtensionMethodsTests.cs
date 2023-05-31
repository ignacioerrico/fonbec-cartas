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
    }
}
