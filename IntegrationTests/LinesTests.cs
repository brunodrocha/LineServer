namespace PerformanceTests
{
    using System.Net;
    using System.Threading.Tasks;

    using FluentAssertions;

    using IntegrationTests;

    public class LinesTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory factory;

        public LinesTests(CustomWebApplicationFactory factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task GetAsync_Should_ReturnOk_WithLineAsBody()
        {
            // Arrange
            var client = this.factory.CreateClient();
            var lineIndex = 1;

            // Act
            var response = await client.GetAsync($"lines/{lineIndex}");

            var body = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            body.Should().Be("1: adipiscing dolore dolore do ipsum adipiscing elit");
        }

        [Fact]
        public async Task GetAsync_Should_ReturnBadRequest_WithLineBeyoundEndOfFile()
        {
            // Arrange
            var client = this.factory.CreateClient();
            var lineIndex = 9;

            // Act
            var response = await client.GetAsync($"lines/{lineIndex}");

            var body = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}