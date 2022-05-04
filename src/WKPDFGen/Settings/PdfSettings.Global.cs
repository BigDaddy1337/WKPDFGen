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

public partial class PdfSettings
{
    /// <summary>
    /// Size of output paper
    /// </summary>
    public PaperSize? PaperSize { get; set; }

    /// <summary>
    /// The height of the output document
    /// </summary>
    [WkHtmlSettings("size.height", true)]
    private string? PaperHeight => PaperSize?.Height;

    /// <summary>
    /// The width of the output document
    /// </summary>
    [WkHtmlSettings("size.width", true)]
    private string? PaperWidth => PaperSize?.Width;
    
    /// <summary>
    /// The orientation of the output document, must be either "Landscape" or "Portrait". Default = "portrait"
    /// </summary>
    [WkHtmlSettings("orientation", true)]
    public Orientation? Orientation { get; set; }

    /// <summary>
    /// Should the output be printed in color or gray scale, must be either "Color" or "Grayscale". Default = "color"
    /// </summary>
    [WkHtmlSettings("colorMode", true)]
    public ColorMode? ColorMode { get; set; }

    /// <summary>
    /// What dpi should we use when printing. Default = 96
    /// </summary>
    [WkHtmlSettings("dpi", true)]
    public int? DPI { get; set; }
    
    /// <summary>
    /// A number that is added to all page numbers when printing headers, footers and table of content. Default = 0
    /// </summary>
    [WkHtmlSettings("pageOffset", true)]
    public int? PageOffset { get; set; }

    /// <summary>
    /// How many copies should we print. Default = 1
    /// </summary>
    [WkHtmlSettings("copies", true)]
    public int? Copies { get; set; }

    /// <summary>
    /// Should the copies be collated. Default = true
    /// </summary>
    [WkHtmlSettings("collate", true)]
    public bool? CopiesCollate { get; set; }

    /// <summary>
    /// Should a outline (table of content in the sidebar) be generated and put into the PDF. Default = true
    /// </summary>
    [WkHtmlSettings("outline", true)]
    public bool? Outline { get; set; }

    /// <summary>
    /// The maximal depth of the outline. Default = 4
    /// </summary>
    [WkHtmlSettings("outlineDepth", true)]
    public int? OutlineDepth { get; set; }

    /// <summary>
    /// If not set to the empty string a XML representation of the outline is dumped to this file. Default = ""
    /// </summary>
    [WkHtmlSettings("dumpOutline", true)]
    public string? DumpOutline { get; set; }

    /// <summary>
    /// The title of the PDF document. Default = ""
    /// </summary>
    [WkHtmlSettings("documentTitle", true)]
    public string? DocumentTitle { get; set; }
    
    /// <summary>
    /// Should we use loss less compression when creating the pdf file. Default = true
    /// </summary>
    [WkHtmlSettings("useCompression", true)]
    public bool? UseCompression { get; set; }
    
    public Margin Margins { get; set; } = new();
    
    /// <summary>
    /// Size of the top margin
    /// </summary>
    [WkHtmlSettings("margin.top", true)]
    private string? MarginTop => Margins.GetMarginValue(Margins.Top);

    /// <summary>
    /// Size of the bottom margin
    /// </summary>
    [WkHtmlSettings("margin.bottom", true)]
    private string? MarginBottom => Margins.GetMarginValue(Margins.Bottom);

    /// <summary>
    /// Size of the left margin
    /// </summary>
    [WkHtmlSettings("margin.left", true)]
    private string? MarginLeft => Margins.GetMarginValue(Margins.Left);

    /// <summary>
    /// Size of the right margin
    /// </summary>
    [WkHtmlSettings("margin.right", true)]
    private string? MarginRight => Margins.GetMarginValue(Margins.Right);

    /// <summary>
    /// The maximal DPI to use for images in the pdf document. Default = 600
    /// </summary>
    [WkHtmlSettings("imageDPI", true)]
    public int? ImageDPI { get; set; }

    /// <summary>
    /// The jpeg compression factor to use when producing the pdf document. Default = 94
    /// </summary>
    [WkHtmlSettings("imageQuality", true)]
    public int? ImageQuality { get; set; }

    /// <summary>
    /// Path of file used to load and store cookies. Default = ""
    /// </summary>
    [WkHtmlSettings("load.cookieJar", true)]
    public string? CookieJar { get; set; }
}