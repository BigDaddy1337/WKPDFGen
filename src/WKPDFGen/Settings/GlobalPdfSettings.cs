using WKPDFGen.Settings.Units;

namespace WKPDFGen.Settings;

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

public class GlobalPdfSettings: IUserSettings
{
    /// <summary>
    /// The orientation of the output document, must be either "Landscape" or "Portrait". Default = "portrait"
    /// </summary>
    [WkHtmlSettings("orientation")]
    public Orientation? Orientation { get; set; }

    /// <summary>
    /// Should the output be printed in color or gray scale, must be either "Color" or "Grayscale". Default = "color"
    /// </summary>
    [WkHtmlSettings("colorMode")]
    public ColorMode? ColorMode { get; set; }

    /// <summary>
    /// Should we use loss less compression when creating the pdf file. Default = true
    /// </summary>
    [WkHtmlSettings("useCompression")]
    public bool? UseCompression { get; set; }

    /// <summary>
    /// What dpi should we use when printing. Default = 96
    /// </summary>
    [WkHtmlSettings("dpi")]
    public int? DPI { get; set; }

    /// <summary>
    /// A number that is added to all page numbers when printing headers, footers and table of content. Default = 0
    /// </summary>
    [WkHtmlSettings("pageOffset")]
    public int? PageOffset { get; set; }

    /// <summary>
    /// How many copies should we print. Default = 1
    /// </summary>
    [WkHtmlSettings("copies")]
    public int? Copies { get; set; }

    /// <summary>
    /// Should the copies be collated. Default = true
    /// </summary>
    [WkHtmlSettings("collate")]
    public bool? Collate { get; set; }

    /// <summary>
    /// Should a outline (table of content in the sidebar) be generated and put into the PDF. Default = true
    /// </summary>
    [WkHtmlSettings("outline")]
    public bool? Outline { get; set; }

    /// <summary>
    /// The maximal depth of the outline. Default = 4
    /// </summary>
    [WkHtmlSettings("outlineDepth")]
    public int? OutlineDepth { get; set; }

    /// <summary>
    /// If not set to the empty string a XML representation of the outline is dumped to this file. Default = ""
    /// </summary>
    [WkHtmlSettings("dumpOutline")]
    public string DumpOutline { get; set; }

    /// <summary>
    /// The path of the output file, if "-" output is sent to stdout, if empty the output is stored in a buffer. Default = ""
    /// </summary>
    [WkHtmlSettings("out")]
    public string Out { get; set; }

    /// <summary>
    /// The title of the PDF document. Default = ""
    /// </summary>
    [WkHtmlSettings("documentTitle")]
    public string DocumentTitle { get; set; }

    /// <summary>
    /// The maximal DPI to use for images in the pdf document. Default = 600
    /// </summary>
    [WkHtmlSettings("imageDPI")]
    public int? ImageDPI { get; set; }

    /// <summary>
    /// The jpeg compression factor to use when producing the pdf document. Default = 94
    /// </summary>
    [WkHtmlSettings("imageQuality")]
    public int? ImageQuality { get; set; }

    /// <summary>
    /// Path of file used to load and store cookies. Default = ""
    /// </summary>
    [WkHtmlSettings("load.cookieJar")]
    public string CookieJar { get; set; }

    /// <summary>
    /// Size of output paper
    /// </summary>
    public PaperSize PaperSize { get; set; }

    /// <summary>
    /// The height of the output document
    /// </summary>
    [WkHtmlSettings("size.height")]
    private string PaperHeight => PaperSize?.Height;

    /// <summary>
    /// The width of the output document
    /// </summary>
    [WkHtmlSettings("size.width")]
    private string PaperWidth => PaperSize?.Width;

    public MarginSettings Margins { get; set; } = new MarginSettings();

    /// <summary>
    /// Size of the left margin
    /// </summary>
    [WkHtmlSettings("margin.left")]
    private string MarginLeft => Margins.GetMarginValue(Margins.Left);

    /// <summary>
    /// Size of the right margin
    /// </summary>
    [WkHtmlSettings("margin.right")]
    private string MarginRight => Margins.GetMarginValue(Margins.Right);

    /// <summary>
    /// Size of the top margin
    /// </summary>
    [WkHtmlSettings("margin.top")]
    private string MarginTop => Margins.GetMarginValue(Margins.Top);

    /// <summary>
    /// Size of the bottom margin
    /// </summary>
    [WkHtmlSettings("margin.bottom")]
    private string MarginBottom => Margins.GetMarginValue(Margins.Bottom);

    /// <summary>
    /// Set viewport size. Not supported in wkhtmltopdf API since v0.12.2.4 
    /// </summary>
    [WkHtmlSettings("viewportSize")]
    public string ViewportSize { get; set; }
}