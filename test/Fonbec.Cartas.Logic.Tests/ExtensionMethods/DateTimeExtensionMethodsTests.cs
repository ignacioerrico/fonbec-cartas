using FluentAssertions;
using FluentAssertions.Execution;
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

        [Fact]
        public void IsBetween_ReturnsTrue_WithUpperBoundNull()
        {
            // Arrange
            var lowerBound = new DateTime(2020, 1, 10);

            var pointInTimeMin = new DateTime(2020, 1, 10);
            var pointInTimeMax = DateTime.MaxValue;

            // Act
            var resultMin = pointInTimeMin.IsBetween(lowerBound);
            var resultMax = pointInTimeMax.IsBetween(lowerBound);

            // Assert
            using (new AssertionScope())
            {
                resultMin.Should().BeTrue();
                resultMax.Should().BeTrue();
            }
        }

        [Theory]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        public void IsBetween_ReturnsTrue_WithUpperBoundNotNull(int day)
        {
            // Arrange
            var lowerBound = new DateTime(2020, 1, 10);
            var upperBound = new DateTime(2020, 1, 15);

            var pointInTime = new DateTime(2020, 1, day);

            // Act
            var result = pointInTime.IsBetween(lowerBound, upperBound);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsBetween_ReturnsFalse_WithUpperBoundNull()
        {
            // Arrange
            var lowerBound = new DateTime(2020, 1, 10);

            var pointInTimeMin = DateTime.MinValue;
            var pointInTimeLeft = new DateTime(2020, 1, 9);

            // Act
            var resultMin = pointInTimeMin.IsBetween(lowerBound);
            var resultMax = pointInTimeLeft.IsBetween(lowerBound);

            // Assert
            using (new AssertionScope())
            {
                resultMin.Should().BeFalse();
                resultMax.Should().BeFalse();
            }
        }

        [Fact]
        public void IsBetween_ReturnsFalse_WithUpperBoundNotNull()
        {
            // Arrange
            var lowerBound = new DateTime(2020, 1, 10);
            var upperBound = new DateTime(2020, 1, 15);

            var lowerBoundLeft = new DateTime(2020, 1, 9);
            var lowerBoundRight = new DateTime(2020, 1, 16);

            // Act
            var resultMin = DateTime.MinValue.IsBetween(lowerBound, upperBound);
            var resultLeft = lowerBoundLeft.IsBetween(lowerBound, upperBound);
            var resultRight = lowerBoundRight.IsBetween(lowerBound, upperBound);
            var resultMax = DateTime.MaxValue.IsBetween(lowerBound, upperBound);

            // Assert
            using (new AssertionScope())
            {
                resultMin.Should().BeFalse();
                resultLeft.Should().BeFalse();
                resultRight.Should().BeFalse();
                resultMax.Should().BeFalse();
            }
        }
    }
}
