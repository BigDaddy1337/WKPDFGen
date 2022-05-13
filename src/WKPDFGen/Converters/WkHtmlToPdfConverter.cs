using System;
using System.Buffers;
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
    Task<Stream> CreateAsync(string html, WKPDFGenConfiguration? configuration = null, CancellationToken token = default);

    Task CreateOnDiskAsync(string html,
                           string path,
                           WKPDFGenConfiguration? configuration = null,
                           CancellationToken token = default);
}

public partial class WkHtmlToPdfConverter : IWkHtmlToPdfConverter
{
    private readonly IWkHtmlWrapper wkHtmlWrapper;
    private readonly ILogger<WkHtmlToPdfConverter> logger;

    private readonly VoidCallback phaseChangedCallback;
    private readonly VoidCallback progressChangedCallback;
    private readonly IntCallback finishedCallback;
    private readonly StringCallback warningCallback;
    private readonly StringCallback errorCallback;

    public WkHtmlToPdfConverter(IWkHtmlWrapper wkHtmlWrapper, ILogger<WkHtmlToPdfConverter> logger)
    {
        this.wkHtmlWrapper = wkHtmlWrapper;
        this.logger = logger;

        phaseChangedCallback = OnPhaseChanged;
        progressChangedCallback = OnProgressChanged;
        finishedCallback = OnFinished;
        warningCallback = OnWarning;
        errorCallback = OnError;
    }

    public Task<Stream> CreateAsync(string html, WKPDFGenConfiguration? configuration = null, CancellationToken token = default)
    {
        var taskCompletionSource = new TaskCompletionSource<Stream>(TaskCreationOptions.RunContinuationsAsynchronously);

        AddItemToProcessingQueue(new(html,
                                     null,
                                     configuration,
                                     taskCompletionSource,
                                     token));

        return taskCompletionSource.Task;
    }

    public Task CreateOnDiskAsync(string html,
                                  string path,
                                  WKPDFGenConfiguration? configuration = null,
                                  CancellationToken token = default)
    {
        var taskCompletionSource = new TaskCompletionSource<Stream>(TaskCreationOptions.RunContinuationsAsynchronously);

        AddItemToProcessingQueue(new(html,
                                     path,
                                     configuration,
                                     taskCompletionSource,
                                     token));

        return taskCompletionSource.Task;
    }

    private Stream Create(string html, string? path, IWKPDFGenConfiguration? configuration = null)
    {
        if (html is null) throw new ArgumentException("html content must be defined.");

        wkHtmlWrapper.Init();

        var (converter, globalSettings, objectSettigns) = CreateConverter(html, path, configuration);

        wkHtmlWrapper.SetPhaseChangedCallback(converter, phaseChangedCallback);
        wkHtmlWrapper.SetProgressChangedCallback(converter, progressChangedCallback);
        wkHtmlWrapper.SetFinishedCallback(converter, finishedCallback);
        wkHtmlWrapper.SetWarningCallback(converter, warningCallback);
        wkHtmlWrapper.SetErrorCallback(converter, errorCallback);

        if (wkHtmlWrapper.Convert(converter) == false)
            throw WkHtmlToPdfException.UnknownExceptionWhileConverting;

        if (path is not null)
        {
            DisposeAll();

            return Stream.Null;
        }

        var result = wkHtmlWrapper.GetConversion(converter);

        DisposeAll();
        
        return result;

        void DisposeAll()
        {
            wkHtmlWrapper.DestroyObjectSetting(objectSettigns);
            wkHtmlWrapper.DestroyGlobalSetting(globalSettings);
            wkHtmlWrapper.DestroyConverter(converter);
        }
    }

    private (IntPtr converter, IntPtr globalSettings, IntPtr objectSettigns) CreateConverter(string html, string? path, IWKPDFGenConfiguration? configuration)
    {
        var globalSettings = wkHtmlWrapper.CreateGlobalSettings();
        var objectSettings = wkHtmlWrapper.CreateObjectSettings();

        if (configuration is not null)
        {
            foreach (var (name, value, isGlobal) in configuration.LibrarySettings)
            {
                var settingsObject = isGlobal
                        ? globalSettings
                        : objectSettings;

                ApplySetting(settingsObject,
                             name,
                             value,
                             isGlobal);
            }
        }

        if (path is not null)
        {
            ApplySetting(globalSettings,
                         "out",
                         path,
                         true);
        }

        var converter = wkHtmlWrapper.CreateConverter(globalSettings);

        AddHtmlContent(converter, objectSettings, html);

        return (converter, globalSettings, objectSettings);
    }

    private void AddHtmlContent(IntPtr converter, IntPtr objectSettings, string html)
    {
        var bufferLength = Encoding.UTF8.GetByteCount(html);
        var buffer = ArrayPool<byte>.Shared.Rent(bufferLength);

        try
        {
            Encoding.UTF8.GetBytes(html,
                                   0,
                                   html.Length,
                                   buffer,
                                   0);

            wkHtmlWrapper.AddObject(converter, objectSettings, buffer);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private void ApplySetting(IntPtr settingsObject,
                              string name,
                              object value,
                              bool isGlobal)
    {
        if (value is bool b)
        {
            wkHtmlWrapper.SetSetting(settingsObject,
                                     name,
                                     b ? "true" : "false",
                                     isGlobal);
        }
        else if (value is double d)
        {
            wkHtmlWrapper.SetSetting(settingsObject,
                                     name,
                                     d.ToString("0.##", CultureInfo.InvariantCulture),
                                     isGlobal);
        }
        else if (value is Dictionary<string, string> dict)
        {
            var index = 0;

            foreach (var (key, val) in dict)
            {
                // https://github.com/wkhtmltopdf/wkhtmltopdf/blob/c754e38b074a75a51327df36c4a53f8962020510/src/lib/reflect.hh#L192

                wkHtmlWrapper.SetSetting(settingsObject,
                                         name + ".append",
                                         null!,
                                         isGlobal);

                wkHtmlWrapper.SetSetting(settingsObject,
                                         $"{name}[{index}]",
                                         key + "\n" + val,
                                         isGlobal);

                index++;
            }
        }
        else
        {
            wkHtmlWrapper.SetSetting(settingsObject,
                                     name,
                                     value.ToString()!,
                                     isGlobal);
        }
    }
}