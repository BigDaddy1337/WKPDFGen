using System;
using System.Data;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace WKPDFGen.Library
{
    public partial class WkHtmlToXService: AssemblyLoadContext
    {
        public void LoadWkHtmlToXLibraryDll(WkPdfOptions options)
        {
            string libPath;
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                libPath = options.OsxLibPath ?? throw new NoNullAllowedException("OsxLibPath option is required for current OS");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                libPath = options.LinuxLibPath ?? throw new NoNullAllowedException("LinuxLibPath option is required for current OS");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                libPath = options.WindowsLibPath ?? throw new NoNullAllowedException("WindowsLibPath option is required for current OS");
            else throw new PlatformNotSupportedException();
            
            LoadUnmanagedDllFromPath(libPath);
        }
    }
}