using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Options;
using WKPDFGen.Library.Utils;

namespace WKPDFGen.Library
{
    public interface IWkHtmlToXService : IDisposable
    {
        void Load();
        
        string GetLibraryVersion();
        
        IntPtr CreateGlobalSettings();
        
        IntPtr CreateObjectSettings();

        void SetSetting(IntPtr settings, string name, string value, bool isGlobal);
        
        IntPtr CreateConverter(IntPtr globalSettings);

        void AddObject(IntPtr converter, IntPtr objectSettings, byte[] data);
        
        bool DoConversion(IntPtr converter);

        void DestroyConverter(IntPtr converter);

        byte[] GetConversionResult(IntPtr converter);

        int SetPhaseChangedCallback(IntPtr converter, VoidCallback callback);

        int SetProgressChangedCallback(IntPtr converter, VoidCallback callback);

        int SetFinishedCallback(IntPtr converter, IntCallback callback);

        int SetWarningCallback(IntPtr converter, StringCallback callback);

        int SetErrorCallback(IntPtr converter, StringCallback callback);

        int GetPhaseCount(IntPtr converter);

        int GetCurrentPhase(IntPtr converter);

        string GetPhaseDescription(IntPtr converter, int phase);

        string GetProgressString(IntPtr converter);
    }
    
    public sealed partial class WkHtmlToXService : IWkHtmlToXService
    {
        private bool isLoaded;

        public WkHtmlToXService(IOptions<WkPdfOptions> wkPdfOptions)
        {
            isLoaded = false;
            
            LoadWkHtmlToXLibraryDll(wkPdfOptions.Value);
        }

        public void Load()
        {
            if (isLoaded) return;

            if (WkHtmlToXBindings.wkhtmltopdf_init(0) == 1)
            {
                isLoaded = true;
            }
        }

        public string GetLibraryVersion()
        {
            return Marshal.PtrToStringAnsi(WkHtmlToXBindings.wkhtmltopdf_version());
        }

        public IntPtr CreateGlobalSettings()
        {
            return WkHtmlToXBindings.wkhtmltopdf_create_global_settings();
        }

        public int SetGlobalSetting(IntPtr settings, string name, string value)
        {
            return WkHtmlToXBindings.wkhtmltopdf_set_global_setting(settings, name, value);
        }
        
        public IntPtr CreateObjectSettings()
        {
            return WkHtmlToXBindings.wkhtmltopdf_create_object_settings();
        }

        public void SetSetting(IntPtr settings, string name, string value, bool isGlobal)
        {
            if (isGlobal) SetGlobalSetting(settings, name, value);
            else SetObjectSetting(settings, name, value);
        }

        public int SetObjectSetting(IntPtr settings, string name, string value)
        {
            return WkHtmlToXBindings.wkhtmltopdf_set_object_setting(settings, name, value);
        }
        
        public IntPtr CreateConverter(IntPtr globalSettings)
        {
            return WkHtmlToXBindings.wkhtmltopdf_create_converter(globalSettings);
        }

        public void AddObject(IntPtr converter, IntPtr objectSettings, byte[] data)
        {
            WkHtmlToXBindings.wkhtmltopdf_add_object(converter, objectSettings, data);
        }
        
        public bool DoConversion(IntPtr converter)
        {
            return WkHtmlToXBindings.wkhtmltopdf_convert(converter);
        }

        public void DestroyConverter(IntPtr converter)
        {
            WkHtmlToXBindings.wkhtmltopdf_destroy_converter(converter);
        }

        public byte[] GetConversionResult(IntPtr converter)
        {
            var length = WkHtmlToXBindings.wkhtmltopdf_get_output(converter, out var resultPointer);
            
            var result = new byte[length];
            
            Marshal.Copy(resultPointer, result, 0, length);
        
            return result;
        }

        public int SetPhaseChangedCallback(IntPtr converter, VoidCallback callback)
        {
            return WkHtmlToXBindings.wkhtmltopdf_set_phase_changed_callback(converter, callback);
        }

        public int SetProgressChangedCallback(IntPtr converter, VoidCallback callback)
        {
            return WkHtmlToXBindings.wkhtmltopdf_set_progress_changed_callback(converter, callback);
        }

        public int SetFinishedCallback(IntPtr converter, IntCallback callback)
        {
            return WkHtmlToXBindings.wkhtmltopdf_set_finished_callback(converter, callback);
        }

        public int SetWarningCallback(IntPtr converter, StringCallback callback)
        {
            return WkHtmlToXBindings.wkhtmltopdf_set_warning_callback(converter, callback);
        }

        public int SetErrorCallback(IntPtr converter, StringCallback callback)
        {
            return WkHtmlToXBindings.wkhtmltopdf_set_error_callback(converter, callback);
        }

        public int GetPhaseCount(IntPtr converter)
        {
            return WkHtmlToXBindings.wkhtmltopdf_phase_count(converter);
        }

        public int GetCurrentPhase(IntPtr converter)
        {
            return WkHtmlToXBindings.wkhtmltopdf_current_phase(converter);
        }

        public string GetPhaseDescription(IntPtr converter, int phase)
        {
            return Marshal.PtrToStringAnsi(WkHtmlToXBindings.wkhtmltopdf_phase_description(converter, phase));
        }

        public string GetProgressString(IntPtr converter)
        {
            return Marshal.PtrToStringAnsi(WkHtmlToXBindings.wkhtmltopdf_progress_string(converter));
        }
        
        public void Dispose()
        {
            WkHtmlToXBindings.wkhtmltopdf_deinit();
        }
    }
}