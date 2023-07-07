using FluentAssertions;
using Fonbec.Cartas.Logic.ExtensionMethods;

namespace Fonbec.Cartas.Logic.Tests.ExtensionMethods
{
    public class DateTimeExtensionMethodsTests
    {
        [Theory]
        [InlineData(2020, 6, 1, "junio de 2020")]
        [InlineData(2020, 6, 15, "junio de 2020")]
        [InlineData(2020, 6, 30, "junio de 2020")]
        [InlineData(2021, 5, 17, "mayo de 2021")]
        public void ToPlanName_Success(int year, int month, int day, string expected)
        {
            // Arrange
            var dateTime = new DateTime(year, month, day);

            // Act
            var result = dateTime.ToPlanName();

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(2020, 6, 1, "1 de junio de 2020")]
        [InlineData(2020, 6, 15, "15 de junio de 2020")]
        [InlineData(2020, 6, 30, "30 de junio de 2020")]
        [InlineData(2021, 5, 17, "17 de mayo de 2021")]
        public void ToLocalizedDate_Success(int year, int month, int day, string expected)
        {
            // Arrange
            var dateTime = new DateTime(year, month, day);

            // Act
            var result = dateTime.ToLocalizedDate();

            // Assert
            result.Should().Be(expected);
        }
    }
}
