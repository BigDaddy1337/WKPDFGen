using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WKPDFGen;

namespace AspNetCoreExample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWkPdfGenerator(options =>
            {
                options.LinuxLibPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "0.12.5", "libwkhtmltox.so");
                options.WindowsLibPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "0.12.5", "libwkhtmltox.dll");
                options.OsxLibPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "0.12.5", "libwkhtmltox.dylib");
            });

            services.AddHostedService<ExampleBackgroundWorker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            });
        }
    }
}