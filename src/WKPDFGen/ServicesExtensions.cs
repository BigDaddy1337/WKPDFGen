using System;
using Microsoft.Extensions.DependencyInjection;
using WKPDFGen.Converters;
using WKPDFGen.Library;

namespace WKPDFGen
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddWkPdfGenerator(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IWkHtmlToXService, WkHtmlToXService>();
            services.AddSingleton<IConverter, SynchronizedConverter>();

            return services;
        }
    }
}