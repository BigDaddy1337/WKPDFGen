using System;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WKPDFGen.Library;

public interface IWkHtmlWrapper : IDisposable
{
    void Init();

    void DeInit();
        
    string? GetLibraryVersion();
        
    IntPtr CreateGlobalSettings();
        
    IntPtr CreateObjectSettings();
    
    int DestroyGlobalSetting(IntPtr settings);
    
    int DestroyObjectSetting(IntPtr settings);

    void SetSetting(IntPtr settings, string name, string value, bool isGlobal);
        
    IntPtr CreateConverter(IntPtr globalSettings);

    void AddObject(IntPtr converter, IntPtr objectSettings, byte[] data);
        
    bool Convert(IntPtr converter);

    void DestroyConverter(IntPtr converter);

    Stream GetConversion(IntPtr converter);

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
    
    private readonly string libraryPath;

    public WkHtmlWrapper(IOptions<WkPdfOptions> wkPdfOptions, ILogger<WkHtmlWrapper> logger)
    {
        this.logger = logger;
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            libraryPath = wkPdfOptions.Value.OsxLibPath ?? throw new NoNullAllowedException("OsxLibPath option is required for current OS");
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            libraryPath = wkPdfOptions.Value.LinuxLibPath ?? throw new NoNullAllowedException("LinuxLibPath option is required for current OS");
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            libraryPath = wkPdfOptions.Value.WindowsLibPath ?? throw new NoNullAllowedException("WindowsLibPath option is required for current OS");
        else throw new PlatformNotSupportedException();

        NativeLibrary.SetDllImportResolver(typeof(WkHtmlWrapper).Assembly, NativeLibraryImportResolver);
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
    
    public void DeInit()
    {
        WkHtmlBindings.wkhtmltopdf_deinit();
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

    public int DestroyGlobalSetting(IntPtr settings)
    {
        logger.LogDebug("[WkHTMLtoPDF] GlobalSetting destroing");

        return WkHtmlBindings.wkhtmltopdf_destroy_global_settings(settings);
    }

    public int DestroyObjectSetting(IntPtr settings)
    {
        logger.LogDebug("[WkHTMLtoPDF] ObjectSetting destroing");

        return WkHtmlBindings.wkhtmltopdf_destroy_object_settings(settings);
    }

    public void SetSetting(IntPtr settings,
                           string name,
                           string value,
                           bool isGlobal)
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
        logger.LogDebug("[WkHTMLtoPDF] Added content object {DataBytesLength} bytes to object settings", data.Length);

        WkHtmlBindings.wkhtmltopdf_add_object(converter, objectSettings, data);
    }

    public bool Convert(IntPtr converter)
    {
        return WkHtmlBindings.wkhtmltopdf_convert(converter);
    }

    public void DestroyConverter(IntPtr converter)
    {
        WkHtmlBindings.wkhtmltopdf_destroy_converter(converter);
        
        logger.LogDebug("[WkHTMLtoPDF] Converter destroyed");
    }

    public unsafe Stream GetConversion(IntPtr converter)
    {
        var length = WkHtmlBindings.wkhtmltopdf_get_output(converter, out var unmanaged);

        logger.LogDebug("[WkHTMLtoPDF] Fetch conversion result {ConversionResultBytesLength} bytes", length);

        return new UnmanagedMemoryStream((byte*)unmanaged.ToPointer(),
                                         length,
                                         length,
                                         FileAccess.Read);
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
        DeInit();
        
        FreeLibraryHandle();
    }
}