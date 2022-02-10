using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WKPDFGen.Exceptions;
using WKPDFGen.Library;

namespace WKPDFGen.Converters;

public interface IWkHtmlToPdfConverter
{
    byte[] Convert(string html, IConfiguration configuration);
    
    Task<byte[]> ConvertAsync(string html, IConfiguration configuration, CancellationToken token = default);
    
    MemoryStream ConvertAsStream(string html, IConfiguration configuration);
}
    
public partial class WkHtmlToPdfConverter : IWkHtmlToPdfConverter
{
    private readonly IWkHtmlWrapper wkHtmlWrapper;
    private readonly ILogger<WkHtmlToPdfConverter> logger;

    public WkHtmlToPdfConverter(IWkHtmlWrapper wkHtmlWrapper, ILogger<WkHtmlToPdfConverter> logger)
    {
        this.wkHtmlWrapper = wkHtmlWrapper;
        this.logger = logger;
    }
    
    public byte[] Convert(string html, IConfiguration configuration)
    {
        if (html is null || html.Length == 0) 
            throw new ArgumentException("html content must be defined.");
        
        if (configuration is null) 
            throw new ArgumentException("configuration must be defined.");
        
        wkHtmlWrapper.Init();

        var converter = CreateConverter(html, configuration);

        wkHtmlWrapper.SetPhaseChangedCallback(converter, OnPhaseChanged);
        wkHtmlWrapper.SetProgressChangedCallback(converter, OnProgressChanged);
        wkHtmlWrapper.SetFinishedCallback(converter, OnFinished);
        wkHtmlWrapper.SetWarningCallback(converter, OnWarning);
        wkHtmlWrapper.SetErrorCallback(converter, OnError);

        if (wkHtmlWrapper.Convert(converter) == false)
            throw WkHtmlToPdfException.UnknownExceptionWhileConverting;
        
        var result = wkHtmlWrapper.GetConversionBytes(converter);

        wkHtmlWrapper.DestroyConverter(converter);

        return result;
    }

    public Task<byte[]> ConvertAsync(string html, IConfiguration configuration, CancellationToken token = default)
    {
        if (html is null || html.Length == 0) 
            throw new ArgumentException("html content must be defined.");
        
        if (configuration is null) 
            throw new ArgumentException("configuration must be defined.");
        
        var taskCompletionSource = new TaskCompletionSource<byte[]>();
        
        wkHtmlWrapper.Init();

        var converter = CreateConverter(html, configuration);
        
        try
        {

            token.ThrowIfCancellationRequested();

            wkHtmlWrapper.SetPhaseChangedCallback(converter, OnPhaseChanged);

            wkHtmlWrapper.SetProgressChangedCallback(converter, _ =>
            {
                OnProgressChanged(converter);

                token.ThrowIfCancellationRequested();
            });

            wkHtmlWrapper.SetFinishedCallback(converter, (_, success) =>
            {
                OnFinished(converter, success);

                if (success == 1)
                    taskCompletionSource.SetResult(wkHtmlWrapper.GetConversionBytes(converter));
                else
                    taskCompletionSource.SetException(WkHtmlToPdfException.UnknownExceptionWhileConverting);
            });

            wkHtmlWrapper.SetWarningCallback(converter, OnWarning);

            wkHtmlWrapper.SetErrorCallback(converter, OnError);

            wkHtmlWrapper.Convert(converter);
        }
        catch (Exception e)
        {
            taskCompletionSource.SetException(e);
        }
        finally
        {
            wkHtmlWrapper.DestroyConverter(converter);
        }
        
        return taskCompletionSource.Task;
    }
    
    public MemoryStream ConvertAsStream(string html, IConfiguration configuration)
    {
        if (html is null || html.Length == 0) 
            throw new ArgumentException("html content must be defined.");
        
        if (configuration is null) 
            throw new ArgumentException("configuration must be defined.");
        
        wkHtmlWrapper.Init();

        var converter = CreateConverter(html, configuration);

        wkHtmlWrapper.SetPhaseChangedCallback(converter, OnPhaseChanged);
        wkHtmlWrapper.SetProgressChangedCallback(converter, OnProgressChanged);
        wkHtmlWrapper.SetFinishedCallback(converter, OnFinished);
        wkHtmlWrapper.SetWarningCallback(converter, OnWarning);
        wkHtmlWrapper.SetErrorCallback(converter, OnError);

        if (wkHtmlWrapper.Convert(converter) == false)
            throw WkHtmlToPdfException.UnknownExceptionWhileConverting;
        
        var result = wkHtmlWrapper.GetConversionStream(converter);

        wkHtmlWrapper.DestroyConverter(converter);

        return result;
    }

    private IntPtr CreateConverter(string html, IConfiguration configuration)
    {
        var globalSettings = wkHtmlWrapper.CreateGlobalSettings();
        var objectSettings = wkHtmlWrapper.CreateObjectSettings();
        
        foreach (var (name, value, isGlobal) in configuration.LibrarySettings)
        {
            var settingsObject = isGlobal 
                ? globalSettings 
                : objectSettings;
            
            ApplySetting(settingsObject, name, value, isGlobal);
        }

        var converter = wkHtmlWrapper.CreateConverter(globalSettings);
        
        wkHtmlWrapper.AddObject(converter, objectSettings, Encoding.UTF8.GetBytes(html));
            
        return converter;
    }
        
    private void ApplySetting(IntPtr settingsObject, string name, object value, bool isGlobal)
    {
        if (value is bool b)
        {
            wkHtmlWrapper.SetSetting(settingsObject, name, b ? "true" : "false", isGlobal);
        }
        else if (value is double d)
        {
            wkHtmlWrapper.SetSetting(settingsObject, name, d.ToString("0.##", CultureInfo.InvariantCulture), isGlobal);
        }
        else if (value is Dictionary<string, string> dict)
        {
            var index = 0;

            foreach (var (key, val) in dict)
            {
                // https://github.com/wkhtmltopdf/wkhtmltopdf/blob/c754e38b074a75a51327df36c4a53f8962020510/src/lib/reflect.hh#L192
                    
                wkHtmlWrapper.SetSetting(settingsObject, name + ".append", null!, isGlobal);
                wkHtmlWrapper.SetSetting(settingsObject, $"{name}[{index}]", key + "\n" + val, isGlobal);

                index++;
            }
        }
        else
        {
            wkHtmlWrapper.SetSetting(settingsObject, name, value.ToString()!, isGlobal);
        }
    }
}