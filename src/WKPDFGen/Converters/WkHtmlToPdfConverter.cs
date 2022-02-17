﻿using System;
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
    Task<Stream> CreateAsync(string html, PDFConfiguration? configuration = null, CancellationToken token = default);

    Task CreateOnDiskAsync(string html,
                           string path,
                           PDFConfiguration? configuration = null,
                           CancellationToken token = default);
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

    public Task<Stream> CreateAsync(string html, PDFConfiguration? configuration = null, CancellationToken token = default)
    {
        var taskCompletionSource = new TaskCompletionSource<Stream>();

        AddItemToProcessingQueue(new(html,
                                     null,
                                     configuration,
                                     taskCompletionSource,
                                     token));

        return taskCompletionSource.Task;
    }

    public Task CreateOnDiskAsync(string html,
                                  string path,
                                  PDFConfiguration? configuration = null,
                                  CancellationToken token = default)
    {
        var taskCompletionSource = new TaskCompletionSource<Stream>();

        AddItemToProcessingQueue(new(html,
                                     path,
                                     configuration,
                                     taskCompletionSource,
                                     token));

        return taskCompletionSource.Task;
    }

    private Stream Create(string html, string? path, IConfiguration? configuration = null)
    {
        if (html is null) throw new ArgumentException("html content must be defined.");

        wkHtmlWrapper.Init();

        var converter = CreateConverter(html, path, configuration);

        wkHtmlWrapper.SetPhaseChangedCallback(converter, OnPhaseChanged);
        wkHtmlWrapper.SetProgressChangedCallback(converter, OnProgressChanged);
        wkHtmlWrapper.SetFinishedCallback(converter, OnFinished);
        wkHtmlWrapper.SetWarningCallback(converter, OnWarning);
        wkHtmlWrapper.SetErrorCallback(converter, OnError);

        if (wkHtmlWrapper.Convert(converter) == false)
            throw WkHtmlToPdfException.UnknownExceptionWhileConverting;

        if (path is not null)
        {
            wkHtmlWrapper.DestroyConverter(converter);

            return Stream.Null;
        }

        var result = wkHtmlWrapper.GetConversion(converter);

        wkHtmlWrapper.DestroyConverter(converter);
        
        return result;
    }

    private IntPtr CreateConverter(string html, string? path, IConfiguration? configuration)
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

        return converter;
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