using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MockupGeneratorLinux.Settings;
using MockupGeneratorLinux.Services;
using MockupGeneratorLinux.Abstractions;
using Microsoft.Extensions.Options;

namespace MockupGeneratorLinux
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Environment.SetEnvironmentVariable("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT", "1");

            using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory);
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<PathSettings>(context.Configuration.GetSection("Paths"));
                    services.AddTransient<IGreenFinder, GreenFinder>();
                    services.AddTransient<IImageProcessor, ImageProcessor>();
                    services.AddTransient<IResizer, Resizer>();
                })
                .Build();

            using var scope = host.Services.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<IImageProcessor>();

            var fileName = await processor.GetNextFileAsync();
            if (fileName is null)
            {
                Console.WriteLine("Нет файлов для обработки.");
                return;
            }

            await processor.ProcessAsync(fileName);
            Console.WriteLine($"Обработка файла {fileName} завершена.");
        }
    }
}