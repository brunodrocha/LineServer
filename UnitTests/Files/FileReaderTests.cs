namespace UnitTests.Files
{
    using System;

    using FluentAssertions;

    using Microsoft.Extensions.Options;

    using Moq;

    using Presentation.Files;

    public class FileReaderTests
    {
        private readonly FileReaderWithFileStream fileReader;
        private readonly Mock<IOptions<FileReaderSettings>> settingsMock;
        private readonly Mock<ILineIndexer> lineIndexerMock;

        public FileReaderTests()
        {
            this.settingsMock = new Mock<IOptions<FileReaderSettings>>(MockBehavior.Strict);
            this.lineIndexerMock = new Mock<ILineIndexer>(MockBehavior.Strict);
            this.settingsMock
                .Setup(s => s.Value)
                .Returns(new FileReaderSettings { FilePath = "Files\\unit_test_file.txt" });
            this.fileReader = new FileReaderWithFileStream(this.settingsMock.Object, this.lineIndexerMock.Object);
        }

        [Theory]
        [InlineData(0, 0, "oan")]
        [InlineData(1, 5, "")]
        [InlineData(2, 7, "c,")]
        [InlineData(3, 11, "a[")]
        public async Task GetLineAsync_Should_ReturnLine(int index, int lineStart, string expectedResult)
        {
            // Arrange
            this.lineIndexerMock
                .Setup(li => li.GetLineStart(index))
                .Returns(lineStart);

            // Act
            var result = await this.fileReader.GetLineAsync(index);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
