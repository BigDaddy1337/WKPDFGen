using System.Collections.Generic;
using System.Reflection;
using WKPDFGen.Settings;

namespace WKPDFGen;

public interface IUserSettings
{
    // Just a marker
}
    
public record LibrarySetting(string Name, object Value, bool IsGlobal);

public interface IWKPDFGenConfiguration
{
    List<LibrarySetting> LibrarySettings { get; }
}

public class WKPDFGenConfiguration : IWKPDFGenConfiguration
{
    public WKPDFGenConfiguration(params IUserSettings?[] settings)
    {
        FillLibrarySettings(settings);
    }

    public List<LibrarySetting> LibrarySettings { get; } = new();

    private void FillLibrarySettings(params IUserSettings?[] userSettingsCollections)
    {
        foreach (var userSettingCollection in userSettingsCollections)
        {
            if (userSettingCollection is null) continue;
            
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

                    LibrarySettings.Add(new LibrarySetting(attribute.Name, propertyValue, attribute.IsGlobal));
                        
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