using NUnit.Framework;
using WKPDFGen.Settings;
using WKPDFGen.Settings.Units;

namespace WKPDFGen.Tests;

public class ConfigurationTests
{
    [Test]
    public void FillLibrarySettingsTest()
    {
        var pdfConfig = new PDFConfiguration(
            new PdfSettings
            {
                PagesCount = true,
                HeaderSettings = new HeaderSettings
                {
                    FontSize = 14,
                    Line = false
                },
                FooterSettings = new FooterSettings
                {
                    FontSize = 14,
                    Line = false,
                    Spacing = 2.0
                }
            }, 
            new GlobalPdfSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Copies = 1
            }
        );
        
        Assert.IsNotNull(pdfConfig.LibrarySettings);
        Assert.IsNotEmpty(pdfConfig.LibrarySettings);
        Assert.AreEqual(11, pdfConfig.LibrarySettings.Count);
    }
}