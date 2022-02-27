namespace WKPDFGen.Settings;

public class PdfSettings: IUserSettings
{
    /// <summary>
    /// The URL or path of the web page to convert, if "-" input is read from stdin. Default = ""
    /// </summary>
    [WkHtmlSettings("page")]
    public string? Page { get; set; }

    /// <summary>
    /// Should external links in the HTML document be converted into external pdf links. Default = true
    /// </summary>
    [WkHtmlSettings("useExternalLinks")]
    public bool? UseExternalLinks { get; set; }

    /// <summary>
    /// Should internal links in the HTML document be converted into pdf references. Default = true
    /// </summary>
    [WkHtmlSettings("useLocalLinks")]
    public bool? UseLocalLinks { get; set; }

    /// <summary>
    /// Should we turn HTML forms into PDF forms. Default = false
    /// </summary>
    [WkHtmlSettings("produceForms")]
    public bool? ProduceForms { get; set; }

    /// <summary>
    /// Should the sections from this document be included in the outline and table of content. Default = false
    /// </summary>
    [WkHtmlSettings("includeInOutline")]
    public bool? IncludeInOutline { get; set; }

    /// <summary>
    /// Should we count the pages of this document, in the counter used for TOC, headers and footers. Default = false
    /// </summary>
    [WkHtmlSettings("pagesCount")]
    public bool? PagesCount { get; set; }

    public WebSettings WebSettings { get; set; } = new();

    public HeaderSettings HeaderSettings { get; set; } = new();

    public FooterSettings FooterSettings { get; set; } = new();

    public LoadSettings LoadSettings { get; set; } = new();
}