using System;
using System.Runtime.InteropServices;
using WKPDFGen.Library.Utils;

namespace WKPDFGen.Library
{
    public static class WkHtmlToXBindings
    {
        private const string Dllname = "libwkhtmltox";

        /// <summary>
        /// This value "extends" UnmanagedType enum from System.Runtime.InteropServices v4.1.0 which doesn't have LPUTF8Str (enum value is 48) enumartion defined
        /// </summary>
        private const short Lputf8Str = 48;

        private const CharSet Charset = CharSet.Unicode;

        [DllImport(Dllname, CharSet = Charset, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr wkhtmltopdf_version();

        [DllImport(Dllname, CharSet = Charset, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_init(int useGraphics);

        [DllImport(Dllname, CharSet = Charset, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_deinit();

        [DllImport(Dllname, CharSet = Charset, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr wkhtmltopdf_create_global_settings();

        [DllImport(Dllname, CharSet = Charset)]
        public static extern int wkhtmltopdf_set_global_setting(
            IntPtr settings,
            [MarshalAs(Lputf8Str)]
            string name,
            [MarshalAs(Lputf8Str)]
            string value
        );

        [DllImport(Dllname, CharSet = Charset, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr wkhtmltopdf_create_object_settings();

        [DllImport(Dllname, CharSet = Charset)]
        public static extern int wkhtmltopdf_set_object_setting(
            IntPtr settings,
            [MarshalAs(Lputf8Str)]
            string name,
            [MarshalAs(Lputf8Str)]
            string value
        );

        [DllImport(Dllname, CharSet = Charset, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr wkhtmltopdf_create_converter(IntPtr globalSettings);

        [DllImport(Dllname, CharSet = Charset, CallingConvention = CallingConvention.Cdecl)]
        public static extern void wkhtmltopdf_add_object(IntPtr converter, IntPtr objectSettings, byte[] data);

        [DllImport(Dllname, CharSet = Charset, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool wkhtmltopdf_convert(IntPtr converter);

        [DllImport(Dllname, CharSet = Charset, CallingConvention = CallingConvention.Cdecl)]
        public static extern void wkhtmltopdf_destroy_converter(IntPtr converter);

        [DllImport(Dllname, CharSet = Charset, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_get_output(IntPtr converter, out IntPtr data);

        [DllImport(Dllname, CharSet = Charset, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_set_phase_changed_callback(IntPtr converter, [MarshalAs(UnmanagedType.FunctionPtr)] VoidCallback callback);

        [DllImport(Dllname, CharSet = Charset, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_set_progress_changed_callback(IntPtr converter, [MarshalAs(UnmanagedType.FunctionPtr)] VoidCallback callback);

        [DllImport(Dllname, CharSet = Charset, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_set_finished_callback(IntPtr converter, [MarshalAs(UnmanagedType.FunctionPtr)] IntCallback callback);

        [DllImport(Dllname, CharSet = Charset, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_set_warning_callback(IntPtr converter, [MarshalAs(UnmanagedType.FunctionPtr)] StringCallback callback);

        [DllImport(Dllname, CharSet = Charset, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_set_error_callback(IntPtr converter, [MarshalAs(UnmanagedType.FunctionPtr)] StringCallback callback);

        [DllImport(Dllname, CharSet = Charset, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_phase_count(IntPtr converter);

        [DllImport(Dllname, CharSet = Charset, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_current_phase(IntPtr converter);

        [DllImport(Dllname, CharSet = Charset, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr wkhtmltopdf_phase_description(IntPtr converter, int phase);

        [DllImport(Dllname, CharSet = Charset, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr wkhtmltopdf_progress_string(IntPtr converter);
    }
}
