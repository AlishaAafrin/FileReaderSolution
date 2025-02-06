using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using FileConsumerSolution;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        await host.RunAsync(); 
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;

                // Registerign DbContext 
                services.AddDbContext<DataContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

                // Registering the Consumer Service as a Background Service
                services.AddHostedService<ConsumerService>();

                // Configure logging
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.AddDebug();

                    if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    {
                        builder.AddEventLog();
                    }
                });
            });
}
