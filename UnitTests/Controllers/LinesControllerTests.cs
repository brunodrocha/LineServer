namespace UnitTests.Controllers
{
    using AutoFixture;

    using FluentAssertions;

    using Microsoft.AspNetCore.Mvc;

    using Moq;

    using Presentation.Controllers;
    using Presentation.Files;

    public class LinesControllerTests
    {
        private readonly IFixture fixture;
        private readonly LinesController controller;
        private readonly Mock<IFileReader> fileReaderMock;

        public LinesControllerTests()
        {
            this.fixture = new Fixture();
            this.fileReaderMock = new Mock<IFileReader>(MockBehavior.Strict);
            this.controller = new LinesController(this.fileReaderMock.Object);
        }

        [Fact]
        public async Task GetAsync_Should_ReturnOk_WithLineAsBody()
        {
            // Arrange
            var index = this.fixture.Create<int>();
            var line = this.fixture.Create<string>();

            this.fileReaderMock
                .Setup(fr => fr.GetLineAsync(index))
                .ReturnsAsync(line);

            // Act
            var result = await this.controller.GetAsync(index);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().BeEquivalentTo(line);
        }

        [Fact]
        public async Task GetAsync_Should_ReturnBadRequest_WhenArgumentOutOfRangeExceptionIsThrown()
        {
            // Arrange
            var index = this.fixture.Create<int>();
            var line = this.fixture.Create<string>();

            this.fileReaderMock
                .Setup(fr => fr.GetLineAsync(index))
                .ThrowsAsync(this.fixture.Create<ArgumentOutOfRangeException>());

            // Act
            var result = await this.controller.GetAsync(index);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)result).StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task GetAsync_Should_ReturnStatusCode500_WhenExceptionIsThrown()
        {
            // Arrange
            var index = this.fixture.Create<int>();
            var line = this.fixture.Create<string>();

            this.fileReaderMock
                .Setup(fr => fr.GetLineAsync(index))
                .ThrowsAsync(this.fixture.Create<Exception>());

            // Act
            var result = await this.controller.GetAsync(index);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be(500);
        }
    }
}
