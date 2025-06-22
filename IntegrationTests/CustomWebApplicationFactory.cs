namespace IntegrationTests
{
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public CustomWebApplicationFactory()
        {
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "FilePath", "integration_test_file.txt" }
                });
            });
            return base.CreateHost(builder);
        }
    }
}
