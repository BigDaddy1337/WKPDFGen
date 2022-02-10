using System;
using Microsoft.Extensions.DependencyInjection;
using WKPDFGen.Converters;
using WKPDFGen.Library;

namespace WKPDFGen;

public class WkPdfOptions
{
    public string WindowsLibPath { get; set; }
        
    public string LinuxLibPath { get; set; }
        
    public string OsxLibPath { get; set; }
}
    
public static class ServicesExtensions
{
    public static IServiceCollection AddWkPdfGenerator(this IServiceCollection services, Action<WkPdfOptions> options)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (options == null) throw new ArgumentNullException(nameof(options));

        services.Configure(options);

        services.AddSingleton<IWkHtmlWrapper, WkHtmlWrapper>();
        services.AddSingleton<IWkHtmlToPdfConverter, WkHtmlToPdfConverter>();

        return services;
    }
}