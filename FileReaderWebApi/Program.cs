using FileConsumerSolution;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Registering the DbContext with PostgreSQL
builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));


// Adding services to the container.
builder.Services.AddControllers();

// Configure logging (Console logging is added here)
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();  
    logging.AddConsole();      
    logging.AddDebug();        
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
