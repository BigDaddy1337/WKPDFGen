using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WKPDFGen.Library;

public partial class WkHtmlWrapper
{
    private IntPtr libraryHandle = IntPtr.Zero;

    private IntPtr ImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName != WkHtmlBindings.Dllname) throw new NotImplementedException($"Library {libraryName} was unexpected.");

        if (libraryHandle == IntPtr.Zero)
            NativeLibrary.TryLoad(libraryPath, out libraryHandle);

        return libraryHandle;
    }


    private void FreeLibraryHandle()
    {
        if (libraryHandle == IntPtr.Zero) return;

        NativeLibrary.Free(libraryHandle);
    }
}