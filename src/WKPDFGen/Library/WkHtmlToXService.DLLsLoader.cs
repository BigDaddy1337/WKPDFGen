using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace WKPDFGen.Library
{
    public partial class WkHtmlToXService: AssemblyLoadContext
    {
        public static string LibVersion = "0.12.5";
        
        public void LoadWkHtmlToXLibraryDll()
        {
            var libPath = string.Empty;

            var current = AppDomain.CurrentDomain.BaseDirectory ?? string.Empty;
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                libPath = Path.Combine(current, LibVersion, "libwkhtmltox.dylib");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                libPath = Path.Combine(current, LibVersion,"libwkhtmltox.so");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                libPath = Path.Combine(current, LibVersion,"libwkhtmltox.dll");

            LoadUnmanagedDllFromPath(libPath);
        }
    }
}