using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WKPDFGen;
using WKPDFGen.Converters;

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
                var nativeLibsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "0.12.6");
                
                options.LinuxLibPath = Path.Combine(nativeLibsPath, "libwkhtmltox.so.0.12.6");
                options.WindowsLibPath = Path.Combine(nativeLibsPath, "libwkhtmltox.dll");
                options.OsxLibPath = Path.Combine(nativeLibsPath, "libwkhtmltox.0.12.6.dylib");
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
                endpoints.MapGet("/",
                                 async context =>
                                 {
                                     var converter = context.RequestServices.GetService<IWkHtmlToPdfConverter>();
                                     
                                     var html = await File.ReadAllTextAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Simple.html"));

                                     var pdfStream = await converter!.CreateAsync(html);

                                     await pdfStream.CopyToAsync(context.Response.Body);
                                     
                                     await context.Response.CompleteAsync();
                                 });
            });
        }
    }
}