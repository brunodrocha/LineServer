using Presentation.Files;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.Configure<FileReaderSettings>(builder.Configuration);
        builder.Services.AddSingleton<ILineIndexer, LineIndexer>();
        builder.Services.AddSingleton<IFileReader, FileReaderWithMemoryMap>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var lineIndexer = scope.ServiceProvider.GetRequiredService<ILineIndexer>();
            lineIndexer.BuildIndexes();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
