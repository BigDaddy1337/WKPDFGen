using WKPDFGen.Settings.Attributes;

namespace WKPDFGen.Settings.ChildSettings
{
    public class FooterSettings : IUserSettings
    {
        /// <summary>
        /// The font size to use for the footer. Default = 12
        /// </summary>
        [WkHtmlSetting("footer.fontSize")]
        public int? FontSize { get; set; }

        /// <summary>
        /// The name of the font to use for the footer. Default = "Ariel"
        /// </summary>
        [WkHtmlSetting("footer.fontName")]
        public string FontName { get; set; }

        /// <summary>
        /// The string to print in the left part of the footer, note that some sequences are replaced in this string, see the wkhtmltopdf manual. Default = ""
        /// </summary>
        [WkHtmlSetting("footer.left")]
        public string Left { get; set; }

        /// <summary>
        /// The text to print in the right part of the footer, note that some sequences are replaced in this string, see the wkhtmltopdf manual. Default = ""
        /// </summary>
        [WkHtmlSetting("footer.center")]
        public string Center { get; set; }

        /// <summary>
        /// The text to print in the right part of the footer, note that some sequences are replaced in this string, see the wkhtmltopdf manual. Default = ""
        /// </summary>
        [WkHtmlSetting("footer.right")]
        public string Right { get; set; }

        /// <summary>
        /// Whether a line should be printed above the footer. Default = false
        /// </summary>
        [WkHtmlSetting("footer.line")]
        public bool? Line { get; set; }

        /// <summary>
        /// The amount of space to put between the footer and the content, e.g. "1.8". Be aware that if this is too large the footer will be printed outside the pdf document. This can be corrected with the margin.bottom setting. Default = 0.00
        /// </summary>
        [WkHtmlSetting("footer.spacing")]
        public double? Spacing { get; set; }

        /// <summary>
        /// Url for a HTML document to use for the footer. Default = ""
        /// </summary>
        [WkHtmlSetting("footer.htmlUrl")]
        public string HtmUrl { get; set; }
    }
}
