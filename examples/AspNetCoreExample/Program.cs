using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace AspNetCoreExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                              .UseUrls("http://0.0.0.0:5000")
                              .ConfigureLogging(logging =>
                              {
                                  logging.SetMinimumLevel(LogLevel.Trace);
                                  logging.AddSimpleConsole(c => c.ColorBehavior = LoggerColorBehavior.Enabled);
                              });

                });
    }
}