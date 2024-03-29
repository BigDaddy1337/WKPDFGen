using System;
using System.Runtime.InteropServices;

namespace WKPDFGen.Library;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void IntCallback(IntPtr converter, int integer);
        
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void StringCallback(IntPtr converter, [MarshalAs(UnmanagedType.LPStr)] string str);
        
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void VoidCallback(IntPtr converter);