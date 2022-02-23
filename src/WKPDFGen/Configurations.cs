using System.Collections.Generic;
using System.Reflection;
using WKPDFGen.Settings;

namespace WKPDFGen;

public interface IUserSettings
{
    // Just a marker
}
    
public record LibrarySetting(string Name, object Value, bool IsGlobal);

public interface IConfiguration
{
    List<LibrarySetting> LibrarySettings { get; }
}

public abstract class ConfigurationBase : IConfiguration
{
    protected ConfigurationBase(params IUserSettings?[] userSettingCollections)
    {
        FillLibrarySettings(userSettingCollections);
    }

    public List<LibrarySetting> LibrarySettings { get; } = new();

    private void FillLibrarySettings(params IUserSettings?[] userSettingCollections)
    {
        foreach (var userSettingCollection in userSettingCollections)
        {
            if (userSettingCollection is null) continue;

            var isGlobalSettings = userSettingCollection is GlobalPdfSettings;

            var properties = userSettingCollection
                             .GetType()
                             .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(userSettingCollection);
                if (propertyValue is null) continue;
                
                foreach (var attr in property.GetCustomAttributes())
                {
                    if (attr is not WkHtmlSettingsAttribute attribute) continue;

                    LibrarySettings.Add(new LibrarySetting(attribute.Name, propertyValue, isGlobalSettings));
                        
                    break;
                }
                
                if (propertyValue is IUserSettings childSetting)
                {
                    FillLibrarySettings(childSetting);
                }
            }
        }
    }
}

// ReSharper disable once InconsistentNaming
/// <summary>
/// Details https://wkhtmltopdf.org/libwkhtmltox/pagesettings.html
/// </summary>
public class PDFConfiguration : ConfigurationBase
{
    public PDFConfiguration(PdfSettings pdfSettings, GlobalPdfSettings globalPdfSettings)
        : base(pdfSettings, globalPdfSettings)
    {
    }
}