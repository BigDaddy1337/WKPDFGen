using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace WKPDFGen.Library;

public partial class WkHtmlWrapper
{
    private IntPtr libraryHandle = IntPtr.Zero;

    private IntPtr NativeLibraryImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName != WkHtmlBindings.Dllname) throw new NotImplementedException($"Library {libraryName} was unexpected.");

        if (libraryHandle == IntPtr.Zero)
        {
            logger.LogDebug("[WkHTMLtoPDF] Importing native library from path {path}", libraryPath);

            NativeLibrary.TryLoad(libraryPath, out libraryHandle);
            
            if (libraryHandle == IntPtr.Zero)
                logger.LogWarning("[WkHTMLtoPDF] Importing native library from path {path} failed for some reason, check OS architecture compatibility", libraryPath);
            else
                logger.LogDebug("[WkHTMLtoPDF] Library imported from path {path}", libraryPath);
        }

        return libraryHandle;
    }


    private void FreeLibraryHandle()
    {
        if (libraryHandle == IntPtr.Zero) return;

        NativeLibrary.Free(libraryHandle);
    }
}