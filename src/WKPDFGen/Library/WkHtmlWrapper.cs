﻿using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WKPDFGen.Library;

public interface IWkHtmlWrapper : IDisposable
{
    void Init();
        
    string? GetLibraryVersion();
        
    IntPtr CreateGlobalSettings();
        
    IntPtr CreateObjectSettings();

    void SetSetting(IntPtr settings, string name, string value, bool isGlobal);
        
    IntPtr CreateConverter(IntPtr globalSettings);

    void AddObject(IntPtr converter, IntPtr objectSettings, byte[] data);
        
    bool Convert(IntPtr converter);

    void DestroyConverter(IntPtr converter);

    byte[] GetConversionBytes(IntPtr converter);

    MemoryStream GetConversionStream(IntPtr converter);

    int SetPhaseChangedCallback(IntPtr converter, VoidCallback callback);

    int SetProgressChangedCallback(IntPtr converter, VoidCallback callback);

    int SetFinishedCallback(IntPtr converter, IntCallback callback);

    int SetWarningCallback(IntPtr converter, StringCallback callback);

    int SetErrorCallback(IntPtr converter, StringCallback callback);

    int GetPhaseCount(IntPtr converter);

    int GetCurrentPhase(IntPtr converter);

    string? GetPhaseDescription(IntPtr converter, int phase);

    string? GetProgressString(IntPtr converter);
}
    
public sealed partial class WkHtmlWrapper : IWkHtmlWrapper
{
    private readonly ILogger<WkHtmlWrapper> logger;

    private bool isLoaded;

    public WkHtmlWrapper(IOptions<WkPdfOptions> wkPdfOptions, ILogger<WkHtmlWrapper> logger)
    {
        this.logger = logger;

        LoadWkHtmlLibraryDll(wkPdfOptions.Value);
    }

    public void Init()
    {
        if (isLoaded) return;

        if (WkHtmlBindings.wkhtmltopdf_init(0) == 1)
        {
            var version = GetLibraryVersion();
            
            logger.LogInformation("[WkHTMLtoPDF] Initialized library with version {version}", version);
            
            isLoaded = true;
        }
    }

    public string? GetLibraryVersion()
    {
        return Marshal.PtrToStringAnsi(WkHtmlBindings.wkhtmltopdf_version());
    }

    public IntPtr CreateGlobalSettings()
    {
        return WkHtmlBindings.wkhtmltopdf_create_global_settings();
    }

    public int SetGlobalSetting(IntPtr settings, string name, string value)
    {
        logger.LogDebug("[WkHTMLtoPDF] Applying setting {settingName}:{settingValue} to global settings", name, value);

        return WkHtmlBindings.wkhtmltopdf_set_global_setting(settings, name, value);
    }
        
    public IntPtr CreateObjectSettings()
    {
        return WkHtmlBindings.wkhtmltopdf_create_object_settings();
    }

    public void SetSetting(IntPtr settings, string name, string value, bool isGlobal)
    {
        if (isGlobal) SetGlobalSetting(settings, name, value);
        else SetObjectSetting(settings, name, value);
    }

    public int SetObjectSetting(IntPtr settings, string name, string value)
    {
        logger.LogDebug("[WkHTMLtoPDF] Applying setting {settingName}:{settingValue} to object settings", name, value);

        return WkHtmlBindings.wkhtmltopdf_set_object_setting(settings, name, value);
    }
        
    public IntPtr CreateConverter(IntPtr globalSettings)
    {
        return WkHtmlBindings.wkhtmltopdf_create_converter(globalSettings);
    }

    public void AddObject(IntPtr converter, IntPtr objectSettings, byte[] data)
    {
        WkHtmlBindings.wkhtmltopdf_add_object(converter, objectSettings, data);
    }
        
    public bool Convert(IntPtr converter)
    {
        return WkHtmlBindings.wkhtmltopdf_convert(converter);
    }

    public void DestroyConverter(IntPtr converter)
    {
        WkHtmlBindings.wkhtmltopdf_destroy_converter(converter);
    }

    public byte[] GetConversionBytes(IntPtr converter)
    {
        var length = WkHtmlBindings.wkhtmltopdf_get_output(converter, out var resultPointer);
            
        var result = new byte[length];
   
        Marshal.Copy(resultPointer, result, 0, length);
        
        return result;
    }
    
    public unsafe MemoryStream GetConversionStream(IntPtr converter)
    {
        var length = WkHtmlBindings.wkhtmltopdf_get_output(converter, out var resultPointer);
        
        var buffer = new Span<byte>(resultPointer.ToPointer(), length);

        var stream = new MemoryStream(buffer.Length);
        
        var sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
        
        try
        {
            buffer.CopyTo(sharedBuffer);
            
            stream.Write(sharedBuffer, 0, buffer.Length);
        }
        finally { ArrayPool<byte>.Shared.Return(sharedBuffer); }

        stream.Seek(0, SeekOrigin.Begin);
        
        return stream;
    }

    public int SetPhaseChangedCallback(IntPtr converter, VoidCallback callback)
    {
        return WkHtmlBindings.wkhtmltopdf_set_phase_changed_callback(converter, callback);
    }

    public int SetProgressChangedCallback(IntPtr converter, VoidCallback callback)
    {
        return WkHtmlBindings.wkhtmltopdf_set_progress_changed_callback(converter, callback);
    }

    public int SetFinishedCallback(IntPtr converter, IntCallback callback)
    {
        return WkHtmlBindings.wkhtmltopdf_set_finished_callback(converter, callback);
    }

    public int SetWarningCallback(IntPtr converter, StringCallback callback)
    {
        return WkHtmlBindings.wkhtmltopdf_set_warning_callback(converter, callback);
    }

    public int SetErrorCallback(IntPtr converter, StringCallback callback)
    {
        return WkHtmlBindings.wkhtmltopdf_set_error_callback(converter, callback);
    }

    public int GetPhaseCount(IntPtr converter)
    {
        return WkHtmlBindings.wkhtmltopdf_phase_count(converter);
    }

    public int GetCurrentPhase(IntPtr converter)
    {
        return WkHtmlBindings.wkhtmltopdf_current_phase(converter);
    }

    public string? GetPhaseDescription(IntPtr converter, int phase)
    {
        return Marshal.PtrToStringAnsi(WkHtmlBindings.wkhtmltopdf_phase_description(converter, phase));
    }

    public string? GetProgressString(IntPtr converter)
    {
        return Marshal.PtrToStringAnsi(WkHtmlBindings.wkhtmltopdf_progress_string(converter));
    }
    
    public void Dispose()
    {
        WkHtmlBindings.wkhtmltopdf_deinit();
    }
}