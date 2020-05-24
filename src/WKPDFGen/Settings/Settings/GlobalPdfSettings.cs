using WKPDFGen.Settings.Attributes;
using WKPDFGen.Settings.ChildSettings;
using WKPDFGen.Settings.Units;

namespace WKPDFGen.Settings.Settings
{
    public enum ColorMode
    {
        Color,
        Grayscale
    }

    public enum Orientation
    {
        Landscape,
        Portrait
    }

    public class GlobalPdfSettings : IUserSettings
    {
        /// <summary>
        /// The orientation of the output document, must be either "Landscape" or "Portrait". Default = "portrait"
        /// </summary>
        [WkHtmlSetting("orientation")]
        public Orientation? Orientation { get; set; }

        /// <summary>
        /// Should the output be printed in color or gray scale, must be either "Color" or "Grayscale". Default = "color"
        /// </summary>
        [WkHtmlSetting("colorMode")]
        public ColorMode? ColorMode { get; set; }

        /// <summary>
        /// Should we use loss less compression when creating the pdf file. Default = true
        /// </summary>
        [WkHtmlSetting("useCompression")]
        public bool? UseCompression { get; set; }

        /// <summary>
        /// What dpi should we use when printing. Default = 96
        /// </summary>
        [WkHtmlSetting("dpi")]
        public int? DPI { get; set; }

        /// <summary>
        /// A number that is added to all page numbers when printing headers, footers and table of content. Default = 0
        /// </summary>
        [WkHtmlSetting("pageOffset")]
        public int? PageOffset { get; set; }

        /// <summary>
        /// How many copies should we print. Default = 1
        /// </summary>
        [WkHtmlSetting("copies")]
        public int? Copies { get; set; }

        /// <summary>
        /// Should the copies be collated. Default = true
        /// </summary>
        [WkHtmlSetting("collate")]
        public bool? Collate { get; set; }

        /// <summary>
        /// Should a outline (table of content in the sidebar) be generated and put into the PDF. Default = true
        /// </summary>
        [WkHtmlSetting("outline")]
        public bool? Outline { get; set; }

        /// <summary>
        /// The maximal depth of the outline. Default = 4
        /// </summary>
        [WkHtmlSetting("outlineDepth")]
        public int? OutlineDepth { get; set; }

        /// <summary>
        /// If not set to the empty string a XML representation of the outline is dumped to this file. Default = ""
        /// </summary>
        [WkHtmlSetting("dumpOutline")]
        public string DumpOutline { get; set; }

        /// <summary>
        /// The path of the output file, if "-" output is sent to stdout, if empty the output is stored in a buffer. Default = ""
        /// </summary>
        [WkHtmlSetting("out")]
        public string Out { get; set; }

        /// <summary>
        /// The title of the PDF document. Default = ""
        /// </summary>
        [WkHtmlSetting("documentTitle")]
        public string DocumentTitle { get; set; }

        /// <summary>
        /// The maximal DPI to use for images in the pdf document. Default = 600
        /// </summary>
        [WkHtmlSetting("imageDPI")]
        public int? ImageDPI { get; set; }

        /// <summary>
        /// The jpeg compression factor to use when producing the pdf document. Default = 94
        /// </summary>
        [WkHtmlSetting("imageQuality")]
        public int? ImageQuality { get; set; }

        /// <summary>
        /// Path of file used to load and store cookies. Default = ""
        /// </summary>
        [WkHtmlSetting("load.cookieJar")]
        public string CookieJar { get; set; }

        /// <summary>
        /// Size of output paper
        /// </summary>
        public PaperSize PaperSize { get; set; }

        /// <summary>
        /// The height of the output document
        /// </summary>
        [WkHtmlSetting("size.height")]
        private string PaperHeight => PaperSize?.Height;

        /// <summary>
        /// The width of the output document
        /// </summary>
        [WkHtmlSetting("size.width")]
        private string PaperWidth => PaperSize?.Width;

        public MarginSettings Margins { get; set; } = new MarginSettings();

        /// <summary>
        /// Size of the left margin
        /// </summary>
        [WkHtmlSetting("margin.left")]
        private string MarginLeft => Margins.GetMarginValue(Margins.Left);

        /// <summary>
        /// Size of the right margin
        /// </summary>
        [WkHtmlSetting("margin.right")]
        private string MarginRight => Margins.GetMarginValue(Margins.Right);

        /// <summary>
        /// Size of the top margin
        /// </summary>
        [WkHtmlSetting("margin.top")]
        private string MarginTop => Margins.GetMarginValue(Margins.Top);

        /// <summary>
        /// Size of the bottom margin
        /// </summary>
        [WkHtmlSetting("margin.bottom")]
        private string MarginBottom => Margins.GetMarginValue(Margins.Bottom);

        /// <summary>
        /// Set viewport size. Not supported in wkhtmltopdf API since v0.12.2.4 
        /// </summary>
        [WkHtmlSetting("viewportSize")]
        public string ViewportSize { get; set; }
    }
}
