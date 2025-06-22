namespace UnitTests.Files
{
    using System;

    using FluentAssertions;

    using Microsoft.Extensions.Options;

    using Moq;

    using Presentation.Files;

    public class LineIndexerTests
    {
        private readonly LineIndexer lineIndexer;
        private readonly Mock<IOptions<FileReaderSettings>> settingsMock;

        public LineIndexerTests()
        {
            this.settingsMock = new Mock<IOptions<FileReaderSettings>>(MockBehavior.Strict);
            this.settingsMock
                .Setup(s => s.Value)
                .Returns(new FileReaderSettings { FilePath = "Files\\unit_test_file.txt" });
            this.lineIndexer = new LineIndexer(this.settingsMock.Object);
        }

        [Fact]
        public void TotalLines_Should_ReturnTotalFileLines()
        {
            // Arrange
            this.lineIndexer.BuildIndexes();

            // Act
            var result = this.lineIndexer.TotalLines;

            // Assert
            result.Should().Be(4);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(10)]
        public void GetLineStart_NonExistentLine_Should_ThrowArgumentOutOfRangeException(int index)
        {
            // Arrange
            this.lineIndexer.BuildIndexes();

            // Act
            var result = () => this.lineIndexer.GetLineStart(index);

            // Assert
            result.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 5)]
        [InlineData(2, 7)]
        [InlineData(3, 11)]
        public void GetLineStart_Should_ReturnLineStartIndexList(int index, long expectedLineStart)
        {
            // Arrange
            this.lineIndexer.BuildIndexes();

            // Act
            var result = this.lineIndexer.GetLineStart(index);

            // Assert
            result.Should().Be(expectedLineStart);
        }
    }
}
