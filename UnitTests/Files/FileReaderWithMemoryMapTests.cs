namespace UnitTests.Files
{
    using System;

    using FluentAssertions;

    using Microsoft.Extensions.Options;

    using Moq;

    using Presentation.Files;

    public class FileReaderWithMemoryMapTests
    {
        private readonly FileReaderWithMemoryMap fileReader;
        private readonly Mock<IOptions<FileReaderSettings>> settingsMock;
        private readonly Mock<ILineIndexer> lineIndexerMock;

        public FileReaderWithMemoryMapTests()
        {
            this.settingsMock = new Mock<IOptions<FileReaderSettings>>(MockBehavior.Strict);
            this.lineIndexerMock = new Mock<ILineIndexer>(MockBehavior.Strict);
            this.settingsMock
                .Setup(s => s.Value)
                .Returns(new FileReaderSettings { FilePath = "Files\\unit_test_file.txt" });
            this.fileReader = new FileReaderWithMemoryMap(this.settingsMock.Object, this.lineIndexerMock.Object);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(10)]
        public void GetLineAsync_NonExistentFile_Should_ThrowArgumentOutOfRangeException(int index)
        {
            // Arrange

            // Act
            var result = async () => await this.fileReader.GetLineAsync(index);

            // Assert
            result.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(0, 0, 5, "oan")]
        [InlineData(1, 5, 7, "")]
        [InlineData(2, 7, 11, "c,")]
        public async Task GetLineAsync_Should_ReturnLine(int index, int lineStart, int nextLineStart, string expectedResult)
        {
            // Arrange
            this.lineIndexerMock
                .Setup(li => li.TotalLines)
                .Returns(4);

            this.lineIndexerMock
                .Setup(li => li.GetLineStart(index))
                .Returns(lineStart);

            this.lineIndexerMock
                .Setup(li => li.GetLineStart(index + 1))
                .Returns(nextLineStart);

            // Act
            var result = await this.fileReader.GetLineAsync(index);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetLineAsync_LastLine_Should_ReturnLine()
        {
            // Arrange
            var index = 3;
            var lineStart = 11;

            this.lineIndexerMock
                .Setup(li => li.TotalLines)
                .Returns(4);

            this.lineIndexerMock
                .Setup(li => li.GetLineStart(index))
                .Returns(lineStart);

            // Act
            var result = await this.fileReader.GetLineAsync(index);

            // Assert
            result.Should().BeEquivalentTo("a[");
        }
    }
}
