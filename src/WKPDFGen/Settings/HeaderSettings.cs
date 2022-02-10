namespace WKPDFGen.Settings;

public class HeaderSettings: IUserSettings
{
    /// <summary>
    /// The font size to use for the header. Default = 12
    /// </summary>
    [WkHtmlSettings("header.fontSize")]
    public int? FontSize { get; set; }

    /// <summary>
    /// The name of the font to use for the header. Default = "Ariel"
    /// </summary>
    [WkHtmlSettings("header.fontName")]
    public string FontName { get; set; }

    /// <summary>
    /// The string to print in the left part of the header, note that some sequences are replaced in this string, see the wkhtmltopdf manual. Default = ""
    /// </summary>
    [WkHtmlSettings("header.left")]
    public string Left { get; set; }

    /// <summary>
    /// The text to print in the right part of the header, note that some sequences are replaced in this string, see the wkhtmltopdf manual. Default = ""
    /// </summary>
    [WkHtmlSettings("header.center")]
    public string Center { get; set; }

    /// <summary>
    /// The text to print in the right part of the header, note that some sequences are replaced in this string, see the wkhtmltopdf manual. Default = ""
    /// </summary>
    [WkHtmlSettings("header.right")]
    public string Right { get; set; }

    /// <summary>
    /// Whether a line should be printed under the header. Default = false
    /// </summary>
    [WkHtmlSettings("header.line")]
    public bool? Line { get; set; }

    /// <summary>
    /// The amount of space to put between the header and the content, e.g. "1.8". Be aware that if this is too large the header will be printed outside the pdf document. This can be corrected with the margin.top setting. Default = 0.00
    /// </summary>
    [WkHtmlSettings("header.spacing")]
    public double? Spacing { get; set; }

    /// <summary>
    /// Url for a HTML document to use for the header. Default = ""
    /// </summary>
    [WkHtmlSettings("header.htmlUrl")]
    public string HtmUrl { get; set; }
}