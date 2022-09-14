using Serilog;
using Serilog.Events;
using VerificaFTP.Factories;
using VerificaFTP.Settings;

namespace VerificaFTP
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var host = WindowsCreateHostBuilder(args)
                                .ConfigureLogging(loggingBuilder =>
                                {
                                    var configuration = new ConfigurationBuilder()
                                        .AddJsonFile("appsettings.json")
                                        .Build();

                                    Log.Logger = new LoggerConfiguration()
                                                        .MinimumLevel.Debug()
                                                        .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                                                        .Enrich.FromLogContext()
                                                        .Enrich.WithThreadId()
                                                        .WriteTo.File(@"Logs/VerificaFTP_.log", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{SourceContext}][{ThreadId}][{Level:u3}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day)
                                                        .CreateLogger();
                                }).Build();

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um ero ao Criar o Host.");
                Console.WriteLine(ex.Message);
            }
        }

        private static IHostBuilder WindowsCreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
#if DEBUG
                 .UseConsoleLifetime()
#else
                 .UseWindowsService()
#endif
                 .ConfigureServices((hostContext, services) =>
                 {
                     services.AddOptions();

                     services.Configure<FtpSettings>(hostContext.Configuration.GetSection("FTP"));
                     services.Configure<PathSettings>(hostContext.Configuration.GetSection("Path"));

                     _ = services.AddSingleton<FactoryFtpClient>();

                     services.AddHostedService<Worker>();
                 })
                .UseSerilog();
    }
}